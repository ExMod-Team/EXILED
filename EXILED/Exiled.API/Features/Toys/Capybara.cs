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
        public static CapybaraToy Prefab { get; } = PrefabHelper.GetPrefab<CapybaraToy>(PrefabType.CapybaraToy);

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
        /// <param name="position">The local position of the <see cref="Capybara"/>.</param>
        /// <returns>The new <see cref="Capybara"/>.</returns>
        public static Capybara Create(Vector3 position) => Create(position: position, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Capybara"/> with a specific position and rotation.
        /// </summary>
        /// <param name="position">The local position of the <see cref="Capybara"/>.</param>
        /// <param name="rotation">The local rotation of the <see cref="Capybara"/>.</param>
        /// <returns>The new <see cref="Capybara"/>.</returns>
        public static Capybara Create(Vector3 position, Quaternion rotation) => Create(position: position, rotation: rotation, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Capybara"/> based on a Transform.
        /// </summary>
        /// <param name="transform">The transform to spawn at.</param>
        /// <returns>The new <see cref="Capybara"/>.</returns>
        public static Capybara Create(Transform transform) => Create(parent: transform, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Capybara"/>.
        /// </summary>
        /// <param name="parent">The transform to create this <see cref="Capybara"/> on.</param>
        /// <param name="position">The local position of the <see cref="Capybara"/>.</param>
        /// <param name="rotation">The local rotation of the <see cref="Capybara"/>.</param>
        /// <param name="scale">The local scale of the <see cref="Capybara"/>.</param>
        /// <param name="collidable">Whether the capybara has collision enabled.</param>
        /// <param name="spawn">Whether the <see cref="Capybara"/> should be initially spawned.</param>
        /// <returns>The new <see cref="Capybara"/>.</returns>
        public static Capybara Create(Transform parent = null, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, bool collidable = true, bool spawn = true)
        {
            Capybara toy = parent ? new(Object.Instantiate(Prefab, parent)) : new(Object.Instantiate(Prefab));

            toy.Collidable = collidable;
            toy.Transform.localPosition = position ?? Vector3.zero;
            toy.Transform.localRotation = rotation ?? Quaternion.identity;
            toy.Transform.localScale = scale ?? Vector3.one;

            if (spawn)
                toy.Spawn();

            return toy;
        }
    }
}