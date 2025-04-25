// -----------------------------------------------------------------------
// <copyright file="LockerSpawnPoint.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Lockers;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Handles the spawn point inside a locker.
    /// </summary>
    public class LockerSpawnPoint : SpawnPoint
    {
        /// <summary>
        /// Gets or sets the zone where the locker is located.
        /// </summary>
        public ZoneType Zone { get; set; } = ZoneType.Unspecified;

        /// <summary>
        /// Gets or sets a value indicating whether to use a random locker chamber's position for spawning.
        /// If <see langword="true"/>, <see cref="Offset"/> will be ignored.
        /// </summary>
        public bool UseChamber { get; set; }

        /// <summary>
        /// Gets or sets the offset position within the locker where the spawn point is located, relative to the locker's origin.
        /// </summary>
        public Vector3 Offset { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the type of the <see cref="Locker"/>.
        /// </summary>
        public LockerType Type { get; set; } = LockerType.Unknown;

        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
        [YamlIgnore]
        public override string Name
        {
            get => Zone.ToString();
            set => throw new InvalidOperationException("The name of this type of SpawnPoint cannot be changed.");
        }

        /// <inheritdoc/>
        [YamlIgnore]
        public override Vector3 Position
        {
            get
            {
                GetSpawningInfo(out _, out _, out Vector3 position);
                return position;
            }
            set => throw new InvalidOperationException("The position of this type of SpawnPoint cannot be changed.");
        }

        /// <summary>
        /// Gets the spawn info.
        /// </summary>
        /// <param name="locker">The locker to spawn in.</param>
        /// <param name="chamber">The chamber to spawn in. Null when <see cref="UseChamber"/> is false.</param>
        /// <param name="position">The position to spawn in.</param>
        /// <exception cref="NullReferenceException">No locker was found.</exception>
        public void GetSpawningInfo(out Locker locker, out Chamber chamber, out Vector3 position)
        {
            locker = Locker.Random(Zone, Type) ?? throw new NullReferenceException($"No locker found of type {Type} in {Zone}.");
            chamber = UseChamber ? locker.Chambers.GetRandomValue() : null;
            position = chamber?.GetRandomSpawnPoint() ?? (Offset == Vector3.zero ? locker.Position : locker.Transform.TransformPoint(Offset));
        }
    }
}
