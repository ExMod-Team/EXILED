// -----------------------------------------------------------------------
// <copyright file="SelectingRespawnTeamEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.EventArgs.Server
{
    using Sexiled.API.Enums;
    using Sexiled.API.Features.Waves;
    using Sexiled.Events.EventArgs.Interfaces;
    using Respawning.Waves;

    /// <summary>
    /// Contains all information before selecting the team to respawn next.
    /// </summary>
    public class SelectingRespawnTeamEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectingRespawnTeamEventArgs"/> class.
        /// </summary>
        /// <param name="wave"><inheritdoc cref="Wave"/>.</param>
        public SelectingRespawnTeamEventArgs(SpawnableWaveBase wave)
        {
            Wave = new TimedWave((TimeBasedWave)wave);
        }

        /// <summary>
        /// Gets <see cref="SpawnableFaction"/> that represents the team chosen to spawn.
        /// </summary>
        public SpawnableFaction Team => Wave.SpawnableFaction;

        /// <summary>
        /// Gets or sets <see cref="TimedWave"/> that is selected.
        /// </summary>
        public TimedWave Wave { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;
    }
}