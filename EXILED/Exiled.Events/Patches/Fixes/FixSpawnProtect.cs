// -----------------------------------------------------------------------
// <copyright file="FixSpawnProtect.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;

    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.DamageHandlers;
    using HarmonyLib;
    using PlayerStatsSystem;

    /// <summary>
    /// Patches the <see cref="SpawnProtected.GetDamageModifier(float, PlayerStatsSystem.DamageHandlerBase, HitboxType)"/> delegate.
    /// Fix than SpawnProtect was protecting from litterally everything leading to SoftLock or Strange Gameplay like out of bound.
    /// Bug reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/2196).
    /// </summary>
    [HarmonyPatch(typeof(SpawnProtected), nameof(SpawnProtected.GetDamageModifier))]
    public class FixSpawnProtect
    {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// .
        /// </summary>
        public static List<DamageType> IgnoredDamageType = new List<DamageType>()
        {
            // Soft Lock the game
            DamageType.Warhead,

            // This shouldn't be protected
            DamageType.FriendlyFireDetector,

            // 100% chance of death
            DamageType.Decontamination,
            DamageType.SeveredEyes,
            DamageType.SeveredHands,
        };

        private static void Postfix(float baseDamage, PlayerStatsSystem.DamageHandlerBase handler, HitboxType hitboxType, ref float __result)
        {
            if (__result > 0)
                return;
            DamageType damageType = DamageTypeExtensions.GetDamageType(handler);
            if (IgnoredDamageType.Contains(damageType))
                __result = 1;
            else if (damageType is DamageType.Crushed && baseDamage == StandardDamageHandler.KillValue)
                __result = 1;
        }
    }
}
