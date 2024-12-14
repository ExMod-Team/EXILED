// -----------------------------------------------------------------------
// <copyright file="ExplosiveGrenade.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Enums;

    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;

    using InventorySystem.Items.ThrowableProjectiles;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="ExplosionGrenade"/>.
    /// </summary>
    public class ExplosiveGrenade : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public ExplosiveGrenade(ThrowableItem itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the grenade.</param>
        /// <param name="player">The owner of the grenade. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        internal ExplosiveGrenade(ItemType type, Player player = null)
            : this((ThrowableItem)(player ?? Server.Host).Inventory.CreateItemInstance(new(type, 0), true))
        {
        }

        /// <summary>
        /// Gets or sets the maximum radius of the grenade.
        /// </summary>
        public float MaxRadius { get; set; }

        /// <summary>
        /// Gets or sets the multiplier for damage against <see cref="Side.Scp"/> players.
        /// </summary>
        public float ScpDamageMultiplier { get; set; }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Burned"/> effect will last.
        /// </summary>
        public float BurnDuration { get; set; }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Deafened"/> effect will last.
        /// </summary>
        public float DeafenDuration { get; set; }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Concussed"/> effect will last.
        /// </summary>
        public float ConcussDuration { get; set; }

        /// <summary>
        /// Spawns an active grenade on the map at the specified location.
        /// </summary>
        /// <param name="position">The location to spawn the grenade.</param>
        /// <param name="owner">Optional: The <see cref="Player"/> owner of the grenade.</param>
        /// <returns>Spawned <see cref="ExplosionGrenadeProjectile">grenade</see>.</returns>
        public ExplosionGrenadeProjectile SpawnActive(Vector3 position, Player owner = null)
        {
#if DEBUG
            Log.Debug($"Spawning active grenade: {FuseTime}");
#endif

            Projectile projectile = CreateProjectile<ExplosionGrenadeProjectile>(position, Quaternion.identity);

            projectile.PreviousOwner = owner;

            projectile.Activate();

            return projectile;
        }

        /// <summary>
        /// Returns the ExplosiveGrenade in a human readable format.
        /// </summary>
        /// <returns>A string containing ExplosiveGrenade-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}|";

        /// <summary>
        /// Clones current <see cref="ExplosiveGrenade"/> object.
        /// </summary>
        /// <returns> New <see cref="ExplosiveGrenade"/> object. </returns>
        public override Item Clone() => new ExplosiveGrenade(Type)
        {
            MaxRadius = MaxRadius,
            ScpDamageMultiplier = ScpDamageMultiplier,
            BurnDuration = BurnDuration,
            DeafenDuration = DeafenDuration,
            ConcussDuration = ConcussDuration,
            FuseTime = FuseTime,
            PinPullTime = PinPullTime,
            Repickable = Repickable,
        };

        /// <inheritdoc/>
        internal override void ReadPickupInfo(Pickup pickup)
        {
            base.ReadPickupInfo(pickup);
            if (pickup is ExplosiveGrenadePickup explosiveGrenadePickup)
            {
                MaxRadius = explosiveGrenadePickup.MaxRadius;
                ScpDamageMultiplier = explosiveGrenadePickup.ScpDamageMultiplier;
                BurnDuration = explosiveGrenadePickup.BurnDuration;
                DeafenDuration = explosiveGrenadePickup.DeafenDuration;
                ConcussDuration = explosiveGrenadePickup.ConcussDuration;
                FuseTime = explosiveGrenadePickup.FuseTime;
            }
        }

        /// <inheritdoc/>
        protected override void InitializeProperties(ThrowableItem throwable)
        {
            base.InitializeProperties(throwable);

            if (throwable.Projectile is ExplosionGrenade grenade)
            {
                MaxRadius = grenade._maxRadius;
                ScpDamageMultiplier = grenade._scpDamageMultiplier;
                BurnDuration = grenade._burnedDuration;
                DeafenDuration = grenade._deafenedDuration;
                ConcussDuration = grenade._concussedDuration;
            }
        }
    }
}