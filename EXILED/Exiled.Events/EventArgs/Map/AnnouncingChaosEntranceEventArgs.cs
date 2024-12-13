// -----------------------------------------------------------------------
// <copyright file="AnnouncingChaosEntranceEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features.Waves;
    using Exiled.Events.EventArgs.Interfaces;
    using Respawning.Announcements;

    /// <summary>
    /// Contains all information before Chaos wave entrance.
    /// </summary>
    public class AnnouncingChaosEntranceEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncingChaosEntranceEventArgs"/> class.
        /// </summary>
        /// <param name="announcement"><inheritdoc cref="Wave"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AnnouncingChaosEntranceEventArgs(WaveAnnouncementBase announcement, bool isAllowed = true)
        {
            Wave = TimedWave.GetTimedWaves().Find(x => x.Announcement == announcement);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the entering wave.
        /// </summary>
        public TimedWave Wave { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}