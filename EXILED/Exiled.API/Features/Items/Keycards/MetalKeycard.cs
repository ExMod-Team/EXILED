// -----------------------------------------------------------------------
// <copyright file="MetalKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.Keycards
{
    using Exiled.API.Interfaces.Keycards;

    using Interactables.Interobjects.DoorUtils;

    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;

    using UnityEngine;

    /// <summary>
    /// Represents the Metal Custom Keycard.
    /// </summary>
    public class MetalKeycard : CustomKeycardItem, INameTagKeycard, ILabelKeycard, IWearKeycard, ISerialNumberKeycard
    {
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
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        internal MetalKeycard(ItemType type)
            : base(type)
        {
        }

        /// <inheritdoc cref="INameTagKeycard.NameTag"/>
        public string NameTag
        {
            get => DataDict[Serial].NameTag;
            set
            {
                DataDict[Serial].NameTag = value;
                Resync();
            }
        }

        /// <inheritdoc cref="ILabelKeycard.Label"/>
        public string Label
        {
            get => DataDict[Serial].Label;
            set
            {
                DataDict[Serial].Label = value;
                Resync();
            }
        }

        /// <inheritdoc cref="ILabelKeycard.LabelColor"/>
        public Color LabelColor
        {
            get => DataDict[Serial].LabelColor ?? Color.clear;
            set
            {
                DataDict[Serial].LabelColor = value;
                Resync();
            }
        }

        /// <inheritdoc cref="IWearKeycard.Wear"/>
        /// <remarks>Capped from 0-5 for Site-02 keycards, returns 255 if no wear level is found.</remarks>
        public byte Wear
        {
            get => DataDict[Serial].Wear;
            set
            {
                DataDict[Serial].Wear = value;

                Resync();
            }
        }

        /// <inheritdoc cref="ISerialNumberKeycard.SerialNumber"/>
        public string SerialNumber
        {
            get => DataDict[Serial].SerialNumber;
            set
            {
                DataDict[Serial].SerialNumber = value;

                Resync();
            }
        }

        /// <summary>
        /// Creates a <see cref="MetalKeycard"/>.
        /// </summary>
        /// <param name="keycardLevels">The permissions of the keycard.</param>
        /// <param name="permissionsColor">The color of the permissions of the keycard.</param>
        /// <param name="itemName">The inventory name of the keycard.</param>
        /// <param name="color">The color of the keycard.</param>
        /// <param name="nameTag">The name of the owner of the keycard.</param>
        /// <param name="label">The label on the keycard.</param>
        /// <param name="labelColor">The color of the label on the keycard.</param>
        /// <param name="wear">How worn the keycard looks (capped from 0-5).</param>
        /// <param name="serialNumber">The serial number of the keycard (numbers only, 12 max).</param>
        /// <returns>The new <see cref="MetalKeycard"/>.</returns>
        public static MetalKeycard Create(KeycardLevels keycardLevels, Color permissionsColor, string itemName, Color color, string nameTag, string label, Color labelColor, byte wear, string serialNumber)
        {
            MetalKeycard keycard = Create<MetalKeycard>(ItemType.KeycardCustomMetalCase);
            keycard.KeycardLevels = keycardLevels;
            keycard.PermissionsColor = permissionsColor;
            keycard.ItemName = itemName;
            keycard.Color = color;
            keycard.NameTag = nameTag;
            keycard.Label = label;
            keycard.LabelColor = labelColor;
            keycard.Wear = wear;
            keycard.SerialNumber = serialNumber;
            return keycard;
        }
    }
}