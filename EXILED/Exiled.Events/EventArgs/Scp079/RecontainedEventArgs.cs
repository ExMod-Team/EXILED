// -----------------------------------------------------------------------
// <copyright file="RecontainedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains information after SCP-079 gets recontained.
    /// </summary>
    public class RecontainedEventArgs : IScp079Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecontainedEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="scp079Recontainer">
        /// <inheritdoc cref="Recontainer" />
        /// </param>
        public RecontainedEventArgs(Player player, PlayerRoles.PlayableScps.Scp079.Scp079Recontainer scp079Recontainer)
        {
            Player = player;
            Scp079 = player.Role.As<Scp079Role>();
            Recontainer = scp079Recontainer;
            Attacker = Player.Get(scp079Recontainer._activatorGlass.LastAttacker);
            IsAutomatic = scp079Recontainer._activatorGlass.LastAttacker.IsSet;
        }

        /// <summary>
        /// Gets the player that previously controlled SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp079Role Scp079 { get; }

        /// <summary>
        /// Gets the instance that handle SCP-079 recontained proccess.
        /// </summary>
        public PlayerRoles.PlayableScps.Scp079.Scp079Recontainer Recontainer { get; }

        /// <summary>
        /// Gets the player who recontained SCP-079.
        /// </summary>
        public Player Attacker { get; }

        /// <summary>
        /// Gets a value indicating whether the recontainment has been made automatically or by triggering the process.
        /// </summary>
        public bool IsAutomatic { get; }
    }
}
