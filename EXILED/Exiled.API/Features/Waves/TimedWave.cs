// -----------------------------------------------------------------------
// <copyright file="TimedWave.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Waves
{
    using System.Collections.Generic;

    using System.Linq;

    using PlayerRoles;

    using Respawning;

    using Respawning.Waves;

    /// <summary>
    /// Represents a timed wave.
    /// </summary>
    public class TimedWave
    {
        private readonly TimeBasedWave timedWave;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedWave"/> class.
        /// </summary>
        /// <param name="wave">
        /// The <see cref="TimeBasedWave"/> that this class should be based off of.
        /// </param>
        public TimedWave(TimeBasedWave wave)
        {
            timedWave = wave;
        }

        /// <summary>
        /// Gets the name of the wave timer.
        /// </summary>
        public string Name => timedWave.GetType().Name;

        /// <summary>
        /// Gets a value indicating whether the wave is a mini wave.
        /// </summary>
        public bool IsMiniWave => timedWave is IMiniWave;

        /// <summary>
        /// Gets the wave timer instance.
        /// </summary>
        public WaveTimer Timer => new(timedWave.Timer);

        /// <summary>
        /// Gets the faction of this wave.
        /// </summary>
        public Faction Faction => timedWave.TargetFaction;

        /// <summary>
        /// Gets the team of this wave.
        /// </summary>
        public SpawnableTeamType Team => timedWave.TargetFaction.GetSpawnableTeam();

        /// <summary>
        /// Gets the maximum amount of people that can spawn in this wave.
        /// </summary>
        public int MaxAmount => timedWave.MaxWaveSize;

        /// <summary>
        /// Get the timed waves for the specified faction.
        /// </summary>
        /// <param name="faction">
        /// The faction to get the waves for.
        /// </param>
        /// <param name="waves">
        /// The waves if found.
        /// </param>
        /// <returns>
        /// A value indicating whether the wave were found.
        /// </returns>
        public static bool TryGetTimedWaves(Faction faction, out List<TimedWave> waves)
        {
            List<SpawnableWaveBase> spawnableWaveBases = WaveManager.Waves.Where(w => w is TimeBasedWave wave && wave.TargetFaction == faction).ToList();
            if(!spawnableWaveBases.Any())
            {
                waves = null;
                return false;
            }

            waves = spawnableWaveBases.Select(w => new TimedWave((TimeBasedWave)w)).ToList();
            return true;
        }

        /// <summary>
        /// Get the timed wave for the specified team.
        /// </summary>
        /// <param name="team">
        /// The team to get the wave for.
        /// </param>
        /// <param name="waves">
        /// The waves if found.
        /// </param>
        /// <returns>
        /// A value indicating whether the wave were found.
        /// </returns>
        public static bool TryGetTimedWaves(SpawnableTeamType team, out List<TimedWave> waves)
        {
            if (team == SpawnableTeamType.None)
            {
                waves = null;
                return false;
            }

            Faction faction = team == SpawnableTeamType.NineTailedFox ? Faction.FoundationStaff : Faction.FoundationEnemy;

            return TryGetTimedWaves(faction, out waves);
        }

        /// <summary>
        /// Get the timed wave for the specified type.
        /// </summary>
        /// <param name="wave">
        /// The wave type to get.
        /// </param>
        /// <typeparam name="T">
        /// The type of wave to get. Must be a <see cref="TimeBasedWave"/>. I.e. <see cref="NtfSpawnWave"/> or <see cref="NtfMiniWave"/>.
        /// </typeparam>
        /// <returns>
        /// A value indicating whether the wave was found.
        /// </returns>
        public static bool TryGetTimedWave<T>(out TimedWave wave)
            where T : TimeBasedWave
        {
            foreach (SpawnableWaveBase waveBase in WaveManager.Waves)
            {
                if (waveBase is not TimeBasedWave timeWave || timeWave.GetType() != typeof(T))
                    continue;

                wave = new(timeWave);
                return true;
            }

            wave = null;
            return false;
        }

        /// <summary>
        /// Get all timed waves.
        /// </summary>
        /// <returns>
        /// A list of all timed waves.
        /// </returns>
        public static List<TimedWave> GetTimedWaves()
        {
            List<TimedWave> waves = new();
            foreach (SpawnableWaveBase wave in WaveManager.Waves)
            {
                if (wave is TimeBasedWave timeBasedWave)
                {
                    waves.Add(new (timeBasedWave));
                }
            }

            return waves;
        }

        /// <summary>
        /// Destroys this wave.
        /// </summary>
        public void Destroy()
        {
            timedWave.Destroy();
        }

        /// <summary>
        /// Populates this wave with the specified amount of roles.
        /// </summary>
        /// <param name="queue">
        /// The queue to populate.
        /// </param>
        /// <param name="amount">
        /// The amount of people to populate.
        /// </param>
        public void PopulateQueue(Queue<RoleTypeId> queue, int amount)
        {
            timedWave.PopulateQueue(queue, amount);
        }
    }
}