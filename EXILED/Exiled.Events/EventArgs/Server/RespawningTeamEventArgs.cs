// -----------------------------------------------------------------------
// <copyright file="RespawningTeamEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.API.Features.Waves;
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles;
    using PlayerRoles.Spectating;
    using Respawning;
    using Respawning.Objectives;
    using Respawning.Waves;

    /// <summary>
    /// Contains all information before spawning a wave.
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
        /// <param name="maxRespawn">
        /// <inheritdoc cref="MaximumRespawnAmount" />
        /// </param>
        /// <param name="wave">
        /// <inheritdoc cref="NextKnownTeam" />
        /// </param>
        public RespawningTeamEventArgs(List<Player> players, int maxRespawn, SpawnableWaveBase wave)
        {
            Players = players;
            if (Players.Remove(null))
            {
                string debug = string.Empty;
                foreach (ReferenceHub hub in ReferenceHub.AllHubs)
                {
                    if (WaveSpawner.CanBeSpawned(hub) && hub.roleManager.CurrentRole is SpectatorRole spectatorRole)
                        debug += $"({Player.Get(hub)}) {hub.GetNickname()} (ActiveTime: {spectatorRole.ActiveTime}) [{hub.authManager.InstanceMode}]\n";
                }

                Log.Error("(RespawningTeamEventArgs) preventing a null player to spawn:\n" + debug);
            }

            MaximumRespawnAmount = maxRespawn;
            SpawnQueue = new();
            Wave = new TimedWave((TimeBasedWave)wave);
            Wave.PopulateQueue(SpawnQueue, MaximumRespawnAmount);
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
        /// Gets or sets a value indicating what the next wave is.
        /// </summary>
        public TimedWave Wave { get; set; }

        /// <summary>
        /// Gets a value indicating what the next respawnable team is.
        /// </summary>
        public Faction NextKnownTeam => Wave.Faction;

        /// <summary>
        /// Gets or sets a value indicating whether the spawn can occur.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets or sets the RoleTypeId spawn queue.
        /// </summary>
        public Queue<RoleTypeId> SpawnQueue { get; set; }
    }
}
