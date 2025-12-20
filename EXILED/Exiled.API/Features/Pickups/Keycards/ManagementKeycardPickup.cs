// -----------------------------------------------------------------------
// <copyright file="ManagementKeycardPickup.cs" company="ExMod Team">
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
    /// Represents the Management Custom Keycard.
    /// </summary>
    public class ManagementKeycardPickup : CustomKeycardPickup, ILabelKeycard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementKeycardPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The <see cref="ItemPickupBase"/> to encapsulate.</param>
        internal ManagementKeycardPickup(KeycardPickup pickupBase)
            : base(pickupBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementKeycardPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup to create.</param>
        internal ManagementKeycardPickup(ItemType type)
            : base(type)
        {
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

        /// <summary>
        /// Creates an UnSpawned <see cref="ManagementKeycardPickup"/>, to spawn the pickup, call <see cref="Pickup.Spawn(Vector3, Quaternion?, Player)"/> on the returned instance.
        /// </summary>
        /// <param name="keycardLevels">The permissions of the keycard.</param>
        /// <param name="permissionsColor">The color of the permissions of the keycard.</param>
        /// <param name="itemName">The inventory name of the keycard.</param>
        /// <param name="color">The color of the keycard.</param>
        /// <param name="label">The label on the keycard.</param>
        /// <param name="labelColor">The color of the label on the keycard.</param>
        /// <returns>The new <see cref="ManagementKeycardPickup"/>.</returns>
        public static ManagementKeycardPickup Create(KeycardLevels keycardLevels, Color permissionsColor, string itemName, Color color, string label, Color labelColor)
        {
            ManagementKeycardPickup keycardPickup = Create<ManagementKeycardPickup>(ItemType.KeycardCustomManagement);
            keycardPickup.KeycardLevels = keycardLevels;
            keycardPickup.PermissionsColor = permissionsColor;
            keycardPickup.ItemName = itemName;
            keycardPickup.Color = color;
            keycardPickup.Label = label;
            keycardPickup.LabelColor = labelColor;
            return keycardPickup;
        }
    }
}