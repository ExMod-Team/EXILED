// -----------------------------------------------------------------------
// <copyright file="AdminToy.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System.Collections.Generic;
    using System.Linq;

    using AdminToys;

    using Enums;
    using Exiled.API.Interfaces;
    using Footprinting;
    using InventorySystem.Items;
    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="AdminToys.AdminToyBase"/>.
    /// </summary>
    public abstract class AdminToy : IWorldSpace
    {
        /// <summary>
        /// A dictionary of all <see cref="AdminToys.AdminToyBase"/>'s that have been converted into <see cref="AdminToy"/>.
        /// </summary>
        internal static readonly Dictionary<AdminToyBase, AdminToy> BaseToAdminToy = new(new ComponentsEqualityComparer());

        private static readonly Dictionary<AdminToyType, PrefabType> TypeLookup = new()
        {
            { AdminToyType.ShootingTargetSport, PrefabType.SportTarget },
            { AdminToyType.ShootingTargetClassD, PrefabType.DBoyTarget },
            { AdminToyType.ShootingTargetBinary, PrefabType.BinaryTarget },
            { AdminToyType.LightSource, PrefabType.LightSourceToy },
            { AdminToyType.PrimitiveObject, PrefabType.PrimitiveObjectToy },
            { AdminToyType.Speaker, PrefabType.SpeakerToy },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminToy"/> class.
        /// </summary>
        /// <param name="toyAdminToyBase">The <see cref="AdminToys.AdminToyBase"/> to be wrapped.</param>
        /// <param name="type">The <see cref="AdminToyType"/> of the object.</param>
        internal AdminToy(AdminToyBase toyAdminToyBase, AdminToyType type)
        {
            AdminToyBase = toyAdminToyBase;
            ToyType = type;

            BaseToAdminToy.Add(toyAdminToyBase, this);
        }

        /// <summary>
        /// Gets a list of all <see cref="AdminToy"/>'s on the server.
        /// </summary>
        public static IReadOnlyCollection<AdminToy> List => BaseToAdminToy.Values;

        /// <summary>
        /// Gets the original <see cref="AdminToys.AdminToyBase"/>.
        /// </summary>
        public AdminToyBase AdminToyBase { get; }

        /// <summary>
        /// Gets the <see cref="AdminToyType"/>.
        /// </summary>
        public AdminToyType ToyType { get; }

        /// <summary>
        /// Gets or sets who spawn the Primitive AdminToy.
        /// </summary>
        public Player Player
        {
            get => Player.Get(Footprint);
            set => Footprint = value.Footprint;
        }

        /// <summary>
        /// Gets or sets the Footprint of the player who spawned the AdminToy.
        /// </summary>
        public Footprint Footprint
        {
            get => AdminToyBase.SpawnerFootprint;
            set => AdminToyBase.SpawnerFootprint = value;
        }

        /// <summary>
        /// Gets or sets the position of the toy.
        /// </summary>
        public Vector3 Position
        {
            get => AdminToyBase.transform.position;
            set
            {
                AdminToyBase.transform.position = value;
                AdminToyBase.NetworkPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the toy.
        /// </summary>
        public Quaternion Rotation
        {
            get => AdminToyBase.transform.rotation;
            set
            {
                AdminToyBase.transform.rotation = value;
                AdminToyBase.NetworkRotation = value;
            }
        }

        /// <summary>
        /// Gets or sets the scale of the toy.
        /// </summary>
        public Vector3 Scale
        {
            get => AdminToyBase.transform.localScale;
            set
            {
                AdminToyBase.transform.localScale = value;
                AdminToyBase.NetworkScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the movement smoothing value of the toy.
        /// <para>
        /// Higher values reflect smoother movements.
        /// <br /> - 60 is an ideal value.
        /// </para>
        /// </summary>
        public byte MovementSmoothing
        {
            get => AdminToyBase.MovementSmoothing;
            set => AdminToyBase.NetworkMovementSmoothing = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsStatic.
        /// </summary>
        public bool IsStatic
        {
            get => AdminToyBase.IsStatic;
            set => AdminToyBase.NetworkIsStatic = value;
        }

        /// <summary>
        /// Gets the <see cref="AdminToy"/> belonging to the <see cref="AdminToys.AdminToyBase"/>.
        /// </summary>
        /// <param name="adminToyBase">The <see cref="AdminToys.AdminToyBase"/> instance.</param>
        /// <returns>The corresponding <see cref="AdminToy"/> instance.</returns>
        public static AdminToy Get(AdminToyBase adminToyBase)
        {
            if (adminToyBase == null)
                return null;

            if (BaseToAdminToy.TryGetValue(adminToyBase, out AdminToy adminToy))
                return adminToy;

            return adminToyBase switch
            {
                PrimitiveObjectToy primitiveObjectToy => new Primitive(primitiveObjectToy),
                LightSourceToy lightSourceToy => new Light(lightSourceToy),
                SpeakerToy speakerToy => new Speaker(speakerToy),
                ShootingTarget shootingTarget => shootingTarget.gameObject.name switch
                {
                    "TargetBinary" => new ShootingTargetToy(shootingTarget, AdminToyType.ShootingTargetBinary),
                    "TargetClassD" => new ShootingTargetToy(shootingTarget, AdminToyType.ShootingTargetClassD),
                    "TargetSport" => new ShootingTargetToy(shootingTarget, AdminToyType.ShootingTargetSport),
                    _ => throw new System.NotImplementedException($"ShootingTarget Name: {shootingTarget.gameObject.name}"),
                },
                _ => throw new System.NotImplementedException($"AdminToyBase Name: {adminToyBase.gameObject.name}")
            };
        }

        /// <summary>
        /// Create the <see cref="AdminToy"/> belonging to the <see cref="AdminToys.AdminToyBase"/>.
        /// </summary>
        /// <param name="adminToyType">The <see cref="AdminToyType"/>.</param>
        /// <param name="position">The <see cref="Vector3"/> position where the <see cref="AdminToy"/> will spawn.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation of the <see cref="AdminToy"/>.</param>
        /// <returns>The corresponding <see cref="AdminToy"/>.</returns>
        public static AdminToy Create(AdminToyType adminToyType, Vector3 position = default, Quaternion? rotation = null)
        {
            if (!TypeLookup.TryGetValue(adminToyType, out PrefabType prefabType))
                throw new System.NotImplementedException($"AdminToy::Create(AdminToyType, Vector3, Quaternion?) AdminToyType: {adminToyType}");
            return Get(PrefabHelper.Spawn<AdminToyBase>(prefabType, position, rotation));
        }

        /// <summary>
        /// Gets the <see cref="AdminToy"/> by <see cref="AdminToys.AdminToyBase"/>.
        /// </summary>
        /// <param name="adminToyBase">The <see cref="AdminToys.AdminToyBase"/> to convert into an admintoy.</param>
        /// <typeparam name="T">The specified <see cref="AdminToy"/> type.</typeparam>
        /// <returns>The admintoy wrapper for the given <see cref="AdminToys.AdminToyBase"/>.</returns>
        public static T Get<T>(AdminToyBase adminToyBase)
            where T : AdminToy => Get(adminToyBase) as T;

        /// <summary>
        /// Gets the <see cref="AdminToy"/> by <see cref="AdminToys.AdminToyBase"/>.
        /// </summary>
        /// <param name="adminToyType">The <see cref="AdminToyType"/> to convert into an admintoy.</param>
        /// <param name="position">The <see cref="Vector3"/> position where the <see cref="AdminToy"/> will spawn.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation of the <see cref="AdminToy"/>.</param>
        /// <typeparam name="T">The specified <see cref="AdminToy"/> type.</typeparam>
        /// <returns>The admintoy wrapper for the given <see cref="AdminToys.AdminToyBase"/>.</returns>
        public static T Create<T>(AdminToyType adminToyType, Vector3 position = default, Quaternion? rotation = null)
            where T : AdminToy => Create(adminToyType, position, rotation) as T;

        /// <summary>
        /// Spawns the toy into the game. Use <see cref="UnSpawn"/> to remove it.
        /// </summary>
        public void Spawn() => NetworkServer.Spawn(AdminToyBase.gameObject);

        /// <summary>
        /// Removes the toy from the game. Use <see cref="Spawn"/> to bring it back.
        /// </summary>
        public void UnSpawn() => NetworkServer.UnSpawn(AdminToyBase.gameObject);

        /// <summary>
        /// Destroys the toy.
        /// </summary>
        public void Destroy()
        {
            BaseToAdminToy.Remove(AdminToyBase);
            NetworkServer.Destroy(AdminToyBase.gameObject);
        }
    }
}