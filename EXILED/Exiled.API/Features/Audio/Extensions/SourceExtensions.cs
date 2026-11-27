// -----------------------------------------------------------------------
// <copyright file="SourceExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Audio.PcmSources;
    using Exiled.API.Features.Toys;
    using Exiled.API.Interfaces.Audio;

    /// <summary>
    /// Provides extension methods for playing audio for pre-made sources on Speaker instances.
    /// </summary>
    public static class SourceExtensions
    {
        /// <summary>
        /// Plays the live voice of a specific player through this speaker.
        /// </summary>
        /// <param name="speaker">The speaker through which to play the audio.</param>
        /// <param name="player">The player whose voice will be broadcasted.</param>
        /// <param name="blockOriginalVoice">If <c>true</c>, prevents the player's original voice message's from being heard while broadcasting.</param>
        /// <param name="clearQueue">If <c>true</c>, clears the upcoming tracks in the playlist before starting playback.</param>
        /// <returns><c>true</c> if the playback started successfully; otherwise, <c>false</c>.</returns>
        public static bool PlayFromPlayer(this Speaker speaker, Player player, bool blockOriginalVoice = false, bool clearQueue = true)
        {
            if (player == null)
            {
                Log.Error("[Speaker] Source player cannot be null when streaming live microphone!");
                return false;
            }

            PlayerVoiceSource source;
            try
            {
                source = new PlayerVoiceSource(player, blockOriginalVoice);
            }
            catch (Exception ex)
            {
                Log.Error($"[Speaker] Failed to initialize live voice stream for player '{player.Nickname}' ({player.Id}).\nException Details: {ex}");
                return false;
            }

            return speaker.Play(source, clearQueue);
        }

        /// <summary>
        /// Plays the specified text as speech through this speaker using the VoiceRss TTS service.
        /// </summary>
        /// <param name="speaker">The speaker that will play the generated speech.</param>
        /// <param name="text"> The text to convert to speech.(Length limited by 100KB).</param>
        /// <param name="apiKeys"> Your VoiceRSS API keys. Get a free key at <see href="https://www.voicerss.org/registration.aspx"/>.</param>
        /// <param name="language"> The language and locale code for the TTS voice. See <see href="https://www.voicerss.org/api/"/> for all supported language codes.</param>
        /// <param name="voice"> Optional specific voice name for the selected language.(See <see href="https://www.voicerss.org/api/"/> for available voices per language.)</param>
        /// <param name="rate"> Speech rate from -10 (slowest) to 10 (fastest). Defaults to 0 (normal speed).</param>
        /// <param name="clearQueue">If <c>true</c>, clears the upcoming tracks in the playlist before starting playback.</param>
        /// <returns><c>true</c> if the TTS playback started successfully; otherwise, <c>false</c>.</returns>
        public static bool PlayTts(this Speaker speaker, string text, IEnumerable<string> apiKeys, string language = "en-us", string voice = null, int rate = 0, bool clearQueue = true)
        {
            VoiceRssTtsSource ttsSource;
            try
            {
                ttsSource = new(text, apiKeys, language, voice, rate);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to create TTS source: {ex.Message}");
                return false;
            }

            return speaker.Play(ttsSource, clearQueue);
        }

        /// <summary>
        /// Plays multiple <see cref="IPcmSource"/> instances mixed together.
        /// </summary>
        /// <param name="speaker">The speaker through which to play the audio.</param>
        /// <param name="sources">The collection of PCM sources to mix and play.</param>
        /// <param name="clearQueue">If <c>true</c>, clears the upcoming tracks in the playlist before starting playback.</param>
        /// <returns><c>true</c> if at least one source was successfully mixed; otherwise, <c>false</c>.</returns>
        public static bool PlayMixed(this Speaker speaker, IEnumerable<IPcmSource> sources, bool clearQueue = true)
        {
            if (sources == null || !sources.Any())
            {
                Log.Error("[Speaker] No sources provided for PlayMixed!");
                return false;
            }

            if (clearQueue)
                speaker.TrackQueue.Clear();

            bool anyAdded = false;

            foreach (IPcmSource source in sources)
            {
                if (source == null)
                    continue;

                if (speaker.AddMixed(source))
                    anyAdded = true;
            }

            return anyAdded;
        }

        /// <summary>
        /// Dynamically mixes a new audio source into the currently playing audio without interrupting it.
        /// </summary>
        /// <param name="speaker">The speaker through which to play the audio.</param>
        /// <param name="extraSource">The additional <see cref="IPcmSource"/> to mix with the current playback.</param>
        /// <returns><c>true</c> if the source was successfully mixed or started; otherwise, <c>false</c>.</returns>
        public static bool AddMixed(this Speaker speaker, IPcmSource extraSource)
        {
            if (extraSource == null)
            {
                Log.Error("[Speaker] Provided extra IPcmSource for mixing is null!");
                return false;
            }

            if (extraSource is ILiveSource)
                speaker.Pitch = 1.0f;

            IPcmSource currentSource = speaker.CurrentSource;

            if ((!speaker.IsPlaying && !speaker.IsPaused) || currentSource == null || currentSource.Ended)
                return speaker.Play(extraSource, false);

            if (currentSource is MixerSource currentMixer)
            {
                currentMixer.AddSource(extraSource);
                return true;
            }

            try
            {
                IPcmSource oldSource = currentSource;
                MixerSource newMixer = new([oldSource, extraSource]);
                speaker.CurrentSource = newMixer;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[Speaker] Failed to transition to MixerSource on the fly!\nException Details: {ex}");
                return false;
            }
        }
    }
}