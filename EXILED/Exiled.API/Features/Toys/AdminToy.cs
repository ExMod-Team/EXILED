// -----------------------------------------------------------------------
// <copyright file="AdminToy.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System.Collections.Generic;

    using AdminToys;

    using Enums;
    using Exiled.API.Interfaces;
    using Footprinting;
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
        /// Gets the <see cref="UnityEngine.GameObject"/> of the toy.
        /// </summary>
        public GameObject GameObject => AdminToyBase.gameObject;

        /// <summary>
        /// Gets the <see cref="UnityEngine.Transform"/> of the toy.
        /// </summary>
        public Transform Transform => AdminToyBase.transform;

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
                LightSourceToy lightSourceToy => new Light(lightSourceToy),
                PrimitiveObjectToy primitiveObjectToy => new Primitive(primitiveObjectToy),
                ShootingTarget shootingTarget => new ShootingTargetToy(shootingTarget),
                SpeakerToy speakerToy => new Speaker(speakerToy),
                CapybaraToy capybaraToy => new Capybara(capybaraToy),
                Scp079CameraToy scp079CameraToy => new CameraToy(scp079CameraToy),
                InvisibleInteractableToy invisibleInteractableToy => new InteractableToy(invisibleInteractableToy),
                TextToy textToy => new Text(textToy),
                WaypointToy waypointToy => new Waypoint(waypointToy),
                _ => throw new System.NotImplementedException(),
            };
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