// -----------------------------------------------------------------------
// <copyright file="InteractingLockerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using API.Features;
    using Exiled.API.Features.Lockers;
    using Interfaces;

    /// <summary>
    /// Contains all information before a player interacts with a locker.
    /// </summary>
    public class InteractingLockerEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingLockerEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="locker">
        /// <inheritdoc cref="InteractingLocker" />
        /// </param>
        /// <param name="colliderId">
        /// <inheritdoc cref="InteractingChamber" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public InteractingLockerEventArgs(Player player, MapGeneration.Distributors.Locker locker, byte colliderId, bool isAllowed)
        {
            Player = player;
            InteractingLocker = API.Features.Lockers.Locker.Get(locker);
            InteractingChamber = API.Features.Lockers.Chamber.Get(locker.Chambers[colliderId]);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="MapGeneration.Distributors.Locker" /> instance.
        /// </summary>
        [Obsolete("Use InteractingLocker instead.")]
        public MapGeneration.Distributors.Locker Locker => InteractingLocker.Base;

        /// <summary>
        /// Gets the interacting chamber.
        /// </summary>
        [Obsolete("Use InteractingChamber instead.")]
        public MapGeneration.Distributors.LockerChamber Chamber => InteractingChamber.Base;

        /// <summary>
        /// Gets the locker which is containing <see cref="InteractingChamber"/>.
        /// </summary>
        public Locker InteractingLocker { get; }

        /// <summary>
        /// Gets the interacting chamber.
        /// </summary>
        public Chamber InteractingChamber { get; }

        /// <summary>
        /// Gets the chamber id.
        /// </summary>
        [Obsolete("Use Chamber::Id instead.")]
        public byte ChamberId => InteractingChamber.Id;

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can interact with the locker.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's interacting with the locker.
        /// </summary>
        public Player Player { get; }
    }
}