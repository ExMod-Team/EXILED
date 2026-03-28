// -----------------------------------------------------------------------
// <copyright file="FoundPositionEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp2536
{
    using Christmas.Scp2536;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after SCP-2536 chooses target for spawning.
    /// </summary>
    public class FoundPositionEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FoundPositionEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="spawnpoint"><inheritdoc cref="Spawnpoint"/></param>
        public FoundPositionEventArgs(Player player, Scp2536Spawnpoint spawnpoint)
        {
            Player = player;
            Spawnpoint = spawnpoint;
        }

        /// <summary>
        /// Gets the player near whom SCP-2536 will spawn.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the spawn point where SCP will spawn.
        /// </summary>
        public Scp2536Spawnpoint Spawnpoint { get; set; }
    }
}