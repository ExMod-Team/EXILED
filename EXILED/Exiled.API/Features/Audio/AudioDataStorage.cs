// -----------------------------------------------------------------------
// <copyright file="AudioDataStorage.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;

    using Exiled.API.Structs.Audio;

    using MEC;

    using RoundRestarting;

    using UnityEngine.Networking;

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
        /// Loads and stores a local .wav file under the specified name.
        /// </summary>
        /// <param name="name">The unique storage key to assign to this audio.</param>
        /// <param name="path">The absolute path to the local .wav file.</param>
        /// <returns><c>true</c> if the file was successfully loaded and stored; otherwise, <c>false</c>.</returns>
        public static bool AddWav(string name, string path)
        {
            if (!ValidateName(name))
                return false;

            if (AudioStorage.ContainsKey(name))
            {
                Log.Warn($"[AudioDataStorage] An entry with the key '{name}' already exists. Skipping add.");
                return false;
            }

            if (path.StartsWith("http"))
            {
                Log.Error($"[AudioDataStorage] '{path}' is a URL. Use AudioDataStorage.AddUrl() for web sources.");
                return false;
            }

            if (!File.Exists(path))
            {
                Log.Error($"[AudioDataStorage] Local file not found: '{path}'");
                return false;
            }

            try
            {
                AudioData parsed = WavUtility.WavToPcm(path);
                return AudioStorage.TryAdd(name, parsed);
            }
            catch (Exception ex)
            {
                Log.Error($"[AudioDataStorage] Failed to load '{path}' into storage:\n{ex}");
                return false;
            }
        }

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
        /// Starts an asynchronous download of a .wav file from the specified URL and adds it to the  storage.
        /// </summary>
        /// <param name="name">The unique storage key to assign.</param>
        /// <param name="url">The HTTP or HTTPS URL pointing to a valid .wav file.</param>
        /// <returns>A <see cref="CoroutineHandle"/> for the running download coroutine.</returns>
        public static CoroutineHandle AddWavUrl(string name, string url) => Timing.RunCoroutine(AddUrlCoroutine(name, url));

        /// <summary>
        /// Starts an asynchronous download of a .wav file from the specified URL and adds it to the storage.
        /// </summary>
        /// <param name="name">The unique storage key to assign.</param>
        /// <param name="url">The HTTP or HTTPS URL pointing to a valid .wav file.</param>
        /// <returns>A MEC-compatible <see cref="IEnumerator{T}"/> of <see cref="float"/>.</returns>
        public static IEnumerator<float> AddUrlCoroutine(string name, string url)
        {
            if (!ValidateName(name))
                yield break;

            if (string.IsNullOrEmpty(url) || !url.StartsWith("http"))
            {
                Log.Error($"[AudioDataStorage] Invalid URL for key '{name}': '{url}'. Must start with http/https.");
                yield break;
            }

            if (AudioStorage.ContainsKey(name))
            {
                Log.Warn($"[AudioDataStorage] An entry with the key '{name}' already exists. Skipping download.");
                yield break;
            }

            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return Timing.WaitUntilDone(www.SendWebRequest());

            if (www.result != UnityWebRequest.Result.Success)
            {
                Log.Error($"[AudioDataStorage] Download failed for '{url}': {www.error}");
                yield break;
            }

            try
            {
                AudioData parsed = WavUtility.WavToPcm(www.downloadHandler.data);
                parsed.TrackInfo.Path = url;
                AudioStorage.TryAdd(name, parsed);
            }
            catch (Exception ex)
            {
                Log.Error($"[AudioDataStorage] Failed to parse downloaded WAV from '{url}':\n{ex}");
            }
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

        private static bool ValidateName(string name)
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
