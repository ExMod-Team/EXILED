// -----------------------------------------------------------------------
// <copyright file="ProjectileType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    using Extensions;

    /// <summary>
    /// Projectile types.
    /// </summary>
    /// <seealso cref="ItemExtensions.GetItemType(ProjectileType)"/>
    /// <seealso cref="ItemExtensions.GetProjectileType(ItemType)"/>
    public enum ProjectileType
    {
        /// <summary>
        /// Not a projectile.
        /// </summary>
        None,

        /// <summary>
        /// High explosive grenade.
        /// Used by <see cref="ItemType.GrenadeHE"/>.
        /// </summary>
        FragGrenade,

        /// <summary>
        /// Flashbang.
        /// Used by <see cref="ItemType.GrenadeFlash"/>.
        /// </summary>
        Flashbang,

        /// <summary>
        /// SCP-018 ball.
        /// Used by <see cref="ItemType.SCP018"/>.
        /// </summary>
        Scp018,

        /// <summary>
        /// SCP-2176 lightbulb.
        /// Used by <see cref="ItemType.SCP2176"/>.
        /// </summary>
        Scp2176,

        /// <summary>
        /// Coal.
        /// Used by <see cref="ItemType.Coal"/>.
        /// </summary>
        // [Obsolete("Only availaible for Christmas and AprilFools.")]
        Coal,

        /// <summary>
        /// SpecialCoal.
        /// Used by <see cref="ItemType.SpecialCoal"/>.
        /// </summary>
        // [Obsolete("Only availaible for Christmas and AprilFools.")]
        SpecialCoal,

        /// <summary>
        /// Snowball.
        /// Used by <see cref="ItemType.Snowball"/>.
        /// </summary>
        // [Obsolete("Only availaible for Christmas.")]
        Snowball,
    }
}