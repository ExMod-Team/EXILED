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
    using Exiled.API.Interfaces;
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

            [Obsolete("Not functional anymore", true)]
            set => _ = value;
        }

        /// <summary>
        /// Returns the Keycard in a human readable format.
        /// </summary>
        /// <returns>A string containing Keycard-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Permissions}|";
    }
}