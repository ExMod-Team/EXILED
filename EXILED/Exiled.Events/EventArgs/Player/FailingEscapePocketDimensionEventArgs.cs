// -----------------------------------------------------------------------
// <copyright file="FailingEscapePocketDimensionEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information before a player dies from walking through an incorrect exit in the pocket dimension.
    /// </summary>
    public class FailingEscapePocketDimensionEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailingEscapePocketDimensionEventArgs" /> class.
        /// </summary>
        /// <param name="pocketDimensionTeleport">
        /// <inheritdoc cref="Teleporter" />
        /// </param>
        /// <param name="hub">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public FailingEscapePocketDimensionEventArgs(PocketDimensionTeleport pocketDimensionTeleport, ReferenceHub hub, bool isAllowed = true)
        {
            Player = Player.Get(hub);
            Teleporter = pocketDimensionTeleport;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the PocketDimensionTeleport the player walked into.
        /// </summary>
        public PocketDimensionTeleport Teleporter { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player dies by failing the pocket dimension escape.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's escaping the pocket dimension.
        /// </summary>
        public Player Player { get; }
    }
}