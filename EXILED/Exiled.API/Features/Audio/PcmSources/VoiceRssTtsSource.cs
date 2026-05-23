// -----------------------------------------------------------------------
// <copyright file="VoiceRssTtsSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.PcmSources
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
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

        private static readonly ConcurrentDictionary<string, DateTime> BlacklistKeys = new();

        private IPcmSource internalSource;
        private CancellationTokenSource cts;
        private CoroutineHandle downloadRoutine;

        private volatile bool isReady;
        private volatile bool isFailed;
        private volatile bool isDisposed;

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

            cts = new CancellationTokenSource();
            TrackInfo = new TrackData { Path = $"VoiceRssTts: {text}", Duration = 0.0 };
            downloadRoutine = Timing.RunCoroutine(DownloadRoutine(text, apiKeys, language, voice, rate));
        }

        /// <inheritdoc/>
        public TrackData TrackInfo { get; private set; }

        /// <inheritdoc/>
        public double TotalDuration => isReady && internalSource != null ? internalSource.TotalDuration : 0.0;

        /// <inheritdoc/>
        public double CurrentTime
        {
            get => isReady && internalSource != null ? internalSource.CurrentTime : 0.0;
            set => Seek(value);
        }

        /// <inheritdoc/>
        public bool Ended => isFailed || isDisposed || (isReady && internalSource != null && internalSource.Ended);

        /// <inheritdoc/>
        public bool IsFailed => isFailed;

        /// <inheritdoc/>
        public bool IsReady => isReady;

        /// <inheritdoc/>
        public int Read(float[] buffer, int offset, int count)
        {
            if (isFailed || isDisposed)
                return 0;

            if (!isReady || internalSource == null)
            {
                Array.Clear(buffer, offset, count);
                return count;
            }

            return internalSource.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public void Seek(double seconds)
        {
            if (isReady && internalSource != null)
                internalSource.Seek(seconds);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            if (isReady && internalSource != null)
                internalSource.Reset();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            isReady = false;
            isDisposed = true;

            if (downloadRoutine.IsRunning)
                downloadRoutine.IsRunning = false;

            CancellationTokenSource localCts = Interlocked.Exchange(ref cts, null);
            if (localCts != null)
            {
                localCts.Cancel();
                localCts.Dispose();
            }

            internalSource?.Dispose();
            internalSource = null;
        }

        private IEnumerator<float> DownloadRoutine(string text, IEnumerable<string> apiKeys, string language, string voice, int rate)
        {
            CancellationToken token = cts.Token;
            string clampedRate = Math.Clamp(rate, -10, 10).ToString();
            string textEscaped = Uri.EscapeDataString(text);
            string langEscaped = Uri.EscapeDataString(language);
            string voiceEscaped = string.IsNullOrEmpty(voice) ? string.Empty : $"&v={Uri.EscapeDataString(voice)}";

            bool successfulDownload = false;
            byte[] rawBytes = null;

            foreach (string apiKey in apiKeys)
            {
                if (string.IsNullOrWhiteSpace(apiKey))
                    continue;

                if (BlacklistKeys.TryGetValue(apiKey, out DateTime exhaustedAt))
                {
                    if (DateTime.UtcNow.Day == exhaustedAt.Day)
                        continue;

                    BlacklistKeys.Remove(apiKey, out _);
                }

                string url = $"{ApiEndpoint}?key={Uri.EscapeDataString(apiKey)}&hl={langEscaped}&c=WAV&f={AudioFormat}&r={clampedRate}&src={textEscaped}{voiceEscaped}";

                UnityWebRequest request;
                try
                {
                    request = UnityWebRequest.Get(url);
                }
                catch (Exception ex)
                {
                    Log.Error($"[VoiceRssTtsSource] Failed to create request for URL '{url}'. Error: {ex.Message}");
                    break;
                }

                using (request)
                {
                    yield return Timing.WaitUntilDone(request.SendWebRequest());

                    if (isDisposed || token.IsCancellationRequested)
                        yield break;

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Log.Error($"[VoiceRssTtsSource] Network error: {request.error}");
                        break;
                    }

                    string responseText = request.downloadHandler.text;
                    if (!string.IsNullOrEmpty(responseText) && responseText.StartsWith("ERROR: "))
                    {
                        string errorMessage = responseText[7..].Trim();

                        if (errorMessage.Contains("limit") || errorMessage.Contains("expired") || errorMessage.Contains("inactive") || errorMessage.Contains("API key"))
                        {
                            Log.Warn($"[VoiceRssTtsSource] Key exhausted: '{apiKey}'. Error: {errorMessage}. Trying next key...");
                            BlacklistKeys[apiKey] = DateTime.UtcNow;
                            continue;
                        }

                        Log.Error($"[VoiceRssTtsSource] API error: {errorMessage}");
                        break;
                    }

                    rawBytes = request.downloadHandler.data;
                    successfulDownload = true;
                    break;
                }
            }

            if (!successfulDownload)
            {
                isFailed = true;
                yield break;
            }

            if (isDisposed || token.IsCancellationRequested)
                yield break;

            Task<AudioData> toPcmTask = Task.Run(() => WavUtility.WavToPcm(rawBytes), token);

            yield return Timing.WaitUntilTrue(() => toPcmTask.IsCompleted || isDisposed || token.IsCancellationRequested);

            if (isDisposed || token.IsCancellationRequested)
                yield break;

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