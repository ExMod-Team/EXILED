// -----------------------------------------------------------------------
// <copyright file="PlayerHandlers.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Events
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomRoles.API;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.EventArgs.Player;

    /// <summary>
    /// Handles general events for players.
    /// </summary>
    public class PlayerHandlers
    {
        private static readonly HashSet<SpawnReason> ValidSpawnReasons = new()
        {
            SpawnReason.RoundStart,
            SpawnReason.Respawn,
            SpawnReason.LateJoin,
            SpawnReason.Revived,
            SpawnReason.Escaped,
            SpawnReason.ItemUsage,
        };

        private readonly CustomRoles plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerHandlers"/> class.
        /// </summary>
        /// <param name="plugin">The <see cref="CustomRoles"/> plugin instance.</param>
        public PlayerHandlers(CustomRoles plugin)
        {
            this.plugin = plugin;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.WaitingForPlayers"/>
        internal void OnWaitingForPlayers()
        {
            foreach (CustomRole role in CustomRole.Registered)
            {
                role.SpawnedPlayers = 0;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.SpawningRagdoll"/>
        internal void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (plugin.StopRagdollPlayers.Contains(ev.Player))
            {
                ev.IsAllowed = false;
                plugin.StopRagdollPlayers.Remove(ev.Player);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.Spawning"/>
        internal void OnSpawned(SpawnedEventArgs ev)
        {
            if (!ValidSpawnReasons.Contains(ev.Reason) || ev.Player.HasAnyCustomRole())
            {
                return;
            }

            float totalChance = 0f;
            List<CustomRole> eligibleRoles = new(8);

            foreach (CustomRole role in CustomRole.Registered)
            {
                if (role.Role == ev.Player.Role.Type && !role.IgnoreSpawnSystem && role.SpawnChance > 0 && !role.Check(ev.Player) && (role.SpawnProperties is null || role.SpawnedPlayers < role.SpawnProperties.Limit))
                {
                    eligibleRoles.Add(role);
                    totalChance += role.SpawnChance;
                }
            }

            if (eligibleRoles.Count == 0)
            {
                return;
            }

            float lotterySize = Math.Max(100f, totalChance);
            float randomRoll = (float)Loader.Loader.Random.NextDouble() * lotterySize;

            if (randomRoll >= totalChance)
            {
                return;
            }

            foreach (CustomRole candidateRole in eligibleRoles)
            {
                if (randomRoll >= candidateRole.SpawnChance)
                {
                    randomRoll -= candidateRole.SpawnChance;
                    continue;
                }

                if (candidateRole.SpawnProperties is null)
                {
                    candidateRole.AddRole(ev.Player);
                    break;
                }

                int newSpawnCount = candidateRole.SpawnedPlayers++;
                if (newSpawnCount <= candidateRole.SpawnProperties.Limit)
                {
                    candidateRole.AddRole(ev.Player);
                    break;
                }
                else
                {
                    candidateRole.SpawnedPlayers--;
                    randomRoll -= candidateRole.SpawnChance;
                }
            }
        }
    }
}
