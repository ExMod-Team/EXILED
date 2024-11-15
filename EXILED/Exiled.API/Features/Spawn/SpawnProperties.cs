// -----------------------------------------------------------------------
// <copyright file="SpawnProperties.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Extensions;

    /// <summary>
    /// Handles special properties of spawning an item.
    /// </summary>
    public class SpawnProperties
    {
        /// <summary>
        /// Gets or sets a value indicating how many of the item can be spawned when the round starts.
        /// </summary>
        public uint Limit { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible dynamic spawn points.
        /// </summary>
        public List<DynamicSpawnPoint> DynamicSpawnPoints { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible static spawn points.
        /// </summary>
        public List<StaticSpawnPoint> StaticSpawnPoints { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible role-based spawn points.
        /// </summary>
        public List<RoleSpawnPoint> RoleSpawnPoints { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible room-based spawn points.
        /// </summary>
        public List<RoomSpawnPoint> RoomSpawnPoints { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible locker-based spawn points.
        /// </summary>
        public List<LockerSpawnPoint> LockerSpawnPoints { get; set; } = new();

        /// <summary>
        /// Counts how many spawn points are in this instance.
        /// </summary>
        /// <returns>How many spawn points there are.</returns>
        public int Count() => DynamicSpawnPoints.Count + StaticSpawnPoints.Count + RoleSpawnPoints.Count + RoomSpawnPoints.Count + LockerSpawnPoints.Count;

        /// <summary>
        /// Gets a random spawn point from all available.
        /// </summary>
        /// <returns>A random spawn point or <c>null</c> if <see cref="Count"/> is 0.</returns>
        public SpawnPoint GetRandomSpawnPoint()
        {
            List<SpawnPoint> points = new(DynamicSpawnPoints);
            points.AddRange(StaticSpawnPoints);
            points.AddRange(RoleSpawnPoints);
            points.AddRange(RoomSpawnPoints);
            points.AddRange(LockerSpawnPoints);

            for (int i = 0; i < 20; i++)
            {
                SpawnPoint point = points.GetRandomValue();

                if (point.Chance < UnityEngine.Random.value * 100)
                    return point;
            }

            return points.GetRandomValue();
        }

        /// <summary>
        /// Gets a random spawn point from all available.
        /// </summary>
        /// <param name="filter">A filter to choose a spawn point from.</param>
        /// <returns>A random spawn point or <c>null</c> if <see cref="Count"/> is 0.</returns>
        public SpawnPoint GetRandomSpawnPoint(Func<SpawnPoint, bool> filter)
        {
            List<SpawnPoint> points = new(DynamicSpawnPoints);
            points.AddRange(StaticSpawnPoints);
            points.AddRange(RoleSpawnPoints);
            points.AddRange(RoomSpawnPoints);
            points.AddRange(LockerSpawnPoints);

            for (int i = 0; i < 20; i++)
            {
                SpawnPoint point = points.GetRandomValue();

                if (point.Chance < UnityEngine.Random.value * 100)
                    return point;
            }

            return points.GetRandomValue(filter);
        }
    }
}