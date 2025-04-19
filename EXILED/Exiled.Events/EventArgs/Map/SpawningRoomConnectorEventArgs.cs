// -----------------------------------------------------------------------
// <copyright file="SpawningRoomConnectorEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.Events.EventArgs.Interfaces;
    using MapGeneration.RoomConnectors;

    /// <summary>
    /// Contains all information before spawning the connector between rooms.
    /// </summary>
    public class SpawningRoomConnectorEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningRoomConnectorEventArgs" /> class.
        /// </summary>
        /// <param name="connectorType">The connector type the game is trying to spawn.</param>
        public SpawningRoomConnectorEventArgs(SpawnableRoomConnectorType connectorType)
        {
            ConnectorType = connectorType;
        }

        /// <summary>
        /// Gets or sets which Connector the game should spawn.
        /// </summary>
        public SpawnableRoomConnectorType ConnectorType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the connector can be spawned.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
