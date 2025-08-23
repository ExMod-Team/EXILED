// -----------------------------------------------------------------------
// <copyright file="MetalKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.Keycards
{
    using System.Text;

    using Exiled.API.Features.Pools;
    using Exiled.API.Interfaces.Keycards;

    using Interactables.Interobjects.DoorUtils;

    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;

    using UnityEngine;

    /// <summary>
    /// Represents the Metal Custom Keycard.
    /// </summary>
    public class MetalKeycard : CustomKeycard, INameTagKeycard, ILabelKeycard, IWearKeycard, ISerialLabelKeycard
    {
        private CustomSerialNumberDetail serialNumberDetail;
        private bool serialNumberSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetalKeycard"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        internal MetalKeycard(KeycardItem itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetalKeycard"/> class.
        /// </summary>
        /// <param name="itemType">The <see cref="ItemType"/> of the item to create.</param>
        internal MetalKeycard(ItemType itemType)
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
        /// <remarks>Capped from 0-5 for Site-02 keycards, returns 255 if no wear level is found.</remarks>
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

        /// <inheritdoc cref="ISerialLabelKeycard.SerialLabel"/>
        public string SerialLabel
        {
            get
            {
                if (!serialNumberSet)
                {
                    serialNumberDetail = GetDetail<CustomSerialNumberDetail>();
                    serialNumberSet = true;
                }

                StringBuilder builder = StringBuilderPool.Pool.Get(12);

                foreach (Renderer digit in Gfx.SerialNumberDigits)
                {
                    int index = SerialNumberDetail.DigitMats[serialNumberDetail._sourceMaterial].IndexOf(digit.sharedMaterial);

                    if (index is 10)
                    {
                        builder.Append('-');
                        continue;
                    }

                    builder.Append(index);
                }

                return StringBuilderPool.Pool.ToStringReturn(builder);
            }

            set
            {
                if (!serialNumberSet)
                {
                    serialNumberDetail = GetDetail<CustomSerialNumberDetail>();
                    serialNumberSet = true;
                }

                if (value.Length > 12)
                    value = value.Substring(0, 12);

                foreach (Renderer digit in Gfx.SerialNumberDigits)
                {
                    digit.sharedMaterial = serialNumberDetail.GetDigitMaterial(value[0] - 48);
                }

                Resync();
            }
        }

        /// <summary>
        /// Creates a <see cref="MetalKeycard"/>.
        /// </summary>
        /// <param name="permissions">The permissions of the keycard.</param>
        /// <param name="permissionsColor">The color of the permissions of the keycard.</param>
        /// <param name="itemName">The inventory name of the keycard.</param>
        /// <param name="color">The color of the keycard.</param>
        /// <param name="nameTag">The name of the owner of the keycard.</param>
        /// <param name="label">The label on the keycard.</param>
        /// <param name="labelColor">The color of the label on the keycard.</param>
        /// <param name="wear">How worn the keycard looks (capped from 0-5).</param>
        /// <param name="serialLabel">The serial label of the keycard (numbers only, 12 max).</param>
        /// <returns>The new <see cref="MetalKeycard"/>.</returns>
        public static MetalKeycard Create(KeycardLevels permissions, Color permissionsColor, string itemName, Color color, string nameTag, string label, Color labelColor, byte wear, string serialLabel)
        {
            MetalKeycard keycard = (MetalKeycard)Item.Create(ItemType.KeycardCustomManagement);
            keycard.Permissions = permissions;
            keycard.PermissionsColor = permissionsColor;
            keycard.ItemName = itemName;
            keycard.Color = color;
            keycard.NameTag = nameTag;
            keycard.Label = label;
            keycard.LabelColor = labelColor;
            keycard.Wear = wear;
            keycard.SerialLabel = serialLabel;
            return keycard;
        }
    }
}