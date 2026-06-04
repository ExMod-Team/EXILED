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
        public static TextToy Prefab { get; internal set; }

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
        /// Converts TextToy to Text.
        /// </summary>
        /// <param name="textToy">The TextToy.</param>
        /// <returns>EXILED Text.</returns>
        public static implicit operator Text(TextToy textToy) => (Text)Get(textToy);

        /// <summary>
        /// Creates a new <see cref="Text"/>.
        /// </summary>
        /// <param name="position">The local position of the <see cref="Text"/>.</param>
        /// <param name="text">The text content to display.</param>
        /// <returns>The new <see cref="Text"/>.</returns>
        public static Text Create(Vector3 position, string text) => Create(position: position, text: text, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Text"/>.
        /// </summary>
        /// <param name="position">The local position of the <see cref="Text"/>.</param>
        /// <param name="text">The text content to display.</param>
        /// <param name="displaySize">The display size of the text.</param>
        /// <returns>The new <see cref="Text"/>.</returns>
        public static Text Create(Vector3 position, string text, Vector2 displaySize) => Create(position: position, text: text, displaySize: displaySize, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Text"/>.
        /// </summary>
        /// <param name="parent">The transform to create this <see cref="Text"/> on.</param>
        /// <param name="position">The local position of the <see cref="Text"/>.</param>
        /// <param name="rotation">The local rotation of the <see cref="Text"/>.</param>
        /// <param name="scale">The local scale of the <see cref="Text"/>.</param>
        /// <param name="text">The text content to display.</param>
        /// <param name="displaySize">The display size of the text.</param>
        /// <param name="spawn">Whether the <see cref="Text"/> should be initially spawned.</param>
        /// <returns>The new <see cref="Text"/>.</returns>
        public static Text Create(Transform parent = null, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, string text = "Hello World!", Vector2? displaySize = null, bool spawn = true)
        {
            Text toy = new(Object.Instantiate(Prefab, parent))
            {
                DisplaySize = displaySize ?? new Vector2(50, 50),
                LocalPosition = position ?? Vector3.zero,
                LocalRotation = rotation ?? Quaternion.identity,
                Scale = scale ?? Vector3.one,
                TextFormat = text,
            };

            if (spawn)
                toy.Spawn();

            return toy;
        }
    }
}