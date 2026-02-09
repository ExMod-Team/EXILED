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

    using Exiled.API.Features.Pools;

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
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.LoadsConstant(64)) + offset;

            newInstructions[index].opcode = OpCodes.Blt_S;

            index = newInstructions.FindLastIndex(x => x.LoadsConstant(64)) + offset;

            newInstructions[index].opcode = OpCodes.Blt_S;

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}