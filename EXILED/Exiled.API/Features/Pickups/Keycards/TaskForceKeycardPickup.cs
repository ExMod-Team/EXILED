// -----------------------------------------------------------------------
// <copyright file="TaskForceKeycardPickup.cs" company="ExMod Team">
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
    /// Represents the Task Force Custom Keycard.
    /// </summary>
    public class TaskForceKeycardPickup : CustomKeycardPickup, INameTagKeycard, ISerialNumberKeycard, IRankKeycard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskForceKeycardPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The <see cref="ItemPickupBase"/> to encapsulate.</param>
        internal TaskForceKeycardPickup(KeycardPickup pickupBase)
            : base(pickupBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskForceKeycardPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup to create.</param>
        internal TaskForceKeycardPickup(ItemType type)
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

        /// <inheritdoc cref="IRankKeycard.Rank"/>
        public byte Rank
        {
            get => CustomKeycardItem.DataDict[Serial].Rank;
            set
            {
                CustomKeycardItem.DataDict[Serial].Rank = value;

                Resync();
            }
        }

        /// <summary>
        /// Creates a <see cref="TaskForceKeycardPickup"/>.
        /// </summary>
        /// <param name="keycardLevels">The permissions of the keycard.</param>
        /// <param name="permissionsColor">The color of the permissions of the keycard.</param>
        /// <param name="itemName">The inventory name of the keycard.</param>
        /// <param name="color">The color of the keycard.</param>
        /// <param name="nameTag">The name of the owner of the keycard.</param>
        /// <param name="serialNumber">The serial number of the keycard (numbers only, 12 max).</param>
        /// <param name="rank">The rank of the keycard (capped from 0-3).</param>
        /// <returns>The new <see cref="TaskForceKeycardPickup"/>.</returns>
        public static TaskForceKeycardPickup Create(KeycardLevels keycardLevels, Color permissionsColor, string itemName, Color color, string nameTag, string serialNumber, byte rank)
        {
            TaskForceKeycardPickup keycardPickup = Create<TaskForceKeycardPickup>(ItemType.KeycardCustomTaskForce);
            keycardPickup.KeycardLevels = keycardLevels;
            keycardPickup.PermissionsColor = permissionsColor;
            keycardPickup.ItemName = itemName;
            keycardPickup.Color = color;
            keycardPickup.NameTag = nameTag;
            keycardPickup.SerialNumber = serialNumber;
            keycardPickup.Rank = rank;
            return keycardPickup;
        }
    }
}