// -----------------------------------------------------------------------
// <copyright file="SavingByAntiScp207EventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a player is saved from death by the Anti-SCP-207 effect.
    /// </summary>
    public class SavingByAntiScp207EventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SavingByAntiScp207EventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who is being saved.</param>
        /// <param name="damageAmount">The amount of damage that would have been applied.</param>
        /// <param name="handler">The damage handler that describes the damage.</param>
        /// <param name="hitboxType">The hitbox that was hit.</param>
        public SavingByAntiScp207EventArgs(ReferenceHub player, float damageAmount, DamageHandlerBase handler, HitboxType hitboxType)
        {
            Player = Player.Get(player);

            Handler = handler;
            HitboxType = hitboxType;
            DamageAmount = damageAmount;
            DamageMultiplier = (Player.Health + Player.ArtificialHealth - AntiScp207.DeathSaveHealth) / damageAmount;
            IsAllowed = true;
        }

        /// <summary>
        /// Gets the player who is being saved.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the amount of damage that would have been applied.
        /// </summary>
        public float DamageAmount { get; }

        /// <summary>
        /// Gets or sets the multiplier for the damage that is applied when the event is allowed.
        /// </summary>
        public float DamageMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the multiplier for the damage that if event denied.
        /// </summary>
        public float DeniedDamageMultiplier { get; set; } = 1;

        /// <summary>
        /// Gets the damage handler that describes the incoming damage.
        /// </summary>
        public DamageHandlerBase Handler { get; }

        /// <summary>
        /// Gets the hitbox that was hit.
        /// </summary>
        public HitboxType HitboxType { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event is allowed.
        /// If set to <c>false</c>, the event will be denied.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
