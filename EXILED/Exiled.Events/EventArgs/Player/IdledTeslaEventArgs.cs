// -----------------------------------------------------------------------
// <copyright file="IdledTeslaEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Interfaces;

    /// <summary>
    /// Contains all information after Idling a tesla.
    /// </summary>
    public class IdledTeslaEventArgs : IPlayerEvent, ITeslaEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdledTeslaEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="teslaGate">
        /// <inheritdoc cref="Tesla" />
        /// </param>
        public IdledTeslaEventArgs(Player player, TeslaGate teslaGate)
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
    }
}