// -----------------------------------------------------------------------
// <copyright file="UsedItemEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using InventorySystem.Items.Usables;

    /// <summary>
    /// Contains all information after a player used an item.
    /// </summary>
    public class UsedItemEventArgs : IPlayerEvent, IUsableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsedItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="item">
        /// <inheritdoc cref="Item" />
        /// </param>
        /// <param name="causedByHolstering">
        /// <inheritdoc cref="CausedByHolstering"/>
        /// </param>
        public UsedItemEventArgs(ReferenceHub player, UsableItem item, bool causedByHolstering)
        {
            Player = Player.Get(player);
            Usable = Item.Get(item) as Usable;
            CausedByHolstering = causedByHolstering;
        }

        /// <summary>
        /// Gets the item that the player used.
        /// </summary>
        public Usable Usable { get; }

        /// <inheritdoc/>
        public Item Item => Usable;

        /// <summary>
        /// Gets the player who used the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets a value indicating whether this event was triggered by a player switching items (true) or by waiting after using the item (false).
        /// </summary>
        /// <remarks>Use this value if you wish to keep the bug where you could switch items quickly to skip this event.</remarks>
        public bool CausedByHolstering { get; }
    }
}