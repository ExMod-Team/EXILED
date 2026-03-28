// -----------------------------------------------------------------------
// <copyright file="HurtingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.DamageHandlers;
    using Interfaces;
    using PlayerStatsSystem;

    using CustomAttackerHandler = API.Features.DamageHandlers.AttackerDamageHandler;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// Contains all information before a player gets damaged.
    /// </summary>
    public class HurtingEventArgs : IAttackerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HurtingEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="damageHandler">
        /// <inheritdoc cref="DamageHandler" />
        /// </param>
        public HurtingEventArgs(Player target, DamageHandlerBase damageHandler)
        {
            DamageHandler = new CustomDamageHandler(target, damageHandler);

            Player = target;

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
        /// Gets or sets the amount of inflicted damage.
        /// </summary>
        public float Amount
        {
            get => DamageHandler.Damage;
            set => DamageHandler.Damage = value;
        }

        /// <inheritdoc/>
        public CustomDamageHandler DamageHandler { get; set; }

        /// <summary>
        /// Gets a value indicating whether the incoming damage is an instant kill.
        /// </summary>
        public bool IsInstantKill => Amount == StandardDamageHandler.KillValue;

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;
    }
}