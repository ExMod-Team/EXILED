// -----------------------------------------------------------------------
// <copyright file="UnloadingWeaponEventArgs.cs" company="ExMod Team">
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
    /// Contains all information before a player's weapon is unloaded.
    /// </summary>
    public class UnloadingWeaponEventArgs : IPlayerEvent, IFirearmEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadingWeaponEventArgs" /> class.
        /// </summary>
        /// <param name="firearm">
        /// The firearm being unloaded.
        /// </param>
        /// <param name="isAllowed">
        /// Indicates whether the weapon unloading is allowed.
        /// </param>
        public UnloadingWeaponEventArgs(InventorySystem.Items.Firearms.Firearm firearm, bool isAllowed)
        {
            Firearm = Item.Get<Firearm>(firearm);
            Player = Firearm.Owner;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the weapon can be unloaded.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm" /> being unloaded.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the player who's unloading the weapon.
        /// </summary>
        public Player Player { get; }
    }
}
