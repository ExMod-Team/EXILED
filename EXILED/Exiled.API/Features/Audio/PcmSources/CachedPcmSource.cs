// -----------------------------------------------------------------------
// <copyright file="CachedPcmSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.PcmSources
{
    using System;
    using System.IO;

    using Exiled.API.Features.Audio;
    using Exiled.API.Interfaces.Audio;
    using Exiled.API.Structs.Audio;

    using VoiceChat;

    /// <summary>
    /// Provides an <see cref="IPcmSource"/> that plays audio data directly from the <see cref="AudioDataStorage"/> for optimized, repeated playback.
    /// </summary>
    public sealed class CachedPcmSource : IPcmSource
    {
        private readonly float[] data;
        private int pos;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedPcmSource"/> class by fetching already cached audio using its name.
        /// </summary>
        /// <param name="name">The name/key of the audio in the cache.</param>
        public CachedPcmSource(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Log.Error("[CachedPcmSource] Cannot initialize CachedPcmSource. Cache name cannot be null or empty.");
                throw new ArgumentException("Cache name cannot be null or empty.", nameof(name));
            }

            if (!AudioDataStorage.AudioStorage.TryGetValue(name, out AudioData cachedAudio))
            {
                Log.Error($"[CachedPcmSource] Audio with name '{name}' not found in AudioDataStorage.");
                throw new FileNotFoundException($"Audio '{name}' is not cached. Please cache it first using AudioDataStorage");
            }

            data = cachedAudio.Pcm;
            TrackInfo = cachedAudio.TrackInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedPcmSource"/> class by fetching cached audio or injecting raw PCM samples into the cache if not present.
        /// </summary>
        /// <param name="name">The custom name/key to assign to this audio in the cache.</param>
        /// <param name="pcm">The raw PCM audio samples (float array).</param>
        public CachedPcmSource(string name, float[] pcm)
        {
            if (string.IsNullOrEmpty(name) || pcm == null || pcm.Length == 0)
            {
                Log.Error($"[CachedPcmSource] Cannot initialize CachedPcmSource. Invalid name or empty PCM data for '{name}'.");
                throw new ArgumentException("Name or PCM data cannot be null.");
            }

            if (!AudioDataStorage.AudioStorage.ContainsKey(name))
            {
                if (!AudioDataStorage.Add(name, pcm))
                {
                    Log.Error($"[CachedPcmSource] Failed to load raw PCM data into cache under the name '{name}'.");
                    throw new InvalidOperationException($"Failed to cache PCM data for '{name}'.");
                }
            }

            if (!AudioDataStorage.AudioStorage.TryGetValue(name, out AudioData cachedAudio))
            {
                Log.Error($"[CachedPcmSource] Audio with name '{name}' could not be retrieved from storage after adding.");
                throw new InvalidOperationException($"Failed to retrieve '{name}' from storage after caching.");
            }

            data = cachedAudio.Pcm;
            TrackInfo = cachedAudio.TrackInfo;
        }

        /// <inheritdoc/>
        public TrackData TrackInfo { get; }

        /// <inheritdoc/>
        public bool Ended => pos >= data.Length;

        /// <inheritdoc/>
        public double TotalDuration => (double)data.Length / VoiceChatSettings.SampleRate;

        /// <inheritdoc/>
        public double CurrentTime
        {
            get => (double)pos / VoiceChatSettings.SampleRate;
            set => Seek(value);
        }

        /// <inheritdoc/>
        public int Read(float[] buffer, int offset, int count)
        {
            int read = Math.Min(count, data.Length - pos);
            Array.Copy(data, pos, buffer, offset, read);
            pos += read;

            return read;
        }

        /// <inheritdoc/>
        public void Seek(double seconds)
        {
            long targetIndex = (long)(seconds * VoiceChatSettings.SampleRate);
            pos = (int)Math.Max(0, Math.Min(targetIndex, data.Length));
        }

        /// <inheritdoc/>
        public void Reset() => pos = 0;

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}