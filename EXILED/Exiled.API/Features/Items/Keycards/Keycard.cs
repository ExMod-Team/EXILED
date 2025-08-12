// -----------------------------------------------------------------------
// <copyright file="Keycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using InventorySystem.Items.Keycards;
namespace Exiled.API.Features.Items.Keycards
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using InventorySystem.Items;

    /// <summary>
    /// A base class for all keycard items.
    /// </summary>
    public class Keycard : Item
    {
        private CustomPermsDetail permsDetail;

        /// <summary>
        /// Initializes a new instance of the <see cref="Keycard"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        public Keycard(ItemBase itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Keycard"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        internal Keycard(ItemType type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating the permissions this keycard has.
        /// </summary>
        public KeycardPermissions Permissions
        {
            get => permsDetail;
            set
            {
                permsDetail._customLevels.
                Resync();
            }
        }

        /// <summary>
        /// Resyncs all properties of the keycard.
        /// Gets called by all setters by default.
        /// </summary>
        public void Resync()
        {
            MirrorExtensions.ResyncKeycardItem(this);
        }
    }
}