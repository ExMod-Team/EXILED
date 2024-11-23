// -----------------------------------------------------------------------
// <copyright file="StaticSpawnPoint.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{
    using UnityEngine;

    /// <summary>
    /// Handles static spawn locations.
    /// </summary>
    public class StaticSpawnPoint : SpawnPoint
    {
        /// <inheritdoc/>
        public override string Name { get; set; }

        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
        public override Vector3 Position { get; set; }
    }
}