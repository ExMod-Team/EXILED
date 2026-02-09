// -----------------------------------------------------------------------
// <copyright file="IStructureSync.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using Exiled.API.Enums;

    using MapGeneration.Distributors;

    using Mirror;

    /// <summary>
    /// Represents an object with a <see cref="StructurePositionSync"/>.
    /// </summary>
    public interface IStructureSync
    {
        /// <summary>
        /// Gets the <see cref="StructurePositionSync"/> of this structure.
        /// </summary>
        public StructurePositionSync PositionSync { get; }

        /// <summary>
        /// Respawns the structure.
        /// </summary>
        /// <remarks>Called after every position or rotation change.</remarks>
        public void Respawn()
        {
            // not a prefab so respawning will just permanently destroy it and second call will disconnect clients
            if (this is Features.Lockers.Locker { Type: LockerType.MicroHid })
                return;

            if (NetworkServer.spawned.ContainsKey(PositionSync.netId))
                NetworkServer.UnSpawn(PositionSync.gameObject);

            NetworkServer.Spawn(PositionSync.gameObject);
        }
    }
}