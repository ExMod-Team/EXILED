// -----------------------------------------------------------------------
// <copyright file="EscapedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles;
    using Respawning;

    /// <summary>
    /// Contains all information after player has escaped.
    /// </summary>
    public class EscapedEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="escapeScenario"><inheritdoc cref="EscapeScenario"/></param>
        public EscapedEventArgs(Player player, EscapeScenario escapeScenario)
        {
            Player = player;
            EscapeScenario = escapeScenario;
            Team = EscapeScenario is EscapeScenario.Scientist or EscapeScenario.CuffedClassD ? SpawnableTeamType.NineTailedFox : SpawnableTeamType.ChaosInsurgency;
            Tickets = Team == SpawnableTeamType.ChaosInsurgency ? 4 : 3;
            OldRole = EscapeScenario is EscapeScenario.Scientist or EscapeScenario.CuffedScientist ? RoleTypeId.Scientist : RoleTypeId.ClassD;
            EscapeTime = (int)Math.Ceiling(player.Role.ActiveTime.TotalSeconds);
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the type of escape.
        /// </summary>
        public EscapeScenario EscapeScenario { get; }

        /// <summary>
        /// Gets the <see cref="SpawnableTeamType"/> that gained tickets for this escape.
        /// </summary>
        public SpawnableTeamType Team { get; }

        /// <summary>
        /// Gets the amount of tickets gained for this escape.
        /// </summary>
        public int Tickets { get; }

        /// <summary>
        /// Gets the previous role for this player.
        /// </summary>
        public RoleTypeId OldRole { get; }

        /// <summary>
        /// Gets the time in seconds since round started.
        /// </summary>
        public int EscapeTime { get; }
    }
}