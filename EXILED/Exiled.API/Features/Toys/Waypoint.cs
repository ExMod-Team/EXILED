// -----------------------------------------------------------------------
// <copyright file="Waypoint.cs" company="ExMod Team">
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

    /// <summary>
    /// A wrapper class for <see cref="WaypointToy"/>.
    /// </summary>
    public class Waypoint : AdminToy, IWrapper<WaypointToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Waypoint"/> class.
        /// </summary>
        /// <param name="waypointToy">The <see cref="WaypointToy"/> of the toy.</param>
        internal Waypoint(WaypointToy waypointToy)
            : base(waypointToy, AdminToyType.WaypointToy) => Base = waypointToy;

        /// <summary>
        /// Gets the prefab.
        /// </summary>
        public static WaypointToy Prefab => PrefabHelper.GetPrefab<WaypointToy>(PrefabType.WaypointToy);

        /// <summary>
        /// Gets the base <see cref="WaypointToy"/>.
        /// </summary>
        public WaypointToy Base { get; }

        /// <summary>
        /// Gets or sets the Waypoint shown.
        /// </summary>
        public float Priority
        {
            get => Base.NetworkPriority;
            set => Base.NetworkPriority = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Bounds are shown for Debug.
        /// </summary>
        public bool VisualizeBounds
        {
            get => Base.NetworkVisualizeBounds;
            set => Base.NetworkVisualizeBounds = value;
        }

        /// <summary>
        /// Gets or sets the bounds this waypoint encapsulates.
        /// </summary>
        public Bounds Bounds
        {
            get => new(Position, Base.NetworkBoundsSize);
            set => Base.NetworkBoundsSize = value.size;
        }

        /// <summary>
        /// Gets or sets the bounds size this waypoint encapsulates.
        /// </summary>
        public Vector3 BoundsSize
        {
            get => Base.NetworkBoundsSize;
            set => Base.NetworkBoundsSize = value;
        }

        /// <summary>
        /// Gets the id of the Waypoint used for <see cref="RelativePositioning.RelativePosition.WaypointId"/>.
        /// </summary>
        public byte WaypointId => Base._waypointId;

        /// <summary>
        /// Creates a new <see cref="Waypoint"/> at the specified position.
        /// </summary>
        /// <param name="position">The position of the <see cref="Waypoint"/>.</param>
        /// <returns>The new <see cref="Waypoint"/>.</returns>
        public static Waypoint Create(Vector3 position) => Create(position: position);

        /// <summary>
        /// Creates a new <see cref="Waypoint"/> with a specific position and size (bounds).
        /// </summary>
        /// <param name="position">The position of the <see cref="Waypoint"/>.</param>
        /// <param name="size">The size of the bounds (Applied to NetworkBoundsSize).</param>
        /// <returns>The new <see cref="Waypoint"/>.</returns>
        public static Waypoint Create(Vector3 position, Vector3 size) => Create(position: position, scale: size);

        /// <summary>
        /// Creates a new <see cref="Waypoint"/> based on a Transform.
        /// </summary>
        /// <param name="transform">The transform to spawn at (LocalScale is applied to Bounds).</param>
        /// <returns>The new <see cref="Waypoint"/>.</returns>
        public static Waypoint Create(Transform transform) => Create(transform: transform, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Waypoint"/>.
        /// </summary>
        /// <param name="position">The position of the <see cref="Waypoint"/>.</param>
        /// <param name="rotation">The rotation of the <see cref="Waypoint"/>.</param>
        /// <param name="scale">The size of the bounds (This is NOT localScale, it applies to NetworkBoundsSize).</param>
        /// <param name="priority">The priority of the waypoint.</param>
        /// <param name="visualizeBounds">Whether to visualize the bounds.</param>
        /// <param name="spawn">Whether the <see cref="Waypoint"/> should be initially spawned.</param>
        /// <returns>The new <see cref="Waypoint"/>.</returns>
        public static Waypoint Create(Vector3? position = null, Vector3? rotation = null, Vector3? scale = null, float priority = 0f, bool visualizeBounds = false, bool spawn = true)
        {
            Waypoint toy = new(Object.Instantiate(Prefab))
            {
                Position = position ?? Vector3.zero,
                Rotation = Quaternion.Euler(rotation ?? Vector3.zero),
                BoundsSize = scale ?? Vector3.one * 255.9961f,
                Priority = priority,
                VisualizeBounds = visualizeBounds,
            };

            if (spawn)
                toy.Spawn();

            return toy;
        }

        /// <summary>
        /// Creates a new <see cref="Waypoint"/> from a Transform.
        /// </summary>
        /// <param name="transform">The transform to create this <see cref="Waypoint"/> on.</param>
        /// <param name="priority">The priority of the waypoint.</param>
        /// <param name="visualizeBounds">Whether to visualize the bounds.</param>
        /// <param name="spawn">Whether the <see cref="Waypoint"/> should be initially spawned.</param>
        /// <param name="worldPositionStays">Whether the <see cref="Waypoint"/> should keep the same world position.</param>
        /// <returns>The new <see cref="Waypoint"/>.</returns>
        public static Waypoint Create(Transform transform, float priority = 0f, bool visualizeBounds = false, bool spawn = true, bool worldPositionStays = true)
        {
            Waypoint toy = new(Object.Instantiate(Prefab, transform, worldPositionStays))
            {
                Position = transform.position,
                Rotation = transform.rotation,
                BoundsSize = transform.localScale,
                Priority = priority,
                VisualizeBounds = visualizeBounds,
            };

            if (spawn)
                toy.Spawn();

            return toy;
        }
    }
}