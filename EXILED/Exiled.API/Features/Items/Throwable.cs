// -----------------------------------------------------------------------
// <copyright file="Throwable.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.API.Interfaces;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for throwable items.
    /// </summary>
    public class Throwable : Item, IWrapper<ThrowableItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Throwable"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public Throwable(ThrowableItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
            InitializeProperties(itemBase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Throwable"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the throwable item.</param>
        /// <param name="player">The owner of the throwable item. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this throwable is custom using Exiled.CustomItems.</remarks>
        internal Throwable(ItemType type, Player player = null)
            : this((ThrowableItem)(player ?? Server.Host).Inventory.CreateItemInstance(new(type, 0), true))
        {
        }

        /// <summary>
        /// Gets the <see cref="ThrowableItem"/> base for this item.
        /// </summary>
        public new ThrowableItem Base { get; }

        /// <summary>
        /// Gets or sets how long the fuse will last.
        /// </summary>
        public float FuseTime { get; set; }

        /// <summary>
        /// Gets or sets the amount of time it takes to pull the pin.
        /// </summary>
        public float PinPullTime
        {
            get => Base._pinPullTime;
            set => Base._pinPullTime = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether players can pickup grenade after throw.
        /// </summary>
        public bool Repickable
        {
            get => Base._repickupable;
            set => Base._repickupable = value;
        }

        /// <summary>
        /// Throws the item.
        /// </summary>
        /// <param name="fullForce">Whether to use full or weak force.</param>
        /// this.ServerThrow(projectileSettings.StartVelocity, projectileSettings.UpwardsFactor, projectileSettings.StartTorque, startVel);
        public void Throw(bool fullForce = true)
        {
            ThrowableItem.ProjectileSettings settings = fullForce ? Base.FullThrowSettings : Base.WeakThrowSettings;

            Base.ServerThrow(settings.StartVelocity, settings.UpwardsFactor, settings.StartTorque, ThrowableNetworkHandler.GetLimitedVelocity(Owner?.Velocity ?? Vector3.one));
        }

        /// <summary>
        /// Creates the <see cref="Projectile"/> that based on this <see cref="Item"/>.
        /// </summary>
        /// <param name="position">The location to spawn the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="spawn">Whether the <see cref="Projectile"/> should be initially spawned.</param>
        /// <returns>The created <see cref="Projectile"/>.</returns>
        /// <remarks><see cref="Projectile"/> wont be activated, use <see cref="Projectile.Activate"/> to activate spawned <see cref="Projectile"/>.</remarks>
        public virtual Projectile CreateProjectile(Vector3 position, Quaternion rotation = default, bool spawn = true)
        {
            ThrownProjectile ipb = Object.Instantiate(Base.Projectile, position, rotation);

            ipb.Info = new PickupSyncInfo(Type, Weight, Serial);

            Projectile projectile = Pickup.Get<Projectile>(ipb);

            projectile.ReadThrowableItemInfo(this);

            if (spawn)
                projectile.Spawn();

            return projectile;
        }

        /// <summary>
        /// Creates the <see cref="Projectile"/> that based on this <see cref="Item"/>.
        /// </summary>
        /// <param name="position">The location to spawn the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="spawn">Whether the <see cref="Projectile"/> should be initially spawned.</param>
        /// <typeparam name="T">The specified <see cref="Projectile"/> type.</typeparam>
        /// <returns>The created <see cref="Projectile"/>.</returns>
        /// <remarks><see cref="Projectile"/> wont be activated, use <see cref="Projectile.Activate"/> to activate spawned <see cref="Projectile"/>.</remarks>
        public T CreateProjectile<T>(Vector3 position, Quaternion rotation = default, bool spawn = true)
            where T : Projectile => CreateProjectile(position, rotation, spawn) as T;

        /// <summary>
        /// Clones current <see cref="Throwable"/> object.
        /// </summary>
        /// <returns> New <see cref="Throwable"/> object. </returns>
        public override Item Clone() => new Throwable(Type)
        {
            PinPullTime = PinPullTime,
            Repickable = Repickable,
        };

        /// <summary>
        /// Returns the Throwable in a human readable format.
        /// </summary>
        /// <returns>A string containing Throwable-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{PinPullTime}|";

        /// <summary>
        /// initialize throwable item properties.
        /// </summary>
        /// <param name="throwable">target throwable.</param>
        protected virtual void InitializeProperties(ThrowableItem throwable)
        {
            if (throwable.Projectile is TimeGrenade timeGrenade)
            {
                FuseTime = timeGrenade._fuseTime;
            }
        }
    }
}
