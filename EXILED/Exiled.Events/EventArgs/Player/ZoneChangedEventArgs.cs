// -----------------------------------------------------------------------
// <copyright file="ZoneChangedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains the information when a player changes zones.
    /// </summary>
    public class ZoneChangedEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneChangedEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player whose zone has changed.</param>
        /// <param name="oldRoom">The previous room the player was in.</param>
        /// <param name="newRoom">The new room the player entered.</param>
        /// <param name="oldZone">The previous zone the player was in.</param>
        /// <param name="newZone">The new zone the player entered.</param>
        public ZoneChangedEventArgs(Player player, Room oldRoom, Room newRoom, ZoneType oldZone, ZoneType newZone)
        {
            Player = player;
            OldRoom = oldRoom;
            NewRoom = newRoom;
            OldZone = oldZone;
            NewZone = newZone;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the previous zone the player was in.
        /// </summary>
        public ZoneType OldZone { get; }

        /// <summary>
        /// Gets the new zone the player entered.
        /// </summary>
        public ZoneType NewZone { get; }

        /// <summary>
        /// Gets the previous room the player was in.
        /// </summary>
        public Room OldRoom { get; }

        /// <summary>
        /// Gets the new room the player entered.
        /// </summary>
        public Room NewRoom { get; }
    }
}