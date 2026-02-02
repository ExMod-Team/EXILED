// -----------------------------------------------------------------------
// <copyright file="Capybara.cs" company="ExMod Team">
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
    /// A wrapper class for <see cref="CapybaraToy"/>.
    /// </summary>
    public class Capybara : AdminToy, IWrapper<CapybaraToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Capybara"/> class.
        /// </summary>
        /// <param name="capybaraToy">The <see cref="CapybaraToy"/> of the toy.</param>
        internal Capybara(CapybaraToy capybaraToy)
            : base(capybaraToy, AdminToyType.Capybara) => Base = capybaraToy;

        /// <summary>
        /// Gets the prefab.
        /// </summary>
        public static CapybaraToy Prefab => PrefabHelper.GetPrefab<CapybaraToy>(PrefabType.CapybaraToy);

        /// <summary>
        /// Gets the base <see cref="CapybaraToy"/>.
        /// </summary>
        public CapybaraToy Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the capybara can be collided with.
        /// </summary>
        public bool Collidable
        {
            get => Base.NetworkCollisionsEnabled;
            set => Base.NetworkCollisionsEnabled = value;
        }

        /// <summary>
        /// Creates a new <see cref="Capybara"/> at the specified position.
        /// </summary>
        /// <param name="position">The position of the <see cref="Capybara"/>.</param>
        /// <returns>The new <see cref="Capybara"/>.</returns>
        public static Capybara Create(Vector3 position) => Create(position: position, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Capybara"/> with a specific position and rotation.
        /// </summary>
        /// <param name="position">The position of the <see cref="Capybara"/>.</param>
        /// <param name="rotation">The rotation of the <see cref="Capybara"/>.</param>
        /// <returns>The new <see cref="Capybara"/>.</returns>
        public static Capybara Create(Vector3 position, Vector3 rotation) => Create(position: position, rotation: rotation, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Capybara"/> based on a Transform.
        /// </summary>
        /// <param name="transform">The transform to spawn at.</param>
        /// <returns>The new <see cref="Capybara"/>.</returns>
        public static Capybara Create(Transform transform) => Create(transform: transform, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Capybara"/>.
        /// </summary>
        /// <param name="position">The position of the <see cref="Capybara"/>.</param>
        /// <param name="rotation">The rotation of the <see cref="Capybara"/>.</param>
        /// <param name="scale">The scale of the <see cref="Capybara"/>.</param>
        /// <param name="collidable">Whether the capybara has collision enabled.</param>
        /// <param name="spawn">Whether the <see cref="Capybara"/> should be initially spawned.</param>
        /// <returns>The new <see cref="Capybara"/>.</returns>
        public static Capybara Create(Vector3? position = null, Vector3? rotation = null, Vector3? scale = null, bool collidable = true, bool spawn = true)
        {
            Capybara toy = new(Object.Instantiate(Prefab))
            {
                Position = position ?? Vector3.zero,
                Rotation = Quaternion.Euler(rotation ?? Vector3.zero),
                Scale = scale ?? Vector3.one,
                Collidable = collidable,
            };

            if (spawn)
                toy.Spawn();

            return toy;
        }

        /// <summary>
        /// Creates a new <see cref="Capybara"/> from a Transform.
        /// </summary>
        /// <param name="transform">The transform to create this <see cref="Capybara"/> on.</param>
        /// <param name="collidable">Whether the capybara has collision enabled.</param>
        /// <param name="spawn">Whether the <see cref="Capybara"/> should be initially spawned.</param>
        /// <param name="worldPositionStays">Whether the <see cref="Capybara"/> should keep the same world position.</param>
        /// <returns>The new <see cref="Capybara"/>.</returns>
        public static Capybara Create(Transform transform, bool collidable = true, bool spawn = true, bool worldPositionStays = true)
        {
            Capybara toy = new(Object.Instantiate(Prefab, transform, worldPositionStays))
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.localScale,
                Collidable = collidable,
            };

            if (spawn)
                toy.Spawn();

            return toy;
        }
    }
}