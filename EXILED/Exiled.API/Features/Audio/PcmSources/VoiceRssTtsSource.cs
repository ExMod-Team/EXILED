// -----------------------------------------------------------------------
// <copyright file="VoiceRssTtsSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.PcmSources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Exiled.API.Features;
    using Exiled.API.Interfaces.Audio;
    using Exiled.API.Structs.Audio;

    using MEC;

    using UnityEngine.Networking;

    /// <summary>
    /// Provides a <see cref="IPcmSource"/> that converts text to speech using the <see href="https://www.voicerss.org/">VoiceRSS</see> Text-to-Speech API.
    /// </summary>
    public sealed class VoiceRssTtsSource : IPcmSource, IAsyncPcmSource
    {
        private const string ApiEndpoint = "https://api.voicerss.org/";
        private const string AudioFormat = "48khz_16bit_mono";

        private static readonly Dictionary<string, DateTime> BlacklistKeys = new();

        private IPcmSource internalSource;
        private UnityWebRequest webRequest;
        private CoroutineHandle downloadRoutine;

        private volatile bool isReady = false;
        private volatile bool isFailed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceRssTtsSource"/> class.
        /// </summary>
        /// <param name="text"> The text to convert to speech.(Length limited by 100KB).</param>
        /// <param name="apiKey"> Your VoiceRSS API key. Get a free key at <see href="https://www.voicerss.org/registration.aspx"/>.</param>
        /// <param name="language"> The language and locale code for the TTS voice. See <see href="https://www.voicerss.org/api/"/> for all supported language codes.</param>
        /// <param name="voice"> Optional specific voice name for the selected language.(See <see href="https://www.voicerss.org/api/"/> for available voices per language.)</param>
        /// <param name="rate"> Speech rate from -10 (slowest) to 10 (fastest). Defaults to 0 (normal speed).</param>
        public VoiceRssTtsSource(string text, string apiKey, string language = "en-us", string voice = null, int rate = 0)
            : this(text, new[] { apiKey }, language, voice, rate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceRssTtsSource"/> class.
        /// </summary>
        /// <param name="text"> The text to convert to speech.(Length limited by 100KB).</param>
        /// <param name="apiKeys"> Your VoiceRSS API keys. Get a free key at <see href="https://www.voicerss.org/registration.aspx"/>.</param>
        /// <param name="language"> The language and locale code for the TTS voice. See <see href="https://www.voicerss.org/api/"/> for all supported language codes.</param>
        /// <param name="voice"> Optional specific voice name for the selected language.(See <see href="https://www.voicerss.org/api/"/> for available voices per language.)</param>
        /// <param name="rate"> Speech rate from -10 (slowest) to 10 (fastest). Defaults to 0 (normal speed).</param>
        public VoiceRssTtsSource(string text, IEnumerable<string> apiKeys, string language = "en-us", string voice = null, int rate = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                isFailed = true;
                Log.Error("[VoiceRssTtsSource] Text cannot be null or empty.");
                throw new ArgumentException("Text cannot be null or empty.", nameof(text));
            }

            if (apiKeys == null || !apiKeys.Any())
            {
                isFailed = true;
                Log.Error("[VoiceRssTtsSource] At least one API key must be provided.");
                throw new ArgumentException("API key collection cannot be null or empty.", nameof(apiKeys));
            }

            TrackInfo = new TrackData { Path = $"VoiceRssTts: {text}", Duration = 0.0 };
            downloadRoutine = Timing.RunCoroutine(DownloadRoutine(text, apiKeys, language, voice, rate));
        }

        /// <summary>
        /// Gets the metadata of the loaded track.
        /// </summary>
        public TrackData TrackInfo { get; private set; }

        /// <summary>
        /// Gets the total duration of the audio in seconds. Returns 0 while the download is in progress.
        /// </summary>
        public double TotalDuration => isReady && internalSource != null ? internalSource.TotalDuration : 0.0;

        /// <summary>
        /// Gets or sets the current playback position in seconds.
        /// </summary>
        public double CurrentTime
        {
            get => isReady && internalSource != null ? internalSource.CurrentTime : 0.0;
            set => Seek(value);
        }

        /// <summary>
        /// Gets a value indicating whether playback has ended or the download has failed.
        /// </summary>
        public bool Ended => isFailed || (isReady && internalSource != null && internalSource.Ended);

        /// <summary>
        /// Gets a value indicating whether the source failed to load.
        /// </summary>
        public bool IsFailed => isFailed;

        /// <summary>
        /// Reads PCM data from the audio source into the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to fill with PCM data.</param>
        /// <param name="offset">The offset in the buffer at which to begin writing.</param>
        /// <param name="count">The maximum number of samples to read.</param>
        /// <returns>The number of samples read.</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            if (isFailed)
                return 0;

            if (!isReady || internalSource == null)
            {
                Array.Clear(buffer, offset, count);
                return count;
            }

            return internalSource.Read(buffer, offset, count);
        }

        /// <summary>
        /// Seeks to the specified position in seconds.
        /// </summary>
        /// <param name="seconds">The target position in seconds.</param>
        public void Seek(double seconds)
        {
            if (isReady && internalSource != null)
                internalSource.Seek(seconds);
        }

        /// <summary>
        /// Resets playback to the beginning.
        /// </summary>
        public void Reset()
        {
            if (isReady && internalSource != null)
                internalSource.Reset();
        }

        /// <summary>
        /// Releases all resources used by the <see cref="VoiceRssTtsSource"/>.
        /// </summary>
        public void Dispose()
        {
            if (downloadRoutine.IsRunning)
                downloadRoutine.IsRunning = false;

            webRequest?.Abort();
            webRequest?.Dispose();
            internalSource?.Dispose();
        }

        private IEnumerator<float> DownloadRoutine(string text, IEnumerable<string> apiKeys, string language, string voice, int rate)
        {
            string clampedRate = Math.Clamp(rate, -10, 10).ToString();
            string textEscaped = Uri.EscapeDataString(text);
            string langEscaped = Uri.EscapeDataString(language);
            string voiceEscaped = string.IsNullOrEmpty(voice) ? string.Empty : $"&v={Uri.EscapeDataString(voice)}";

            bool successfulDownload = false;

            foreach (string apiKey in apiKeys)
            {
                if (string.IsNullOrWhiteSpace(apiKey))
                    continue;

                if (BlacklistKeys.TryGetValue(apiKey, out DateTime exhaustedAt))
                {
                    if (DateTime.UtcNow.Day == exhaustedAt.Day)
                        continue;

                    BlacklistKeys.Remove(apiKey);
                }

                string url = $"{ApiEndpoint}?key={Uri.EscapeDataString(apiKey)}&hl={langEscaped}&c=WAV&f={AudioFormat}&r={clampedRate}&src={textEscaped}{voiceEscaped}";

                webRequest?.Dispose();
                try
                {
                    webRequest = UnityWebRequest.Get(url);
                }
                catch (Exception ex)
                {
                    Log.Error($"[VoiceRssTtsSource] Failed to get Url '{url}. Error: {ex.Message}");
                    break;
                }

                yield return Timing.WaitUntilDone(webRequest.SendWebRequest());

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Log.Error($"[VoiceRssTtsSource] Network Error: {webRequest.error}.");
                    break;
                }

                string responseText = webRequest.downloadHandler.text;
                if (!string.IsNullOrEmpty(responseText) && responseText.StartsWith("ERROR: "))
                {
                    string errorMessage = responseText[7..].Trim();

                    if (errorMessage.Contains("limit") || errorMessage.Contains("expired") || errorMessage.Contains("inactive") || errorMessage.Contains("API key"))
                    {
                        Log.Warn($"[VoiceRssTtsSource] Key issue, key: '{apiKey}', Error : {errorMessage}. Switching to another key...");
                        BlacklistKeys[apiKey] = DateTime.UtcNow;
                        continue;
                    }
                    else
                    {
                        Log.Error($"[VoiceRssTtsSource] API Error: {errorMessage}");
                        break;
                    }
                }

                successfulDownload = true;
                break;
            }

            if (!successfulDownload)
            {
                isFailed = true;
                webRequest?.Dispose();
                webRequest = null;
                yield break;
            }

            byte[] rawBytes = webRequest.downloadHandler.data;
            webRequest.Dispose();
            webRequest = null;

            Task<AudioData> toPcmTask = Task.Run(() => WavUtility.WavToPcm(rawBytes));

            yield return Timing.WaitUntilTrue(() => toPcmTask.IsCompleted);

            if (toPcmTask.IsFaulted)
            {
                Log.Error($"[VoiceRssTtsSource] Error read the downloaded file! \nError: {toPcmTask.Exception?.InnerException?.Message ?? toPcmTask.Exception?.Message}");
                isFailed = true;
                yield break;
            }

            AudioData audioData = toPcmTask.Result;
            audioData.TrackInfo.Path = $"VoiceRSS: {text}";

            try
            {
                internalSource = new PreloadedPcmSource(audioData.Pcm);
                TrackInfo = audioData.TrackInfo;
                isReady = true;
            }
            catch (Exception ex)
            {
                Log.Error($"[VoiceRssTtsSource] Failed to create internal source! \nError: {ex.InnerException?.Message ?? ex.Message}");
                isFailed = true;
            }
        }
    }
}