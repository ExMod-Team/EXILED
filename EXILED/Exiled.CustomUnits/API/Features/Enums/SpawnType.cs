// -----------------------------------------------------------------------
// <copyright file="SpawnType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomUnits.API.Features.Enums
{
    /// <summary>
    /// All available spawn types for <see cref="CustomUnit"/>.
    /// </summary>
    public enum SpawnType
    {
        /// <summary>
        /// Unit can be spawned only via command.
        /// </summary>
        None,

        /// <summary>
        /// Unit can replace another non-custom unit if it has enough tickets.
        /// </summary>
        Ticket,

        /// <summary>
        /// Unit can replace another custom unit with specified chance.
        /// </summary>
        Chance,
    }
}