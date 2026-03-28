// -----------------------------------------------------------------------
// <copyright file="DryfiringWeaponEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using BaseFirearm = InventorySystem.Items.Firearms.Firearm;

    /// <summary>
    /// Contains all information before a player's weapon is dryfired.
    /// </summary>
    public class DryfiringWeaponEventArgs : IPlayerEvent, IFirearmEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DryfiringWeaponEventArgs" /> class.
        /// </summary>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm" />
        /// </param>
        public DryfiringWeaponEventArgs(BaseFirearm firearm)
        {
            Firearm = Item.Get<Firearm>(firearm);
            Player = Firearm.Owner;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the weapon can be dryfired.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm" /> being dryfired.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the player who's dryfiring the weapon.
        /// </summary>
        public Player Player { get; }
    }
}