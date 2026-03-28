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

    using Exiled.API.Enums;
    using PlayerRoles;
    using Respawning;
    using Respawning.Announcements;
    using Respawning.Waves;

    /// <summary>
    /// Represents a timed wave.
    /// </summary>
    public class TimedWave
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimedWave"/> class.
        /// </summary>
        /// <param name="wave">
        /// The <see cref="TimeBasedWave"/> that this class should be based off of.
        /// </param>
        public TimedWave(TimeBasedWave wave) => Base = wave;

        /// <summary>
        /// Gets the base <see cref="TimeBasedWave"/>.
        /// </summary>
        public TimeBasedWave Base { get; }

        /// <summary>
        /// Gets the name of the wave timer.
        /// </summary>
        public string Name => Base.GetType().Name;

        /// <summary>
        /// Gets a value indicating whether the wave is a mini wave.
        /// </summary>
        public bool IsMiniWave => Base is IMiniWave;

        /// <summary>
        /// Gets the wave timer instance.
        /// </summary>
        public WaveTimer Timer => new(Base.Timer);

        /// <summary>
        /// Gets the faction of this wave.
        /// </summary>
        public Faction Faction => Base.TargetFaction;

        /// <summary>
        /// Gets the team of this wave.
        /// </summary>
        public Team Team => Base.TargetFaction.GetSpawnableTeam();

        /// <summary>
        /// Gets the spawnable faction for this wave.
        /// </summary>
        public SpawnableFaction SpawnableFaction => Faction switch
        {
            Faction.FoundationStaff when IsMiniWave => SpawnableFaction.NtfMiniWave,
            Faction.FoundationStaff => SpawnableFaction.NtfWave,
            Faction.FoundationEnemy when IsMiniWave => SpawnableFaction.ChaosMiniWave,
            _ => SpawnableFaction.ChaosWave
        };

        /// <summary>
        /// Gets the maximum amount of people that can spawn in this wave.
        /// </summary>
        public int MaxAmount => Base.MaxWaveSize;

        /// <summary>
        /// Gets the <see cref="WaveAnnouncementBase"/> for this wave.
        /// </summary>
        /// <remarks>Wave must implement <see cref="IAnnouncedWave"/>.</remarks>
        public WaveAnnouncementBase Announcement => Base is IAnnouncedWave announcedWave ? announcedWave.Announcement : null;

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
        /// Get the timed waves for the specified faction.
        /// </summary>
        /// <param name="team">The faction to get the waves for.</param>
        /// <param name="waves">The waves if found.</param>
        /// <returns>A value indicating whether the wave were found.</returns>
        public static bool TryGetTimedWaves(Team team, out List<TimedWave> waves)
        {
            List<SpawnableWaveBase> spawnableWaveBases = WaveManager.Waves.Where(w => w is TimeBasedWave wave && wave.TargetFaction.GetSpawnableTeam() == team).ToList();
            if(!spawnableWaveBases.Any())
            {
                waves = null;
                return false;
            }

            waves = spawnableWaveBases.Select(w => new TimedWave((TimeBasedWave)w)).ToList();
            return true;
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

                wave = new TimedWave(timeWave);
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
                    waves.Add(new TimedWave(timeBasedWave));
                }
            }

            return waves;
        }

        /// <summary>
        /// Destroys this wave.
        /// </summary>
        public void Destroy() => Base.Destroy();

        /// <summary>
        /// Populates this wave with the specified amount of roles.
        /// </summary>
        /// <param name="queue">
        /// The queue to populate.
        /// </param>
        /// <param name="amount">
        /// The amount of people to populate.
        /// </param>
        public void PopulateQueue(Queue<RoleTypeId> queue, int amount) => Base.PopulateQueue(queue, amount);

        /// <summary>
        /// Plays the announcement for this wave.
        /// </summary>
        /// <remarks>Wave must implement <see cref="IAnnouncedWave"/>.</remarks>
        public void PlayAnnouncement() => Announcement?.PlayAnnouncement([], Base as IAnnouncedWave);

        /// <summary>
        /// Plays the announcement for this wave.
        /// </summary>
        /// <remarks>Wave must implement <see cref="IAnnouncedWave"/>.</remarks>
        /// <param name="players">The list of Player to spawn.</param>
        public void PlayAnnouncement(IEnumerable<Player> players) => Announcement?.PlayAnnouncement(players.Select(x => x.ReferenceHub).ToList(), Base as IAnnouncedWave);
    }
}