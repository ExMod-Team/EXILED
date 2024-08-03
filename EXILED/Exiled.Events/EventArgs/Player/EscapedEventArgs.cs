// -----------------------------------------------------------------------
// <copyright file="EscapedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

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
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the type of escape.
        /// </summary>
        public EscapeScenario EscapeScenario { get; }
    }
}