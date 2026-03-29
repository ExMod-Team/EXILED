// -----------------------------------------------------------------------
// <copyright file="MetalKeycardPickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Keycards
{
    using Exiled.API.Features.Items.Keycards;
    using Exiled.API.Interfaces.Keycards;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items.Keycards;
    using InventorySystem.Items.Pickups;
    using UnityEngine;

    /// <summary>
    /// Represents the Metal Custom Keycard.
    /// </summary>
    public class MetalKeycardPickup : CustomKeycardPickup, INameTagKeycard, ILabelKeycard, IWearKeycard, ISerialNumberKeycard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetalKeycardPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The <see cref="ItemPickupBase"/> to encapsulate.</param>
        internal MetalKeycardPickup(KeycardPickup pickupBase)
            : base(pickupBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetalKeycardPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup to create.</param>
        internal MetalKeycardPickup(ItemType type)
            : base(type)
        {
        }

        /// <inheritdoc cref="INameTagKeycard.NameTag"/>
        public string NameTag
        {
            get => CustomKeycardItem.DataDict[Serial].NameTag;
            set
            {
                CustomKeycardItem.DataDict[Serial].NameTag = value;
                Resync();
            }
        }

        /// <inheritdoc cref="ILabelKeycard.Label"/>
        public string Label
        {
            get => CustomKeycardItem.DataDict[Serial].Label;
            set
            {
                CustomKeycardItem.DataDict[Serial].Label = value;
                Resync();
            }
        }

        /// <inheritdoc cref="ILabelKeycard.LabelColor"/>
        public Color LabelColor
        {
            get => CustomKeycardItem.DataDict[Serial].LabelColor ?? Color.clear;
            set
            {
                CustomKeycardItem.DataDict[Serial].LabelColor = value;
                Resync();
            }
        }

        /// <inheritdoc cref="IWearKeycard.Wear"/>
        /// <remarks>Capped from 0-5 for Site-02 keycards, returns 255 if no wear level is found.</remarks>
        public byte Wear
        {
            get => CustomKeycardItem.DataDict[Serial].Wear;
            set
            {
                CustomKeycardItem.DataDict[Serial].Wear = value;

                Resync();
            }
        }

        /// <inheritdoc cref="ISerialNumberKeycard.SerialNumber"/>
        public string SerialNumber
        {
            get => CustomKeycardItem.DataDict[Serial].SerialNumber;
            set
            {
                CustomKeycardItem.DataDict[Serial].SerialNumber = value;

                Resync();
            }
        }

        /// <summary>
        /// Creates a <see cref="MetalKeycardPickup"/>.
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
        /// <returns>The new <see cref="MetalKeycardPickup"/>.</returns>
        public static MetalKeycardPickup Create(KeycardLevels keycardLevels, Color permissionsColor, string itemName, Color color, string nameTag, string label, Color labelColor, byte wear, string serialNumber)
        {
            MetalKeycardPickup keycardPickup = Create<MetalKeycardPickup>(ItemType.KeycardCustomMetalCase);
            keycardPickup.KeycardLevels = keycardLevels;
            keycardPickup.PermissionsColor = permissionsColor;
            keycardPickup.ItemName = itemName;
            keycardPickup.Color = color;
            keycardPickup.NameTag = nameTag;
            keycardPickup.Label = label;
            keycardPickup.LabelColor = labelColor;
            keycardPickup.Wear = wear;
            keycardPickup.SerialNumber = serialNumber;
            return keycardPickup;
        }
    }
}