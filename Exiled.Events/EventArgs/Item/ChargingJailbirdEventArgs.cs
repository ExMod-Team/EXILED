// -----------------------------------------------------------------------
// <copyright file="ChargingJailbirdEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a player starts charging an <see cref="Jailbird"/>.
    /// </summary>
    public class ChargingJailbirdEventArgs : IItemEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChargingJailbirdEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who is attempting to charge the Jailbird.</param>
        /// <param name="jailbird">The jailbird being charged.</param>
        /// <param name="isAllowed">Whether the item is allowed to be charged.</param>
        public ChargingJailbirdEventArgs(ReferenceHub player, InventorySystem.Items.ItemBase jailbird, bool isAllowed = true)
        {
            Player = Player.Get(player);
            Jailbird = Item.Get<Jailbird>(jailbird);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's charging an item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Jailbird"/> that is being charged.
        /// </summary>
        public Jailbird Jailbird { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Item"/> that is being charged.
        /// </summary>
        public Item Item => Jailbird;

        /// <summary>
        /// Gets or sets a value indicating whether the Jailbird can be charged.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
