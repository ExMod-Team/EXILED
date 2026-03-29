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
    /// Contains all information before Chaos wave entrance.
    /// </summary>
    public class AnnouncingChaosEntranceEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncingChaosEntranceEventArgs"/> class.
        /// </summary>
        /// <param name="announcement"><inheritdoc cref="Wave"/></param>
        /// <param name="builder"><inheritdoc cref="Words"/></param>
        public AnnouncingChaosEntranceEventArgs(WaveAnnouncementBase announcement, StringBuilder builder)
        {
            Wave = TimedWave.GetTimedWaves().Find(x => x.Announcement == announcement);
            Words = builder;
        }

        /// <summary>
        /// Gets the entering wave.
        /// </summary>
        public TimedWave Wave { get; }

        /// <summary>
        /// Gets the <see cref="StringBuilder"/> of the words that C.A.S.S.I.E will say.
        /// <remarks>It doesn't affect the subtitle part that will be sent to the client.</remarks>
        /// </summary>
        public StringBuilder Words { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;
    }
}