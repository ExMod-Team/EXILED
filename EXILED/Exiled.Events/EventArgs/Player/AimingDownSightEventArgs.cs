// -----------------------------------------------------------------------
// <copyright file="AimingDownSightEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using FirearmBase = InventorySystem.Items.Firearms.Firearm;

    /// <summary>
    /// Contains all information when a player aims.
    /// </summary>
    public class AimingDownSightEventArgs : IPlayerEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AimingDownSightEventArgs" /> class.
        /// </summary>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm" />
        /// </param>
        /// <param name="adsIn">
        /// <inheritdoc cref="AdsIn" />
        /// </param>
        public AimingDownSightEventArgs(FirearmBase firearm, bool adsIn)
        {
            Firearm = Item.Get<Firearm>(firearm);
            Player = Firearm.Owner;
            AdsIn = adsIn;
        }

        /// <summary>
        /// Gets a value indicating whether the player is aiming down sight in.
        /// </summary>
        public bool AdsIn { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm" /> used to trigger the aim action.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the player who's triggering the aim action.
        /// </summary>
        public Player Player { get; }
    }
}