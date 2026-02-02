// -----------------------------------------------------------------------
// <copyright file="Text.cs" company="ExMod Team">
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
    /// A wrapper class for <see cref="TextToy"/>.
    /// </summary>
    public class Text : AdminToy, IWrapper<TextToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Text"/> class.
        /// </summary>
        /// <param name="textToy">The <see cref="TextToy"/> of the toy.</param>
        internal Text(TextToy textToy)
            : base(textToy, AdminToyType.TextToy) => Base = textToy;

        /// <summary>
        /// Gets the prefab.
        /// </summary>
        public static TextToy Prefab => PrefabHelper.GetPrefab<TextToy>(PrefabType.TextToy);

        /// <summary>
        /// Gets the base <see cref="TextToy"/>.
        /// </summary>
        public TextToy Base { get; }

        /// <summary>
        /// Gets or sets the Text shown.
        /// </summary>
        public string TextFormat
        {
            get => Base.Network_textFormat;
            set => Base.Network_textFormat = value;
        }

        /// <summary>
        /// Gets or sets the size of the Display Size of the Text.
        /// </summary>
        public Vector2 DisplaySize
        {
            get => Base.Network_displaySize;
            set => Base.Network_displaySize = value;
        }

        /// <summary>
        /// Creates a new <see cref="Text"/> at the specified position.
        /// </summary>
        /// <param name="position">The position of the <see cref="Text"/>.</param>
        /// <param name="text">The text content to display.</param>
        /// <returns>The new <see cref="Text"/>.</returns>
        public static Text Create(Vector3 position, string text) => Create(position: position, text: text, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Text"/> with a specific position, text, and display size.
        /// </summary>
        /// <param name="position">The position of the <see cref="Text"/>.</param>
        /// <param name="text">The text content to display.</param>
        /// <param name="displaySize">The display size of the text.</param>
        /// <returns>The new <see cref="Text"/>.</returns>
        public static Text Create(Vector3 position, string text, Vector2 displaySize) => Create(position: position, text: text, displaySize: displaySize, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Text"/> based on a Transform.
        /// </summary>
        /// <param name="transform">The transform to spawn at.</param>
        /// <param name="text">The text content to display.</param>
        /// <returns>The new <see cref="Text"/>.</returns>
        public static Text Create(Transform transform, string text) => Create(transform: transform, text: text, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Text"/>.
        /// </summary>
        /// <param name="position">The position of the <see cref="Text"/>.</param>
        /// <param name="rotation">The rotation of the <see cref="Text"/>.</param>
        /// <param name="scale">The scale of the <see cref="Text"/>.</param>
        /// <param name="text">The text content to display.</param>
        /// <param name="displaySize">The display size of the text.</param>
        /// <param name="spawn">Whether the <see cref="Text"/> should be initially spawned.</param>
        /// <returns>The new <see cref="Text"/>.</returns>
        public static Text Create(Vector3? position = null, Vector3? rotation = null, Vector3? scale = null, string text = "Default Text", Vector2? displaySize = null, bool spawn = true)
        {
            Text textToy = new(Object.Instantiate(Prefab))
            {
                Position = position ?? Vector3.zero,
                Rotation = Quaternion.Euler(rotation ?? Vector3.zero),
                Scale = scale ?? Vector3.one,
                TextFormat = text,
                DisplaySize = displaySize ?? Vector2.one,
            };

            if (spawn)
                textToy.Spawn();

            return textToy;
        }

        /// <summary>
        /// Creates a new <see cref="Text"/> from a Transform.
        /// </summary>
        /// <param name="transform">The transform to create this <see cref="Text"/> on.</param>
        /// <param name="text">The text content to display.</param>
        /// <param name="displaySize">The display size of the text.</param>
        /// <param name="spawn">Whether the <see cref="Text"/> should be initially spawned.</param>
        /// <param name="worldPositionStays">Whether the <see cref="Text"/> should keep the same world position.</param>
        /// <returns>The new <see cref="Text"/>.</returns>
        public static Text Create(Transform transform, string text = "Default Text", Vector2? displaySize = null, bool spawn = true, bool worldPositionStays = true)
        {
            Text textToy = new(Object.Instantiate(Prefab, transform, worldPositionStays))
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.localScale,
                TextFormat = text,
                DisplaySize = displaySize ?? Vector2.one,
            };

            if (spawn)
                textToy.Spawn();

            return textToy;
        }
    }
}
