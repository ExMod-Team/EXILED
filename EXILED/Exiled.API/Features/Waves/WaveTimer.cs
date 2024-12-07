// -----------------------------------------------------------------------
// <copyright file="WaveTimer.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Waves
{
    using System;

    using System.Collections.Generic;

    using System.Linq;

    using PlayerRoles;

    using Respawning;

    using Respawning.Waves;

    /// <summary>
    /// Represents a wave timer.
    /// </summary>
    public class WaveTimer
    {
        /// <summary>
        /// Get the native <see cref="Respawning.Waves.WaveTimer"/>.
        /// </summary>
        private readonly Respawning.Waves.WaveTimer waveTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveTimer"/> class.
        /// </summary>
        /// <param name="wave">The <see cref="Respawning.Waves.WaveTimer"/> that this class should be based off of.</param>
        public WaveTimer(Respawning.Waves.WaveTimer wave)
        {
            waveTimer = wave;
        }

        /// <summary>
        /// Gets the name of the wave timer.
        /// </summary>
        public string Name => waveTimer._wave.GetType().Name;

        /// <summary>
        /// Gets a value indicating whether the wave is a mini wave.
        /// </summary>
        public bool IsMiniWave => waveTimer._wave is IMiniWave;

        /// <summary>
        /// Gets the amount of time left before the wave spawns.
        /// </summary>
        public TimeSpan TimeLeft => TimeSpan.FromSeconds(waveTimer.TimeLeft);

        /// <summary>
        /// Gets the amount of time passed since the last wave spawned.
        /// </summary>
        public TimeSpan TimePassed => TimeSpan.FromSeconds(waveTimer.TimePassed);

        /// <summary>
        /// Gets the amount of time left before this wave unpause.
        /// </summary>
        public TimeSpan PauseTimeLeft => TimeSpan.FromSeconds(waveTimer.PauseTimeLeft);

        /// <summary>
        /// Gets the amount of time this wave has been paused for.
        /// </summary>
        public TimeSpan PausedFor => TimeSpan.FromSeconds(waveTimer._pauseTimer);

        /// <summary>
        /// Gets a value indicating whether this wave is paused.
        /// </summary>
        public bool IsPaused => waveTimer.IsPaused;

        /// <summary>
        /// Gets a value indicating whether this wave is ready to spawn.
        /// </summary>
        public bool IsReady => waveTimer.IsReadyToSpawn;

        /// <summary>
        /// Gets a value indicating whether this wave is out of respawns.
        /// </summary>
        public bool IsRespawnable => !waveTimer.IsOutOfRespawns;

        /// <summary>
        /// Gets the default amount of time between a respawn of this wave.
        /// </summary>
        public float DefaultSpawnInterval => waveTimer.DefaultSpawnInterval;

        /// <summary>
        /// Gets the actual amount of time between a respawn of this wave.
        /// </summary>
        public float SpawnInterval => waveTimer.SpawnIntervalSeconds;

        /// <summary>
        /// Get the wave timers for the specified faction.
        /// </summary>
        /// <param name="faction">The faction.</param>
        /// <param name="waves">The waves, if any.</param>
        /// <returns>A bool indicating if waves were found.</returns>
        public static bool TryGetWaveTimers(Faction faction, out List<WaveTimer> waves)
        {
            if (!TimedWave.TryGetTimedWaves(faction, out List<TimedWave> timedWaves))
            {
                waves = null;
                return false;
            }

            waves = timedWaves.Select(wave => wave.Timer).ToList();
            return true;
        }

        /// <summary>
        /// Gets the wave timers for the specified team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="waves">The waves, if any.</param>
        /// <returns>A bool indicating if waves were found.</returns>
        public static bool TryGetWaveTimers(SpawnableTeamType team, out List<WaveTimer> waves)
        {
            if (!TimedWave.TryGetTimedWaves(team, out List<TimedWave> timedWaves))
            {
                waves = null;
                return false;
            }

            waves = timedWaves.Select(wave => wave.Timer).ToList();
            return true;
        }

        /// <summary>
        /// Gets all wave timers.
        /// </summary>
        /// <returns>A list of all wave timers.</returns>
        public static List<WaveTimer> GetWaveTimers()
        {
            return TimedWave.GetTimedWaves().Select(l => l.Timer).ToList();
        }

        /// <summary>
        /// Destroys this wave timer.
        /// </summary>
        public void Destroy()
        {
            waveTimer.Destroy();
        }

        /// <summary>
        /// Pauses this wave timer.
        /// </summary>
        /// <param name="seconds">
        /// The amount of time to pause this wave timer for.
        /// </param>
        public void Pause(float seconds)
        {
            waveTimer.Pause(seconds);
        }

        /// <summary>
        /// Unpauses this wave timer.
        /// </summary>
        public void Unpause()
        {
            waveTimer.Pause(0);
        }

        /// <summary>
        /// Resets this wave timer.
        /// </summary>
        /// <param name="resetInterval">
        /// A value indicating whether the <see cref="SpawnInterval"/> should be reset.
        /// </param>
        public void Reset(bool resetInterval = true)
        {
            waveTimer.Reset(resetInterval);
        }

        /// <summary>
        /// Update the timer.
        /// </summary>
        public void Update()
        {
            waveTimer.Update();
        }

        /// <summary>
        /// Add time to the wave timer.
        /// </summary>
        /// <param name="seconds">
        /// The amount of time to add in seconds.
        /// </param>
        public void AddTime(float seconds)
        {
            waveTimer.AddTime(seconds);
        }

        /// <summary>
        /// Set the amount of time before the wave spawns.
        /// </summary>
        /// <param name="time">
        /// The amount of time before the wave spawns.
        /// </param>
        public void SetTime(TimeSpan time) => SetTime((float)time.TotalSeconds);

        /// <summary>
        /// Set the amount of time before the wave spawns.
        /// </summary>
        /// <param name="seconds">
        /// The amount of time before the wave spawns, in seconds.
        /// </param>
        public void SetTime(float seconds)
        {
            waveTimer.SetTime(seconds);
        }
    }
}