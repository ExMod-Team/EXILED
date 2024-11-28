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
    using System.Linq;

    using CustomPlayerEffects;
    using Enums;
    using Extensions;
    using PlayerRoles;
    using Respawning;
    using Respawning.Waves;
    using Respawning.Waves.Generic;
    using UnityEngine;

    // TODO: Write docs for everything in this class

    /// <summary>
    /// A set of tools to handle team respawns more easily.
    /// </summary>
    public static class Respawn
    {
        private static GameObject ntfHelicopterGameObject;
        private static GameObject chaosCarGameObject;

        /// <summary>
        /// Gets or sets docs1.
        /// </summary>
        public static List<SpawnableWaveBase> PausedWaves { get; set; } = new();

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
        public static SpawnableFaction NextKnownSpawnableFaction
        {
            get => WaveManager._nextWave.GetSpawnableFaction();
            set => WaveManager._nextWave = WaveManager.Waves.Find(x => x.TargetFaction == value.GetFaction());
        }

        /// <summary>
        /// Gets the next known <see cref="SpawnableTeamType"/> that will spawn.
        /// </summary>
        public static SpawnableTeamType NextKnownTeam => NextKnownSpawnableFaction.GetFaction().GetSpawnableTeam();

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
        /// Docs1.
        /// </summary>
        /// <param name="spawnableFaction">Docs2.</param>
        /// <param name="spawnableWaveBase">Docs45.</param>
        /// <returns>Docs3.</returns>
        public static bool TryGetWaveBase(SpawnableFaction spawnableFaction, out SpawnableWaveBase spawnableWaveBase)
        {
            spawnableWaveBase = WaveManager.Waves.Find(x => x.GetSpawnableFaction() == spawnableFaction);
            return spawnableWaveBase is not null;
        }

        /// <summary>
        /// Docs1.
        /// </summary>
        /// <param name="faction">Docs2.</param>
        /// <param name="spawnableWaveBases">Docs3.</param>
        /// <returns>Docs4.</returns>
        public static bool TryGetWaveBases(Faction faction, out IEnumerable<SpawnableWaveBase> spawnableWaveBases)
        {
            List<SpawnableWaveBase> spawnableWaves = new();
            spawnableWaves.AddRange(WaveManager.Waves.Where(x => x.TargetFaction == faction));

            if (spawnableWaves.IsEmpty())
            {
                spawnableWaveBases = null;
                return false;
            }

            spawnableWaveBases = spawnableWaves;
            return true;
        }

        /// <summary>
        /// Docs1.
        /// </summary>
        /// <param name="spawnableTeamType">Docs2.</param>
        /// <param name="spawnableWaveBases">Docs3.</param>
        /// <returns>Docs4.</returns>
        public static bool TryGetWaveBases(SpawnableTeamType spawnableTeamType, out IEnumerable<SpawnableWaveBase> spawnableWaveBases)
        {
            return TryGetWaveBases(spawnableTeamType.GetFaction(), out spawnableWaveBases);
        }

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="faction">Docs1.</param>
        /// <param name="seconds">Docs2.</param>
        public static void AdvanceTimer(Faction faction, float seconds) => WaveManager.AdvanceTimer(faction, seconds);

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="faction">Docs1.</param>
        /// <param name="time">Docs2.</param>
        public static void AdvanceTimer(Faction faction, TimeSpan time) => AdvanceTimer(faction, (float)time.TotalSeconds);

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="spawnableTeamType">Docs1.</param>
        /// <param name="seconds">Docs2.</param>
        public static void AdvanceTimer(SpawnableTeamType spawnableTeamType, float seconds) => AdvanceTimer(spawnableTeamType.GetFaction(), seconds);

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="spawnableTeamType">Docs1.</param>
        /// <param name="time">Docs2.</param>
        public static void AdvanceTimer(SpawnableTeamType spawnableTeamType, TimeSpan time) => AdvanceTimer(spawnableTeamType.GetFaction(), time);

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="spawnableFaction">Docs1.</param>
        /// <param name="seconds">Docs2.</param>
        public static void AdvanceTimer(SpawnableFaction spawnableFaction, float seconds)
        {
            foreach (SpawnableWaveBase spawnableWaveBase in WaveManager.Waves)
            {
                TimeBasedWave timeBasedWave = (TimeBasedWave)spawnableWaveBase;
                if (timeBasedWave.GetSpawnableFaction() == spawnableFaction)
                {
                    timeBasedWave.Timer.AddTime(Mathf.Abs(seconds));
                }
            }
        }

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="spawnableFaction">Docs1.</param>
        /// <param name="time">Docs2.</param>
        public static void AdvanceTimer(SpawnableFaction spawnableFaction, TimeSpan time) => AdvanceTimer(spawnableFaction, (float)time.TotalSeconds);

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
            if (TryGetWaveBase(SpawnableFaction.NtfWave, out SpawnableWaveBase wave))
                PlayEffect(wave);
        }

        /// <summary>
        /// Summons the <see cref="Side.ChaosInsurgency"/> van.
        /// </summary>
        /// <remarks>This will also trigger Music effect.</remarks>
        public static void SummonChaosInsurgencyVan()
        {
            if (TryGetWaveBase(SpawnableFaction.ChaosWave, out SpawnableWaveBase wave))
                PlayEffect(wave);
        }

        /// <summary>
        /// Grants tickets to a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="faction">The <see cref="SpawnableTeamType"/> to grant tickets to.</param>
        /// <param name="amount">The amount of tickets to grant.</param>
        /// <returns>Success.</returns>
        public static bool GrantTokens(Faction faction, int amount)
        {
            if (TryGetWaveBases(faction, out IEnumerable<SpawnableWaveBase> waveBases))
            {
                foreach (ILimitedWave limitedWave in waveBases.OfType<ILimitedWave>())
                {
                    limitedWave.RespawnTokens += amount;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes tickets from a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="faction">The <see cref="SpawnableTeamType"/> to remove tickets from.</param>
        /// <param name="amount">The amount of tickets to remove.</param>
        /// <returns>Success.</returns>
        public static bool RemoveTokens(Faction faction, int amount)
        {
            if (TryGetWaveBases(faction, out IEnumerable<SpawnableWaveBase> waveBases))
            {
                foreach (ILimitedWave limitedWave in waveBases.OfType<ILimitedWave>())
                {
                    limitedWave.RespawnTokens = Math.Max(0, limitedWave.RespawnTokens - amount);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Modify tickets from a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="faction">The <see cref="SpawnableTeamType"/> to modify tickets from.</param>
        /// <param name="amount">The amount of tickets to modify.</param>
        /// <returns>Success.</returns>
        public static bool ModifyTokens(Faction faction, int amount)
        {
            if (TryGetWaveBases(faction, out IEnumerable<SpawnableWaveBase> waveBases))
            {
                foreach (ILimitedWave limitedWave in waveBases.OfType<ILimitedWave>())
                {
                    limitedWave.RespawnTokens = amount;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the amount of tickets from a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="faction"><see cref="SpawnableTeamType"/>'s faction.</param>
        /// <returns>Tickets of team or <c>-1</c> if team doesn't depend on tickets.</returns>
        public static int GetTokens(SpawnableFaction faction)
        {
            if (TryGetWaveBase(faction, out SpawnableWaveBase wave) && wave is ILimitedWave limitedWave)
                return limitedWave.RespawnTokens;

            return -1;
        }

        /// <summary>
        /// Forces a spawn of the given <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="spawnableTeamType">The <see cref="SpawnableTeamType"/> to spawn.</param>
        /// <param name="isMini">Docs.</param>
        public static void ForceWave(SpawnableTeamType spawnableTeamType, bool isMini = false)
        {
            ForceWave(spawnableTeamType.GetFaction(), isMini);
        }

        /// <summary>
        /// Forces a spawn of the given <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="faction">The <see cref="SpawnableTeamType"/> to spawn.</param>
        /// <param name="isMini">Docs.</param>
        public static void ForceWave(Faction faction, bool isMini = false)
        {
            if (faction.TryGetSpawnableFaction(out SpawnableFaction spawnableFaction, isMini))
            {
                ForceWave(spawnableFaction);
            }
        }

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="spawnableFaction">Docs1.</param>
        public static void ForceWave(SpawnableFaction spawnableFaction)
        {
            if (TryGetWaveBase(spawnableFaction, out SpawnableWaveBase spawnableWaveBase))
            {
                ForceWave(spawnableWaveBase);
            }
        }

        /// <summary>
        /// Docs.
        /// </summary>
        /// <param name="wave">Docs1.</param>
        public static void ForceWave(SpawnableWaveBase wave)
        {
            WaveManager.Spawn(wave);
        }

        // These two need testing, Beryl said that this was originally intended to work and it still might

        /// <summary>
        /// Docs1.
        /// </summary>
        public static void PauseWaves()
        {
            PausedWaves.Clear();
            PausedWaves.AddRange(WaveManager.Waves);
            WaveManager.Waves.Clear();
        }

        /// <summary>
        /// Docs1.
        /// </summary>
        public static void ResumeWaves()
        {
            WaveManager.Waves.Clear();
            WaveManager.Waves.AddRange(PausedWaves);
            PausedWaves.Clear();
        }
    }
}