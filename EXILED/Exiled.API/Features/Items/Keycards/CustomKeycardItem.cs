// -----------------------------------------------------------------------
// <copyright file="CustomKeycardItem.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.Keycards
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Pools;
    using Exiled.API.Interfaces.Keycards;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;
    using UnityEngine;

    /// <summary>
    /// A base class for all custom keycard items.
    /// </summary>
    public abstract class CustomKeycardItem : Keycard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomKeycardItem"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        internal CustomKeycardItem(KeycardItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;

            if (!DataDict.ContainsKey(Serial))
                DataDict[Serial] = new KeycardData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomKeycardItem"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        internal CustomKeycardItem(ItemType type)
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
            get => DataDict[Serial].PermissionsColor ?? Color.clear;
            set
            {
                DataDict[Serial].PermissionsColor = value;

                Resync();
            }
        }

        /// <summary>
        /// Gets or sets the name of this keycard within the inventory.
        /// </summary>
        public string ItemName
        {
            get => DataDict[Serial].ItemName;
            set
            {
                DataDict[Serial].ItemName = value;

                Resync();
            }
        }

        /// <summary>
        /// Gets or sets the color of this keycard.
        /// </summary>
        public Color Color
        {
            get => DataDict[Serial].Color ?? Color.clear;
            set
            {
                DataDict[Serial].Color = value;

                Resync();
            }
        }

        /// <summary>
        /// Gets a dictionary of item serials to their keycards data.
        /// </summary>
        internal static Dictionary<ushort, KeycardData> DataDict { get; } = new();

        /// <summary>
        /// Gets an array of ItemTypes of each keycard.
        /// </summary>
        internal static ItemType[] AllKeycards { get; } =
        {
            ItemType.KeycardJanitor, ItemType.KeycardScientist, ItemType.KeycardResearchCoordinator, ItemType.KeycardContainmentEngineer, ItemType.KeycardZoneManager, ItemType.KeycardFacilityManager, ItemType.KeycardGuard, ItemType.KeycardMTFPrivate, ItemType.KeycardMTFOperative, ItemType.KeycardMTFCaptain, ItemType.KeycardChaosInsurgency, ItemType.KeycardO5,
        };

        /// <summary>
        /// Gets an array of ItemTypes of each Site02 keycard.
        /// </summary>
        internal static ItemType[] AllSite02 { get; } = { ItemType.KeycardJanitor, ItemType.KeycardScientist, ItemType.KeycardResearchCoordinator, ItemType.KeycardContainmentEngineer };

        /// <summary>
        /// Gets an array of ItemTypes of each Management keycard.
        /// </summary>
        internal static ItemType[] AllManagement { get; } = { ItemType.KeycardZoneManager, ItemType.KeycardFacilityManager };

        /// <summary>
        /// Gets an array of ItemTypes of each MetalCase keycard.
        /// </summary>
        internal static ItemType[] AllMetalCase { get; } = { ItemType.KeycardGuard };

        /// <summary>
        /// Gets an array of ItemTypes of each TaskForce keycard.
        /// </summary>
        internal static ItemType[] AllTaskForce { get; } = { ItemType.KeycardMTFPrivate, ItemType.KeycardMTFOperative, ItemType.KeycardMTFCaptain };

        /// <summary>
        /// Finds a <see cref="ItemType"/> for a keycard by checking if keycard properties match.
        /// </summary>
        /// /// <param name="matchDesign">Whether to use design to check keycards.</param>
        /// <param name="matchPerms">Whether to use permissions to check keycards.</param>
        /// <param name="matchColors">Whether to use colors to check keycards.</param>
        /// <returns>If 1 match is found, returns the matched value. Otherwise, returns <see cref="ItemType"/>.<see cref="ItemType.None"/>.</returns>
        /// <remarks>Unoptimized for now, but shouldn't be too bad.</remarks>
        public ItemType FindMatch(bool matchDesign, bool matchPerms, bool matchColors)
        {
            List<ItemType> matches = ListPool<ItemType>.Pool.Get();

            ItemType[] toIterate = Type switch
            {
                _ when !matchDesign => AllKeycards,
                ItemType.KeycardCustomSite02 => AllSite02,
                ItemType.KeycardCustomManagement => AllManagement,
                ItemType.KeycardCustomMetalCase => AllMetalCase,
                ItemType.KeycardCustomTaskForce => AllTaskForce,
                _ => throw new ArgumentOutOfRangeException(nameof(Type), Type.ToString()),
            };

            ILabelKeycard label1 = this as ILabelKeycard;
            foreach (ItemType type in toIterate)
            {
                KeycardItem keycard = type.GetTemplate<KeycardItem>();

                foreach (DetailBase detail in keycard.Details)
                {
                    if (detail is PredefinedPermsDetail permsDetail)
                    {
                        if (matchPerms && permsDetail.Levels.Permissions != KeycardLevels.Permissions)
                            goto cont;
                    }

                    if (label1 is not null && detail is TranslatedLabelDetail label2)
                    {
                        if (matchColors && label1.LabelColor != label2._textColor)
                            goto cont;
                    }
                }

                goto add;

                cont:
                continue;

                add:
                matches.Add(type);
            }

            ItemType value = matches.Count is not 1 ? ItemType.None : matches[0];

            ListPool<ItemType>.Pool.Return(matches);

            return value;
        }

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

            if (this is IRankKeycard rank)
                CustomRankDetail._index = rank.Rank;

            MirrorExtensions.ResyncKeycardItem(this);
        }

        /// <summary>
        /// Returns the Keycard in a human readable format.
        /// </summary>
        /// <returns>A string containing Keycard-related data.</returns>
        public override string ToString() => $"{Type} ={ItemName}= ({Serial}) [{Weight}] *{Scale}* |{Permissions}|";
    }
}