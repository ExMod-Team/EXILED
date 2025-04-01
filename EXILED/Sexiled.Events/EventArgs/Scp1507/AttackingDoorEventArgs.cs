// -----------------------------------------------------------------------
// <copyright file="AttackingDoorEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.EventArgs.Scp1507
{
    using System;

    using Sexiled.API.Features;
    using Sexiled.API.Features.Doors;
    using Sexiled.API.Features.Roles;
    using Sexiled.Events.EventArgs.Interfaces;
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Contains all information before SCP-1507 attacks door.
    /// </summary>
    [Obsolete("Only availaible for Christmas and AprilFools.")]
    public class AttackingDoorEventArgs : IScp1507Event, IDeniableEvent, IDoorEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackingDoorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="doorVariant"><inheritdoc cref="Door"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AttackingDoorEventArgs(Player player, DoorVariant doorVariant, bool isAllowed = true)
        {
            Player = player;
            Scp1507 = player.Role.As<Scp1507Role>();
            Door = Door.Get(doorVariant);
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp1507Role Scp1507 { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Door Door { get; }
    }
}