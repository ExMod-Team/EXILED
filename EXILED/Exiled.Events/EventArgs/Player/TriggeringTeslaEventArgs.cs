// -----------------------------------------------------------------------
// <copyright file="TriggeringTeslaEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Interfaces;

    /// <summary>
    /// Contains all information before triggering a tesla.
    /// </summary>
    public class TriggeringTeslaEventArgs : IPlayerEvent, ITeslaEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggeringTeslaEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="teslaGate">
        /// <inheritdoc cref="Tesla" />
        /// </param>
        public TriggeringTeslaEventArgs(Player player, TeslaGate teslaGate)
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

        /// <summary>
        /// Gets or sets a value indicating whether the tesla will be deactivated (Both Idle and Activation).
        /// </summary>
        public bool DisableTesla { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player will cause the tesla going to be activated.
        /// </summary>
        public bool IsTriggerable { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the tesla can Idle.
        /// </summary>
        public bool CanIdle { get; set; } = true;
    }
}