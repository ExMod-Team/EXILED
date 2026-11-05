// -----------------------------------------------------------------------
// <copyright file="WavExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1129 // Do not use default value type constructor
namespace Exiled.API.Features.Audio.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Audio.PcmSources;
    using Exiled.API.Features.Toys;
    using Exiled.API.Interfaces.Audio;
    using Exiled.API.Structs.Audio;

    /// <summary>
    /// Provides methods to play 16-bit, mono, 48 kHz WAV files or web streams via Speaker.
    /// </summary>
    public static class WavExtensions
    {
        /// <summary>
        /// Plays a local wav file or web URL through this speaker. (File must be 16-bit, mono, and 48kHz.)
        /// </summary>
        /// <param name="speaker">The speaker through which to play the audio.</param>
        /// <param name="path">The path/url or custom name(if <paramref name="useCache"/> is true) to the wav file.</param>
        /// <param name="clearQueue">If <c>true</c>, clears the upcoming tracks in the playlist before starting playback.</param>
        /// <param name="stream">If <c>true</c>, the file will be streamed from disk when played; otherwise, it will be loaded into memory (Ignored for web URLs).</param>
        /// <param name="useCache">If <c>true</c>, loads the audio via <see cref="CachedPcmSource"/> for optimized playback.</param>
        /// <returns><c>true</c> if the audio file was successfully found, loaded, and playback started; otherwise, <c>false</c>.</returns>
        public static bool PlayWav(this Speaker speaker, string path, bool clearQueue = true, bool stream = false, bool useCache = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.Error("[Speaker] Provided path/url or name cannot be null or empty!");
                return false;
            }

            if (!useCache && !WavUtility.TryValidatePath(path, out string errorMessage))
            {
                Log.Error($"[Speaker] {errorMessage}");
                return false;
            }

            IPcmSource newSource;
            try
            {
                newSource = WavUtility.CreatePcmSource(path, stream, useCache);
            }
            catch (Exception ex)
            {
                Log.Error($"[Speaker] Failed to initialize audio source for file at path: '{path}'.\nException Details: {ex}");
                return false;
            }

            return speaker.Play(newSource, clearQueue);
        }

        /// <summary>
        /// Converts provided paths/URLs to sources and plays them mixed together.
        /// </summary>
        /// <param name="speaker">The speaker through which to play the audio.</param>
        /// <param name="paths">The collection of paths or URLs to the audio files.</param>
        /// <param name="clearQueue">If <c>true</c>, clears the upcoming tracks in the playlist before starting playback.</param>
        /// <param name="stream">If <c>true</c>, streams local files from disk. (Ignored for web URLs).</param>
        /// <param name="useCache">If <c>true</c>, utilizes <see cref="CachedPcmSource"/> for the sources.</param>
        /// <returns><c>true</c> if at least one valid path was loaded and started; otherwise, <c>false</c>.</returns>
        public static bool PlayMixedWav(this Speaker speaker, IEnumerable<string> paths, bool clearQueue = true, bool stream = false, bool useCache = false)
        {
            if (paths == null || !paths.Any())
            {
                Log.Error("[Speaker] No paths provided for PlayMixedWav!");
                return false;
            }

            List<IPcmSource> createdSources = new();

            foreach (string path in paths)
            {
                if (string.IsNullOrEmpty(path))
                {
                    Log.Warn("[Speaker] One of the provided paths for PlayMixedWav is null or empty. Skipping this entry.");
                    continue;
                }

                if (!useCache && !WavUtility.TryValidatePath(path, out string error))
                {
                    Log.Error($"[Speaker] Skipping invalid path in mix: {path}. Reason: {error}");
                    continue;
                }

                try
                {
                    IPcmSource source = WavUtility.CreatePcmSource(path, stream, useCache);
                    if (source != null)
                        createdSources.Add(source);
                }
                catch (Exception ex)
                {
                    Log.Error($"[Speaker] Failed to create source for mix from '{path}': {ex.Message}");
                }
            }

            if (createdSources.Count == 0)
                return false;

            return speaker.PlayMixed(createdSources, clearQueue);
        }

        /// <summary>
        /// Helper method to easily queue a .wav file/url with stream support.
        /// </summary>
        /// <param name="speaker">The speaker through which to queue the track.</param>
        /// <param name="name">An optional name or identifier for this track in the queue. This is only used for reference.</param>
        /// <param name="path">The path/url or custom name(if <paramref name="useCache"/> is true) to the wav file.</param>
        /// <param name="isStream">If <c>true</c>, the file will be streamed from disk when played; otherwise, it will be loaded into memory (Ignored for web URLs).</param>
        /// <param name="useCache">If <c>true</c>, loads the audio via <see cref="CachedPcmSource"/> for optimized playback.</param>
        /// <returns><c>true</c> if successfully queued or started.</returns>
        public static bool QueueWavTrack(this Speaker speaker, string name, string path, bool isStream = false, bool useCache = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.Error("[Speaker] Provided path or cache name cannot be null or empty!");
                return false;
            }

            if (!useCache && !WavUtility.TryValidatePath(path, out string errorMessage))
            {
                Log.Error($"[Speaker] {errorMessage}");
                return false;
            }

            return speaker.QueueTrack(new QueuedTrack(name, () => WavUtility.CreatePcmSource(path, isStream, useCache)));
        }
    }
}