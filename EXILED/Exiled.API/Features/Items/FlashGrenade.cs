// -----------------------------------------------------------------------
// <copyright file="FlashGrenade.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;

    using InventorySystem.Items.ThrowableProjectiles;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="FlashbangGrenade"/>.
    /// </summary>
    public class FlashGrenade : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlashGrenade"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public FlashGrenade(ThrowableItem itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlashGrenade"/> class, as well as a new flash grenade item.
        /// </summary>
        /// <param name="player">The owner of the grenade. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        internal FlashGrenade(Player player = null)
            : this((ThrowableItem)(player ?? Server.Host).Inventory.CreateItemInstance(new(ItemType.GrenadeFlash, 0), true))
        {
        }

        /// <summary>
        /// Gets or sets the minimum duration of player can take the effect.
        /// </summary>
        public float MinimalDurationEffect { get; set; }

        /// <summary>
        /// Gets or sets the additional duration of the <see cref="EffectType.Blinded"/> effect.
        /// </summary>
        public float AdditionalBlindedEffect { get; set; }

        /// <summary>
        /// Gets or sets the how mush the flash grenade going to be intensified when explode at <see cref="RoomType.Surface"/>.
        /// </summary>
        public float SurfaceDistanceIntensifier { get; set; }

        /// <summary>
        /// Spawns an active grenade on the map at the specified location.
        /// </summary>
        /// <param name="position">The location to spawn the grenade.</param>
        /// <param name="owner">Optional: The <see cref="Player"/> owner of the grenade.</param>
        /// <returns>Spawned <see cref="FlashbangProjectile">grenade</see>.</returns>
        public FlashbangProjectile SpawnActive(Vector3 position, Player owner = null)
        {
#if DEBUG
            Log.Debug($"Spawning active grenade: {FuseTime}");
#endif

            FlashbangProjectile projectile = CreateProjectile<FlashbangProjectile>(position, Quaternion.identity);

            projectile.PreviousOwner = owner;

            projectile.Activate();

            return projectile;
        }

        /// <summary>
        /// Clones current <see cref="FlashGrenade"/> object.
        /// </summary>
        /// <returns> New <see cref="FlashGrenade"/> object. </returns>
        public override Item Clone() => new FlashGrenade()
        {
            MinimalDurationEffect = MinimalDurationEffect,
            AdditionalBlindedEffect = AdditionalBlindedEffect,
            SurfaceDistanceIntensifier = SurfaceDistanceIntensifier,
            FuseTime = FuseTime,
            Repickable = Repickable,
            PinPullTime = PinPullTime,
        };

        /// <summary>
        /// Returns the FlashGrenade in a human readable format.
        /// </summary>
        /// <returns>A string containing FlashGrenade-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}|";

        /// <inheritdoc/>
        internal override void ReadPickupInfo(Pickup pickup)
        {
            base.ReadPickupInfo(pickup);
            if (pickup is FlashGrenadePickup flashGrenadePickup)
            {
                MinimalDurationEffect = flashGrenadePickup.MinimalDurationEffect;
                AdditionalBlindedEffect = flashGrenadePickup.AdditionalBlindedEffect;
                SurfaceDistanceIntensifier = flashGrenadePickup.SurfaceDistanceIntensifier;
                FuseTime = flashGrenadePickup.FuseTime;
            }
        }

        /// <inheritdoc/>
        protected override void InitializeProperties(ThrowableItem throwable)
        {
            base.InitializeProperties(throwable);

            if (throwable.Projectile is FlashbangGrenade grenade)
            {
                MinimalDurationEffect = grenade._minimalEffectDuration;
                AdditionalBlindedEffect = grenade._additionalBlurDuration;
                SurfaceDistanceIntensifier = grenade._surfaceZoneDistanceIntensifier;
            }
        }
    }
}
