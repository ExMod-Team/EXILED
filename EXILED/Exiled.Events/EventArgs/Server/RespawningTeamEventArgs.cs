// -----------------------------------------------------------------------
// <copyright file="RespawningTeamEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles;
    using Respawning;
    using Respawning.Waves;

    /// <summary>
    /// Contains all information before spawning a wave of <see cref="SpawnableTeamType.NineTailedFox" /> or
    /// <see cref="SpawnableTeamType.ChaosInsurgency" />.
    /// </summary>
    public class RespawningTeamEventArgs : IDeniableEvent
    {
        private int maximumRespawnAmount;

        /// <summary>
        /// Initializes a new instance of the <see cref="RespawningTeamEventArgs" /> class.
        /// </summary>
        /// <param name="players">
        /// <inheritdoc cref="Players" />
        /// </param>
        /// <param name="queue">
        /// <inheritdoc cref="SpawnQueue" />
        /// </param>
        /// <param name="maxRespawn">
        /// <inheritdoc cref="MaximumRespawnAmount" />
        /// </param>
        /// <param name="respawningTeam">
        /// <inheritdoc cref="RespawningTeam" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public RespawningTeamEventArgs(List<Player> players, Queue<RoleTypeId> queue, int maxRespawn, SpawnableWaveBase respawningTeam, bool isAllowed = true)
        {
            Players = players;
            MaximumRespawnAmount = maxRespawn;
            SpawnQueue = queue;
            RespawningTeam = respawningTeam;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the list of players that are going to be respawned.
        /// </summary>
        public List<Player> Players { get; }

        /// <summary>
        /// Gets or sets the maximum amount of respawnable players.
        /// </summary>
        public int MaximumRespawnAmount
        {
            get => maximumRespawnAmount;
            set
            {
                if (value < maximumRespawnAmount)
                {
                    if (Players.Count > value)
                        Players.RemoveRange(value, Players.Count - value);
                }

                maximumRespawnAmount = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating what the currently respawning team is.
        /// </summary>
        public SpawnableWaveBase RespawningTeam { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the spawn can occur.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets the RoleTypeId spawn queue.
        /// </summary>
        public Queue<RoleTypeId> SpawnQueue { get; set; }
    }
}
