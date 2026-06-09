// -----------------------------------------------------------------------
// <copyright file="PreloadedPcmSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.PcmSources
{
    using System;
    using System.IO;
    using System.Threading;
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
        private readonly object trackInfoLock = new();

        private int pos;
        private float[] data;
        private CancellationTokenSource cts;

        private volatile bool isReady;
        private volatile bool isFailed;
        private volatile bool isDisposed;

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
        /// Initializes a new instance of the <see cref="PreloadedPcmSource"/> class.
        /// </summary>
        /// <param name="path">The path to the audio file.</param>
        public PreloadedPcmSource(string path)
        {
            TrackInfo = new TrackData { Path = path, Duration = 0.0 };

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Task.Run(() => ReadAudio(path), token);
        }

        /// <inheritdoc/>
        public TrackData TrackInfo
        {
            get
            {
                lock (trackInfoLock)
                    return field;
            }

            private set
            {
                lock (trackInfoLock)
                    field = value;
            }
        }

        /// <inheritdoc/>
        public bool Ended => isFailed || isDisposed || (isReady && data != null && pos >= data.Length);

        /// <inheritdoc/>
        public double TotalDuration => isReady && data != null ? (double)data.Length / VoiceChatSettings.SampleRate : 0.0;

        /// <inheritdoc/>
        public double CurrentTime
        {
            get => isReady && data != null ? (double)pos / VoiceChatSettings.SampleRate : 0.0;
            set => Seek(value);
        }

        /// <inheritdoc/>
        public bool IsFailed => isFailed;

        /// <inheritdoc/>
        public bool IsReady => isReady;

        /// <inheritdoc/>
        public int Read(float[] buffer, int offset, int count)
        {
            if (isFailed || isDisposed)
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

        /// <inheritdoc/>
        public void Seek(double seconds)
        {
            if (!isReady || data == null || data.Length == 0)
                return;

            pos = (int)Math.Clamp(seconds * VoiceChatSettings.SampleRate, 0, data.Length);
        }

        /// <inheritdoc/>
        public void Reset() => pos = 0;

        /// <inheritdoc/>
        public void Dispose()
        {
            isDisposed = true;
            isReady = false;

            CancellationTokenSource localCts = Interlocked.Exchange(ref cts, null);
            if (localCts != null)
            {
                localCts.Cancel();
                localCts.Dispose();
            }
        }

        private void ReadAudio(string path)
        {
            try
            {
                AudioData result = WavUtility.WavToPcm(path);
                data = result.Pcm;
                TrackInfo = result.TrackInfo;
                isReady = true;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Log.Error($"[PreloadedPcmSource] Failed to load audio from path: {path} | Error: {ex.Message}");
                isFailed = true;
            }
        }
    }
}