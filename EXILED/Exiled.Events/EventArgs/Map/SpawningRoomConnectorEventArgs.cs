// -----------------------------------------------------------------------
// <copyright file="SpawningRoomConnectorEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using MapGeneration.RoomConnectors;
    using MapGeneration.RoomConnectors.Spawners;

    /// <summary>
    /// Contains all information before spawning the connector between rooms.
    /// </summary>
    public class SpawningRoomConnectorEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningRoomConnectorEventArgs" /> class.
        /// </summary>
        /// <param name="roomConnectorSpawnpointBase">The RoomConnectorSpawnpointBase.</param>
        /// <param name="connectorType">The connector type the game is trying to spawn.</param>
        public SpawningRoomConnectorEventArgs(RoomConnectorSpawnpointBase roomConnectorSpawnpointBase, SpawnableRoomConnectorType connectorType)
        {
            RoomConnectorSpawnpoint = roomConnectorSpawnpointBase;
            ConnectorType = connectorType;
            RoomForward = Room.Get(RoomConnectorSpawnpoint._parentRoom);
            RoomBackward = Room.Get(RoomConnectorSpawnpoint.transform.position + (RoomConnectorSpawnpoint.transform.forward * -1));
        }

        /// <summary>
        /// Gets the RoomConnectorSpawnpointBase.
        /// </summary>
        public RoomConnectorSpawnpointBase RoomConnectorSpawnpoint { get; }

        /// <summary>
        /// Gets the Room forward of the Connector.
        /// </summary>
        public Room RoomForward { get; }

        /// <summary>
        /// Gets the Room Backward of the Connector.
        /// </summary>
        public Room RoomBackward { get; }

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
