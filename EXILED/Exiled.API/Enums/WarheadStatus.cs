// -----------------------------------------------------------------------
// <copyright file="WarheadStatus.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    /// <summary>
    /// All the available warhead statuses.
    /// </summary>
    /// <seealso cref="Features.Warhead.Status"/>
    [Flags]
    public enum WarheadStatus
    {
        /// <summary>
        /// The warhead is not armed.
        /// </summary>
        NotArmed = 0,

        /// <summary>
        /// The warhead is armed.
        /// </summary>
        Armed = 1,

        /// <summary>
        /// The warhead detonation is in progress.
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// The warhead has detonated.
        /// </summary>
        Detonated = 4,

        /// <summary>
        /// The warhead is on cooldown.
        /// </summary>
        OnCooldown = 8,
    }
}