// -----------------------------------------------------------------------
// <copyright file="JailbirdChargeCompleteEventArgs.cs" company="ExMod Team">
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
    /// Contains all information when a player completes charging a <see cref="Jailbird"/>.
    /// </summary>
    public class JailbirdChargeCompleteEventArgs : IItemEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JailbirdChargeCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who completed the charge.</param>
        /// <param name="jailbird">The Jailbird item whose charge is complete.</param>
        /// <param name="isAllowed">Whether the Jailbird is allowed to attack after charging.</param>
        public JailbirdChargeCompleteEventArgs(ReferenceHub player, InventorySystem.Items.ItemBase jailbird, bool isAllowed = true)
        {
            Player = Player.Get(player);
            Jailbird = Item.Get<Jailbird>(jailbird);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who completed the charge.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Jailbird"/> that was fully charged.
        /// </summary>
        public Jailbird Jailbird { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Item"/> associated with the charged Jailbird.
        /// </summary>
        public Item Item => Jailbird;

        /// <summary>
        /// Gets or sets a value indicating whether the Jailbird is allowed to attack after charging.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
