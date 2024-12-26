// -----------------------------------------------------------------------
// <copyright file="UncuffReason.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Reasons that player gets uncuffed.
    /// </summary>
    public enum UncuffReason
    {
        /// <summary>
        /// Uncuffed by a player.
        /// </summary>
        Player,

        /// <summary>
        /// Uncuffed due to the distance between cuffer and target.
        /// </summary>
        OutOfRange,

        /// <summary>
        /// Uncuffed due to the cuffer no longer alive.
        /// </summary>
        CufferDied,
    }
}
