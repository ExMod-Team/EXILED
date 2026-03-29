// -----------------------------------------------------------------------
// <copyright file="RespawnedTeamEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using Respawning;
    using Respawning.Waves;

    /// <summary>
    /// Contains all information after team spawns.
    /// </summary>
    public class RespawnedTeamEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RespawnedTeamEventArgs"/> class.
        /// </summary>
        /// <param name="hubs"><inheritdoc cref="Players"/></param>
        /// <param name="wave"><inheritdoc cref="Wave"/></param>
        public RespawnedTeamEventArgs(SpawnableWaveBase wave, IEnumerable<ReferenceHub> hubs)
        {
            Players = hubs.Select(Player.Get);
            Wave = wave;
        }

        /// <summary>
        /// Gets the list of spawned players.
        /// </summary>
        public IEnumerable<Player> Players { get; }

        /// <summary>
        /// Gets the spawned team.
        /// </summary>
        public SpawnableWaveBase Wave { get; }
    }
}
