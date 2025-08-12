// -----------------------------------------------------------------------
// <copyright file="RemovedObserverEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp173
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information after a player stops looking at SCP-173.
    /// </summary>
    public class RemovedObserverEventArgs : IScp173Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovedObserverEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Scp173" /></param>
        /// <param name="observer"><inheritdoc cref="Player" /></param>
        public RemovedObserverEventArgs(Player player, Player observer)
        {
            Scp173 = player.Role.As<Scp173Role>();
            Player = player;
            Observer = observer;
        }

        /// <inheritdoc />
        public Scp173Role Scp173 { get; }

        /// <summary>
        /// Gets the player who's controlling SCP-173.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the target who no longer sees SCP-173.
        /// </summary>
        public Player Observer { get; }
    }
}