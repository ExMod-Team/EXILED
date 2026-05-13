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
        private UnityWebRequest webRequest;
        private CoroutineHandle downloadRoutine;

        private volatile bool isReady = false;
        private volatile bool isFailed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebWavPcmSource"/> class.
        /// </summary>
        /// <param name="url">The direct URL to the .wav file.</param>
        public WebWavPcmSource(string url)
        {
            TrackInfo = default;
            downloadRoutine = Timing.RunCoroutine(Download(url));
        }

        /// <summary>
        /// Gets the metadata of the preloaded track.
        /// </summary>
        public TrackData TrackInfo { get; private set; }

        /// <summary>
        /// Gets the total duration of the audio in seconds.
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
        /// Gets a value indicating whether the end of the playback has been reached.
        /// </summary>
        public bool Ended => isFailed || (isReady && internalSource != null && internalSource.Ended);

        /// <inheritdoc/>
        public bool IsFailed => isFailed;

        /// <inheritdoc/>
        public bool IsReady => isReady;

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
        /// Seeks to the specified position in the playback.
        /// </summary>
        /// <param name="seconds">The position in seconds to seek to.</param>
        public void Seek(double seconds)
        {
            if (isReady && internalSource != null)
                internalSource.CurrentTime = seconds;
        }

        /// <summary>
        /// Resets the playback position to the start.
        /// </summary>
        public void Reset()
        {
            if (isReady && internalSource != null)
                internalSource.Reset();
        }

        /// <summary>
        /// Releases all resources used by the <see cref="WebWavPcmSource"/>.
        /// </summary>
        public void Dispose()
        {
            if (downloadRoutine.IsRunning)
                downloadRoutine.IsRunning = false;

            webRequest?.Abort();
            webRequest?.Dispose();
            internalSource?.Dispose();
        }

        private IEnumerator<float> Download(string url)
        {
            try
            {
                webRequest = UnityWebRequest.Get(url);
            }
            catch (Exception ex)
            {
                Log.Error($"[WebWavPcmSource] Failed to download audio! URL: {url} | Error: {ex.Message}");
                isFailed = true;
                webRequest?.Dispose();
                webRequest = null;
                yield break;
            }

            yield return Timing.WaitUntilDone(webRequest.SendWebRequest());

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Log.Error($"[WebWavPcmSource] Failed to download audio! URL: {url} | Error: {webRequest.error}");
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
                Log.Error($"[WebPreloadWavPcmSource] Failed to read the downloaded file! Ensure the link points to a valid .WAV file. Error: {toPcmTask.Exception?.InnerException?.Message ?? toPcmTask.Exception?.Message}");
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
                Log.Error($"[WebPreloadWavPcmSource] Failed to read the downloaded file! Ensure the link points to a valid .WAV file. Error:  {ex.InnerException?.Message ?? ex.Message}");
                isFailed = true;
            }
        }
    }
}