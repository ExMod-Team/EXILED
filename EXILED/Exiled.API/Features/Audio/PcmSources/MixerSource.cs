// -----------------------------------------------------------------------
// <copyright file="MixerSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.PcmSources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Interfaces.Audio;
    using Exiled.API.Structs.Audio;

    using UnityEngine;

    /// <summary>
    /// Provides an <see cref="IPcmSource"/> that dynamically mixes multiple audio sources together in real-time.
    /// <para>
    /// This allows playing overlapping sounds (e.g., background music + voice announcements) simultaneously
    /// through a single speaker without needing multiple Voice Controller IDs.
    /// </para>
    /// </summary>
    public sealed class MixerSource : IPcmSource
    {
        private readonly List<IPcmSource> sources = new();
        private float[] tempBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MixerSource"/> class with the specified initial sources.
        /// </summary>
        /// <param name="initialSources">An array of <see cref="IPcmSource"/> instances to mix.</param>
        public MixerSource(IEnumerable<IPcmSource> initialSources)
        {
            if (initialSources != null)
                sources.AddRange(initialSources.Where(s => s != null));

            TrackInfo = new TrackData { Path = "Audio Mixer", Duration = 0 };
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mixer should stay alive and output silence even when all internal sources have finished playing.
        /// </summary>
        public bool KeepAlive { get; set; } = false;

        /// <summary>
        /// Gets the metadata of the mixer track.
        /// </summary>
        public TrackData TrackInfo { get; }

        /// <summary>
        /// Gets the maximum total duration of all active sources in the mixer, in seconds.
        /// </summary>
        public double TotalDuration => sources.Count > 0 ? sources.Max(x => x.TotalDuration) : 0.0;

        /// <summary>
        /// Gets or sets the current playback position in seconds across all active sources.
        /// </summary>
        public double CurrentTime
        {
            get => sources.Count > 0 ? sources.Max(x => x.CurrentTime) : 0.0;
            set => Seek(value);
        }

        /// <summary>
        /// Gets a value indicating whether all internal sources have ended and <see cref="KeepAlive"/> is set to false.
        /// </summary>
        public bool Ended => !KeepAlive && (sources.Count == 0 || sources.All(x => x.Ended));

        /// <summary>
        /// Reads a sequence of mixed PCM samples from all active sources into the specified buffer.
        /// </summary>
        /// <param name="buffer">The destination buffer to fill with mixed PCM data.</param>
        /// <param name="offset">The zero-based index in <paramref name="buffer"/> at which to begin writing.</param>
        /// <param name="count">The maximum number of samples to read and mix.</param>
        /// <returns>The number of samples written to the <paramref name="buffer"/>.</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            if (tempBuffer == null || tempBuffer.Length < count)
                tempBuffer = new float[count];

            Array.Clear(buffer, offset, count);
            int maxRead = 0;

            for (int i = sources.Count - 1; i >= 0; i--)
            {
                IPcmSource src = sources[i];

                if (src.Ended)
                {
                    src.Dispose();
                    sources.RemoveAt(i);
                    continue;
                }

                int read = src.Read(tempBuffer, 0, count);
                if (read > maxRead)
                    maxRead = read;

                for (int j = 0; j < read; j++)
                    buffer[offset + j] += tempBuffer[j];
            }

            for (int i = 0; i < maxRead; i++)
                buffer[offset + i] = Mathf.Clamp(buffer[offset + i], -1f, 1f);

            return KeepAlive ? count : maxRead;
        }

        /// <summary>
        /// Seeks to the specified position in seconds for all active sources in the mixer.
        /// </summary>
        /// <param name="seconds">The target position in seconds.</param>
        public void Seek(double seconds)
        {
            foreach (IPcmSource pcmSource in sources)
                pcmSource.Seek(seconds);
        }

        /// <summary>
        /// Resets the playback position to the start for all active sources in the mixer.
        /// </summary>
        public void Reset()
        {
            foreach (IPcmSource pcmSource in sources)
                pcmSource.Reset();
        }

        /// <summary>
        /// Releases all resources used by the <see cref="MixerSource"/> and automatically disposes of all internal sources.
        /// </summary>
        public void Dispose()
        {
            foreach (IPcmSource pcmSource in sources)
                pcmSource?.Dispose();

            sources.Clear();
        }

        /// <summary>
        /// Dynamically adds a new <see cref="IPcmSource"/> to the mixer while it is playing.
        /// </summary>
        /// <param name="source">The audio source to add.</param>
        public void AddSource(IPcmSource source)
        {
            if (source != null)
                sources.Add(source);
        }

        /// <summary>
        /// Dynamically removes an existing <see cref="IPcmSource"/> from the mixer.
        /// </summary>
        /// <param name="source">The audio source to remove.</param>
        /// <param name="dispose">If <c>true</c>, automatically calls Dispose on the removed source.</param>
        public void RemoveSource(IPcmSource source, bool dispose = true)
        {
            if (source == null)
                return;

            if (dispose)
                source.Dispose();

            sources.Remove(source);
        }
    }
}