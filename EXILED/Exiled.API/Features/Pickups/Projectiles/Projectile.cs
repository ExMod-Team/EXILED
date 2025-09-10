// -----------------------------------------------------------------------
// <copyright file="Projectile.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for Projectile.
    /// </summary>
    public class Projectile : Pickup, IWrapper<ThrownProjectile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Projectile"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="ThrownProjectile"/> class.</param>
        internal Projectile(ThrownProjectile pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Projectile"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal Projectile(ItemType type)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase itemBase) || itemBase is not ThrowableItem throwable)
                return;

            throwable.Projectile.gameObject.SetActive(false);
            base.Base = Base = Object.Instantiate(throwable.Projectile);
            throwable.Projectile.gameObject.SetActive(true);

            PickupSyncInfo psi = new()
            {
                ItemId = type,
                Serial = ItemSerialGenerator.GenerateNext(),
                WeightKg = itemBase.Weight,
            };

            Info = psi;
            BaseToPickup.Add(Base, this);
        }

        /// <summary>
        /// Gets the <see cref="ThrownProjectile"/> that this class is encapsulating.
        /// </summary>
        public new ThrownProjectile Base { get; }

        /// <summary>
        /// Gets the <see cref="Enums.ProjectileType"/> of the item.
        /// </summary>
        public ProjectileType ProjectileType => Type.GetProjectileType();

        /// <summary>
        /// Creates and returns a new <see cref="Projectile"/> with the proper inherited subclass.
        /// <para>
        /// Based on the <paramref name="projectiletype"/>, the returned <see cref="Projectile"/> can be casted into a subclass to gain more control over the object.
        /// <br />The following have their own respective classes:
        /// <br />- FragGrenade can be casted to <see cref="ExplosionGrenadeProjectile"/>.
        /// <br />- Flashbang can be casted to <see cref="FlashbangProjectile"/>.
        /// <br />- Scp018 A and B variants can be casted to <see cref="Scp018Projectile"/>.
        /// <br />- Scp2176 can be casted to <see cref="Scp2176Projectile"/>.
        /// </para>
        /// <para>
        /// Projectile that are not listed will cause an Exception.
        /// </para>
        /// </summary>
        /// <param name="projectiletype">The <see cref="Enums.ProjectileType"/> of the projectile.</param>
        /// <returns>The created <see cref="Projectile"/>.</returns>
        public static Projectile Create(ProjectileType projectiletype) => projectiletype switch
        {
            ProjectileType.Scp018 => new Scp018Projectile(),
            ProjectileType.Flashbang => new FlashbangProjectile(),
            ProjectileType.Scp2176 => new Scp2176Projectile(),
            ProjectileType.FragGrenade => new ExplosionGrenadeProjectile(ItemType.GrenadeHE),
            _ => throw new Exception($"ProjectileType does not contain a valid value : {projectiletype}"),
        };

        /// <summary>
        /// Creates and returns a new <see cref="Projectile"/> with the proper inherited subclass.
        /// <para>
        /// Based on the <paramref name="projectiletype"/>, the returned <see cref="Projectile"/> can be casted into a subclass to gain more control over the object.
        /// <br />The following have their own respective classes:
        /// <br />- FragGrenade can be casted to <see cref="ExplosionGrenadeProjectile"/>.
        /// <br />- Flashbang can be casted to <see cref="FlashbangProjectile"/>.
        /// <br />- Scp018 A and B variants can be casted to <see cref="Scp018Projectile"/>.
        /// <br />- Scp2176 can be casted to <see cref="Scp2176Projectile"/>.
        /// </para>
        /// <para>
        /// Projectile that are not listed will cause an Exception.
        /// </para>
        /// </summary>
        /// <param name="projectiletype">The <see cref="Enums.ProjectileType"/> of the projectile.</param>
        /// <typeparam name="T">The specified <see cref="Projectile"/> type.</typeparam>
        /// <returns>The created <see cref="Projectile"/>.</returns>
        public static T Create<T>(ProjectileType projectiletype)
            where T : Projectile => Create(projectiletype) as T;

        /// <summary>
        /// Creates and spawns a <see cref="Projectile"/>.
        /// </summary>
        /// <param name="type">The <see cref="ProjectileType"/> of the projectile.</param>
        /// <param name="position">The position to spawn the <see cref="Projectile"/> at.</param>
        /// <param name="rotation">The rotation to spawn the <see cref="Projectile"/>.</param>
        /// <param name="shouldBeActive">Whether the <see cref="Projectile"/> should be in active state after spawn.</param>
        /// <param name="previousOwner">An optional previous owner of the item.</param>
        /// <returns>The <see cref="Projectile"/>. See documentation of <see cref="Create"/> for more information on casting.</returns>
        public static Projectile CreateAndSpawn(ProjectileType type, Vector3 position, Quaternion? rotation = null, bool shouldBeActive = true, Player previousOwner = null) => Create(type).Spawn(position, rotation, shouldBeActive, previousOwner);

        /// <summary>
        /// Creates and spawns a <see cref="Projectile"/>.
        /// </summary>
        /// <param name="type">The <see cref="ProjectileType"/> of the projectile.</param>
        /// <param name="position">The position to spawn the <see cref="Projectile"/> at.</param>
        /// <param name="rotation">The rotation to spawn the <see cref="Projectile"/>.</param>
        /// <param name="shouldBeActive">Whether the <see cref="Projectile"/> should be in active state after spawn.</param>
        /// <param name="previousOwner">An optional previous owner of the item.</param>
        /// <typeparam name="T">The specified <see cref="Projectile"/> type.</typeparam>
        /// <returns>The <see cref="Projectile"/>. See documentation of <see cref="Create"/> for more information on casting.</returns>
        public static T CreateAndSpawn<T>(ProjectileType type, Vector3 position, Quaternion? rotation = null, bool shouldBeActive = true, Player previousOwner = null)
            where T : Projectile => CreateAndSpawn(type, position, rotation, shouldBeActive, previousOwner) as T;

        /// <summary>
        /// Activates the current <see cref="Projectile"/>.
        /// </summary>
        public void Activate() => Base.ServerActivate();

        /// <summary>
        /// Spawns a <see cref="Projectile"/>.
        /// </summary>
        /// <param name="position">The position to spawn the <see cref="Projectile"/> at.</param>
        /// <param name="rotation">The rotation to spawn the <see cref="Projectile"/>.</param>
        /// <param name="shouldBeActive">Whether the <see cref="Projectile"/> should be in active state after spawn.</param>
        /// <param name="previousOwner">An optional previous owner of the item.</param>
        /// <returns>The spawned <see cref="Projectile"/>.</returns>
        public Projectile Spawn(Vector3 position, Quaternion? rotation = null, bool shouldBeActive = true, Player previousOwner = null)
        {
            Position = position;
            Rotation = rotation ?? Quaternion.identity;
            PreviousOwner = previousOwner;
            Spawn();

            if (shouldBeActive)
                Activate();

            return this;
        }

        /// <summary>
        /// Returns the ProjectilePickup in a human readable format.
        /// </summary>
        /// <returns>A string containing ProjectilePickup-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{IsLocked}- ={InUse}=";
    }
}
