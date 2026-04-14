// -----------------------------------------------------------------------
// <copyright file="PreloadedPcmSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.PcmSources
{
    using System;
    using System.Threading.Tasks;

    using Exiled.API.Features.Audio;
    using Exiled.API.Interfaces.Audio;
    using Exiled.API.Structs.Audio;

    using VoiceChat;

    /// <summary>
    /// Provides a <see cref="IPcmSource"/> preloaded with Pcm data or file.
    /// </summary>
    public sealed class PreloadedPcmSource : IPcmSource, IAsyncPcmSource
    {
        private float[] data;
        private int pos;

        private volatile bool isReady = false;
        private volatile bool isFailed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreloadedPcmSource"/> class.
        /// </summary>
        /// <param name="path">The path to the audio file.</param>
        public PreloadedPcmSource(string path)
        {
            TrackInfo = new TrackData { Path = path, Duration = 0.0 };

            Task.Run(() =>
            {
                try
                {
                    AudioData result = WavUtility.WavToPcm(path);
                    data = result.Pcm;
                    TrackInfo = result.TrackInfo;
                    isReady = true;
                }
                catch (Exception ex)
                {
                    Log.Error($"[PreloadedPcmSource] Failed to load audio from path: {path} | Error: {ex.Message}");
                    isFailed = true;
                }
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreloadedPcmSource"/> class.
        /// </summary>
        /// <param name="pcmData">The raw PCM float array.</param>
        public PreloadedPcmSource(float[] pcmData)
        {
            data = pcmData;
            isReady = true;
            TrackInfo = new TrackData { Duration = TotalDuration };
        }

        /// <summary>
        /// Gets the metadata of the loaded track.
        /// </summary>
        public TrackData TrackInfo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the end of the PCM data buffer has been reached.
        /// </summary>
        public bool Ended => isFailed || (isReady && pos >= data.Length);

        /// <summary>
        /// Gets the total duration of the audio in seconds.
        /// </summary>
        public double TotalDuration => isReady && data != null ? (double)data.Length / VoiceChatSettings.SampleRate : 0.0;

        /// <summary>
        /// Gets or sets the current playback position in seconds.
        /// </summary>
        public double CurrentTime
        {
            get => isReady ? (double)pos / VoiceChatSettings.SampleRate : 0.0;
            set => Seek(value);
        }

        /// <summary>
        /// Gets a value indicating whether the source failed to load.
        /// </summary>
        public bool IsFailed => isFailed;

        /// <summary>
        /// Reads a sequence of PCM samples from the preloaded buffer into the specified array.
        /// </summary>
        /// <param name="buffer">The destination array to copy the samples into.</param>
        /// <param name="offset">The zero-based index in <paramref name="buffer"/> at which to begin storing the data.</param>
        /// <param name="count">The maximum number of samples to read.</param>
        /// <returns>The number of samples read into <paramref name="buffer"/>.</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            if (isFailed)
                return 0;

            if (!isReady || data == null)
            {
                Array.Clear(buffer, offset, count);
                return count;
            }

            int read = Math.Min(count, data.Length - pos);
            Array.Copy(data, pos, buffer, offset, read);
            pos += read;

            return read;
        }

        /// <summary>
        /// Seeks to the specified position in seconds.
        /// </summary>
        /// <param name="seconds">The target position in seconds.</param>
        public void Seek(double seconds)
        {
            if (!isReady || data == null)
                return;

            long targetIndex = (long)(seconds * VoiceChatSettings.SampleRate);
            pos = (int)Math.Max(0, Math.Min(targetIndex, data.Length));
        }

        /// <summary>
        /// Resets the read position to the beginning of the PCM data buffer.
        /// </summary>
        public void Reset()
        {
            pos = 0;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}