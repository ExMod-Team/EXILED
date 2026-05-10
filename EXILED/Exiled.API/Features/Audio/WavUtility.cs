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

    using Exiled.API.Features.Audio.PcmSources;
    using Exiled.API.Interfaces.Audio;
    using Exiled.API.Structs.Audio;

    using VoiceChat;

    /// <summary>
    /// Provides utility methods for working with WAV audio files.
    /// </summary>
    public static class WavUtility
    {
        private const float Divide = 1f / 32768f;

        /// <summary>
        /// Evaluates the given local path or URL and returns the appropriate <see cref="IPcmSource"/> for .wav playback.
        /// </summary>
        /// <param name="path">The local file path or web URL of the .wav file.</param>
        /// <param name="stream">If <c>true</c>, streams local files directly from disk. If <c>false</c>, preloads them into memory (Ignored for web URLs).</param>
        /// <param name="cache">If <c>true</c>, loads the audio via <see cref="CachedPcmSource"/> for zero-latency memory playback.</param>
        /// <returns>An initialized <see cref="IPcmSource"/>.</returns>
        public static IPcmSource CreatePcmSource(string path, bool stream = false, bool cache = false)
        {
            if (cache)
                return new CachedPcmSource(path, path);

            if (path.StartsWith("http"))
                return new WebWavPcmSource(path);

            if (stream)
                return new WavStreamSource(path);

            return new PreloadedPcmSource(path);
        }

        /// <summary>
        /// Converts a WAV file at the specified path to a PCM float array.
        /// </summary>
        /// <param name="path">The file path of the WAV file to convert.</param>
        /// <returns>A <see cref="AudioData"/> containing an array of floats representing the PCM data and its TrackData.</returns>
        public static AudioData WavToPcm(string path)
        {
            if (!File.Exists(path))
            {
                Log.Error($"[WavUtility] The specified local file does not exist, path: `{path}`");
                throw new FileNotFoundException("File does not exist");
            }

            if (!path.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                Log.Error($"[WavUtility] The file type '{Path.GetExtension(path)}' is not supported for wav utility. Please use .wav file.");
                throw new InvalidDataException("Unsupported WAV format.");
            }

            using FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            int length = (int)fs.Length;

            byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(length);

            try
            {
                int bytesRead = fs.Read(rentedBuffer, 0, length);
                using MemoryStream ms = new(rentedBuffer, 0, bytesRead);

                AudioData result = ParseWavSpanToPcm(ms, rentedBuffer.AsSpan(0, bytesRead));
                result.TrackInfo.Path = path;

                return result;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rentedBuffer);
            }
        }

        /// <summary>
        /// Converts a WAV byte array to a PCM float array.
        /// </summary>
        /// <param name="data">The raw bytes of the WAV file.</param>
        /// <returns>A <see cref="AudioData"/> containing an array of floats representing the PCM data and its TrackData.</returns>
        public static AudioData WavToPcm(byte[] data)
        {
            using MemoryStream ms = new(data, 0, data.Length);

            return ParseWavSpanToPcm(ms, data.AsSpan());
        }

        /// <summary>
        /// Parses the WAV header from the provided stream and converts the remaining audio data span into a PCM float array.
        /// </summary>
        /// <param name="stream">The stream used to read and skip the WAV header.</param>
        /// <param name="audioData">The complete span of WAV audio data including the header.</param>
        /// <returns>A tuple containing an array of floats representing the PCM data and its TrackData.</returns>
        public static AudioData ParseWavSpanToPcm(Stream stream, ReadOnlySpan<byte> audioData)
        {
            TrackData metaData = SkipHeader(stream);

            int headerOffset = (int)stream.Position;
            int dataLength = audioData.Length - headerOffset;

            ReadOnlySpan<short> samples = MemoryMarshal.Cast<byte, short>(audioData.Slice(headerOffset, dataLength));

            float[] pcm = new float[samples.Length];

            for (int i = 0; i < samples.Length; i++)
                pcm[i] = samples[i] * Divide;

            return new(pcm, metaData);
        }

        /// <summary>
        /// Skips the WAV file header and validates that the format is PCM16 mono with the specified sample rate.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read from.</param>
        /// <returns>A <see cref="TrackData"/> struct containing the parsed file information.</returns>
        public static TrackData SkipHeader(Stream stream)
        {
            TrackData trackData = new();

            if (stream.Length < 12)
            {
                Log.Error("[WavUtility] WAV file is too short to contain a valid header.");
                throw new InvalidDataException("WAV file is too short to contain a valid header.");
            }

            Span<byte> headerBuffer = stackalloc byte[12];
            stream.Read(headerBuffer);

            int rate = 0;
            int bits = 0;
            int channels = 0;

            Span<byte> chunkHeader = stackalloc byte[8];
            while (true)
            {
                if (stream.Position + 8 > stream.Length)
                {
                    Log.Error("[WavUtility] WAV file ended prematurely while parsing chunks.");
                    throw new InvalidDataException("WAV file ended prematurely while parsing chunks.");
                }

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
                    channels = BinaryPrimitives.ReadInt16LittleEndian(fmtData.Slice(2, 2));
                    rate = BinaryPrimitives.ReadInt32LittleEndian(fmtData.Slice(4, 4));
                    bits = BinaryPrimitives.ReadInt16LittleEndian(fmtData.Slice(14, 2));

                    if (format != 1 || channels != 1 || rate != VoiceChatSettings.SampleRate || bits != 16)
                    {
                        Log.Error($"[WavUtility] Invalid WAV format (format={format}, channels={channels}, rate={rate}, bits={bits}). Expected PCM16, mono and {VoiceChatSettings.SampleRate}Hz.");
                        throw new InvalidDataException("Unsupported WAV format.");
                    }

                    if (chunkSize > 16)
                        stream.Seek(chunkSize - 16, SeekOrigin.Current);
                }

                // 'LIST' chunk
                else if (chunkId == 0x5453494C)
                {
                    Span<byte> listType = stackalloc byte[4];
                    stream.Read(listType);
                    uint type = BinaryPrimitives.ReadUInt32LittleEndian(listType);

                    // 'INFO' chunk
                    if (type == 0x4F464E49)
                    {
                        int bytesToRead = chunkSize - 4;
                        byte[] infoBytes = ArrayPool<byte>.Shared.Rent(bytesToRead);
                        stream.Read(infoBytes, 0, bytesToRead);

                        int offset = 0;
                        while (offset < bytesToRead - 8)
                        {
                            uint infoId = BinaryPrimitives.ReadUInt32LittleEndian(infoBytes.AsSpan(offset, 4));
                            int infoSize = BinaryPrimitives.ReadInt32LittleEndian(infoBytes.AsSpan(offset + 4, 4));
                            offset += 8;

                            if (infoSize > 0 && offset + infoSize <= bytesToRead)
                            {
                                string value = System.Text.Encoding.UTF8.GetString(infoBytes, offset, infoSize).TrimEnd('\0');

                                if (infoId == 0x4D414E49)
                                    trackData.Title = value;
                                else if (infoId == 0x54524149)
                                    trackData.Artist = value;
                            }

                            offset += infoSize;
                            if (infoSize % 2 != 0)
                                offset++;
                        }

                        ArrayPool<byte>.Shared.Return(infoBytes);
                    }
                    else
                    {
                        stream.Seek(chunkSize - 4, SeekOrigin.Current);
                    }
                }

                // 'data' chunk
                else if (chunkId == 0x61746164)
                {
                    int bytesPerSample = bits / 8;
                    if (bytesPerSample > 0 && channels > 0 && rate > 0)
                        trackData.Duration = (double)chunkSize / (rate * channels * bytesPerSample);

                    return trackData;
                }
                else
                {
                    stream.Seek(chunkSize, SeekOrigin.Current);
                }

                if (stream.Position >= stream.Length)
                {
                    Log.Error("[WavUtility] WAV file does not contain a 'data' chunk.");
                    throw new InvalidDataException("Missing 'data' chunk in WAV file.");
                }
            }

            return trackData;
        }

        /// <summary>
        /// Validates a given local file path or web URL to ensure it is suitable for WAV processing.
        /// </summary>
        /// <param name="path">The local file path or web URL to validate.</param>
        /// <param name="errorMessage">Outputs a specific error message explaining why the validation failed. Returns <see cref="string.Empty"/> if successful.</param>
        /// <returns><c>true</c> if the path is valid and safe to process; otherwise, <c>false</c>.</returns>
        public static bool TryValidatePath(string path, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                errorMessage = "Provided path or URL cannot be null or empty!";
                return false;
            }

            if (path.StartsWith("http"))
                return true;

            if (!File.Exists(path))
            {
                errorMessage = $"The specified local file does not exist. Path: `{path}`";
                return false;
            }

            if (!path.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = $"Unsupported file format! Only .wav files are allowed. Path: `{path}`";
                return false;
            }

            return true;
        }
    }
}