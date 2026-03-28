// -----------------------------------------------------------------------
// <copyright file="ReloadedWeaponEventArgs.cs" company="ExMod Team">
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
    /// Contains all information after a player's weapon is reloaded.
    /// </summary>
    public class ReloadedWeaponEventArgs : IPlayerEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReloadedWeaponEventArgs" /> class.
        /// </summary>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm" />
        /// </param>
        public ReloadedWeaponEventArgs(InventorySystem.Items.Firearms.Firearm firearm)
        {
            Firearm = Item.Get<Firearm>(firearm);
            Player = Firearm.Owner;
        }

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