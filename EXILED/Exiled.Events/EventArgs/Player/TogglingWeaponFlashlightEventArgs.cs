// -----------------------------------------------------------------------
// <copyright file="TogglingWeaponFlashlightEventArgs.cs" company="ExMod Team">
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
    /// Contains all information before a player toggles the weapon's flashlight.
    /// </summary>
    public class TogglingWeaponFlashlightEventArgs : IPlayerEvent, IFirearmEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TogglingWeaponFlashlightEventArgs" /> class.
        /// </summary>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm" />
        /// </param>
        /// <param name="oldState">
        /// <inheritdoc cref="NewState" />
        /// </param>
        public TogglingWeaponFlashlightEventArgs(BaseFirearm firearm, bool oldState)
        {
            Firearm = Item.Get<Firearm>(firearm);
            Player = Firearm.Owner;
            NewState = !oldState;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the new weapon's flashlight state will be enabled.
        /// </summary>
        public bool NewState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the weapon's flashlight can be toggled.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm" /> being held.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the player who's toggling the weapon's flashlight.
        /// </summary>
        public Player Player { get; }
    }
}