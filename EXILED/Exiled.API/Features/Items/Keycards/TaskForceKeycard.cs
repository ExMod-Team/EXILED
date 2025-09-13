// -----------------------------------------------------------------------
// <copyright file="TaskForceKeycard.cs" company="ExMod Team">
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
    /// Represents the Task Force Custom Keycard.
    /// </summary>
    public class TaskForceKeycard : CustomKeycardItem, INameTagKeycard, ISerialNumberKeycard, IRankKeycard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskForceKeycard"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        internal TaskForceKeycard(KeycardItem itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskForceKeycard"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        internal TaskForceKeycard(ItemType type)
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

        /// <inheritdoc cref="IRankKeycard.Rank"/>
        public byte Rank
        {
            get => DataDict[Serial].Rank;
            set
            {
                DataDict[Serial].Rank = value;

                Resync();
            }
        }

        /// <summary>
        /// Creates a <see cref="TaskForceKeycard"/>.
        /// </summary>
        /// <param name="keycardLevels">The permissions of the keycard.</param>
        /// <param name="permissionsColor">The color of the permissions of the keycard.</param>
        /// <param name="itemName">The inventory name of the keycard.</param>
        /// <param name="color">The color of the keycard.</param>
        /// <param name="nameTag">The name of the owner of the keycard.</param>
        /// <param name="serialNumber">The serial number of the keycard (numbers only, 12 max).</param>
        /// <param name="rank">The rank of the keycard (capped from 0-3).</param>
        /// <returns>The new <see cref="TaskForceKeycard"/>.</returns>
        public static TaskForceKeycard Create(KeycardLevels keycardLevels, Color permissionsColor, string itemName, Color color, string nameTag, string serialNumber, byte rank)
        {
            TaskForceKeycard keycard = Create<TaskForceKeycard>(ItemType.KeycardCustomTaskForce);
            keycard.KeycardLevels = keycardLevels;
            keycard.PermissionsColor = permissionsColor;
            keycard.ItemName = itemName;
            keycard.Color = color;
            keycard.NameTag = nameTag;
            keycard.SerialNumber = serialNumber;
            keycard.Rank = rank;
            return keycard;
        }
    }
}