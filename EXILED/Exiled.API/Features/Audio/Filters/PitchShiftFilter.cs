// -----------------------------------------------------------------------
// <copyright file="PitchShiftFilter.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.Filters
{
    using System;

    using Exiled.API.Interfaces.Audio;

    using UnityEngine;

    /// <summary>
    /// A true DSP Granular Pitch Shifter based on the smbPitchShift algorithm.
    /// </summary>
    public sealed class PitchShiftFilter : IAudioFilter
    {
        private const int FftFrameSize = 2048;
        private const int FftFrameSize2 = FftFrameSize / 2;
        private const int MaxFrameLength = 8192;

        private static readonly float[] HannWindow = BuildHannWindow();

        private readonly float[] gInFIFO = new float[MaxFrameLength];
        private readonly float[] gOutFIFO = new float[MaxFrameLength];
        private readonly float[] gFFTworksp = new float[2 * FftFrameSize];
        private readonly float[] gLastPhase = new float[FftFrameSize2 + 1];
        private readonly float[] gSumPhase = new float[FftFrameSize2 + 1];
        private readonly float[] gOutputAccum = new float[2 * FftFrameSize];
        private readonly float[] gAnaFreq = new float[FftFrameSize];
        private readonly float[] gAnaMagn = new float[FftFrameSize];
        private readonly float[] gSynFreq = new float[FftFrameSize];
        private readonly float[] gSynMagn = new float[FftFrameSize];

        private readonly float[] outputBuffer = new float[MaxFrameLength];

        private readonly float[] twiddleCos;
        private readonly float[] twiddleSin;

        private long gRover = 0;

        private int cachedOversample = -1;
        private long stepSize;
        private long inFifoLatency;
        private float expct;

        /// <summary>
        /// Initializes a new instance of the <see cref="PitchShiftFilter"/> class.
        /// </summary>
        /// <param name="pitch">The pitch multiplier. Above 1.0 for higher pitch, below 1.0 for lower pitch.</param>
        /// <param name="oversample">
        /// The overlap factor controlling quality vs CPU usage. Higher values produce better quality but require more CPU. Must be a power of 2. Typical values: 2 (low CPU), 4 (default, balanced), 8 (high quality).
        /// </param>
        public PitchShiftFilter(float pitch = 1.5f, int oversample = 4)
        {
            twiddleCos = new float[FftFrameSize];
            twiddleSin = new float[FftFrameSize];
            PrecomputeTwiddleFactors();

            Pitch = pitch;
            Oversample = oversample;
        }

        /// <summary>
        /// Gets or sets the pitch multiplier applied during playback.
        /// Values above 1.0 produce a higher (thinner) pitch; values below 1.0 produce a lower (deeper) pitch.
        /// </summary>
        public float Pitch
        {
            get;
            set => field = Mathf.Clamp(value, 0.1f, 4.0f);
        }

        /// <summary>
        /// Gets or sets the overlap factor controlling quality versus CPU usage.
        /// Higher values improve quality but increase processing cost. Must be a power of 2.
        /// Typical values: 2 (low CPU), 4 (balanced, default), 8 (high quality).
        /// </summary>
        public int Oversample
        {
            get;
            set
            {
                field = Mathf.Clamp(value, 2, 32);
                cachedOversample = -1;
            }
        }

        /// <inheritdoc/>
        public void Process(float[] frame)
        {
            if (Mathf.Abs(Pitch - 1.0f) < 0.001f)
                return;

            EnsureOversampleConstants();
            SmbPitchShift(Pitch, frame.Length, frame, outputBuffer);

            Array.Copy(outputBuffer, frame, frame.Length);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            Array.Clear(gInFIFO, 0, gInFIFO.Length);
            Array.Clear(gOutFIFO, 0, gOutFIFO.Length);
            Array.Clear(gFFTworksp, 0, gFFTworksp.Length);
            Array.Clear(gLastPhase, 0, gLastPhase.Length);
            Array.Clear(gSumPhase, 0, gSumPhase.Length);
            Array.Clear(gOutputAccum, 0, gOutputAccum.Length);
            Array.Clear(gAnaFreq, 0, gAnaFreq.Length);
            Array.Clear(gAnaMagn, 0, gAnaMagn.Length);
            Array.Clear(gSynFreq, 0, gSynFreq.Length);
            Array.Clear(gSynMagn, 0, gSynMagn.Length);
            Array.Clear(outputBuffer, 0, outputBuffer.Length);

            gRover = 0;
            cachedOversample = -1;
        }

        private static float[] BuildHannWindow()
        {
            float[] window = new float[FftFrameSize];
            for (int i = 0; i < FftFrameSize; i++)
                window[i] = 0.5f - (0.5f * Mathf.Cos(2.0f * Mathf.PI * i / FftFrameSize));

            return window;
        }

        private void PrecomputeTwiddleFactors()
        {
            for (int le = 4, k = 0; le <= FftFrameSize * 2; le <<= 1, k++)
            {
                int le2 = le >> 1;
                float arg = Mathf.PI / (le2 >> 1);
                twiddleCos[k] = Mathf.Cos(arg);
                twiddleSin[k] = Mathf.Sin(arg);
            }
        }

        private void EnsureOversampleConstants()
        {
            if (cachedOversample == Oversample)
                return;

            cachedOversample = Oversample;
            stepSize = FftFrameSize / Oversample;
            inFifoLatency = FftFrameSize - stepSize;
            expct = 2.0f * Mathf.PI * stepSize / FftFrameSize;

            if (gRover == 0)
                gRover = inFifoLatency;
        }

        private void SmbPitchShift(float pitchShift, int numSampsToProcess, float[] indata, float[] outdata)
        {
            float freqPerBin = VoiceChat.VoiceChatSettings.SampleRate / (float)FftFrameSize;

            for (int i = 0; i < numSampsToProcess; i++)
            {
                gInFIFO[gRover] = indata[i];
                outdata[i] = gOutFIFO[gRover - inFifoLatency];
                gRover++;

                if (gRover < FftFrameSize)
                    continue;

                gRover = inFifoLatency;

                for (int k = 0; k < FftFrameSize; k++)
                {
                    gFFTworksp[2 * k] = gInFIFO[k] * HannWindow[k];
                    gFFTworksp[(2 * k) + 1] = 0.0f;
                }

                SmbFft(gFFTworksp, -1);

                for (int k = 0; k <= FftFrameSize2; k++)
                {
                    float real = gFFTworksp[2 * k];
                    float imag = gFFTworksp[(2 * k) + 1];

                    float magn = 2.0f * Mathf.Sqrt((real * real) + (imag * imag));
                    float phase = Mathf.Atan2(imag, real);

                    float tmp = phase - gLastPhase[k];
                    gLastPhase[k] = phase;

                    tmp -= k * expct;

                    long qpd = (long)(tmp / Mathf.PI);
                    if (qpd >= 0)
                        qpd += qpd & 1;
                    else
                        qpd -= qpd & 1;

                    tmp -= Mathf.PI * qpd;

                    tmp = Oversample * tmp / (2.0f * Mathf.PI);
                    tmp = (k * freqPerBin) + (tmp * freqPerBin);

                    gAnaMagn[k] = magn;
                    gAnaFreq[k] = tmp;
                }

                Array.Clear(gSynMagn, 0, FftFrameSize);
                Array.Clear(gSynFreq, 0, FftFrameSize);

                for (int k = 0; k <= FftFrameSize2; k++)
                {
                    long index = (long)(k * pitchShift);
                    if (index <= FftFrameSize2)
                    {
                        gSynMagn[index] += gAnaMagn[k];
                        gSynFreq[index] = gAnaFreq[k] * pitchShift;
                    }
                }

                for (int k = 0; k <= FftFrameSize2; k++)
                {
                    float magn = gSynMagn[k];
                    float tmp = gSynFreq[k];

                    tmp -= k * freqPerBin;
                    tmp /= freqPerBin;
                    tmp = 2.0f * Mathf.PI * tmp / Oversample;
                    tmp += k * expct;

                    gSumPhase[k] += tmp;

                    gFFTworksp[2 * k] = magn * Mathf.Cos(gSumPhase[k]);
                    gFFTworksp[(2 * k) + 1] = magn * Mathf.Sin(gSumPhase[k]);
                }

                Array.Clear(gFFTworksp, FftFrameSize + 2, FftFrameSize - 2);

                SmbFft(gFFTworksp, 1);

                for (int k = 0; k < FftFrameSize; k++)
                    gOutputAccum[k] += 2.0f * HannWindow[k] * gFFTworksp[2 * k] / (FftFrameSize2 * Oversample);

                for (int k = 0; k < stepSize; k++)
                    gOutFIFO[k] = gOutputAccum[k];

                Array.Copy(gOutputAccum, stepSize, gOutputAccum, 0, FftFrameSize);
                Array.Clear(gOutputAccum, FftFrameSize, (int)stepSize);

                Array.Copy(gInFIFO, stepSize, gInFIFO, 0, inFifoLatency);
            }
        }

        private void SmbFft(float[] fftBuffer, int sign)
        {
            for (int i = 2; i < (2 * FftFrameSize) - 2; i += 2)
            {
                int j = 0;
                for (int bitm = 2; bitm < 2 * FftFrameSize; bitm <<= 1)
                {
                    if ((i & bitm) != 0)
                        j++;
                    j <<= 1;
                }

                if (i < j)
                {
                    (fftBuffer[j], fftBuffer[i]) = (fftBuffer[i], fftBuffer[j]);
                    (fftBuffer[j + 1], fftBuffer[i + 1]) = (fftBuffer[i + 1], fftBuffer[j + 1]);
                }
            }

            int stageIndex = 0;
            for (int le = 4; le <= FftFrameSize * 2; le <<= 1, stageIndex++)
            {
                int le2 = le >> 1;
                float wr = twiddleCos[stageIndex];
                float wi = twiddleSin[stageIndex] * sign;
                float ur = 1.0f, ui = 0.0f;

                for (int j = 0; j < le2; j += 2)
                {
                    for (int i = j; i < 2 * FftFrameSize; i += le)
                    {
                        float tr = (fftBuffer[i + le2] * ur) - (fftBuffer[i + le2 + 1] * ui);
                        float ti = (fftBuffer[i + le2] * ui) + (fftBuffer[i + le2 + 1] * ur);
                        fftBuffer[i + le2] = fftBuffer[i] - tr;
                        fftBuffer[i + le2 + 1] = fftBuffer[i + 1] - ti;
                        fftBuffer[i] += tr;
                        fftBuffer[i + 1] += ti;
                    }

                    float newUr = (ur * wr) - (ui * wi);
                    ui = (ur * wi) + (ui * wr);
                    ur = newUr;
                }
            }
        }
    }
}