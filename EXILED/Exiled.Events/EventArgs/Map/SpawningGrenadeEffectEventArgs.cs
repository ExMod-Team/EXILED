// -----------------------------------------------------------------------
// <copyright file="SpawningGrenadeEffectEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.Events.EventArgs.Interfaces;

    using UnityEngine;

    /// <summary>
    /// Contains all information before grenade explosion effect is spawned.
    /// </summary>
    public class SpawningGrenadeEffectEventArgs : IPickupEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningGrenadeEffectEventArgs"/> class.
        /// </summary>
        /// <param name="effectGrenade"><inheritdoc cref="Projectile"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SpawningGrenadeEffectEventArgs(TimeGrenadeProjectile effectGrenade, bool isAllowed = false)
        {
            Projectile = effectGrenade;
            Position = effectGrenade.Position;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a position of this effect.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Pickup Pickup => Projectile;

        /// <summary>
        /// Gets the projectile that is exploding.
        /// </summary>
        public TimeGrenadeProjectile Projectile { get; }
    }
}