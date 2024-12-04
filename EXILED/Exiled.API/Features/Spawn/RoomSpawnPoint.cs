// -----------------------------------------------------------------------
// <copyright file="RoomSpawnPoint.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.API.Features.Spawn
{
    using System;

    using Exiled.API.Enums;

    using UnityEngine;

    using YamlDotNet.Serialization;

    /// <summary>
    /// Represents a spawn point within a specific room in the game.
    /// </summary>
    public class RoomSpawnPoint : SpawnPoint
    {
        /// <summary>
        /// Gets or sets the room type used for this spawn.
        /// </summary>
        public RoomType Room { get; set; }

        /// <summary>
        /// Gets or sets the offset position within the room where the spawn point is located, relative to the room's origin.
        /// </summary>
        public Vector3 Offset { get; set; } = Vector3.zero;

        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
        [YamlIgnore]
        public override string Name
        {
            get => Room.ToString();
            set => throw new InvalidOperationException("The name of this type of SpawnPoint cannot be changed.");
        }

        /// <inheritdoc/>
        [YamlIgnore]
        public override Vector3 Position
        {
            get
            {
                Room roomInstance = Features.Room.Get(Room) ?? throw new InvalidOperationException("The room instance could not be found.");

                if (roomInstance.Type == RoomType.Surface)
                    return Offset != Vector3.zero ? Offset : roomInstance.Position;

                return Offset != Vector3.zero ? roomInstance.transform.TransformPoint(Offset) : roomInstance.Position;
            }
            set => throw new InvalidOperationException("The position of this type of SpawnPoint cannot be changed.");
        }
    }
}
