// -----------------------------------------------------------------------
// <copyright file="PlacingMimicPointEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;
    using RelativePositioning;

    /// <summary>
    /// Contains all information before mimicry point is placed.
    /// </summary>
    public class PlacingMimicPointEventArgs : IScp939Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacingMimicPointEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PlacingMimicPointEventArgs(Player player, RelativePosition position, bool isAllowed = true)
        {
            Player = player;
            Scp939 = player.Role.As<Scp939Role>();
            Position = position;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets a position of mimicry point.
        /// </summary>
        public RelativePosition Position { get; set; }
    }
}