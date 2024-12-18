// -----------------------------------------------------------------------
// <copyright file="AdminToyType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Unique identifier for the different types of admin toys.
    /// </summary>
    /// <seealso cref="Features.Toys.AdminToy.ToyType"/>
    public enum AdminToyType
    {
        /// <summary>
        /// Primitive Object toy.
        /// </summary>
        PrimitiveObject,

        /// <summary>
        /// Light source toy.
        /// </summary>
        LightSource,

        /// <summary>
        /// ShootingTarget Radial sport target.
        /// </summary>
        ShootingTargetSport,

        /// <summary>
        /// ShootingTarget D-Class target.
        /// </summary>
        ShootingTargetClassD,

        /// <summary>
        /// ShootingTarget Binary target.
        /// </summary>
        ShootingTargetBinary,

        /// <summary>
        /// Speaker toy.
        /// </summary>
        Speaker,
    }
}