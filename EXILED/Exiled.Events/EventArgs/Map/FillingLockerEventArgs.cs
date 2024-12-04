// -----------------------------------------------------------------------
// <copyright file="FillingLockerEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using System;

    using Exiled.API.Features.Lockers;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Pickups;
    using MapGeneration.Distributors;

    /// <summary>
    /// Contains all information before the server spawns an item in locker.
    /// </summary>
    public class FillingLockerEventArgs : IDeniableEvent, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FillingLockerEventArgs" /> class.
        /// </summary>
        /// <param name="pickupBase">
        /// <inheritdoc cref="Pickup" />
        /// </param>
        /// <param name="lockerChamber">
        /// <inheritdoc cref="LockerChamber" />
        /// </param>
        public FillingLockerEventArgs(ItemPickupBase pickupBase, LockerChamber lockerChamber)
        {
            Pickup = Pickup.Get(pickupBase);
            Chamber = Chamber.Get(lockerChamber);
        }

        /// <summary>
        /// Gets a value indicating the item being spawned.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets a value indicating the target locker chamber.
        /// </summary>
        [Obsolete("Use Chamber instead.")]
        public LockerChamber LockerChamber => Chamber.Base;

        /// <summary>
        /// Gets a locker which is containing <see cref="Chamber"/>.
        /// </summary>
        public API.Features.Lockers.Locker Locker => Chamber.Locker;

        /// <summary>
        /// Gets a chamber which is filling.
        /// </summary>
        public Chamber Chamber { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the item can be spawned.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}