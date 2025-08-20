// -----------------------------------------------------------------------
// <copyright file="Site02Keycard.cs" company="ExMod Team">
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
    /// Represents the Site-02 Custom Keycard.
    /// </summary>
    public class Site02Keycard : CustomKeycard, INameTagKeycard, ILabelKeycard, IWearKeycard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Site02Keycard"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        internal Site02Keycard(KeycardItem itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Site02Keycard"/> class.
        /// </summary>
        /// <param name="itemType">The <see cref="ItemType"/> of the item to create.</param>
        internal Site02Keycard(ItemType itemType)
            : base(itemType)
        {
        }

        /// <inheritdoc cref="INameTagKeycard.NameTag"/>
        public string NameTag
        {
            get => Gfx.NameFields[0].text;

            set
            {
                Gfx.NameFields[0].text = value;
                Resync();
            }
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

        /// <inheritdoc cref="IWearKeycard.Wear"/>
        /// <remarks>Capped from 0-4 for Site-02 keycards, returns 255 if no wear level is found.</remarks>
        public byte Wear
        {
            get
            {
                for (byte i = 0; i < Gfx.ElementVariants.Length; i++)
                {
                    GameObject obj = Gfx.ElementVariants[i];

                    if (obj.activeSelf)
                        return i;
                }

                return byte.MaxValue;
            }

            set
            {
                for (byte i = 0; i < Gfx.ElementVariants.Length; i++)
                {
                    Gfx.ElementVariants[i].SetActive(i == value);
                }

                Resync();
            }
        }
    }
}