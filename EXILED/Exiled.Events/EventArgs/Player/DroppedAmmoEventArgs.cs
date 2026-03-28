// -----------------------------------------------------------------------
// <copyright file="DroppedAmmoEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System.Collections.Generic;

    using API.Enums;
    using API.Features;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Pickups;
    using Interfaces;

    using AmmoPickup = API.Features.Pickups.AmmoPickup;

    /// <summary>
    /// Contains all information after a player drops ammo.
    /// </summary>
    public class DroppedAmmoEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DroppedAmmoEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="itemType">
        /// <inheritdoc cref="ItemType"/>
        /// </param>
        /// <param name="amount">
        /// <inheritdoc cref="Amount" />
        /// </param>
        /// <param name="ammoPickups">
        /// <inheritdoc cref="AmmoPickups" />
        /// </param>
        public DroppedAmmoEventArgs(Player player, ItemType itemType, ushort amount, List<InventorySystem.Items.Firearms.Ammo.AmmoPickup> ammoPickups)
        {
            Player = player;
            ItemType = itemType;
            AmmoType = ItemExtensions.GetAmmoType(itemType);
            Amount = amount;
            AmmoPickups = Pickup.Get<AmmoPickup>(ammoPickups);
        }

        /// <summary>
        /// Gets the type of dropped item instead of <see cref="API.Enums.AmmoType"/>.
        /// </summary>
        public ItemType ItemType { get; }

        /// <summary>
        /// Gets the type of dropped ammo.
        /// </summary>
        public AmmoType AmmoType { get; }

        /// <summary>
        /// Gets the amount of dropped ammo.
        /// </summary>
        public ushort Amount { get; }

        /// <summary>
        /// Gets the dropped ammo pickups.
        /// </summary>
        public IEnumerable<AmmoPickup> AmmoPickups { get; }

        /// <summary>
        /// Gets the player who dropped the ammo.
        /// </summary>
        public Player Player { get; }
    }
}
