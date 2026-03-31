// -----------------------------------------------------------------------
// <copyright file="IdlingTeslaEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Interfaces;

    /// <summary>
    /// Contains all information before Idling a tesla.
    /// </summary>
    public class IdlingTeslaEventArgs : IPlayerEvent, ITeslaEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdlingTeslaEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="teslaGate">
        /// <inheritdoc cref="Tesla" />
        /// </param>
        public IdlingTeslaEventArgs(Player player, TeslaGate teslaGate)
        {
            Player = player;
            Tesla = teslaGate;
        }

        /// <summary>
        /// Gets the player who triggered the tesla.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the Tesla.
        /// </summary>
        public TeslaGate Tesla { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player will be detected by the tesla.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}