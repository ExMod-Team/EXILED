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
    using MapGeneration;

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
        public ZoneChangedEventArgs(ReferenceHub player, RoomIdentifier oldRoom, RoomIdentifier newRoom)
        {
            Player = Player.Get(player);
            OldRoom = Room.Get(oldRoom);
            NewRoom = Room.Get(newRoom);
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the previous room the player was in.
        /// </summary>
        public Room OldRoom { get; }

        /// <summary>
        /// Gets the new room the player entered.
        /// </summary>
        public Room NewRoom { get; }

        /// <summary>
        /// Gets the previous zone the player was in.
        /// </summary>
        public ZoneType OldZone => OldRoom.Zone;

        /// <summary>
        /// Gets the new zone the player entered.
        /// </summary>
        public ZoneType NewZone => NewRoom.Zone;
    }
}
