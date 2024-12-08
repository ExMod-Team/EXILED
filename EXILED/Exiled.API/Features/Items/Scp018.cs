// -----------------------------------------------------------------------
// <copyright file="Scp018.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Features.Pickups.Projectiles;

    using InventorySystem.Items.ThrowableProjectiles;

    using UnityEngine;

    using BaseScp018Projectile = InventorySystem.Items.ThrowableProjectiles.Scp018Projectile;

    using Scp018Projectile = Pickups.Projectiles.Scp018Projectile;

    /// <summary>
    /// A wrapper class for <see cref="BaseScp018Projectile"/> item.
    /// </summary>
    public class Scp018 : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp018"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public Scp018(ThrowableItem itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp018"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the grenade.</param>
        /// <param name="player">The owner of the grenade. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        internal Scp018(ItemType type, Player player = null)
            : this((ThrowableItem)(player ?? Server.Host).Inventory.CreateItemInstance(new(type, 0), true))
        {
        }

        /// <summary>
        /// Gets or sets the time for SCP-018 not to ignore the friendly fire.
        /// </summary>
        public float FriendlyFireTime { get; set; }

        /// <summary>
        /// Spawns an active grenade on the map at the specified location.
        /// </summary>
        /// <param name="position">The location to spawn the grenade.</param>
        /// <param name="owner">Optional: The <see cref="Player"/> owner of the grenade.</param>
        /// <returns>Spawned <see cref="Scp018Projectile">grenade</see>.</returns>
        public Scp018Projectile SpawnActive(Vector3 position, Player owner = null)
        {
#if DEBUG
            Log.Debug($"Spawning active grenade: {FuseTime}");
#endif

            Projectile projectile = CreateProjectile(position, Quaternion.identity);

            projectile.PreviousOwner = owner;

            projectile.Activate();

            return (Scp018Projectile)projectile;
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
        public override Item Clone() => new Scp018(Type)
        {
            FriendlyFireTime = FriendlyFireTime,
            FuseTime = FuseTime,
            PinPullTime = PinPullTime,
            Repickable = Repickable,
        };

        /// <inheritdoc/>
        protected override void InitializeProperties(ThrowableItem throwable)
        {
            base.InitializeProperties(throwable);
            if (throwable.Projectile is BaseScp018Projectile grenade)
            {
                FriendlyFireTime = grenade._friendlyFireTime;
            }
        }
    }
}