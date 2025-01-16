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

    /// <summary>
    /// A set of tools to handle team respawns more easily.
    /// </summary>
    public static class Respawn
    {
        private static GameObject ntfHelicopterGameObject;
        private static GameObject chaosCarGameObject;

        /// <summary>
        /// Gets the <see cref="List{T}"/> of paused <see cref="SpawnableWaveBase"/>'s.
        /// </summary>
        public static List<SpawnableWaveBase> PausedWaves { get; } = new();

        /// <summary>
        /// Gets the <see cref="Dictionary{TKey,TValue}"/> containing faction influence.
        /// </summary>
        public static Dictionary<Faction, float> FactionInfluence => FactionInfluenceManager.Influence;

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
        /// Gets the next known <see cref="SpawnableFaction"/> that will spawn.
        /// </summary>
        /// <remarks>This returns <see cref="SpawnableFaction.None">SpawnableFaction.None</see> unless a respawn has already started.</remarks>
        public static SpawnableFaction NextKnownSpawnableFaction => WaveManager._nextWave is not null ? WaveManager._nextWave.GetSpawnableFaction() : SpawnableFaction.None;

        /// <summary>
        /// Gets the current state of the <see cref="WaveManager"/>.
        /// </summary>
        public static WaveQueueState CurrentState => WaveManager.State;

        /// <summary>
        /// Gets a value indicating whether the respawn process for a <see cref="SpawnableWaveBase"/> is currently in progress..
        /// </summary>
        public static bool IsSpawning => WaveManager.State == WaveQueueState.WaveSpawning;

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
        /// Gets a <see cref="List{T}"/> of <see cref="Team"/>s that have spawn protection.
        /// </summary>
        public static List<Team> ProtectedTeams => SpawnProtected.ProtectedTeams;

        /// <summary>
        /// Tries to get a <see cref="SpawnableWaveBase"/>.
        /// </summary>
        /// <param name="spawnableWaveBase">The found <see cref="SpawnableWaveBase"/>.</param>
        /// <typeparam name="T">Type of <see cref="SpawnableWaveBase"/>.</typeparam>
        /// <returns><c>true</c> if <paramref name="spawnableWaveBase"/> was successfully found. Otherwise, <c>false</c>.</returns>
        /// <seealso cref="TryGetWaveBases(PlayerRoles.Faction,out System.Collections.Generic.IEnumerable{Respawning.Waves.SpawnableWaveBase})"/>
        public static bool TryGetWaveBase<T>(out T spawnableWaveBase)
            where T : SpawnableWaveBase => WaveManager.TryGet(out spawnableWaveBase);

        /// <summary>
        /// Tries to get a <see cref="SpawnableWaveBase"/>.
        /// </summary>
        /// <param name="spawnableFaction">A <see cref="SpawnableFaction"/> determining which wave to search for.</param>
        /// <param name="spawnableWaveBase">The found <see cref="SpawnableWaveBase"/>.</param>
        /// <returns><c>true</c> if <paramref name="spawnableWaveBase"/> was successfully found. Otherwise, <c>false</c>.</returns>
        /// <seealso cref="TryGetWaveBases(PlayerRoles.Faction,out System.Collections.Generic.IEnumerable{Respawning.Waves.SpawnableWaveBase})"/>
        public static bool TryGetWaveBase(SpawnableFaction spawnableFaction, out SpawnableWaveBase spawnableWaveBase)
        {
            spawnableWaveBase = WaveManager.Waves.Find(x => x.GetSpawnableFaction() == spawnableFaction);
            return spawnableWaveBase is not null;
        }

        /// <summary>
        /// Tries to get an <see cref="IEnumerable{T}"/> of <see cref="SpawnableWaveBase"/>.
        /// </summary>
        /// <param name="faction">A <see cref="Faction"/> determining which waves to search for.</param>
        /// <param name="spawnableWaveBases">The <see cref="IEnumerable{T}"/> containing found <see cref="SpawnableWaveBase"/>'s if there are any, otherwise <c>null</c>.</param>
        /// <returns><c>true</c> if <paramref name="spawnableWaveBases"/> was successfully found. Otherwise, <c>false</c>.</returns>
        /// <seealso cref="TryGetWaveBase{T}"/>
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
        /// Advances the respawn timer for <see cref="TimeBasedWave"/>s.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> whose <see cref="TimeBasedWave"/>'s timers are to be advanced.</param>
        /// <param name="seconds">Number of seconds to advance the timers by.</param>
        /// <remarks>This advances the timer for both the normal and mini wave.</remarks>
        public static void AdvanceTimer(Faction faction, float seconds) => WaveManager.AdvanceTimer(faction, seconds);

        /// <summary>
        /// Advances the respawn timer for <see cref="TimeBasedWave"/>s.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> whose <see cref="TimeBasedWave"/>'s timers are to be advanced.</param>
        /// <param name="time">A <see cref="TimeSpan"/> representing the amount of time to advance the timers by.</param>
        /// <remarks>This advances the timer for both the normal and mini wave.</remarks>
        public static void AdvanceTimer(Faction faction, TimeSpan time) => AdvanceTimer(faction, (float)time.TotalSeconds);

        /// <summary>
        /// Advances the respawn timer for <see cref="TimeBasedWave"/>s.
        /// </summary>
        /// <param name="spawnableFaction">The <see cref="SpawnableFaction"/> whose <see cref="TimeBasedWave"/>'s timer is to be advanced.</param>
        /// <param name="seconds">Number of seconds to advance the timers by.</param>
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
        /// Advances the respawn timer for <see cref="TimeBasedWave"/>s.
        /// </summary>
        /// <param name="spawnableFaction">The <see cref="SpawnableFaction"/> whose <see cref="TimeBasedWave"/>'s timer is to be advanced.</param>
        /// <param name="time">A <see cref="TimeSpan"/> representing the amount of time to advance the timers by.</param>
        public static void AdvanceTimer(SpawnableFaction spawnableFaction, TimeSpan time) => AdvanceTimer(spawnableFaction, (float)time.TotalSeconds);

        /// <summary>
        /// Play the spawn effect of a <see cref="SpawnableWaveBase"/>.
        /// </summary>
        /// <param name="wave">The <see cref="SpawnableWaveBase"/> whose effect should be played.</param>
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
        /// Summons the Chaos Insurgency van.
        /// </summary>
        /// <remarks>This will also trigger Music effect.</remarks>
        /// <!--not sure if it actually plays the music, needs to be tested-->
        public static void SummonChaosInsurgencyVan()
        {
            if (TryGetWaveBase(SpawnableFaction.ChaosWave, out SpawnableWaveBase wave))
                PlayEffect(wave);
        }

        /// <summary>
        /// Grants tokens to a given <see cref="Faction"/>'s <see cref="ILimitedWave"/>s.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> to whose <see cref="ILimitedWave"/>s to grant tokens.</param>
        /// <param name="amount">The amount of tokens to grant.</param>
        /// <returns><c>true</c> if tokens were successfully granted to an <see cref="ILimitedWave"/>, otherwise <c>false</c>.</returns>
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
        /// Removes tokens from a given <see cref="Faction"/>'s <see cref="ILimitedWave"/>s.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> from whose <see cref="ILimitedWave"/>s to remove tokens.</param>
        /// <param name="amount">The amount of tokens to remove.</param>
        /// <returns><c>true</c> if tokens were successfully removed from an <see cref="ILimitedWave"/>, otherwise <c>false</c>.</returns>
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
        /// Modifies tokens of a given <see cref="Faction"/>'s <see cref="ILimitedWave"/>s by a given amount.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> whose <see cref="ILimitedWave"/>s' tokens are to be modified.</param>
        /// <param name="amount">The amount of tokens to add/remove.</param>
        /// <returns><c>true</c> if tokens were successfully modified for an <see cref="ILimitedWave"/>, otherwise <c>false</c>.</returns>
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
        /// Tries to get the tokens of a given <see cref="SpawnableFaction"/>'s <see cref="ILimitedWave"/>.
        /// </summary>
        /// <param name="spawnableFaction">The <see cref="SpawnableFaction"/> from whose <see cref="ILimitedWave"/> to get the tokens.</param>
        /// <param name="tokens">The amount of tokens an <see cref="ILimitedWave"/> has, if one was found, otherwise <c>0</c>.</param>
        /// <returns><c>true</c> if an <see cref="ILimitedWave"/> was successfully found, otherwise <c>false</c>.</returns>
        public static bool TryGetTokens(SpawnableFaction spawnableFaction, out int tokens)
        {
            if (TryGetWaveBase(spawnableFaction, out SpawnableWaveBase waveBase) && waveBase is ILimitedWave limitedWave)
            {
                tokens = limitedWave.RespawnTokens;
                return true;
            }

            tokens = 0;
            return false;
        }

        /// <summary>
        /// Sets the amount of tokens of an <see cref="ILimitedWave"/> of the given <see cref="SpawnableFaction"/>.
        /// </summary>
        /// <param name="spawnableFaction">The <see cref="SpawnableFaction"/> whose <see cref="ILimitedWave"/>'s tokens to set.</param>
        /// <param name="amount">The amount of tokens to set.</param>
        /// <returns><c>true</c> if tokens were successfully set for an <see cref="ILimitedWave"/>, otherwise <c>false</c>.</returns>
        public static bool SetTokens(SpawnableFaction spawnableFaction, int amount)
        {
            if (TryGetWaveBase(spawnableFaction, out SpawnableWaveBase waveBase) && waveBase is ILimitedWave limitedWave)
            {
                limitedWave.RespawnTokens = amount;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Grants influence to a given <see cref="Faction"/>'s <see cref="ILimitedWave"/>s.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> to whose <see cref="ILimitedWave"/>s to grant influence.</param>
        /// <param name="amount">The amount of influence to grant.</param>
        public static void GrantInfluence(Faction faction, int amount) => FactionInfluenceManager.Add(faction, amount);

        /// <summary>
        /// Removes influence from a given <see cref="Faction"/>'s <see cref="ILimitedWave"/>s.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> from whose <see cref="ILimitedWave"/>s to remove influence.</param>
        /// <param name="amount">The amount of influence to remove.</param>
        public static void RemoveInfluence(Faction faction, int amount) => FactionInfluenceManager.Remove(faction, amount);

        /// <summary>
        /// Get influence to a given <see cref="Faction"/>.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> to get influence.</param>
        /// <returns>Get the faction influence..</returns>
        public static float GetInfluence(Faction faction) => FactionInfluenceManager.Get(faction);

        /// <summary>
        /// Set influence to a given <see cref="Faction"/>.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> to set influence.</param>
        /// <param name="influence">The amount of influence to set.</param>
        public static void SetInfluence(Faction faction, float influence) => FactionInfluenceManager.Set(faction, influence);

        /// <summary>
        /// Starts the spawn sequence of the given <see cref="Faction"/>.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> whose wave to spawn.</param>
        /// <param name="isMini">Whether the wave should be a mini wave or not.</param>
        public static void ForceWave(Faction faction, bool isMini = false)
        {
            if (faction.TryGetSpawnableFaction(out SpawnableFaction spawnableFaction, isMini))
            {
                ForceWave(spawnableFaction);
            }
        }

        /// <summary>
        /// Starts the spawn sequence of the given <see cref="SpawnableFaction"/>.
        /// </summary>
        /// <param name="spawnableFaction">The <see cref="SpawnableFaction"/> whose wave to spawn.</param>
        public static void ForceWave(SpawnableFaction spawnableFaction)
        {
            if (TryGetWaveBase(spawnableFaction, out SpawnableWaveBase spawnableWaveBase))
            {
                ForceWave(spawnableWaveBase);
            }
        }

        /// <summary>
        /// Starts the spawn sequence of the given <see cref="SpawnableWaveBase"/>.
        /// </summary>
        /// <param name="spawnableWaveBase">The <see cref="SpawnableWaveBase"/> to spawn.</param>
        public static void ForceWave(SpawnableWaveBase spawnableWaveBase)
        {
            WaveManager.Spawn(spawnableWaveBase);
        }

        /// <summary>
        /// Pauses respawn waves by removing them from <see cref="WaveManager.Waves">WaveManager.Waves</see> and storing them in <see cref="PausedWaves"/>.
        /// </summary>
        /// <!--Beryl said this should work fine but it requires testing-->
        public static void PauseWaves()
        {
            PausedWaves.Clear();
            PausedWaves.AddRange(WaveManager.Waves);
            WaveManager.Waves.Clear();
        }

        /// <summary>
        /// Resumes respawn waves by filling <see cref="WaveManager.Waves">WaveManager.Waves</see> with values stored in <see cref="PausedWaves"/>.
        /// </summary>
        /// <!--Beryl said this should work fine but it requires testing-->
        /// <remarks>This also clears <see cref="PausedWaves"/>.</remarks>
        public static void ResumeWaves()
        {
            WaveManager.Waves.Clear();
            WaveManager.Waves.AddRange(PausedWaves);
            PausedWaves.Clear();
        }

        /// <summary>
        /// Restarts respawn waves by clearing <see cref="WaveManager.Waves">WaveManager.Waves</see> and filling it with new values..
        /// </summary>
        /// <!--Beryl said this should work fine but it requires testing-->
        /// <remarks>This also clears <see cref="PausedWaves"/>.</remarks>
        public static void RestartWaves()
        {
            WaveManager.Waves.Clear();
            WaveManager.Waves.AddRange(new List<SpawnableWaveBase> { new ChaosMiniWave(), new ChaosSpawnWave(), new NtfMiniWave(), new NtfSpawnWave() });
            PausedWaves.Clear();
        }

        /// <summary>
        /// Tries to get the influence value of a given <see cref="Faction"/>.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> whose influence to get.</param>
        /// <param name="influence">The amount of influence a faction has.</param>
        /// <returns>Whether an entry was successfully found.</returns>
        public static bool TryGetFactionInfluence(Faction faction, out float influence) => FactionInfluence.TryGetValue(faction, out influence);
    }
}