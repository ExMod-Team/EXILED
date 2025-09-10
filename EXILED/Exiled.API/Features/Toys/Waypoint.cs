// -----------------------------------------------------------------------
// <copyright file="Waypoint.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using AdminToys;
    using Enums;
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
        /// <param name="speakerToy">The <see cref="WaypointToy"/> of the toy.</param>
        internal Waypoint(WaypointToy speakerToy)
            : base(speakerToy, AdminToyType.WaypointToy) => Base = speakerToy;

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
            get => Base.Priority;
            set => Base.Priority = value;
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
        /// Gets the id of the Waypoint use for <see cref="RelativePositioning.RelativePosition.WaypointId"/>.
        /// </summary>
        public byte WaypointId => Base._waypointId;
    }
}
