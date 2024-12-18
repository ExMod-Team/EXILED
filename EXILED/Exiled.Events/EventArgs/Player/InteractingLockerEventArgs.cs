// -----------------------------------------------------------------------
// <copyright file="InteractingLockerEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
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
            InteractingLocker = Locker.Get(locker);
            InteractingChamber = Chamber.Get(locker.Chambers[colliderId]);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the locker which is containing <see cref="InteractingChamber"/>.
        /// </summary>
        public Locker InteractingLocker { get; }

        /// <summary>
        /// Gets the interacting chamber.
        /// </summary>
        public Chamber InteractingChamber { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can interact with the locker.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's interacting with the locker.
        /// </summary>
        public Player Player { get; }
    }
}