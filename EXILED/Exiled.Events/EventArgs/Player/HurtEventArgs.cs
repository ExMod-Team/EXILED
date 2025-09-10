// -----------------------------------------------------------------------
// <copyright file="HurtEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.DamageHandlers;

    using Interfaces;

    using CustomAttackerHandler = API.Features.DamageHandlers.AttackerDamageHandler;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// Contains all information before a player gets damaged.
    /// </summary>
    public class HurtEventArgs : IAttackerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HurtEventArgs" /> class.
        /// </summary>
        /// <param name="referenceHub">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="damageHandler">
        /// <inheritdoc cref="DamageHandler" />
        /// </param>
        /// <param name="handlerOutput">
        /// <inheritdoc cref="HandlerOutput" />
        /// </param>
        public HurtEventArgs(ReferenceHub referenceHub, DamageHandlerBase damageHandler, DamageHandlerBase.HandlerOutput handlerOutput)
        {
            Player = Player.Get(referenceHub);
            DamageHandler = new CustomDamageHandler(Player, damageHandler);
            HandlerOutput = handlerOutput;

            if (DamageHandler.BaseIs(out CustomAttackerHandler attackerDamageHandler))
                Attacker = attackerDamageHandler.Attacker;
            else if (damageHandler is GenericDamageHandler genericDamageHandler)
                Attacker = Player.Get(genericDamageHandler.Attacker);
            else
                Attacker = null;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Player Attacker { get; }

        /// <summary>
        /// Gets the amount of inflicted damage.
        /// </summary>
        public float Amount => DamageHandler.Damage;

        /// <summary>
        /// Gets or sets the action than will be made on the player.
        /// </summary>
        public DamageHandlerBase.HandlerOutput HandlerOutput { get; set; }

        /// <inheritdoc/>
        public CustomDamageHandler DamageHandler { get; set; }
    }
}