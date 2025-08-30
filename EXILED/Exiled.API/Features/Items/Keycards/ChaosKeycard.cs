// -----------------------------------------------------------------------
// <copyright file="ChaosKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.Keycards
{
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;
    using InventorySystem.Items.Keycards.Snake;

    /// <summary>
    /// A base class for chaos keycard items.
    /// </summary>
    public class ChaosKeycard : Keycard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosKeycard"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        internal ChaosKeycard(ChaosKeycardItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosKeycard"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        internal ChaosKeycard(ItemType type)
            : this((ChaosKeycardItem)Server.Host.Inventory.CreateItemInstance(new(type, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="KeycardItem"/> this encapsulates.
        /// </summary>
        public new ChaosKeycardItem Base { get; }

        /// <summary>
        /// Gets the <see cref="InventorySystem.Items.Keycards.Snake.SnakeEngine"/> this encapsulates.
        /// </summary>
        public SnakeEngine SnakeEngine => Base._localEngine;

        /// <summary>
        /// Returns the Keycard in a human readable format.
        /// </summary>
        /// <returns>A string containing Keycard-related data.</returns>
        public override string ToString() => $"{Type} == ({Serial}) [{Weight}] *{Scale}* |{Permissions}|";
    }
}