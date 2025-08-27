// -----------------------------------------------------------------------
// <copyright file="ManagementKeycard.cs" company="ExMod Team">
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
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        internal ManagementKeycard(ItemType type)
            : base(type)
        {
        }

        /// <inheritdoc cref="ILabelKeycard.Label"/>
        public string Label
        {
            get => LabelDict.TryGetValue(Serial, out string value) ? value : string.Empty;

            set
            {
                LabelDict[Serial] = value;
                Resync();
            }
        }

        /// <inheritdoc cref="ILabelKeycard.LabelColor"/>
        public Color LabelColor
        {
            get => LabelColorDict.TryGetValue(Serial, out Color32 value) ? value : Color.black;

            set
            {
                LabelColorDict[Serial] = value;
                Resync();
            }
        }

        /// <summary>
        /// Creates a <see cref="ManagementKeycard"/>.
        /// </summary>
        /// <param name="permissions">The permissions of the keycard.</param>
        /// <param name="permissionsColor">The color of the permissions of the keycard.</param>
        /// <param name="itemName">The inventory name of the keycard.</param>
        /// <param name="color">The color of the keycard.</param>
        /// <param name="label">The label on the keycard.</param>
        /// <param name="labelColor">The color of the label on the keycard.</param>
        /// <returns>The new <see cref="ManagementKeycard"/>.</returns>
        public static ManagementKeycard Create(KeycardLevels permissions, Color permissionsColor, string itemName, Color color, string label, Color labelColor)
        {
            ManagementKeycard keycard = (ManagementKeycard)Item.Create(ItemType.KeycardCustomManagement);
            keycard.Permissions = permissions;
            keycard.PermissionsColor = permissionsColor;
            keycard.ItemName = itemName;
            keycard.Color = color;
            keycard.Label = label;
            keycard.LabelColor = labelColor;
            return keycard;
        }
    }
}