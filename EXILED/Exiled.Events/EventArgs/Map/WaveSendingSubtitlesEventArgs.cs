// -----------------------------------------------------------------------
// <copyright file="AnnouncingChaosEntranceEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using System.Text;

    using Exiled.API.Features.Waves;
    using Exiled.Events.EventArgs.Interfaces;
    using Respawning.Announcements;

    /// <summary>
    /// Contains all information before any wave sending subtitles.
    /// </summary>
    public class WaveSendingSubtitlesEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaveSendingSubtitlesEventArgs"/> class.
        /// </summary>
        /// <param name="announcement"><inheritdoc cref="Wave"/></param>
        /// <param name="builder"><inheritdoc cref="Words"/></param>
        public WaveSendingSubtitlesEventArgs(WaveAnnouncementBase announcement, StringBuilder builder)
        {
            Wave = TimedWave.GetTimedWaves().Find(x => x.Announcement == announcement);
            Words = builder;
        }

        /// <summary>
        /// Gets the entering wave.
        /// </summary>
        public TimedWave Wave { get; }

        /// <summary>
        /// Gets the <see cref="StringBuilder"/> of the words that the client will be sent.
        /// <remarks>It doesn't affect the audio part that will be sent to the client. See Announcing events instead.</remarks>
        /// </summary>
        public StringBuilder Words { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;
        
        /// <summary>
        /// If isAllowed is true, run default sendSubtitles if runDefualtCode is true, otherwise, send custom subtitles.
        /// </summary>
        public bool runDefaultCode { get; set; } = true;
    }
}