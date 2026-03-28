// -----------------------------------------------------------------------
// <copyright file="ReloadingWeaponEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    /// <summary>
    /// Contains all information before a player's weapon is reloaded.
    /// </summary>
    public class ReloadingWeaponEventArgs : IPlayerEvent, IFirearmEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReloadingWeaponEventArgs" /> class.
        /// </summary>
        /// <param name="firearm">The firearm being reloaded.</param>
        /// <param name="isAllowed">A value indicating whether the weapon can be reloaded.</param>
        public ReloadingWeaponEventArgs(InventorySystem.Items.Firearms.Firearm firearm, bool isAllowed)
        {
            Firearm = Item.Get<Firearm>(firearm);
            Player = Firearm.Owner;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the weapon can be reloaded.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm" /> being reloaded.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the player who's reloading the weapon.
        /// </summary>
        public Player Player { get; }
    }
}
