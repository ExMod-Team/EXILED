// -----------------------------------------------------------------------
// <copyright file="ManagementKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.Keycards
{
    using Exiled.API.Interfaces.Keycards;

    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;

    using UnityEngine;

    /// <summary>
    /// Represents the Management Custom Keycard.
    /// </summary>
    public class ManagementKeycard : CustomKeycard, ILabelKeycard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementKeycard"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        internal ManagementKeycard(KeycardItem itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementKeycard"/> class.
        /// </summary>
        /// <param name="itemType">The <see cref="ItemType"/> of the item to create.</param>
        internal ManagementKeycard(ItemType itemType)
            : base(itemType)
        {
        }

        /// <inheritdoc cref="ILabelKeycard.Label"/>
        public string Label
        {
            get => Gfx.KeycardLabels[0].text;

            set
            {
                Gfx.KeycardLabels[0].text = value;
                Resync();
            }
        }

        /// <inheritdoc cref="ILabelKeycard.LabelColor"/>
        public Color LabelColor
        {
            get => Gfx.KeycardLabels[0].color;

            set
            {
                Gfx.KeycardLabels[0].color = value;
                Resync();
            }
        }
    }
}