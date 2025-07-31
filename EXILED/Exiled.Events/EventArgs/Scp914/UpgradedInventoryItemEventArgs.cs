// -----------------------------------------------------------------------
// <copyright file="UpgradedInventoryItemEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp914
{
    using API.Features;
    using API.Features.Items;
    using global::Scp914;
    using Interfaces;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information before SCP-914 upgrades an item.
    /// </summary>
    public class UpgradedInventoryItemEventArgs : IItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradedInventoryItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="item">
        /// <inheritdoc cref="Item" />
        /// </param>
        /// <param name="knobSetting">
        /// <inheritdoc cref="KnobSetting" />
        /// </param>
        /// <param name="result">
        /// <inheritdoc cref="Result" />
        /// </param>
        public UpgradedInventoryItemEventArgs(Player player, ItemBase item, Scp914KnobSetting knobSetting, ItemBase[] result)
        {
            Player = player;
            Item = Item.Get(item);
            KnobSetting = knobSetting;
            Result = result;
        }

        /// <summary>
        /// Gets SCP-914 working knob setting.
        /// </summary>
        public Scp914KnobSetting KnobSetting { get; }

        /// <summary>
        /// Gets a list of items to be upgraded inside SCP-914.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets the <see cref="Player" /> who owns the item to be upgraded.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the array of items created as a result of SCP-914 upgraded.
        /// </summary>
        public ItemBase[] Result { get; }
    }
}