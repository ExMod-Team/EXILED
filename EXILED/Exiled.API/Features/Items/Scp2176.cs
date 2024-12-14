// -----------------------------------------------------------------------
// <copyright file="Scp2176.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Features.Pickups.Projectiles;

    using InventorySystem.Items.ThrowableProjectiles;

    using UnityEngine;

    using Scp2176Projectile = Pickups.Projectiles.Scp2176Projectile;

    /// <summary>
    /// A wrapper class for <see cref="ItemType.SCP2176"/>.
    /// </summary>
    public class Scp2176 : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp2176"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public Scp2176(ThrowableItem itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp2176"/> class.
        /// </summary>
        /// <param name="player">The owner of the grenade. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        internal Scp2176(Player player = null)
            : this((ThrowableItem)(player ?? Server.Host).Inventory.CreateItemInstance(new(ItemType.SCP2176, 0), true))
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-2176's next collision will make the dropped sound effect.
        /// </summary>
        public bool DropSound { get; set; }

        /// <summary>
        /// Spawns an active grenade on the map at the specified location.
        /// </summary>
        /// <param name="position">The location to spawn the grenade.</param>
        /// <param name="owner">Optional: The <see cref="Player"/> owner of the grenade.</param>
        /// <returns>Spawned <see cref="Scp2176Projectile">grenade</see>.</returns>
        public Scp2176Projectile SpawnActive(Vector3 position, Player owner = null)
        {
#if DEBUG
            Log.Debug($"Spawning active grenade: {FuseTime}");
#endif

            Projectile projectile = CreateProjectile<Scp2176Projectile>(position, Quaternion.identity);

            projectile.PreviousOwner = owner;

            projectile.Activate();

            return (Scp2176Projectile)projectile;
        }

        /// <summary>
        /// Clones current <see cref="Scp2176"/> object.
        /// </summary>
        /// <returns> New <see cref="Scp2176"/> object. </returns>
        public override Item Clone() => new Scp2176()
        {
            FuseTime = FuseTime,
            PinPullTime = PinPullTime,
            Repickable = Repickable,
        };

        /// <summary>
        /// Returns the ExplosiveGrenade in a human readable format.
        /// </summary>
        /// <returns>A string containing ExplosiveGrenade-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}|";

        /// <inheritdoc/>
        protected override void InitializeProperties(ThrowableItem throwable)
        {
            base.InitializeProperties(throwable);
            if (throwable.Projectile is InventorySystem.Items.ThrowableProjectiles.Scp2176Projectile projectile)
                DropSound = projectile._playedDropSound;
        }
    }
}