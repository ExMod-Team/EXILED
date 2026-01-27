// -----------------------------------------------------------------------
// <copyright file="SingleUseKeycardPickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Keycards
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Items.Keycards;
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// A base class for all keycard pickups.
    /// </summary>
    public class SingleUseKeycardPickup : Pickups.KeycardPickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleUseKeycardPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The <see cref="ItemPickupBase"/> to encapsulate.</param>
        internal SingleUseKeycardPickup(KeycardPickup pickupBase)
            : base(pickupBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleUseKeycardPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup to create.</param>
        internal SingleUseKeycardPickup(ItemType type)
            : base(type)
        {
        }

        /// <inheritdoc cref="SingleUseKeycard.Uses"/>
        public int Uses { get; set; } = 1;

        /// <summary>
        /// Gets or sets the time delay to destroy the Keycard after being used.
        /// </summary>
        public float TimeToDestroy { get; set; }

        /// <inheritdoc/>
        public override KeycardPermissions Permissions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Keycard allow the closing of Doors.
        /// </summary>
        public bool AllowClosingDoors { get; set; }

        /// <summary>
        /// Returns the Keycard in a human readable format.
        /// </summary>
        /// <returns>A string containing Keycard-related data.</returns>
        public override string ToString() => $"{Type} ={AllowClosingDoors}= ({Serial}) [{Weight}] *{Scale}* |{Permissions}|";

        /// <inheritdoc/>
        internal override void ReadItemInfo(Item item)
        {
            base.ReadItemInfo(item);
            if (item is SingleUseKeycard singleUseKeycardItem)
            {
                Uses = singleUseKeycardItem.Uses;
                TimeToDestroy = singleUseKeycardItem.TimeToDestroy;
                Permissions = singleUseKeycardItem.Permissions;
                AllowClosingDoors = singleUseKeycardItem.AllowClosingDoors;
            }
        }

        /// <inheritdoc/>
        protected override void InitializeProperties(ItemBase itemBase)
        {
            base.InitializeProperties(itemBase);
            if (itemBase is SingleUseKeycardItem singleUseKeycardItem)
            {
                TimeToDestroy = singleUseKeycardItem._timeToDestroy;
                Permissions = (KeycardPermissions)singleUseKeycardItem._singleUsePermissions;
                AllowClosingDoors = singleUseKeycardItem._allowClosingDoors;
            }
        }
    }
}