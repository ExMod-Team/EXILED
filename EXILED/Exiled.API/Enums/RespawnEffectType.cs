// -----------------------------------------------------------------------
// <copyright file="RespawnEffectType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Features;

    using PlayerRoles;

    /// <summary>
    /// Layers game respawn effects.
    /// </summary>
    public enum RespawnEffectType
    {
        /// <summary>
        /// Summons the <see cref="Side.ChaosInsurgency"/> van.
        /// </summary>
        SummonChaosInsurgencyVan,

        /// <summary>
        /// Summons the NTF chopper.
        /// </summary>
        SummonNtfChopper,
    }
}