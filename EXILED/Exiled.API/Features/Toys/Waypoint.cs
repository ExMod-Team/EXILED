// -----------------------------------------------------------------------
// <copyright file="Waypoint.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System.Drawing;

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
        public static WaypointToy Prefab { get; } = PrefabHelper.GetPrefab<WaypointToy>(PrefabType.WaypointToy);

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
        /// Creates a new <see cref="Waypoint"/> with a specific position and size (bounds).
        /// </summary>
        /// <param name="position">The position of the <see cref="Waypoint"/>.</param>
        /// <param name="size">The size of the bounds (Applied to NetworkBoundsSize).</param>
        /// <returns>The new <see cref="Waypoint"/>.</returns>
        public static Waypoint Create(Vector3 position, Vector3 size) => Create(position: position, scale: size, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Waypoint"/> based on a Transform.
        /// </summary>
        /// <param name="transform">The transform to spawn at (LocalScale is applied to Bounds).</param>
        /// <param name="size">The size of the bounds (Applied to NetworkBoundsSize).</param>
        /// <returns>The new <see cref="Waypoint"/>.</returns>
        public static Waypoint Create(Transform transform, Vector3 size) => Create(parent: transform, scale: size, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Waypoint"/>.
        /// </summary>
        /// <param name="parent">The transform to create this <see cref="Waypoint"/> on.</param>
        /// <param name="position">The local position of the <see cref="Waypoint"/>.</param>
        /// <param name="rotation">The local rotation of the <see cref="Waypoint"/>.</param>
        /// <param name="scale">The size of the bounds (This is NOT localScale, it applies to NetworkBoundsSize).</param>
        /// <param name="priority">The priority of the waypoint.</param>
        /// <param name="visualizeBounds">Whether to visualize the bounds.</param>
        /// <param name="spawn">Whether the <see cref="Waypoint"/> should be initially spawned.</param>
        /// <returns>The new <see cref="Waypoint"/>.</returns>
        public static Waypoint Create(Transform parent = null, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, float priority = 0f, bool visualizeBounds = false, bool spawn = true)
        {
            Waypoint toy = parent ? new(Object.Instantiate(Prefab, parent)) : new(Object.Instantiate(Prefab));

            toy.Priority = priority;
            toy.VisualizeBounds = visualizeBounds;
            toy.Transform.localPosition = position ?? Vector3.zero;
            toy.Transform.localRotation = rotation ?? Quaternion.identity;
            toy.BoundsSize = scale ?? Vector3.one * 255.9961f;

            if (spawn)
                toy.Spawn();

            return toy;
        }
    }
}