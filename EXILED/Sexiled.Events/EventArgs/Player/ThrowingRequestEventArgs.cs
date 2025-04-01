// -----------------------------------------------------------------------
// <copyright file="ThrowingRequestEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.EventArgs.Player
{
    using Sexiled.API.Enums;
    using Sexiled.API.Features;
    using Sexiled.API.Features.Items;
    using Sexiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// Contains all information before receving a throwing request.
    /// </summary>
    public class ThrowingRequestEventArgs : IItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowingRequestEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Throwable"/></param>
        /// <param name="request"><inheritdoc cref="RequestType"/></param>
        public ThrowingRequestEventArgs(Player player, ThrowableItem item, ThrowableNetworkHandler.RequestType request)
        {
            Player = player;
            Throwable = Item.Get<Throwable>(item);
            RequestType = (ThrowRequest)request;
        }

        /// <summary>
        /// Gets the player who's sending the request.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the item being thrown.
        /// </summary>
        public Throwable Throwable { get; set; }

        /// <inheritdoc/>
        public Item Item => Throwable;

        /// <summary>
        ///  Gets or sets the type of throw being requested.
        /// </summary>
        public ThrowRequest RequestType { get; set; }
    }
}
