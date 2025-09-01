// -----------------------------------------------------------------------
// <copyright file="SingleUseKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.Keycards
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Keycards;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;

    /// <summary>
    /// A base class for SingleUse keycard items.
    /// </summary>
    public class SingleUseKeycard : Keycard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleUseKeycard"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        internal SingleUseKeycard(SingleUseKeycardItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleUseKeycard"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        internal SingleUseKeycard(ItemType type)
            : this((SingleUseKeycardItem)Server.Host.Inventory.CreateItemInstance(new(type, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="KeycardItem"/> this encapsulates.
        /// </summary>
        public new SingleUseKeycardItem Base { get; }

        /// <summary>
        /// Gets or sets the amount of uses remaining on the keycard.
        /// </summary>
        /// <remarks>Multiple uses can look strange for clients (they'll run the discard animation, but can reselect the keycard)
        /// <br/><br/>
        /// Setting this to 0 or lower will destroy the keycard.
        /// </remarks>
        public int Uses
        {
            get => RemainingUses.TryGetValue(Serial, out int value) ? value : 1;
            set
            {
                if (value <= 0)
                {
                    IsDestroyed = true;
                }

                RemainingUses[Serial] = value;
            }
        }

        /// <summary>
        /// Gets or sets the time delay to destroy the Keycard after being used.
        /// </summary>
        public float TimeToDestroy
        {
            get => Base._timeToDestroy;
            set => Base._timeToDestroy = value;
        }

        /// <inheritdoc/>
        public override KeycardPermissions Permissions
        {
            get => (KeycardPermissions)Base._singleUsePermissions;
            set => Base._singleUsePermissions = (DoorPermissionFlags)value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Keycard allow the closing of Doors.
        /// </summary>
        public bool AllowClosingDoors
        {
            get => Base._allowClosingDoors;
            set => Base._allowClosingDoors = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Keycard is destroyed.
        /// </summary>
        public bool IsDestroyed
        {
            get => Base._destroyed;
            set => Base._destroyed = value;
        }

        /// <summary>
        /// Gets a dictionary storing all remaining uses for every <see cref="SingleUseKeycard"/>.
        /// </summary>
        internal static Dictionary<ushort, int> RemainingUses { get; } = new();

        /// <summary>
        /// Returns the Keycard in a human readable format.
        /// </summary>
        /// <returns>A string containing Keycard-related data.</returns>
        public override string ToString() => $"{Type} ={AllowClosingDoors}= ({Serial}) [{Weight}] *{Scale}* |{Permissions}|";

        /// <inheritdoc/>
        internal override void ReadPickupInfoBefore(Pickup pickup)
        {
            if (pickup is SingleUseKeycardPickup singleUseKeycard)
            {
                Uses = singleUseKeycard.Uses;
                TimeToDestroy = singleUseKeycard.TimeToDestroy;
                Permissions = singleUseKeycard.Permissions;
                AllowClosingDoors = singleUseKeycard.AllowClosingDoors;
            }
        }
    }
}