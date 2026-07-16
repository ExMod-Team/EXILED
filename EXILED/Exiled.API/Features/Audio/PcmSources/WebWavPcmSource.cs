// -----------------------------------------------------------------------
// <copyright file="WebWavPcmSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.PcmSources
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Exiled.API.Features;
    using Exiled.API.Interfaces.Audio;
    using Exiled.API.Structs.Audio;

    using MEC;

    using UnityEngine.Networking;

    /// <summary>
    /// Provides a <see cref="IPcmSource"/> that downloads a .wav file from a URL and preloads it for playback.
    /// </summary>
    public sealed class WebWavPcmSource : IPcmSource, IAsyncPcmSource
    {
        private IPcmSource internalSource;
        private CancellationTokenSource cts;
        private CoroutineHandle downloadRoutine;

        private volatile bool isReady;
        private volatile bool isFailed;
        private volatile bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebWavPcmSource"/> class.
        /// </summary>
        /// <param name="url">The direct URL to the .wav file.</param>
        public WebWavPcmSource(string url)
        {
            TrackInfo = default;
            cts = new CancellationTokenSource();
            downloadRoutine = Timing.RunCoroutine(Download(url));
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
            if (isReady)
                internalSource?.Seek(seconds);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            if (isReady)
                internalSource?.Reset();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            isDisposed = true;
            isReady = false;

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

        private IEnumerator<float> Download(string url)
        {
            CancellationToken token = cts.Token;
            byte[] rawBytes = null;

            UnityWebRequest request;
            try
            {
                request = UnityWebRequest.Get(url);
            }
            catch (Exception ex)
            {
                Log.Error($"[WebWavPcmSource] Failed to create request. URL: {url} | Error: {ex.Message}");
                isFailed = true;
                yield break;
            }

            using (request)
            {
                yield return Timing.WaitUntilDone(request.SendWebRequest());

                if (isDisposed || token.IsCancellationRequested)
                    yield break;

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Log.Error($"[WebWavPcmSource] Failed to download! URL: {url} | Error: {request.error}");
                    isFailed = true;
                    yield break;
                }

                rawBytes = request.downloadHandler.data;
            }

            if (isDisposed || token.IsCancellationRequested)
                yield break;

            Task<AudioData> toPcmTask = Task.Run(() => WavUtility.WavToPcm(rawBytes), token);

            yield return Timing.WaitUntilTrue(() => toPcmTask.IsCompleted || token.IsCancellationRequested);

            if (isDisposed || token.IsCancellationRequested)
                yield break;

            if (toPcmTask.IsFaulted)
            {
                Log.Error($"[WebWavPcmSource] Failed to parse WAV. Ensure URL points to a valid 16-bit mono 48kHz WAV.\nError: {toPcmTask.Exception?.InnerException?.Message ?? toPcmTask.Exception?.Message}");
                isFailed = true;
                yield break;
            }

            AudioData audioData = toPcmTask.Result;
            audioData.TrackInfo.Path = url;

            try
            {
                internalSource = new PreloadedPcmSource(audioData.Pcm);
                TrackInfo = audioData.TrackInfo;
                isReady = true;
            }
            catch (Exception ex)
            {
                Log.Error($"[WebWavPcmSource] Failed to create internal source.\nError: {ex.InnerException?.Message ?? ex.Message}");
                isFailed = true;
            }
        }
    }
}