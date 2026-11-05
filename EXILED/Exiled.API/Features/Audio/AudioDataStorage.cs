// -----------------------------------------------------------------------
// <copyright file="AudioDataStorage.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio
{
    using System.Collections.Generic;

    using Exiled.API.Structs.Audio;

    using RoundRestarting;

    /// <summary>
    /// Manages a global in-memory storage of decoded PCM audio data. Once stored, audio can be played using <see cref="PcmSources.CachedPcmSource"/>.
    /// </summary>
    public static class AudioDataStorage
    {
        static AudioDataStorage()
        {
            AudioStorage = new();
            RoundRestart.OnRestartTriggered += OnRoundRestart;
        }

        /// <summary>
        /// Gets the underlying storage, keyed by name.
        /// </summary>
        public static Dictionary<string, AudioData> AudioStorage { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the storage is automatically cleared when a round restart is triggered.
        /// </summary>
        public static bool ClearOnRoundRestart { get; set; } = true;

        /// <summary>
        /// Stores raw PCM audio samples under the specified name.
        /// </summary>
        /// <param name="name">The unique storage key to assign.</param>
        /// <param name="pcm">The raw PCM float array to store.</param>
        /// <returns><c>true</c> if successfully added; otherwise, <c>false</c>.</returns>
        public static bool Add(string name, float[] pcm)
        {
            if (pcm == null)
            {
                Log.Error($"[AudioDataStorage] Cannot store null array for key '{name}'.");
                return false;
            }

            TrackData trackInfo = new()
            {
                Title = name,
                Duration = (double)pcm.Length / VoiceChat.VoiceChatSettings.SampleRate,
            };

            return Add(name, new AudioData(pcm, trackInfo));
        }

        /// <summary>
        /// Stores a fully constructed <see cref="AudioData"/> under the specified name.
        /// </summary>
        /// <param name="name">The unique storage key to assign.</param>
        /// <param name="audioData">The <see cref="AudioData"/> to store.</param>
        /// <returns><c>true</c> if successfully added; otherwise, <c>false</c>.</returns>
        public static bool Add(string name, AudioData audioData)
        {
            if (!ValidateName(name))
                return false;

            if (audioData.Pcm == null || audioData.Pcm.Length == 0)
            {
                Log.Error($"[AudioDataStorage] AudioData for key '{name}' has null or empty PCM.");
                return false;
            }

            if (AudioStorage.ContainsKey(name))
            {
                Log.Warn($"[AudioDataStorage] An entry with the key '{name}' already exists. Skipping add.");
                return false;
            }

            return AudioStorage.TryAdd(name, audioData);
        }

        /// <summary>
        /// Removes a stored audio entry by name.
        /// </summary>
        /// <param name="name">The storage name/key to remove.</param>
        /// <returns><c>true</c> if the entry was found and removed; otherwise, <c>false</c>.</returns>
        public static bool Remove(string name) => AudioStorage.Remove(name, out _);

        /// <summary>
        /// Clears all entries from the audio storage, freeing all associated memory.
        /// </summary>
        public static void Clear() => AudioStorage.Clear();

        /// <summary>
        /// Validates that the storage name (key) is valid.
        /// </summary>
        /// <param name="name">The storage name (key) to validate.</param>
        /// <returns>True when name is valid; otherwise false.</returns>
        internal static bool ValidateName(string name)
        {
            if (!string.IsNullOrEmpty(name))
                return true;

            Log.Error("[AudioDataStorage] Storage name (key) cannot be null or empty.");
            return false;
        }

        private static void OnRoundRestart()
        {
            if (ClearOnRoundRestart)
                Clear();
        }
    }
}