// -----------------------------------------------------------------------
// <copyright file="CameraToy.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using AdminToys;

    using Exiled.API.Enums;
    using Exiled.API.Interfaces;

    using UnityEngine;

    using CameraType = Enums.CameraType;

    /// <summary>
    /// A wrapper class for <see cref="AdminToys.AdminToyBase"/>.
    /// </summary>
    public class CameraToy : AdminToy, IWrapper<Scp079CameraToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraToy"/> class.
        /// </summary>
        /// <param name="scp079CameraToy">The <see cref="Scp079CameraToy"/> of the toy.</param>
        internal CameraToy(Scp079CameraToy scp079CameraToy)
            : base(scp079CameraToy, AdminToyType.CameraToy) => Base = scp079CameraToy;

        /// <summary>
        /// Gets the prefab for EzArm Camera prefab.
        /// </summary>
        public static Scp079CameraToy EzArmCameraPrefab { get; } = PrefabHelper.GetPrefab<Scp079CameraToy>(PrefabType.EzArmCameraToy);

        /// <summary>
        /// Gets the prefab for Ez Camera prefab.
        /// </summary>
        public static Scp079CameraToy EzCameraPrefab { get; } = PrefabHelper.GetPrefab<Scp079CameraToy>(PrefabType.EzCameraToy);

        /// <summary>
        /// Gets the prefab for Hcz Camera prefab.
        /// </summary>
        public static Scp079CameraToy HczCameraPrefab { get; } = PrefabHelper.GetPrefab<Scp079CameraToy>(PrefabType.HczCameraToy);

        /// <summary>
        /// Gets the prefab for Lcz Camera prefab.
        /// </summary>
        public static Scp079CameraToy LczCameraPrefab { get; } = PrefabHelper.GetPrefab<Scp079CameraToy>(PrefabType.LczCameraToy);

        /// <summary>
        /// Gets the prefab for Sz Camera prefab.
        /// </summary>
        public static Scp079CameraToy SzCameraPrefab { get; } = PrefabHelper.GetPrefab<Scp079CameraToy>(PrefabType.SzCameraToy);

        /// <summary>
        /// Gets the base <see cref="Scp079CameraToy"/>.
        /// </summary>
        public Scp079CameraToy Base { get; }

        /// <summary>
        /// Gets or sets the Vertical Restriction.
        /// </summary>
        public Vector2 VerticalConstraint
        {
            get => Base.NetworkVerticalConstraint;
            set => Base.NetworkVerticalConstraint = value;
        }

        /// <summary>
        /// Gets or sets the Horizontal restriction.
        /// </summary>
        public Vector2 HorizontalConstraint
        {
            get => Base.NetworkHorizontalConstraint;
            set => Base.NetworkHorizontalConstraint = value;
        }

        /// <summary>
        /// Gets or sets the Zoom restriction.
        /// </summary>
        public Vector2 ZoomConstraint
        {
            get => Base.NetworkZoomConstraint;
            set => Base.NetworkZoomConstraint = value;
        }

        /// <summary>
        /// Gets or sets the Room where the Camera is associated with.
        /// </summary>
        public Room Room
        {
            get => Room.Get(Base.NetworkRoom);
            set => Base.NetworkRoom = value.Identifier;
        }

        /// <summary>
        /// Gets or sets the Name of the Camera.
        /// </summary>
        public string Name
        {
            get => Base.NetworkLabel;
            set => Base.NetworkLabel = value;
        }

        /// <summary>
        /// Creates a new <see cref="CameraToy"/> with a specified type.
        /// </summary>
        /// <param name="type">The <see cref="CameraType"/> of the camera.</param>
        /// <param name="position">The local position of the camera.</param>
        /// <returns>The new <see cref="CameraToy"/>.</returns>
        public static CameraToy Create(CameraType type, Vector3 position) => Create(type: type, position: position, spawn: true);

        /// <summary>
        /// Creates a new <see cref="CameraToy"/> with a specified type and name.
        /// </summary>
        /// <param name="type">The <see cref="CameraType"/> of the camera.</param>
        /// <param name="position">The local position of the camera.</param>
        /// <param name="name">The name (label) of the camera.</param>
        /// <returns>The new <see cref="CameraToy"/>.</returns>
        public static CameraToy Create(CameraType type, Vector3 position, string name) => Create(type: type, position: position, name: name, spawn: true);

        /// <summary>
        /// Creates a new <see cref="CameraToy"/>.
        /// </summary>
        /// <param name="parent">The transform to create this <see cref="CameraToy"/> on.</param>
        /// <param name="type">The <see cref="CameraType"/> of the camera.</param>
        /// <param name="position">The local position of the camera.</param>
        /// <param name="rotation">The local rotation of the camera.</param>
        /// <param name="scale">The local scale of the camera.</param>
        /// <param name="name">The name (label) of the camera.</param>
        /// <param name="room">The room associated with this camera.</param>
        /// <param name="verticalConstraint">The vertical limits. Leave null to use prefab default.</param>
        /// <param name="horizontalConstraint">The horizontal limits. Leave null to use prefab default.</param>
        /// <param name="zoomConstraint">The zoom limits. Leave null to use prefab default.</param>
        /// <param name="spawn">Whether the camera should be initially spawned.</param>
        /// <returns>The new <see cref="CameraToy"/>.</returns>
        public static CameraToy Create(Transform parent = null, CameraType type = CameraType.EzArmCameraToy, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, string name = "New Camera", Room room = null, Vector2? verticalConstraint = null, Vector2? horizontalConstraint = null, Vector2? zoomConstraint = null, bool spawn = true)
        {
            Scp079CameraToy prefab = type switch
            {
                CameraType.EzArmCameraToy => EzArmCameraPrefab,
                CameraType.EzCameraToy => EzCameraPrefab,
                CameraType.HczCameraToy => HczCameraPrefab,
                CameraType.LczCameraToy => LczCameraPrefab,
                CameraType.SzCameraToy => SzCameraPrefab,
                _ => EzArmCameraPrefab
            };

            CameraToy toy = parent ? new(Object.Instantiate(prefab, parent)) : new(Object.Instantiate(prefab));

            toy.Name = name;
            toy.Transform.localPosition = position ?? Vector3.zero;
            toy.Transform.localRotation = rotation ?? Quaternion.identity;
            toy.Transform.localScale = scale ?? Vector3.one;

            if (verticalConstraint.HasValue)
                toy.VerticalConstraint = verticalConstraint.Value;

            if (horizontalConstraint.HasValue)
                toy.HorizontalConstraint = horizontalConstraint.Value;

            if (zoomConstraint.HasValue)
                toy.ZoomConstraint = zoomConstraint.Value;

            if (room != null)
                toy.Room = room;

            if (spawn)
                toy.Spawn();

            return toy;
        }
    }
}