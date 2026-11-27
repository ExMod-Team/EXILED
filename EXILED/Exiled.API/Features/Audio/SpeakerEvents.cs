// -----------------------------------------------------------------------
// <copyright file="SpeakerEvents.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio
{
    using System;

    using Exiled.API.Structs.Audio;

    using Toys;

    /// <summary>
    /// Contains global event handlers related to the <see cref="Speaker"/> audio system.
    /// </summary>
    public static class SpeakerEvents
    {
        /// <summary>
        /// Invoked when a speaker starts playing an audio track.
        /// </summary>
        public static event Action<Speaker> PlaybackStarted;

        /// <summary>
        /// Invoked when the audio playback of a speaker is paused.
        /// </summary>
        public static event Action<Speaker> PlaybackPaused;

        /// <summary>
        /// Invoked when the audio playback of a speaker is resumed from a paused state.
        /// </summary>
        public static event Action<Speaker> PlaybackResumed;

        /// <summary>
        /// Invoked when the audio playback of a speaker loops back to the beginning.
        /// </summary>
        public static event Action<Speaker> PlaybackLooped;

        /// <summary>
        /// Invoked just before the speaker switches to the next track in the queue.
        /// </summary>
        public static event Action<Speaker, QueuedTrack> TrackSwitching;

        /// <summary>
        /// Invoked when a speaker finishes playing its current audio track.
        /// </summary>
        public static event Action<Speaker> PlaybackFinished;

        /// <summary>
        /// Invoked when a speaker's audio playback is completely stopped.
        /// </summary>
        public static event Action<Speaker> PlaybackStopped;

        /// <summary>
        /// Called when a speaker starts playing an audio track.
        /// </summary>
        /// <param name="speaker">The <see cref="Speaker"/> instance.</param>
        internal static void OnPlaybackStarted(Speaker speaker) => PlaybackStarted?.Invoke(speaker);

        /// <summary>
        /// Called when the audio playback of a speaker is paused.
        /// </summary>
        /// <param name="speaker">The <see cref="Speaker"/> instance.</param>
        internal static void OnPlaybackPaused(Speaker speaker) => PlaybackPaused?.Invoke(speaker);

        /// <summary>
        /// Called when the audio playback of a speaker is resumed from a paused state.
        /// </summary>
        /// <param name="speaker">The <see cref="Speaker"/> instance.</param>
        internal static void OnPlaybackResumed(Speaker speaker) => PlaybackResumed?.Invoke(speaker);

        /// <summary>
        /// Called when the audio playback of a speaker loops back to the beginning.
        /// </summary>
        /// <param name="speaker">The <see cref="Speaker"/> instance.</param>
        internal static void OnPlaybackLooped(Speaker speaker) => PlaybackLooped?.Invoke(speaker);

        /// <summary>
        /// Called just before the speaker switches to the next track in the queue.
        /// </summary>
        /// <param name="speaker">The <see cref="Speaker"/> instance.</param>
        /// <param name="nextTrack">The upcoming <see cref="QueuedTrack"/> to be played.</param>
        internal static void OnTrackSwitching(Speaker speaker, QueuedTrack nextTrack) => TrackSwitching?.Invoke(speaker, nextTrack);

        /// <summary>
        /// Called when a speaker finishes playing its current audio track.
        /// </summary>
        /// <param name="speaker">The <see cref="Speaker"/> instance.</param>
        internal static void OnPlaybackFinished(Speaker speaker) => PlaybackFinished?.Invoke(speaker);

        /// <summary>
        /// Called when a speaker's audio playback is completely stopped.
        /// </summary>
        /// <param name="speaker">The <see cref="Speaker"/> instance.</param>
        internal static void OnPlaybackStopped(Speaker speaker) => PlaybackStopped?.Invoke(speaker);
    }
}