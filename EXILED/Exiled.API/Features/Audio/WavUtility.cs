// -----------------------------------------------------------------------
// <copyright file="WavUtility.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio
{
    using System;
    using System.Buffers;
    using System.Buffers.Binary;
    using System.IO;
    using System.Runtime.InteropServices;

    using VoiceChat;

    /// <summary>
    /// Provides utility methods for working with WAV audio files.
    /// </summary>
    public static class WavUtility
    {
        private const float Divide = 1f / 32768f;

        /// <summary>
        /// Converts a WAV file at the specified path to a PCM float array.
        /// </summary>
        /// <param name="path">The file path of the WAV file to convert.</param>
        /// <returns>An array of floats representing the PCM data.</returns>
        public static float[] WavToPcm(string path)
        {
            using FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            int length = (int)fs.Length;

            byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(length);

            try
            {
                int bytesRead = fs.Read(rentedBuffer, 0, length);

                using MemoryStream ms = new(rentedBuffer, 0, bytesRead);

                SkipHeader(ms);

                int headerOffset = (int)ms.Position;
                int dataLength = bytesRead - headerOffset;

                Span<byte> audioDataSpan = rentedBuffer.AsSpan(headerOffset, dataLength);
                Span<short> samples = MemoryMarshal.Cast<byte, short>(audioDataSpan);

                float[] pcm = new float[samples.Length];

                for (int i = 0; i < samples.Length; i++)
                    pcm[i] = samples[i] * Divide;

                return pcm;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rentedBuffer);
            }
        }

        /// <summary>
        /// Skips the WAV file header and validates that the format is PCM16 mono with the specified sample rate.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read from.</param>
        public static void SkipHeader(Stream stream)
        {
            Span<byte> headerBuffer = stackalloc byte[12];
            stream.Read(headerBuffer);

            Span<byte> chunkHeader = stackalloc byte[8];
            while (true)
            {
                int read = stream.Read(chunkHeader);
                if (read < 8)
                    break;

                uint chunkId = BinaryPrimitives.ReadUInt32LittleEndian(chunkHeader[..4]);
                int chunkSize = BinaryPrimitives.ReadInt32LittleEndian(chunkHeader.Slice(4, 4));

                // 'fmt ' chunk
                if (chunkId == 0x20746D66)
                {
                    Span<byte> fmtData = stackalloc byte[16];
                    stream.Read(fmtData);

                    short format = BinaryPrimitives.ReadInt16LittleEndian(fmtData[..2]);
                    short channels = BinaryPrimitives.ReadInt16LittleEndian(fmtData.Slice(2, 2));
                    int rate = BinaryPrimitives.ReadInt32LittleEndian(fmtData.Slice(4, 4));
                    short bits = BinaryPrimitives.ReadInt16LittleEndian(fmtData.Slice(14, 2));

                    if (format != 1 || channels != 1 || rate != VoiceChatSettings.SampleRate || bits != 16)
                    {
                        Log.Error($"[Speaker] Invalid WAV format (format={format}, channels={channels}, rate={rate}, bits={bits}). Expected PCM16, mono and {VoiceChatSettings.SampleRate}Hz.");
                        throw new InvalidDataException("Unsupported WAV format.");
                    }

                    if (chunkSize > 16)
                        stream.Seek(chunkSize - 16, SeekOrigin.Current);
                }

                // 'data' chunk
                else if (chunkId == 0x61746164)
                {
                    return;
                }
                else
                {
                    stream.Seek(chunkSize, SeekOrigin.Current);
                }

                if (stream.Position >= stream.Length)
                    Log.Error("[Speaker] WAV file does not contain a 'data' chunk.");
            }
        }
    }
}