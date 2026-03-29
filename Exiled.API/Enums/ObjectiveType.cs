// -----------------------------------------------------------------------
// <copyright file="ObjectiveType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// An enum representing the different types of objectives.
    /// </summary>
    /// <seealso cref="Exiled.API.Features.Objectives.Objective" />
    public enum ObjectiveType
    {
        /// <summary>
        /// Unknown objective.
        /// </summary>
        None,

        /// <summary>
        /// Objective that is completed when SCP item is picked up.
        /// </summary>
        ScpItemPickup,

        /// <summary>
        /// Objective that is completed when enemy military is damaged.
        /// </summary>
        HumanDamage,

        /// <summary>
        /// Objective that is completed when enemy military is killed.
        /// </summary>
        HumanKill,

        /// <summary>
        /// Objective that is completed when generator is activated.
        /// </summary>
        GeneratorActivation,

        /// <summary>
        /// Objective that is completed when player escapes.
        /// </summary>
        Escape,
    }
}