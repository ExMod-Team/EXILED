// -----------------------------------------------------------------------
// <copyright file="CustomKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.Keycards
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;
    using Exiled.API.Interfaces.Keycards;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// A base class for all keycard items.
    /// </summary>
    public abstract class CustomKeycard : Keycard
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
            Base = itemBase;

            Resync();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomKeycard"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        internal CustomKeycard(ItemType type)
            : this((KeycardItem)Server.Host.Inventory.CreateItemInstance(new(type, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="KeycardItem"/> this encapsulates.
        /// </summary>
        public new KeycardItem Base { get; }

        /// <summary>
        /// Gets or sets the permissions this keycard has.
        /// </summary>
        public override KeycardPermissions Permissions
        {
            get => CustomPermsDetail.CustomPermissions.TryGetValue(Serial, out DoorPermissionFlags flags) ? (KeycardPermissions)flags : KeycardPermissions.None;

            set
            {
                CustomPermsDetail.CustomPermissions[Serial] = (DoorPermissionFlags)value;

                Resync();
            }
        }

        /// <summary>
        /// Gets or sets the permissions this keycard has.
        /// </summary>
        public KeycardLevels KeycardLevels
        {
            get => new((DoorPermissionFlags)Permissions);

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
            get => PermsColorDict.TryGetValue(Serial, out Color32 value) ? value : Color.black;

            set
            {
                PermsColorDict[Serial] = value;

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
            get => ColorDict.TryGetValue(Serial, out Color32 value) ? value : Color.black;

            set
            {
                ColorDict[Serial] = value;

                Resync();
            }
        }

        /// <summary>
        /// Gets a dictionary of item serials to their permissions color.
        /// </summary>
        internal static Dictionary<ushort, Color32> PermsColorDict { get; } = new();

        /// <summary>
        /// Gets a dictionary of item serials to their color.
        /// </summary>
        internal static Dictionary<ushort, Color32> ColorDict { get; } = new();

        /// <summary>
        /// Gets a dictionary of item serials to their name tag.
        /// </summary>
        internal static Dictionary<ushort, string> NameTagDict { get; } = new();

        /// <summary>
        /// Gets a dictionary of item serials to their label.
        /// </summary>
        internal static Dictionary<ushort, string> LabelDict { get; } = new();

        /// <summary>
        /// Gets a dictionary of item serials to their label's color.
        /// </summary>
        internal static Dictionary<ushort, Color32> LabelColorDict { get; } = new();

        /// <summary>
        /// Gets a dictionary of item serials to their wear.
        /// </summary>
        internal static Dictionary<ushort, byte> WearDict { get; } = new();

        /// <summary>
        /// Gets a dictionary of item serials to their serial number.
        /// </summary>
        internal static Dictionary<ushort, string> SerialNumberDict { get; } = new();

        /// <summary>
        /// Gets a dictionary of item serials to their serial number.
        /// </summary>
        internal static Dictionary<ushort, byte> RankDict { get; } = new();

        /// <summary>
        /// Resyncs all properties of the keycard.
        /// Gets called by all setters by default.
        /// </summary>
        public void Resync()
        {
            // we loveeeeeeeeeeeee NW static fields trusttttttttttttttt I'm not mad at allllllllllll
            CustomPermsDetail._customLevels = KeycardLevels;
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

            if (this is ISerialNumberKeycard serial)
                CustomSerialNumberDetail._customVal = serial.SerialNumber;

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
            return (T)Base.Details.First(detail => detail is T);
        }
    }
}