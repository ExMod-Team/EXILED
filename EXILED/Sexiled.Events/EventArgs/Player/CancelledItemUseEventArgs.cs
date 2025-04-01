// -----------------------------------------------------------------------
// <copyright file="CancelledItemUseEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.EventArgs.Player
{
    using API.Features;
    using Sexiled.API.Features.Items;
    using Sexiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a player cancels usage of an item.
    /// </summary>
    public class CancelledItemUseEventArgs : IPlayerEvent, IUsableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelledItemUseEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player who's stopping the use of an item.</param>
        /// <param name="item">
        /// <inheritdoc cref="Usable" />
        /// </param>
        public CancelledItemUseEventArgs(Player player, Item item)
        {
            Player = player;
            Usable = item is Usable usable ? usable : null;
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