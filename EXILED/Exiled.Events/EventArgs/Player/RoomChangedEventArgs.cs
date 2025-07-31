// -----------------------------------------------------------------------
// <copyright file="RoomChangedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using MapGeneration;

    /// <summary>
    /// Contains the information when a player changes rooms.
    /// </summary>
    public class RoomChangedEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomChangedEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player whose room has changed.</param>
        /// <param name="oldRoom">The room identifier before the change (Can be null on round start).</param>
        /// <param name="newRoom">The room identifier after the change.</param>
        public RoomChangedEventArgs(ReferenceHub player, RoomIdentifier oldRoom, RoomIdentifier newRoom)
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
    }
}
