// -----------------------------------------------------------------------
// <copyright file="CustomKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.Keycards
{
    using System;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Interfaces.Keycards;

    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;

    using UnityEngine;

    /// <summary>
    /// A base class for all keycard items.
    /// </summary>
    public abstract class CustomKeycard : Item
    {
        private CustomItemNameDetail itemNameDetail;
        private bool itemNameSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomKeycard"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        internal CustomKeycard(KeycardItem itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomKeycard"/> class.
        /// </summary>
        /// <param name="itemType">The <see cref="ItemType"/> of the item to create.</param>
        internal CustomKeycard(ItemType itemType)
            : base(itemType)
        {
        }

        /// <summary>
        /// Gets or sets the permissions this keycard has.
        /// </summary>
        public KeycardLevels Permissions
        {
            get => CustomPermsDetail.CustomPermissions.TryGetValue(Serial, out DoorPermissionFlags flags) ? new KeycardLevels(flags) : new KeycardLevels(0, 0, 0);

            set
            {
                CustomPermsDetail.CustomPermissions[Serial] = value.Permissions;

                Resync();
            }
        }

        /// <summary>
        /// Gets or sets the color of this keycard's permissions.
        /// </summary>
        public Color PermissionsColor
        {
            get => Gfx.Material.Instance.GetColor(KeycardGfx.PermsColorHash);

            set
            {
                Gfx.Material.Instance.SetColor(KeycardGfx.PermsColorHash, value);

                Resync();
            }
        }

        /// <summary>
        /// Gets or sets the name of this keycard within the inventory.
        /// </summary>
        public string ItemName
        {
            get
            {
                if (!itemNameSet)
                {
                    itemNameDetail = GetDetail<CustomItemNameDetail>();
                    itemNameSet = true;
                }

                return itemNameDetail.Name;
            }

            set
            {
                if (!itemNameSet)
                {
                    itemNameDetail = GetDetail<CustomItemNameDetail>();
                    itemNameSet = true;
                }

                itemNameDetail.Name = value;

                Resync();
            }
        }

        /// <summary>
        /// Gets or sets the color of this keycard.
        /// </summary>
        public Color Color
        {
            get => Gfx.Material.Instance.GetColor(KeycardGfx.TintColorHash);

            set
            {
                Gfx.Material.Instance.SetColor(KeycardGfx.TintColorHash, value);

                Resync();
            }
        }

        /// <summary>
        /// Gets the <see cref="InventorySystem.Items.Keycards.KeycardGfx"/> of this <see cref="CustomKeycard"/>.
        /// </summary>
        public KeycardGfx Gfx => ((KeycardItem)Base).KeycardGfx;

        /// <summary>
        /// Resyncs all properties of the keycard.
        /// Gets called by all setters by default.
        /// </summary>
        public void Resync()
        {
            // we loveeeeeeeeeeeee NW static fields trusttttttttttttttt I'm not mad at allllllllllll
            CustomPermsDetail._customLevels = Permissions;
            CustomPermsDetail._customColor = PermissionsColor;

            CustomItemNameDetail._customText = ItemName;
            CustomTintDetail._customColor = Color;

            if (this is ILabelKeycard label)
            {
                CustomLabelDetail._customText = label.Label;
                CustomLabelDetail._customColor = label.LabelColor;
            }

            if (this is INameTagKeycard holder)
                NametagDetail._customNametag = holder.NameTag;

            if (this is IWearKeycard wear)
                CustomWearDetail._customWearLevel = wear.Wear;

            if (this is ISerialLabelKeycard serial)
            {
                CustomSerialNumberDetail._customVal = serial.SerialLabel;
            }

            MirrorExtensions.ResyncKeycardItem(this);
        }

        /// <summary>
        /// Gets a specific <see cref="SyncedDetail"/> detail from a <see cref="CustomKeycard"/>.
        /// </summary>
        /// <typeparam name="T">The Type of SyncedDetail.</typeparam>
        /// <returns>The first detail of the desired type. null if not found.</returns>
        public T GetDetail<T>()
            where T : DetailBase
        {
            if (Base is not KeycardItem keycardItem)
                return null;

            return (T)keycardItem.Details.FirstOrDefault(detail => detail is T);
        }

        /// <summary>
        /// Creates a <see cref="CustomKeycard"/> based on its ItemType.
        /// </summary>
        /// <param name="itemBase">The item to create the wrapper from.</param>
        /// <returns>The newly created <see cref="CustomKeycard"/>.</returns>
        internal static CustomKeycard GetKeycard(KeycardItem itemBase)
        {
            return itemBase.ItemTypeId switch
            {
                ItemType.KeycardCustomTaskForce => new TaskForceKeycard(itemBase),
                ItemType.KeycardCustomSite02 => new Site02Keycard(itemBase),
                ItemType.KeycardCustomManagement => new ManagementKeycard(itemBase),
                ItemType.KeycardCustomMetalCase => new MetalKeycard(itemBase),
                _ => throw new ArgumentOutOfRangeException(nameof(ItemType), itemBase.ItemTypeId.ToString()),
            };
        }

        /// <summary>
        /// Creates a <see cref="CustomKeycard"/> based on its ItemType.
        /// </summary>
        /// <param name="type">The type to create the wrapper from.</param>
        /// <returns>The newly created <see cref="CustomKeycard"/>.</returns>
        internal static CustomKeycard GetKeycard(ItemType type)
        {
            return type switch
            {
                ItemType.KeycardCustomTaskForce => new TaskForceKeycard(type),
                ItemType.KeycardCustomSite02 => new Site02Keycard(type),
                ItemType.KeycardCustomManagement => new ManagementKeycard(type),
                ItemType.KeycardCustomMetalCase => new MetalKeycard(type),
                _ => throw new ArgumentOutOfRangeException(nameof(ItemType), type.ToString()),
            };
        }
    }
}