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
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    using Exiled.API.Features.Audio.PcmSources;
    using Exiled.API.Features.Toys;
    using Exiled.API.Interfaces.Audio;
    using Exiled.API.Structs.Audio;

    using MEC;

    using UnityEngine;
    using UnityEngine.Networking;

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
            {
                if (!AudioDataStorage.AudioStorage.ContainsKey(path))
                    CacheWav(path, path);

                return new CachedPcmSource(path);
            }

            if (path.StartsWith("http"))
                return new WebWavPcmSource(path);

            if (stream)
                return new WavStreamSource(path);

            return new PreloadedPcmSource(path);
        }

        /// <summary>
        /// Rents a speaker from the pool, plays a local wav file or web stream one time, and automatically returns it to the pool afterwards. (File must be 16 bit, mono and 48khz.)
        /// </summary>
        /// <param name="path">The path/url or custom name/key (if <paramref name="settings"/> has <see cref="PlaybackSettings.UseCache"/> set to true) to the wav file.</param>
        /// <param name="parent">The parent transform, if any.</param>
        /// <param name="position">The local position of the speaker.</param>
        /// <param name="settings">The optional audio and network settings. If null, default settings are used.</param>
        /// <returns><c>true</c> if the audio file was successfully found, loaded, and playback started; otherwise, <c>false</c>.</returns>
        public static bool PlayWavFromPool(string path, Transform parent = null, Vector3? position = null, in PlaybackSettings? settings = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.Error("[Speaker] Provided path/url or name cannot be null or empty!");
                return false;
            }

            PlaybackSettings settingsFull = settings ?? new PlaybackSettings();

            if (!settingsFull.UseCache && !TryValidatePath(path, out string errorMessage))
            {
                Log.Error($"[Speaker] {errorMessage}");
                return false;
            }

            IPcmSource source;
            try
            {
                source = CreatePcmSource(path, settingsFull.Stream, settingsFull.UseCache);
            }
            catch (Exception ex)
            {
                Log.Error($"[Speaker] Failed to initialize audio source for PlayFromPool. Path: '{path}'.\n{ex}");
                return false;
            }

            return Speaker.PlayFromPool(source, parent, position, settingsFull);
        }

        /// <summary>
        /// Loads and stores a local .wav file under the specified name.
        /// </summary>
        /// <param name="name">The unique storage key to assign to this audio.</param>
        /// <param name="path">The absolute path to the local .wav file.</param>
        /// <returns><c>true</c> if the file was successfully loaded and stored; otherwise, <c>false</c>.</returns>
        public static bool CacheWav(string name, string path)
        {
            if (!AudioDataStorage.ValidateName(name))
                return false;

            if (AudioDataStorage.AudioStorage.ContainsKey(name))
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
                AudioData parsed = WavToPcm(path);
                return AudioDataStorage.AudioStorage.TryAdd(name, parsed);
            }
            catch (Exception ex)
            {
                Log.Error($"[AudioDataStorage] Failed to load '{path}' into storage:\n{ex}");
                return false;
            }
        }

        /// <summary>
        /// Starts an asynchronous download of a .wav file from the specified URL and adds it to the storage.
        /// </summary>
        /// <param name="name">The unique storage key to assign.</param>
        /// <param name="url">The HTTP or HTTPS URL pointing to a valid .wav file.</param>
        /// <returns>A <see cref="CoroutineHandle"/> for the running download coroutine.</returns>
        public static CoroutineHandle CacheWavUrl(string name, string url) => Timing.RunCoroutine(CacheUrlCoroutine(name, url));

        /// <summary>
        /// Starts an asynchronous download of a .wav file from the specified URL and adds it to the storage.
        /// </summary>
        /// <param name="name">The unique storage key to assign.</param>
        /// <param name="url">The HTTP or HTTPS URL pointing to a valid .wav file.</param>
        /// <returns>A MEC-compatible <see cref="IEnumerator{T}"/> of <see cref="float"/>.</returns>
        public static IEnumerator<float> CacheUrlCoroutine(string name, string url)
        {
            if (!AudioDataStorage.ValidateName(name))
                yield break;

            if (string.IsNullOrEmpty(url) || !url.StartsWith("http"))
            {
                Log.Error($"[AudioDataStorage] Invalid URL for key '{name}': '{url}'. Must start with http/https.");
                yield break;
            }

            if (AudioDataStorage.AudioStorage.ContainsKey(name))
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
                AudioData parsed = WavToPcm(www.downloadHandler.data);
                parsed.TrackInfo.Path = url;
                AudioDataStorage.AudioStorage.TryAdd(name, parsed);
            }
            catch (Exception ex)
            {
                Log.Error($"[AudioDataStorage] Failed to parse downloaded WAV from '{url}':\n{ex}");
            }
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
            TrackData trackData = default(TrackData);

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