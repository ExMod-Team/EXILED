// -----------------------------------------------------------------------
// <copyright file="Text.cs" company="ExMod Team">
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
    }
}
