// -----------------------------------------------------------------------
// <copyright file="DroppingAmmoEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Enums;
    using API.Features;
    using Exiled.API.Extensions;
    using Interfaces;

    using PlayerRoles;

    /// <summary>
    /// Contains all information before a player drops ammo.
    /// </summary>
    public class DroppingAmmoEventArgs : IPlayerEvent, IDeniableEvent
    {
        private bool isAllowed = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DroppingAmmoEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="itemType">
        /// <inheritdoc cref="ItemType"/>
        /// </param>
        /// <param name="amount">
        /// <inheritdoc cref="int" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public DroppingAmmoEventArgs(Player player, ItemType itemType, ushort amount, bool isAllowed = true)
        {
            Player = player;
            ItemType = itemType;
            AmmoType = ItemExtensions.GetAmmoType(itemType);
            Amount = amount;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the type of item being dropped instead of <see cref="API.Enums.AmmoType"/>.
        /// For example, if the plugin gives the player one of the items instead of ammo.
        /// </summary>
        public ItemType ItemType { get; }

        /// <summary>
        /// Gets the type of ammo being dropped.
        /// </summary>
        public AmmoType AmmoType { get; }

        /// <summary>
        /// Gets or sets the amount of ammo being dropped.
        /// </summary>
        public ushort Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ammo can be dropped.
        /// </summary>
        public bool IsAllowed
        {
            get
            {
                if (Player.Role == RoleTypeId.Spectator)
                    isAllowed = true;
                return isAllowed;
            }

            set
            {
                if (Player.Role == RoleTypeId.Spectator)
                    value = true;
                isAllowed = value;
            }
        }

        /// <summary>
        /// Gets the player who's dropping the ammo.
        /// </summary>
        public Player Player { get; }
    }
}
