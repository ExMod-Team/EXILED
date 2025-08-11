// -----------------------------------------------------------------------
// <copyright file="KeycardPickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.API.Interfaces;

    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;

    using BaseKeycard = InventorySystem.Items.Keycards.KeycardPickup;

    /// <summary>
    /// A wrapper class for a Keycard pickup.
    /// </summary>
    public class KeycardPickup : Pickup, IWrapper<BaseKeycard>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeycardPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseKeycard"/> class.</param>
        internal KeycardPickup(BaseKeycard pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeycardPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal KeycardPickup(ItemType type)
            : base(type)
        {
            Base = (BaseKeycard)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="KeycardPermissions"/> of the keycard.
        /// </summary>
        public KeycardPermissions Permissions { get; private set; }

        /// <summary>
        /// Gets the <see cref="BaseKeycard"/> that this class is encapsulating.
        /// </summary>
        public new BaseKeycard Base { get; }

        /// <summary>
        /// Creates and returns a <see cref="KeycardPickup"/> representing the provided <see cref="CustomizableKeycardType"/>.
        /// </summary>
        /// <param name="keycardType">The type of keycard to create.</param>
        /// <param name="keycardName">The display name of the keycard.</param>
        /// <param name="labelText">The label text shown on the keycard.</param>
        /// <param name="ownerName">The name of the owner displayed on the keycard.</param>
        /// <param name="permissions">The door permission flags for the card.</param>
        /// <param name="levelsColor">The color representing the permission levels on the keycard.</param>
        /// <param name="tintColor">The tint color of the card.</param>
        /// <param name="labelColor">The color of the label text.</param>
        /// <param name="spawnPosition">The position of the pickup.</param>
        /// <param name="rotation">The rotation of pickup.</param>
        /// <param name="spawn">Whether the pickup will spawn or not.</param>
        /// <returns>The newly created <see cref="KeycardPickup"/>.</returns>
        public static KeycardPickup CreateCustomKeycardPickup(CustomizableKeycardType keycardType, string keycardName, string labelText, string ownerName, DoorPermissionFlags permissions, Color32 levelsColor, Color32 tintColor, Color32 labelColor, Vector3 spawnPosition, Quaternion rotation, bool spawn = false)
        {
            Keycard item = Keycard.CreateCustom(keycardType, keycardName, labelText, ownerName, new KeycardLevels(permissions), levelsColor, tintColor, labelColor);

            KeycardPickup pickup = (KeycardPickup)item.CreatePickup(spawnPosition, rotation, spawn);

            return pickup;
        }

        /// <summary>
        /// Creates and returns a <see cref="KeycardPickup"/> representing the provided <see cref="CustomizableKeycardType"/>.
        /// </summary>
        /// <param name="keycardType">The type of keycard to create.</param>
        /// <param name="keycardName">The display name of the keycard.</param>
        /// <param name="labelText">The label text shown on the keycard.</param>
        /// <param name="ownerName">The name of the owner displayed on the keycard.</param>
        /// <param name="containmentLevel">The containment access level.</param>
        /// <param name="armoryLevel">The armory access level.</param>
        /// <param name="adminLevel">The admin access level.</param>
        /// <param name="levelsColor">The color representing the permission levels on the keycard.</param>
        /// <param name="tintColor">The tint color of the card.</param>
        /// <param name="labelColor">The color of the label text.</param>
        /// <param name="spawnPosition">The position of the pickup.</param>
        /// <param name="rotation">The rotation of pickup.</param>
        /// <param name="spawn">Whether the pickup will spawn or not.</param>
        /// /// <param name="clampLevels">Whether to clamp the access level values to valid ranges.</param>
        /// <returns>The newly created <see cref="KeycardPickup"/>.</returns>
        public static KeycardPickup CreateCustomKeycardPickup(CustomizableKeycardType keycardType, string keycardName, string labelText, string ownerName, int containmentLevel, int armoryLevel, int adminLevel, Color32 levelsColor, Color32 tintColor, Color32 labelColor, Vector3 spawnPosition, Quaternion rotation, bool spawn = false, bool clampLevels = true)
        {
            Keycard item = Keycard.CreateCustom(keycardType, keycardName, labelText, ownerName, new KeycardLevels(containmentLevel, armoryLevel, adminLevel, clampLevels), levelsColor, tintColor, labelColor);

            KeycardPickup pickup = (KeycardPickup)item.CreatePickup(spawnPosition, rotation, spawn);

            return pickup;
        }

        /// <summary>
        /// Creates and returns a <see cref="KeycardPickup"/> representing the provided <see cref="CustomizableKeycardType"/>.
        /// </summary>
        /// <param name="keycardType">The type of keycard to create.</param>
        /// <param name="keycardName">The display name of the keycard.</param>
        /// <param name="labelText">The label text shown on the keycard.</param>
        /// <param name="ownerName">The name of the owner displayed on the keycard.</param>
        /// <param name="levels">The level associated with the keycard.</param>
        /// <param name="levelsColor">The color representing the permission levels on the keycard.</param>
        /// <param name="tintColor">The tint color of the card.</param>
        /// <param name="labelColor">The color of the label text.</param>
        /// <param name="spawnPosition">The position of the pickup.</param>
        /// <param name="rotation">The rotation of pickup.</param>
        /// <param name="spawn">Whether the pickup will spawn or not.</param>
        /// <returns>The newly created <see cref="KeycardPickup"/>.</returns>
        public static KeycardPickup CreateCustomKeycardPickup(CustomizableKeycardType keycardType, string keycardName, string labelText, string ownerName, KeycardLevels levels, Color32 levelsColor, Color32 tintColor, Color32 labelColor, Vector3 spawnPosition, Quaternion rotation, bool spawn = false)
        {
            Keycard item = Keycard.CreateCustom(keycardType, keycardName, labelText, ownerName, levels, levelsColor, tintColor, labelColor);

            KeycardPickup pickup = (KeycardPickup)item.CreatePickup(spawnPosition, rotation, spawn);

            return pickup;
        }

        /// <inheritdoc/>
        internal override void ReadItemInfo(Item item)
        {
            base.ReadItemInfo(item);
            if (item is Keycard keycarditem)
            {
                Permissions = keycarditem.Permissions;
            }
        }

        /// <inheritdoc/>
        protected override void InitializeProperties(ItemBase itemBase)
        {
            base.InitializeProperties(itemBase);
            if (itemBase is KeycardItem keycardItem)
            {
                foreach (DetailBase detail in keycardItem.Details)
                {
                    switch (detail)
                    {
                        case PredefinedPermsDetail predefinedPermsDetail:
                            Permissions = (KeycardPermissions)predefinedPermsDetail.Levels.Permissions;
                            return;
                        case CustomPermsDetail customPermsDetail:
                            Permissions = (KeycardPermissions)customPermsDetail.GetPermissions(null);
                            return;
                    }
                }
            }
        }
    }
}
