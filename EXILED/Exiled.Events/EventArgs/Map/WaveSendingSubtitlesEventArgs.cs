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
        public WaveSendingSubtitlesEventArgs(WaveAnnouncementBase announcement)
        {
            Wave = TimedWave.GetTimedWaves().Find(x => x.Announcement == announcement);
        }

        /// <summary>
        /// Gets the entering wave.
        /// </summary>
        public TimedWave Wave { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;

    }
}