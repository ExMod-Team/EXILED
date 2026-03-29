// -----------------------------------------------------------------------
// <copyright file="Keycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items.Keycards;

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
        /// <param name="owner">The owner of the grenade. Leave <see langword="null"/> for no owner.</param>
        internal Keycard(ItemType type, Player owner = null)
            : this((KeycardItem)(owner ?? Server.Host).Inventory.CreateItemInstance(new(type, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="KeycardItem"/> that this class is encapsulating.
        /// </summary>
        public new KeycardItem Base { get; }

        /// <summary>
        /// Gets or sets the <see cref="KeycardPermissions"/> of the keycard.
        /// </summary>
        public virtual KeycardPermissions Permissions
        {
            get
            {
                foreach (DetailBase detail in Base.Details)
                {
                    if (detail is IDoorPermissionProvider doorPermissionProvider)
                        return (KeycardPermissions)doorPermissionProvider.GetPermissions(null);
                }

                return KeycardPermissions.None;
            }

            set
            {
                foreach (DetailBase detail in Base.Details)
                {
                    if (detail is PredefinedPermsDetail doorPermissionProvider)
                    {
                        KeycardLevels keycardLevels = new((DoorPermissionFlags)value);
                        doorPermissionProvider._containmentLevel = keycardLevels.Containment;
                        doorPermissionProvider._armoryLevel = keycardLevels.Armory;
                        doorPermissionProvider._adminLevel = keycardLevels.Admin;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the Keycard in a human readable format.
        /// </summary>
        /// <returns>A string containing Keycard-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Permissions}|";
    }
}