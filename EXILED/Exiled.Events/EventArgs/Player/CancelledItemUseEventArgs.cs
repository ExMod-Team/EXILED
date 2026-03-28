// -----------------------------------------------------------------------
// <copyright file="CancelledItemUseEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Usables;

    /// <summary>
    /// Contains all information before a player cancels usage of an item.
    /// </summary>
    public class CancelledItemUseEventArgs : IPlayerEvent, IUsableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelledItemUseEventArgs" /> class.
        /// </summary>
        /// <param name="hub">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="usableItem">
        /// <inheritdoc cref="Usable" />
        /// </param>
        public CancelledItemUseEventArgs(ReferenceHub hub, UsableItem usableItem)
        {
            Player = Player.Get(hub);
            Usable = Item.Get<Usable>(usableItem);
        }

        /// <summary>
        /// Gets the item that the player cancelling.
        /// </summary>
        public Usable Usable { get; }

        /// <inheritdoc/>
        public Item Item => Usable;

        /// <summary>
        /// Gets the player who cancelling the item.
        /// </summary>
        public Player Player { get; }
    }
}