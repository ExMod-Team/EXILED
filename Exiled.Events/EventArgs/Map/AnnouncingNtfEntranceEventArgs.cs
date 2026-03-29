// -----------------------------------------------------------------------
// <copyright file="AnnouncingNtfEntranceEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features.Waves;
    using Interfaces;
    using Respawning.Announcements;

    /// <summary>
    /// Contains all information before C.A.S.S.I.E announces the NTF entrance.
    /// </summary>
    public class AnnouncingNtfEntranceEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncingNtfEntranceEventArgs" /> class.
        /// </summary>
        /// <param name="announcement"><inheritdoc cref="Wave"/></param>
        /// <param name="scpsLeft"><inheritdoc cref="ScpsLeft"/></param>
        /// <param name="unitName"><inheritdoc cref="UnitName"/></param>
        /// <param name="unitNumber"><inheritdoc cref="UnitNumber"/></param>
        public AnnouncingNtfEntranceEventArgs(WaveAnnouncementBase announcement, int scpsLeft, string unitName, int unitNumber)
        {
            Wave = TimedWave.GetTimedWaves().Find(x => x.Announcement == announcement);
            ScpsLeft = scpsLeft;
            UnitName = unitName;
            UnitNumber = unitNumber;
        }

        /// <summary>
        /// Gets the entering wave.
        /// </summary>
        public TimedWave Wave { get; }

        /// <summary>
        /// Gets or sets the number of SCPs left.
        /// </summary>
        public int ScpsLeft { get; set; }

        /// <summary>
        /// Gets or sets the NTF unit name.
        /// </summary>
        /// <remarks>If <see cref="Wave"/> is NTF Mini wave, value is <c>null</c> and setter does not work.</remarks>
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets the NTF unit number.
        /// </summary>
        /// <remarks>If <see cref="Wave"/> is NTF Mini wave, value is <c>0</c> and setter does not work.</remarks>
        public int UnitNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the NTF spawn will be announced by C.A.S.S.I.E.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}