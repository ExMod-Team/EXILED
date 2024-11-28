// -----------------------------------------------------------------------
// <copyright file="Respawn.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;

    using CustomPlayerEffects;
    using Enums;
    using PlayerRoles;
    using Respawning;
    using Respawning.Waves;
    using Respawning.Waves.Generic;
    using UnityEngine;

    /// <summary>
    /// A set of tools to handle team respawns more easily.
    /// </summary>
    public static class Respawn
    {
        private static GameObject ntfHelicopterGameObject;
        private static GameObject chaosCarGameObject;

        /// <summary>
        /// Gets the NTF Helicopter's <see cref="GameObject"/>.
        /// </summary>
        public static GameObject NtfHelicopter
        {
            get
            {
                if (ntfHelicopterGameObject == null)
                    ntfHelicopterGameObject = GameObject.Find("Chopper");

                return ntfHelicopterGameObject;
            }
        }

        /// <summary>
        /// Gets the Chaos Van's <see cref="GameObject"/>.
        /// </summary>
        public static GameObject ChaosVan
        {
            get
            {
                if (chaosCarGameObject == null)
                    chaosCarGameObject = GameObject.Find("CIVanArrive");

                return chaosCarGameObject;
            }
        }

        /// <summary>
        /// Gets or sets the next known <see cref="Faction"/> that will spawn.
        /// </summary>
        public static Faction NextKnownFaction
        {
            get => WaveManager._nextWave.TargetFaction;
            set => WaveManager._nextWave = WaveManager.Waves.Find(x => x.TargetFaction == value);
        }

        /// <summary>
        /// Gets the next known <see cref="SpawnableTeamType"/> that will spawn.
        /// </summary>
        public static SpawnableTeamType NextKnownTeam => NextKnownFaction.GetSpawnableTeam();

        /* TODO: Possibly moved to TimedWave
        /// <summary>
        /// Gets or sets the amount of seconds before the next respawn phase will occur.
        /// </summary>
        public static float TimeUntilNextPhase
        {
            get => RespawnManager.Singleton._timeForNextSequence - (float)RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds
            set => RespawnManager.Singleton._timeForNextSequence = (float)RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds + value;
        }

        /// <summary>
        /// Gets a <see cref="TimeSpan"/> indicating the amount of time before the next respawn wave will occur.
        /// </summary>
        public static TimeSpan TimeUntilSpawnWave => TimeSpan.FromSeconds(TimeUntilNextPhase);

        /// <summary>
        /// Gets a <see cref="DateTime"/> indicating the moment in UTC time the next respawn wave will occur.
        /// </summary>
        public static DateTime NextTeamTime => DateTime.UtcNow.AddSeconds(TimeUntilSpawnWave.TotalSeconds);
        */

        /// <summary>
        /// Gets the current state of the <see cref="WaveManager"/>.
        /// </summary>
        public static WaveManager.WaveQueueState CurrentState => WaveManager.State;

        /// <summary>
        /// Gets a value indicating whether a team is currently being spawned or the animations are playing for a team.
        /// </summary>
        public static bool IsSpawning => WaveManager.State == WaveManager.WaveQueueState.WaveSpawning;

        /// <summary>
        /// Gets or sets a value indicating whether spawn protection is enabled.
        /// </summary>
        public static bool ProtectionEnabled
        {
            get => SpawnProtected.IsProtectionEnabled;
            set => SpawnProtected.IsProtectionEnabled = value;
        }

        /// <summary>
        /// Gets or sets the spawn protection time, in seconds.
        /// </summary>
        public static float ProtectionTime
        {
            get => SpawnProtected.SpawnDuration;
            set => SpawnProtected.SpawnDuration = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether spawn protected players can shoot.
        /// </summary>
        public static bool ProtectedCanShoot
        {
            get => SpawnProtected.CanShoot;
            set => SpawnProtected.CanShoot = value;
        }

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="Team"/> that have spawn protection.
        /// </summary>
        public static List<Team> ProtectedTeams => SpawnProtected.ProtectedTeams;

        /// <summary>
        /// Tries to get a <see cref="SpawnableWaveBase"/>.
        /// </summary>
        /// <param name="spawnWave">Found <see cref="SpawnableWaveBase"/>.</param>
        /// <typeparam name="T">Type of <see cref="SpawnableWaveBase"/>.</typeparam>
        /// <returns><c>true</c> if <paramref name="spawnWave"/> was successfully found. Otherwise, <c>false</c>.</returns>
        public static bool TryGetWaveBase<T>(out T spawnWave)
            where T : SpawnableWaveBase => WaveManager.TryGet(out spawnWave);

        /// <summary>
        /// Tries to get a <see cref="SpawnableWaveBase"/> from a <see cref="Faction"/>.
        /// </summary>
        /// <param name="faction">Team's <see cref="Faction"/>.</param>
        /// <param name="spawnWave">Found <see cref="SpawnableWaveBase"/>.</param>
        /// <returns><c>true</c> if <paramref name="spawnWave"/> was successfully found. Otherwise, <c>false</c>.</returns>
        public static bool TryGetWaveBase(Faction faction, out SpawnableWaveBase spawnWave)
            => WaveManager.TryGet(faction, out spawnWave);

        /// <summary>
        /// Tries to get a <see cref="SpawnableWaveBase"/> from a <see cref="SpawnableFaction"/>.
        /// </summary>
        /// <param name="faction">Team's <see cref="SpawnableFaction"/>.</param>
        /// <param name="spawnWave">Found <see cref="SpawnableWaveBase"/>.</param>
        /// <returns><c>true</c> if <paramref name="spawnWave"/> was successfully found. Otherwise, <c>false</c>.</returns>
        public static bool TryGetWaveBase(SpawnableFaction faction, out SpawnableWaveBase spawnWave)
        {
            switch (faction)
            {
                case SpawnableFaction.NtfWave:
                    bool result = TryGetWaveBase(out NtfSpawnWave ntfSpawnWave);
                    spawnWave = ntfSpawnWave;
                    return result;
                case SpawnableFaction.NtfMiniWave:
                    result = TryGetWaveBase(out NtfMiniWave ntfMiniWave);
                    spawnWave = ntfMiniWave;
                    return result;
                case SpawnableFaction.ChaosWave:
                    result = TryGetWaveBase(out ChaosSpawnWave chaosSpawnWave);
                    spawnWave = chaosSpawnWave;
                    return result;
                case SpawnableFaction.ChaosMiniWave:
                    result = TryGetWaveBase(out ChaosMiniWave chaosMiniWave);
                    spawnWave = chaosMiniWave;
                    return result;
            }

            spawnWave = null;
            return false;
        }

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="faction">Docs1.</param>
        /// <param name="time">Docs2.</param>
        public static void AdvanceTime(Faction faction, float time) => WaveManager.AdvanceTimer(faction, time);

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="wave">Docs1.</param>
        public static void SpawnWave(SpawnableWaveBase wave) => WaveManager.Spawn(wave);

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="faction">Docs1.</param>
        /// <param name="mini">Docs2.</param>
        /// <typeparam name="T">Docs3.</typeparam>
        public static void SpawnWave<T>(Faction faction, bool mini)
            where T : SpawnableWaveBase
        {
            if (TryGetWaveBase(out T wave))
                SpawnWave(wave);
        }

        /// <summary>
        /// Play effects when a certain class spawns.
        /// </summary>
        /// <param name="wave">The <see cref="SpawnableWaveBase"/> for which effects should be played.</param>
        public static void PlayEffect(SpawnableWaveBase wave)
        {
            WaveUpdateMessage.ServerSendUpdate(wave, UpdateMessageFlags.Trigger);
        }

        /// <summary>
        /// Summons the NTF chopper.
        /// </summary>
        public static void SummonNtfChopper()
        {
            if (TryGetWaveBase(Faction.FoundationStaff, out SpawnableWaveBase wave))
                PlayEffect(wave);
        }

        /// <summary>
        /// Summons the <see cref="Side.ChaosInsurgency"/> van.
        /// </summary>
        /// <remarks>This will also trigger Music effect.</remarks>
        public static void SummonChaosInsurgencyVan()
        {
            if (TryGetWaveBase(Faction.FoundationEnemy, out SpawnableWaveBase wave))
                PlayEffect(wave);
        }

        /// <summary>
        /// Grants tickets to a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="team">The <see cref="SpawnableTeamType"/> to grant tickets to.</param>
        /// <param name="amount">The amount of tickets to grant.</param>
        public static void GrantTickets(Faction team, int amount)
        {
            if (TryGetWaveBase(team, out SpawnableWaveBase wave) && wave is ILimitedWave limitedWave)
                limitedWave.RespawnTokens += amount;
        }

        /// <summary>
        /// Removes tickets from a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="team">The <see cref="SpawnableTeamType"/> to remove tickets from.</param>
        /// <param name="amount">The amount of tickets to remove.</param>
        public static void RemoveTickets(Faction team, int amount)
        {
            if (TryGetWaveBase(team, out SpawnableWaveBase wave) && wave is ILimitedWave limitedWave)
                limitedWave.RespawnTokens = Math.Max(0, limitedWave.RespawnTokens - amount);
        }

        /// <summary>
        /// Modify tickets from a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="team">The <see cref="SpawnableTeamType"/> to modify tickets from.</param>
        /// <param name="amount">The amount of tickets to modify.</param>
        public static void ModifyTickets(Faction team, int amount)
        {
            if (TryGetWaveBase(team, out SpawnableWaveBase wave) && wave is ILimitedWave limitedWave)
                limitedWave.RespawnTokens = amount;
        }

        /// <summary>
        /// Gets the amount of tickets from a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="faction"><see cref="SpawnableTeamType"/>'s faction.</param>
        /// <returns>Tickets of team or <c>-1</c> if team doesn't depend on tickets.</returns>
        public static int GetTickets(SpawnableFaction faction)
        {
            if (TryGetWaveBase(faction, out SpawnableWaveBase wave) && wave is ILimitedWave limitedWave)
                return limitedWave.RespawnTokens;

            return -1;
        }

        /// <summary>
        /// Forces a spawn of the given <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="team">The <see cref="SpawnableTeamType"/> to spawn.</param>
        public static void ForceWave(Faction team)
        {
            if (TryGetWaveBase(team, out SpawnableWaveBase wave))
                ForceWave(wave);
        }

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="wave">Docs1.</param>
        public static void ForceWave(SpawnableWaveBase wave)
        {
            WaveManager.Spawn(wave);
        }
    }
}