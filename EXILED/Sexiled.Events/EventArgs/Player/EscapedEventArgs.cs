// -----------------------------------------------------------------------
// <copyright file="EscapedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.EventArgs.Player
{
    using System;

    using Sexiled.API.Enums;
    using Sexiled.API.Features;
    using Sexiled.API.Features.Roles;
    using Sexiled.Events.EventArgs.Interfaces;

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
        /// <param name="role"><inheritdoc cref="Role"/></param>
        public EscapedEventArgs(Player player, EscapeScenario escapeScenario, Role role)
        {
            Player = player;
            EscapeScenario = escapeScenario;
            OldRole = role;
            EscapeTime = (int)Math.Ceiling(role.ActiveTime.TotalSeconds);
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the type of escape.
        /// </summary>
        public EscapeScenario EscapeScenario { get; }

        /// <summary>
        /// Gets the previous role for this player.
        /// </summary>
        public Role OldRole { get; }

        /// <summary>
        /// Gets the time in seconds since round started.
        /// </summary>
        public int EscapeTime { get; }
    }
}