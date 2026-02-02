// -----------------------------------------------------------------------
// <copyright file="InteractableToy.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using AdminToys;

    using Exiled.API.Enums;
    using Exiled.API.Interfaces;

    using LabApi.Features.Wrappers;

    using UnityEngine;

    using static AdminToys.InvisibleInteractableToy;

    /// <summary>
    /// A wrapper class for <see cref="InvisibleInteractableToy"/>.
    /// </summary>
    public class InteractableToy : AdminToy, IWrapper<InvisibleInteractableToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractableToy"/> class.
        /// </summary>
        /// <param name="invisibleInteractableToy">The <see cref="InvisibleInteractableToy"/> of the toy.</param>
        internal InteractableToy(InvisibleInteractableToy invisibleInteractableToy)
            : base(invisibleInteractableToy, AdminToyType.InvisibleInteractableToy) => Base = invisibleInteractableToy;

        /// <summary>
        /// Gets the prefab.
        /// </summary>
        public static InvisibleInteractableToy Prefab { get; } = PrefabHelper.GetPrefab<InvisibleInteractableToy>(PrefabType.InvisibleInteractableToy);

        /// <summary>
        /// Gets the base <see cref="InvisibleInteractableToy"/>.
        /// </summary>
        public InvisibleInteractableToy Base { get; }

        /// <summary>
        /// Gets or sets the Shape of the Interactable.
        /// </summary>
        public ColliderShape Shape
        {
            get => Base.NetworkShape;
            set => Base.NetworkShape = value;
        }

        /// <summary>
        /// Gets or sets the time to interact with the Interactable.
        /// </summary>
        public float InteractionDuration
        {
            get => Base.NetworkInteractionDuration;
            set => Base.NetworkInteractionDuration = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the interactable is locked.
        /// </summary>
        public bool IsLocked
        {
            get => Base.NetworkIsLocked;
            set => Base.NetworkIsLocked = value;
        }

        /// <summary>
        /// Creates a new <see cref="InteractableToy"/> at the specified position.
        /// </summary>
        /// <param name="position">The position of the <see cref="InteractableToy"/>.</param>
        /// <returns>The new <see cref="InteractableToy"/>.</returns>
        public static InteractableToy Create(Vector3 position) => Create(position: position, spawn: true);

        /// <summary>
        /// Creates a new <see cref="InteractableToy"/> with a specific position and shape.
        /// </summary>
        /// <param name="position">The position of the <see cref="InteractableToy"/>.</param>
        /// <param name="shape">The shape of the collider.</param>
        /// <returns>The new <see cref="InteractableToy"/>.</returns>
        public static InteractableToy Create(Vector3 position, ColliderShape shape) => Create(position: position, shape: shape, spawn: true);

        /// <summary>
        /// Creates a new <see cref="InteractableToy"/> with a specific position, shape, and interaction duration.
        /// </summary>
        /// <param name="position">The position of the <see cref="InteractableToy"/>.</param>
        /// <param name="shape">The shape of the collider.</param>
        /// <param name="duration">How long the interaction takes.</param>
        /// <returns>The new <see cref="InteractableToy"/>.</returns>
        public static InteractableToy Create(Vector3 position, ColliderShape shape, float duration) => Create(position: position, shape: shape, interactionDuration: duration, spawn: true);

        /// <summary>
        /// Creates a new <see cref="InteractableToy"/> from a Transform.
        /// </summary>
        /// <param name="transform">The transform to create this <see cref="InteractableToy"/> on.</param>
        /// <param name="shape">The shape of the collider.</param>
        /// <param name="interactionDuration">How long the interaction takes.</param>
        /// <param name="isLocked">Whether the object is locked.</param>
        /// <param name="spawn">Whether the <see cref="InteractableToy"/> should be initially spawned.</param>
        /// <param name="worldPositionStays">Whether the <see cref="InteractableToy"/> should keep the same world position.</param>
        /// <returns>The new <see cref="InteractableToy"/>.</returns>
        public static InteractableToy Create(Transform transform, ColliderShape shape = ColliderShape.Sphere, float interactionDuration = 1f, bool isLocked = false, bool spawn = true, bool worldPositionStays = true)
            => Create(parent: transform, shape: shape, interactionDuration: interactionDuration, isLocked: isLocked, spawn: spawn, worldPositionStays: worldPositionStays);

        /// <summary>
        /// Creates a new <see cref="InteractableToy"/>.
        /// </summary>
        /// <param name="position">The local position of the <see cref="InteractableToy"/>.</param>
        /// <param name="rotation">The local rotation of the <see cref="InteractableToy"/>.</param>
        /// <param name="scale">The local scale of the <see cref="InteractableToy"/>.</param>
        /// <param name="shape">The shape of the collider.</param>
        /// <param name="interactionDuration">How long the interaction takes.</param>
        /// <param name="isLocked">Whether the object is locked.</param>
        /// <param name="parent">The transform to create this <see cref="InteractableToy"/> on.</param>
        /// <param name="spawn">Whether the <see cref="InteractableToy"/> should be initially spawned.</param>
        /// <param name="worldPositionStays">Whether the <see cref="InteractableToy"/> should keep the same world position.</param>
        /// <returns>The new <see cref="InteractableToy"/>.</returns>
        public static InteractableToy Create(Vector3? position = null, Vector3? rotation = null, Vector3? scale = null, ColliderShape shape = ColliderShape.Sphere, float interactionDuration = 1f, bool isLocked = false, Transform parent = null, bool spawn = true, bool worldPositionStays = true)
        {
            InteractableToy toy = parent ? new(Object.Instantiate(Prefab, parent, worldPositionStays)) : new(Object.Instantiate(Prefab));

            if (position.HasValue)
                toy.Transform.localPosition = position.Value;

            if (rotation.HasValue)
                toy.Transform.localRotation = Quaternion.Euler(rotation.Value);

            if (scale.HasValue)
                toy.Transform.localScale = scale.Value;

            toy.Shape = shape;
            toy.InteractionDuration = interactionDuration;
            toy.IsLocked = isLocked;

            if (spawn)
                toy.Spawn();

            return toy;
        }
    }
}