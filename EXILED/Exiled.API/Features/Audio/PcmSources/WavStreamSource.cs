// -----------------------------------------------------------------------
// <copyright file="WavStreamSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.PcmSources
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Exiled.API.Features.Audio;
    using Exiled.API.Interfaces.Audio;
    using Exiled.API.Structs.Audio;

    using VoiceChat;

    /// <summary>
    /// Provides a <see cref="IPcmSource"/> from a WAV file stream.
    /// </summary>
    public sealed class WavStreamSource : IPcmSource
    {
        private const float Divide = 1f / 32768f;

        private readonly long endPosition;
        private readonly long startPosition;
        private readonly FileStream stream;

        private byte[] internalBuffer;
        private volatile bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WavStreamSource"/> class.
        /// </summary>
        /// <param name="path">The path to the audio file.</param>
        public WavStreamSource(string path)
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, FileOptions.SequentialScan);
            TrackInfo = WavUtility.SkipHeader(stream);
            startPosition = stream.Position;
            endPosition = stream.Length;
            internalBuffer = ArrayPool<byte>.Shared.Rent(VoiceChatSettings.PacketSizePerChannel * 2);
        }

        /// <inheritdoc/>
        public TrackData TrackInfo { get; }

        /// <inheritdoc/>
        public double TotalDuration => (endPosition - startPosition) / 2.0 / VoiceChatSettings.SampleRate;

        /// <inheritdoc/>
        public double CurrentTime
        {
            get => isDisposed ? 0.0 : (stream.Position - startPosition) / 2.0 / VoiceChatSettings.SampleRate;
            set => Seek(value);
        }

        /// <inheritdoc/>
        public bool Ended => isDisposed || stream.Position >= endPosition;

        /// <inheritdoc/>
        public int Read(float[] buffer, int offset, int count)
        {
            if (isDisposed)
            {
                Array.Clear(buffer, offset, count);
                return 0;
            }

            try
            {
                count = Math.Min(count, buffer.Length - offset);

                if (count <= 0)
                    return 0;

                int bytesNeeded = count * 2;

                if (internalBuffer.Length < bytesNeeded)
                {
                    ArrayPool<byte>.Shared.Return(internalBuffer);
                    internalBuffer = ArrayPool<byte>.Shared.Rent(bytesNeeded);
                }

                int bytesRead = stream.Read(internalBuffer, 0, bytesNeeded);

                if (bytesRead == 0)
                    return 0;

                if (bytesRead % 2 != 0)
                    bytesRead--;

                Span<byte> byteSpan = internalBuffer.AsSpan(0, bytesRead);
                Span<short> shortSpan = MemoryMarshal.Cast<byte, short>(byteSpan);

                for (int i = 0; i < shortSpan.Length; i++)
                    buffer[offset + i] = shortSpan[i] * Divide;

                return shortSpan.Length;
            }
            catch (ObjectDisposedException)
            {
                Array.Clear(buffer, offset, count);
                return 0;
            }
        }

        /// <inheritdoc/>
        public void Seek(double seconds)
        {
            if (isDisposed)
                return;

            long newPos = Math.Clamp(startPosition + ((long)(seconds * VoiceChatSettings.SampleRate) * 2), startPosition, endPosition);

            if (newPos % 2 != 0)
                newPos--;

            stream.Position = newPos;
        }

        /// <inheritdoc/>
        public void Reset() => stream.Position = startPosition;

        /// <inheritdoc/>
        public void Dispose()
        {
            isDisposed = true;
            stream?.Dispose();

            byte[] buf = Interlocked.Exchange(ref internalBuffer, null);
            if (buf != null)
                ArrayPool<byte>.Shared.Return(buf);
        }
    }
}