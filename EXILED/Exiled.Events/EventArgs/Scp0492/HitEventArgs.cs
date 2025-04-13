// -----------------------------------------------------------------------
// <copyright file="HitEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp0492
{
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;
    using PlayerRoles.PlayableScps.Subroutines;

    /// <summary>
    /// Contains all information after zombie sends an attack.
    /// </summary>
    public class HitEventArgs : IScp0492Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HitEventArgs"/> class.
        /// </summary>
        /// <param name="player"> <inheritdoc cref="Player"/></param>
        /// <param name="result"> the result of the attack.</param>
        /// <param name="playerHits"> the list of players who are getting hit.</param>
        public HitEventArgs(ReferenceHub player, AttackResult result, HashSet<ReferenceHub> playerHits)
        {
            Player = Player.Get(player);
            Scp0492 = Player.Role.As<Scp0492Role>();
            Result = result;
            PlayersAffected = playerHits.Select(Player.Get).ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the player who is controlling SCP-049-2.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc />
        public Scp0492Role Scp0492 { get; }

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