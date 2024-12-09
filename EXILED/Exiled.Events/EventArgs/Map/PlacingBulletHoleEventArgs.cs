// -----------------------------------------------------------------------
// <copyright file="PlacingBulletHoleEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using API.Features;
    using Exiled.API.Features.Items;
    using Interfaces;

    using UnityEngine;

    /// <summary>
    /// Contains all information before placing a bullet hole decal.
    /// </summary>
    public class PlacingBulletHoleEventArgs : IFirearmEvent, IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacingBulletHoleEventArgs" /> class.
        /// </summary>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm" />
        /// </param>
        /// <param name="hit">
        /// <inheritdoc cref="RaycastHit" />
        /// </param>
        public PlacingBulletHoleEventArgs(Item firearm, RaycastHit hit)
        {
            Firearm = firearm.As<Firearm>();
            Player = Firearm.Owner;
            Position = hit.point;
            Rotation = Quaternion.LookRotation(hit.normal);
        }

        /// <summary>
        /// Gets or sets the decal position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the decal rotation.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the decal can be placed.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the decal owner.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;
    }
}