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

        /// <summary>
        /// Gets the metadata of the streaming track.
        /// </summary>
        public TrackData TrackInfo { get; }

        /// <summary>
        /// Gets the total duration of the audio in seconds.
        /// </summary>
        public double TotalDuration => (endPosition - startPosition) / 2.0 / VoiceChatSettings.SampleRate;

        /// <summary>
        /// Gets or sets the current playback position in seconds.
        /// </summary>
        public double CurrentTime
        {
            get => (stream.Position - startPosition) / 2.0 / VoiceChatSettings.SampleRate;
            set => Seek(value);
        }

        /// <summary>
        /// Gets a value indicating whether the end of the stream has been reached.
        /// </summary>
        public bool Ended => stream.Position >= endPosition;

        /// <summary>
        /// Reads PCM data from the stream into the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to fill with PCM data.</param>
        /// <param name="offset">The offset in the buffer at which to begin writing.</param>
        /// <param name="count">The maximum number of samples to read.</param>
        /// <returns>The number of samples read.</returns>
        public int Read(float[] buffer, int offset, int count)
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

        /// <summary>
        /// Seeks to the specified position in the stream.
        /// </summary>
        /// <param name="seconds">The position in seconds to seek to.</param>
        public void Seek(double seconds)
        {
            long newPos = Math.Clamp(startPosition + ((long)(seconds * VoiceChatSettings.SampleRate) * 2), startPosition, endPosition);

            if (newPos % 2 != 0)
                newPos--;

            stream.Position = newPos;
        }

        /// <summary>
        /// Resets the stream position to the start.
        /// </summary>
        public void Reset()
        {
            stream.Position = startPosition;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="WavStreamSource"/>.
        /// </summary>
        public void Dispose()
        {
            stream?.Dispose();
            if (internalBuffer != null)
            {
                ArrayPool<byte>.Shared.Return(internalBuffer);
                internalBuffer = null;
            }
        }
    }
}