// -----------------------------------------------------------------------
// <copyright file="InteractingEmergencyButtonEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.Events.EventArgs.Interfaces;
    using Interactables.Interobjects;

    /// <summary>
    /// Contains all information before <see cref="Interactables.Interobjects.EmergencyDoorRelease"/> is pressed.
    /// </summary>
    public class InteractingEmergencyButtonEventArgs : IDoorEvent, IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingEmergencyButtonEventArgs"/> class.
        /// </summary>
        /// <param name="emergencyDoorRelease"><inheritdoc cref="EmergencyReleaseButton"/></param>
        /// <param name="door"><inheritdoc cref="Door"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InteractingEmergencyButtonEventArgs(EmergencyDoorRelease emergencyDoorRelease, Door door, Player player, bool isAllowed = true)
        {
            EmergencyReleaseButton = EmergencyReleaseButton.Get(emergencyDoorRelease);
            Door = door;
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Doors.EmergencyReleaseButton"/> that's being pressed.
        /// </summary>
        public EmergencyReleaseButton EmergencyReleaseButton { get; }

        /// <inheritdoc/>
        public Door Door { get; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}