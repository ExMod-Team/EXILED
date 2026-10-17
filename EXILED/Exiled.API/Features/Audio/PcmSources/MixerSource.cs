// -----------------------------------------------------------------------
// <copyright file="MixerSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.PcmSources
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Pools;
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
        private readonly object sourcesLock = new();

        private volatile IPcmSource[] sources;

        /// <summary>
        /// Initializes a new instance of the <see cref="MixerSource"/> class with the specified initial sources.
        /// </summary>
        /// <param name="initialSources">An array of <see cref="IPcmSource"/> instances to mix.</param>
        public MixerSource(IEnumerable<IPcmSource> initialSources)
        {
            sources = initialSources?.Where(source => source != null).ToArray() ?? Array.Empty<IPcmSource>();
            TrackInfo = new TrackData { Path = "Audio Mixer", Duration = 0 };
        }

        /// <summary>
        /// Gets a value indicating whether this mixer contains any live audio sources.
        /// </summary>
        public bool ContainsLiveSource
        {
            get
            {
                IPcmSource[] currentSources = sources;
                return currentSources.Any(source => source is ILiveSource || (source is MixerSource mixer && mixer.ContainsLiveSource));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mixer should stay alive and output silence even when all internal sources have finished playing.
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <inheritdoc/>
        public TrackData TrackInfo { get; }

        /// <inheritdoc/>
        public double TotalDuration
        {
            get
            {
                IPcmSource[] currentSources = sources;
                return currentSources.Length > 0 ? currentSources.Max(x => x.TotalDuration) : 0.0;
            }
        }

        /// <inheritdoc/>
        public double CurrentTime
        {
            get
            {
                IPcmSource[] currentSources = sources;
                return currentSources.Length > 0 ? currentSources.Max(x => x.CurrentTime) : 0.0;
            }
            set => Seek(value);
        }

        /// <inheritdoc/>
        public bool Ended
        {
            get
            {
                IPcmSource[] currentSources = sources;
                return !KeepAlive && (currentSources.Length == 0 || currentSources.All(x => x.Ended));
            }
        }

        /// <inheritdoc/>
        public int Read(float[] buffer, int offset, int count)
        {
            IPcmSource[] currentSources = sources;

            if (currentSources.Length == 0)
            {
                Array.Clear(buffer, offset, count);
                return KeepAlive ? count : 0;
            }

            Array.Clear(buffer, offset, count);

            float[] temp = ArrayPool<float>.Shared.Rent(count);

            try
            {
                int maxRead = 0;
                bool needsCleanup = false;

                foreach (IPcmSource src in currentSources)
                {
                    if (src.Ended)
                    {
                        needsCleanup = true;
                        continue;
                    }

                    int read = src.Read(temp, 0, count);
                    if (read > maxRead)
                        maxRead = read;

                    for (int i = 0; i < read; i++)
                        buffer[offset + i] += temp[i];
                }

                for (int i = 0; i < maxRead; i++)
                    buffer[offset + i] = Mathf.Clamp(buffer[offset + i], -1f, 1f);

                if (needsCleanup)
                    CleanupEndedSources();

                return KeepAlive ? count : maxRead;
            }
            finally
            {
                ArrayPool<float>.Shared.Return(temp);
            }
        }

        /// <inheritdoc/>
        public void Seek(double seconds)
        {
            IPcmSource[] currentSources = sources;
            foreach (IPcmSource pcmSource in currentSources)
                pcmSource.Seek(seconds);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            IPcmSource[] currentSources = sources;
            foreach (IPcmSource pcmSource in currentSources)
                pcmSource.Reset();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (sourcesLock)
            {
                foreach (IPcmSource pcmSource in sources)
                    pcmSource?.Dispose();

                sources = Array.Empty<IPcmSource>();
            }
        }

        /// <summary>
        /// Dynamically adds a new <see cref="IPcmSource"/> to the mixer while it is playing.
        /// </summary>
        /// <param name="source">The audio source to add.</param>
        public void AddSource(IPcmSource source)
        {
            if (source == null)
                return;

            lock (sourcesLock)
            {
                List<IPcmSource> newList = ListPool<IPcmSource>.Pool.Get(sources);
                newList.Add(source);
                sources = newList.ToArray();
                ListPool<IPcmSource>.Pool.Return(newList);
            }
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

            lock (sourcesLock)
            {
                List<IPcmSource> newList = ListPool<IPcmSource>.Pool.Get(sources);
                if (newList.Remove(source))
                {
                    sources = newList.ToArray();

                    if (dispose)
                        source.Dispose();
                }

                ListPool<IPcmSource>.Pool.Return(newList);
            }
        }

        private void CleanupEndedSources()
        {
            lock (sourcesLock)
            {
                List<IPcmSource> currentSources = ListPool<IPcmSource>.Pool.Get(sources);
                bool changed = false;

                for (int i = currentSources.Count - 1; i >= 0; i--)
                {
                    IPcmSource source = currentSources[i];

                    if (!source.Ended)
                        continue;

                    source.Dispose();
                    currentSources.RemoveAt(i);

                    changed = true;
                }

                if (changed)
                    sources = currentSources.ToArray();

                ListPool<IPcmSource>.Pool.Return(currentSources);
            }
        }
    }
}