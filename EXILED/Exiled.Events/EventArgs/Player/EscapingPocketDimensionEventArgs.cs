// -----------------------------------------------------------------------
// <copyright file="EscapingPocketDimensionEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    using UnityEngine;

    /// <summary>
    /// Contains all information before a player escapes the pocket dimension.
    /// </summary>
    public class EscapingPocketDimensionEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapingPocketDimensionEventArgs" /> class.
        /// </summary>
        /// <param name="pocketDimensionTeleport">
        /// <inheritdoc cref="Teleporter" />
        /// </param>
        /// <param name="hub">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="position">
        /// <inheritdoc cref="TeleportPosition" />
        /// </param>
        public EscapingPocketDimensionEventArgs(PocketDimensionTeleport pocketDimensionTeleport, ReferenceHub hub, Vector3 position)
        {
            Teleporter = pocketDimensionTeleport;
            Player = Player.Get(hub);
            TeleportPosition = position;
        }

        /// <summary>
        /// Gets the PocketDimensionTeleport the player walked into.
        /// </summary>
        public PocketDimensionTeleport Teleporter { get; }

        /// <summary>
        /// Gets the player who's escaping the pocket dimension.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the position in which the player is going to be teleported to.
        /// </summary>
        public Vector3 TeleportPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can successfully escape the pocket dimension.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}