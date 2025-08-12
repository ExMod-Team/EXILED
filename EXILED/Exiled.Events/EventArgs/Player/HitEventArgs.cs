// -----------------------------------------------------------------------
// <copyright file="HitEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;
    using Interfaces;
    using PlayerRoles.PlayableScps.Subroutines;

    /// <summary>
    /// Contains all information after player sends an attack as an SCP.
    /// </summary>
    public class HitEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HitEventArgs"/> class.
        /// </summary>
        /// <param name="player"> <inheritdoc cref="Player"/></param>
        /// <param name="result"> the result of the attack.</param>
        /// <param name="playerHits"> the list of players who are getting hit.</param>
        public HitEventArgs(Player player, AttackResult result, HashSet<ReferenceHub> playerHits)
        {
            Player = player;
            Result = result;
            PlayersAffected = playerHits.Select(Player.Get).ToList().AsReadOnly();
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <summary>
        /// Gets the attack result for the server.
        /// </summary>
        public AttackResult Result { get; }

        /// <summary>
        /// Gets the attack result for the server.
        /// </summary>
        public IReadOnlyCollection<Player> PlayersAffected { get; }
    }
}