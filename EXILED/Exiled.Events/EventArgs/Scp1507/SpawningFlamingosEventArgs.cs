// -----------------------------------------------------------------------
// <copyright file="SpawningFlamingosEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp1507
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles.PlayableScps.Scp1507;
    using Utils.NonAllocLINQ;

    /// <summary>
    /// Contains all information before flamingos get spawned.
    /// </summary>
    // [Obsolete("Only availaible for Christmas and AprilFools.")]
    public class SpawningFlamingosEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningFlamingosEventArgs"/> class.
        /// </summary>
        /// <param name="newAlpha"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SpawningFlamingosEventArgs(Player newAlpha, bool isAllowed = true)
        {
            Player = newAlpha;
            SpawnablePlayers = ReferenceHub.AllHubs.Where(Scp1507Spawner.ValidatePlayer).Select(x => Player.Get(x)).ToHashSet();
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the player which is being spawned as a new alpha flamingo.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Gets or sets all enqueued spawnable players.
        /// </summary>
        public HashSet<Player> SpawnablePlayers { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}