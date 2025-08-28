// -----------------------------------------------------------------------
// <copyright file="KeycardData.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.Keycards
{
    using UnityEngine;

    /// <summary>
    /// A class containing all possible properties of any keycard that aren't stored by the server.
    /// </summary>
    public class KeycardData
    {
        /// <summary>
        /// Gets or sets the permissions color of this <see cref="KeycardData"/>.
        /// </summary>
        public Color32? PermissionsColor { get; set; }

        /// <summary>
        /// Gets or sets the item name of this <see cref="KeycardData"/>.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the color of this <see cref="KeycardData"/>.
        /// </summary>
        public Color32? Color { get; set; }

        /// <summary>
        /// Gets or sets the name tag of this <see cref="KeycardData"/>.
        /// </summary>
        public string NameTag { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the label of this <see cref="KeycardData"/>.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the label color of this <see cref="KeycardData"/>.
        /// </summary>
        public Color32? LabelColor { get; set; }

        /// <summary>
        /// Gets or sets the wear of this <see cref="KeycardData"/>.
        /// </summary>
        public byte Wear { get; set; } = byte.MaxValue;

        /// <summary>
        /// Gets or sets the serial number of this <see cref="KeycardData"/>.
        /// </summary>
        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the rank of this <see cref="KeycardData"/>.
        /// </summary>
        public byte Rank { get; set; } = byte.MaxValue;
    }
}