// -----------------------------------------------------------------------
// <copyright file="ConsumingItemEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a player's consumable item effects are applied.
    /// </summary>
    public class ConsumingItemEventArgs : IPlayerEvent, IDeniableEvent, IConsumableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumingItemEventArgs" /> class.
        /// </summary>
        /// <param name="hub">The player who is consuming the item.</param>
        /// <param name="item">The consumable item to be consumed.</param>
        public ConsumingItemEventArgs(ReferenceHub hub, InventorySystem.Items.Usables.Consumable item)
        {
            Player = Player.Get(hub);
            Consumable = Item.Get(item) as Consumable;
        }

        /// <summary>
        /// Gets the consumable item to be consumed.
        /// </summary>
        public Consumable Consumable { get; }

        /// <inheritdoc/>
        public Item Item => Consumable;

        /// <summary>
        /// Gets the player who is consuming the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the item's being consumed should be allowed or not.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
