// -----------------------------------------------------------------------
// <copyright file="Fix106ItemManager.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp106;

    /// <summary>
    /// Patches the <see cref="Scp106PocketItemManager.GetRandomValidSpawnPosition()"/> method.
    /// Fixes an error caused by this method cuz NW doesn't know how to do array indexing.
    /// </summary>
    [HarmonyPatch(typeof(Scp106PocketItemManager), nameof(Scp106PocketItemManager.GetRandomValidSpawnPosition))]
    public class Fix106ItemManager
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new(instructions);

            // change all (maxExclusive > 64) to (maxExclusive >= 64)
            matcher
                .MatchStartForward(new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)64))
                .Advance(1)
                .ThrowIfInvalid("Transpiler failed first match")
                .SetOpcodeAndAdvance(OpCodes.Blt_S)
                .MatchStartForward(new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)64))
                .Advance(1)
                .ThrowIfInvalid("Transpiler failed second match")
                .SetOpcodeAndAdvance(OpCodes.Blt_S);

            return matcher.Instructions();
        }
    }
}