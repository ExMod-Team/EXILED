// -----------------------------------------------------------------------
// <copyright file="Scp939VisibilityStates.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Features.Roles;

    /// <summary>
    /// Unique identifier for a <see cref="Scp939Role"/>.
    /// </summary>
    public enum Scp939VisibilityState
    {
        /// <summary>
        /// SCP-939 doesnt see an other player, by default FPC role logic.
        /// </summary>
        None,

        /// <summary>
        /// SCP-939 doesnt see an player, by basic SCP-939 logic.
        /// </summary>
        NotSeen,

        /// <summary>
        /// SCP-939 sees an other player, who is teammate SCP.
        /// </summary>
        SeenAsScp,

        /// <summary>
        /// SCP-939 sees an other player due the Alpha Warhead detonation.
        /// </summary>
        SeenByDetonation,

        /// <summary>
        /// SCP-939 sees an other player, due the base-game vision range logic.
        /// </summary>
        SeenByRange,

        /// <summary>
        /// SCP-939 sees an other player for a while, after it's out of range.
        /// </summary>
        SeenByLastTime,
    }
}
