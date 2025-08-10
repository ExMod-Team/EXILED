// -----------------------------------------------------------------------
// <copyright file="Keycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem;
    using InventorySystem.Items.Keycards;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="KeycardItem"/>.
    /// </summary>
    public class Keycard : Item, IWrapper<KeycardItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Keycard"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="KeycardItem"/> class.</param>
        public Keycard(KeycardItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Keycard"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the keycard.</param>
        internal Keycard(ItemType type)
            : this((KeycardItem)Server.Host.Inventory.CreateItemInstance(new(type, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="KeycardItem"/> that this class is encapsulating.
        /// </summary>
        public new KeycardItem Base { get; }

        /// <summary>
        /// Gets or sets the <see cref="KeycardPermissions"/> of the keycard.
        /// </summary>
        public KeycardPermissions Permissions
        {
            get
            {
                foreach (DetailBase detail in Base.Details)
                {
                    switch (detail)
                    {
                        case PredefinedPermsDetail predefinedPermsDetail:
                            return (KeycardPermissions)predefinedPermsDetail.Levels.Permissions;
                        case CustomPermsDetail customPermsDetail:
                            return (KeycardPermissions)customPermsDetail.GetPermissions(null);
                    }
                }

                return KeycardPermissions.None;
            }

            set
            {
                if (!Base.Customizable)
                {
                    Log.Error($"The keycard {Type} is not customizable, so you cannot set permissions on it.");
                    return;
                }

                CustomPermsDetail._customLevels = new KeycardLevels((DoorPermissionFlags)value);

                KeycardDetailSynchronizer.Database.Remove(Serial);
                KeycardDetailSynchronizer.ServerProcessItem(Base);
            }
        }

        /// <summary>
        /// Creates and returns a <see cref="Keycard"/> representing the provided <see cref="CustomizableKeycardType"/>.
        /// </summary>
        /// <param name="keycardType">The type of keycard to create.</param>
        /// <param name="keycardName">The display name of the keycard.</param>
        /// <param name="labelText">The label text shown on the card.</param>
        /// <param name="ownerName">The name of the owner displayed on the card.</param>
        /// <param name="permissions">The door permission flags for the card.</param>
        /// <param name="levelsColor">The color representing the permission levels on the keycard.</param>
        /// <param name="keycardTintColor">The background/tint color of the card.</param>
        /// <param name="labelColor">The color of the label text.</param>
        /// <returns>The newly created <see cref="Keycard"/>.</returns>
        public static Keycard CreateCustom(CustomizableKeycardType keycardType, string keycardName, string labelText, string ownerName, DoorPermissionFlags permissions, Color32 levelsColor, Color32 keycardTintColor, Color32 labelColor) => CreateCustom(keycardType, keycardName, labelText, ownerName, new KeycardLevels(permissions), levelsColor, keycardTintColor, labelColor);

        /// <summary>
        /// Creates and returns a <see cref="Keycard"/> representing the provided <see cref="CustomizableKeycardType"/>.
        /// </summary>
        /// <param name="keycardType">The type of keycard to create.</param>
        /// <param name="keycardName">The display name of the keycard.</param>
        /// <param name="labelText">The label text shown on the card.</param>
        /// <param name="ownerName">The name of the owner displayed on the card.</param>
        /// <param name="containmentLevel">The containment access level.</param>
        /// <param name="armoryLevel">The armory access level.</param>
        /// <param name="adminLevel">The admin access level.</param>
        /// <param name="levelsColor">The color representing the permission levels on the keycard.</param>
        /// <param name="keycardTintColor">The background/tint color of the card.</param>
        /// <param name="labelColor">The color of the label text.</param>
        /// <param name="clampLevels">Whether to clamp the access level values to valid ranges.</param>
        /// <returns>The newly created <see cref="Keycard"/>.</returns>
        public static Keycard CreateCustom(CustomizableKeycardType keycardType, string keycardName, string labelText, string ownerName, int containmentLevel, int armoryLevel, int adminLevel, Color32 levelsColor, Color32 keycardTintColor, Color32 labelColor, bool clampLevels = true) => CreateCustom(keycardType, keycardName, labelText, ownerName, new KeycardLevels(containmentLevel, armoryLevel, adminLevel, clampLevels), levelsColor, keycardTintColor, labelColor);

        /// <summary>
        /// Creates and returns a <see cref="Keycard"/> representing the provided <see cref="CustomizableKeycardType"/>.
        /// </summary>
        /// <param name="keycardType">The type of keycard to create.</param>
        /// <param name="keycardName">The name of keycard.</param>
        /// <param name="labelText">The label text to display on the keycard.</param>
        /// <param name="ownerName">The name of the owner to display on the keycard.</param>
        /// <param name="levels">The level associated with the keycard.</param>
        /// <param name="levelsColor">The color representing the levels on the keycard.</param>
        /// <param name="keycardTintColor">The background color of the keycard.</param>
        /// <param name="labelColor">The color of the label text on the keycard.</param>
        /// <returns>The newly created <see cref="Keycard"/>.</returns>
        public static Keycard CreateCustom(CustomizableKeycardType keycardType, string keycardName, string labelText, string ownerName, KeycardLevels levels, Color32 levelsColor, Color32 keycardTintColor, Color32 labelColor)
        {
            if (!keycardType.GetItemType().TryGetTemplate(out KeycardItem keycard) || !keycard.Customizable)
                throw new ArgumentException($"The provided ItemType {keycardType} is not a valid keycard type or cant be customizable type.", nameof(keycardType));

            NametagDetail._customNametag = ownerName;
            CustomPermsDetail._customLevels = levels;
            CustomLabelDetail._customText = labelText;
            CustomLabelDetail._customColor = labelColor;
            CustomPermsDetail._customColor = levelsColor;
            CustomItemNameDetail._customText = keycardName;
            CustomTintDetail._customColor = keycardTintColor;

            return new Keycard(keycard.ItemTypeId);
        }

        /// <summary>
        /// Returns the Keycard in a human readable format.
        /// </summary>
        /// <returns>A string containing Keycard-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Permissions}|";
    }
}
