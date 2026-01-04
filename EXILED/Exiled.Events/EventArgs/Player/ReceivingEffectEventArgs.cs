// -----------------------------------------------------------------------
// <copyright file="ReceivingEffectEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using API.Features;
    using CustomPlayerEffects;
    using Interfaces;

    /// <summary>
    /// Contains all information before a player receives a <see cref="StatusEffectBase" />.
    /// </summary>
    public class ReceivingEffectEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingEffectEventArgs" /> class.
        /// </summary>
        /// <param name="effect"><inheritdoc cref="Effect"/></param>
        /// <param name="intensity">The intensity the effect is being changed to.</param>
        /// <param name="currentIntensity"><inheritdoc cref="CurrentIntensity"/></param>
        /// <param name="duration"><inheritdoc cref="Duration"/></param>
        public ReceivingEffectEventArgs(StatusEffectBase effect, byte intensity, byte currentIntensity, float duration)
        {
            Player = Player.Get(effect?.Hub);
            if (Player == null)
                Log.Error("Player is Null");
            Effect = effect;
            Intensity = Math.Max(intensity, currentIntensity);
            CurrentIntensity = currentIntensity;
            Duration = intensity is 0 ? 0 : duration;
        }

        /// <summary>
        /// Gets the <see cref="Player" /> receiving the effect.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="StatusEffectBase" /> being received.
        /// </summary>
        public StatusEffectBase Effect { get; }

        /// <summary>
        /// Gets or sets a value indicating how long the effect will last. If its value is 0, then it doesn't always reflect the real effect duration.
        /// </summary>
        public float Duration { get; set; } = 0;

        /// <summary>
        /// Gets or sets the value of the new intensity of the effect.
        /// </summary>
        public byte Intensity { get; set; }

        /// <summary>
        /// Gets the value of the intensity of this effect on the player.
        /// </summary>
        public byte CurrentIntensity { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the effect will be applied.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}