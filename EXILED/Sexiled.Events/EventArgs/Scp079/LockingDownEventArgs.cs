// -----------------------------------------------------------------------
// <copyright file="LockingDownEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.EventArgs.Scp079
{
    using Sexiled.API.Features;
    using Sexiled.API.Features.Roles;
    using Sexiled.Events.EventArgs.Interfaces;

    using MapGeneration;

    /// <summary>
    /// Contains all information before SCP-079 lockdowns a room.
    /// </summary>
    public class LockingDownEventArgs : IScp079Event, IRoomEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockingDownEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="roomIdentifier">
        /// <inheritdoc cref="Room" />
        /// </param>
        /// <param name="auxiliaryPowerCost">
        /// <inheritdoc cref="AuxiliaryPowerCost" />
        /// </param>
        public LockingDownEventArgs(Player player, RoomIdentifier roomIdentifier, float auxiliaryPowerCost)
        {
            Player = player;
            Scp079 = player.Role.As<Scp079Role>();
            Room = Room.Get(roomIdentifier);
            AuxiliaryPowerCost = auxiliaryPowerCost;
            IsAllowed = auxiliaryPowerCost <= Scp079.Energy;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp079Role Scp079 { get; }

        /// <summary>
        /// Gets the <see cref="RoomIdentifier" /> of the room that will be locked down.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets or sets the amount of auxiliary power required to lockdown a room.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-079 can lockdown a room.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}