// -----------------------------------------------------------------------
// <copyright file="EchoFilter.cs" company="ExMod Team">
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
    /// A true DSP Fractional Delay Filter equipped with an RBJ Butterworth Biquad Filter.
    /// </summary>
    public sealed class EchoFilter : IAudioFilter
    {
        private const float MaxDelayMs = 10000f;
        private readonly float[] delayBuffer;
        private readonly int maxBufferLength;

        private int writeIndex;

        private float b0;
        private float b1;
        private float b2;
        private float a1;
        private float a2;
        private float x1;
        private float x2;
        private float y1;
        private float y2;

        /// <summary>
        /// Initializes a new instance of the <see cref="EchoFilter"/> class.
        /// </summary>
        /// <param name="delayMs">The delay time in milliseconds (10 - 10000).</param>
        /// <param name="decay">The feedback multiplier determining how long the echo lasts.</param>
        /// <param name="dry">The volume of the original sound.</param>
        /// <param name="wet">The volume of the echoed sound.</param>
        /// <param name="damp">How much high-frequency is absorbed each bounce (0 = pure digital ring, 1 = heavy muffled echo).</param>
        public EchoFilter(float delayMs = 300f, float decay = 0.5f, float dry = 1.0f, float wet = 0.5f, float damp = 0.3f)
        {
            maxBufferLength = (int)(VoiceChat.VoiceChatSettings.SampleRate * (MaxDelayMs / 1000f));
            delayBuffer = new float[maxBufferLength];

            writeIndex = 0;
            x1 = x2 = y1 = y2 = 0f;

            Delay = delayMs;
            Feedback = decay;
            DryMix = dry;
            WetMix = wet;
            Damping = damp;
        }

        /// <summary>
        /// Gets or sets the delay time in milliseconds. Dynamically adjusts the read head.
        /// </summary>
        public float Delay
        {
            get;
            set => field = Mathf.Clamp(value, 10f, MaxDelayMs);
        }

        /// <summary>
        /// Gets or sets the feedback multiplier. Determines how many times the echo repeats before dying out.
        /// </summary>
        public float Feedback
        {
            get;
            set => field = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Gets or sets the volume of the original (dry) unaffected sound.
        /// </summary>
        public float DryMix
        {
            get;
            set => field = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Gets or sets the volume of the delayed (wet) echoed sound.
        /// </summary>
        public float WetMix
        {
            get;
            set => field = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Gets or sets the damping coefficient. Automatically recalculates the RBJ Biquad coefficients.
        /// </summary>
        public float Damping
        {
            get;
            set
            {
                field = Mathf.Clamp01(value);
                CalculateBiquad(field);
            }
        }

        /// <summary>
        /// Processes the raw PCM audio frame directly before it is encoded and sending.
        /// </summary>
        /// <param name="frame">The array of PCM audio samples.</param>
        public void Process(float[] frame)
        {
            float currentDelayMs = Delay;
            float currentFeedback = Feedback;
            float currentDry = DryMix;
            float currentWet = WetMix;

            float delaySamples = VoiceChat.VoiceChatSettings.SampleRate * (currentDelayMs / 1000f);

            for (int i = 0; i < frame.Length; i++)
            {
                float input = frame[i];
                float readPos = writeIndex - delaySamples;
                if (readPos < 0)
                    readPos += maxBufferLength;

                int index1 = (int)readPos;
                int index2 = (index1 + 1) % maxBufferLength;
                float frac = readPos - index1;

                float delayedSample = (delayBuffer[index1] * (1f - frac)) + (delayBuffer[index2] * frac);
                float filteredSample = (b0 * delayedSample) + (b1 * x1) + (b2 * x2) - (a1 * y1) - (a2 * y2);

                x2 = x1;
                x1 = delayedSample;
                y2 = y1;
                y1 = filteredSample;

                float output = (input * currentDry) + (filteredSample * currentWet);
                delayBuffer[writeIndex] = input + (filteredSample * currentFeedback);

                writeIndex++;
                if (writeIndex >= maxBufferLength)
                    writeIndex = 0;

                frame[i] = output / (1f + Mathf.Abs(output));
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            Array.Clear(delayBuffer, 0, delayBuffer.Length);
            writeIndex = 0;
            x1 = x2 = y1 = y2 = 0f;
        }

        /// <summary>
        /// Calculates the Robert Bristow-Johnson (RBJ) Audio EQ parameters for the low-pass filter.
        /// </summary>
        private void CalculateBiquad(float dampValue)
        {
            float cutoffFrequency = Mathf.Lerp(20000f, 500f, dampValue);

            if (cutoffFrequency >= VoiceChat.VoiceChatSettings.SampleRate / 2f)
                cutoffFrequency = (VoiceChat.VoiceChatSettings.SampleRate / 2f) - 100f;

            float w0 = 2f * Mathf.PI * cutoffFrequency / VoiceChat.VoiceChatSettings.SampleRate;
            float alpha = Mathf.Sin(w0) / (2f * 0.7071f);

            float a0 = 1f + alpha;
            b0 = ((1f - Mathf.Cos(w0)) / 2f) / a0;
            b1 = (1f - Mathf.Cos(w0)) / a0;
            b2 = ((1f - Mathf.Cos(w0)) / 2f) / a0;
            a1 = (-2f * Mathf.Cos(w0)) / a0;
            a2 = (1f - alpha) / a0;
        }
    }
}