// -----------------------------------------------------------------------
// <copyright file="TryRaycastRoomFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using MapGeneration;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoomUtils.TryRaycastRoom"/> to fix the method accidentally returning true and leaving when hitting a IRoomObject with a null original room.
    /// </summary>
    /// <remarks>
    /// Most of the logic comes from https://github.com/KadavasKingdom/SLFixes so shoutout to SlejmUr.
    /// </remarks>
    [HarmonyPatch(typeof(RoomUtils), nameof(RoomUtils.TryRaycastRoom))]
    public class TryRaycastRoomFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // room = comp1.OriginalRoom;
            // return true; -- here
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldc_I4_1);

            // just a check for if they fix this.
            if (newInstructions[index + 3].opcode != OpCodes.Ldarg_2)
            {
                for (int z = 0; z < newInstructions.Count; z++)
                    yield return newInstructions[z];

                ListPool<CodeInstruction>.Pool.Return(newInstructions);
                yield break;
            }

            // after the return, starts the next if statement
            Label skipLabel = newInstructions[index + 2].labels.First();

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // if (room is null)
                // goto skipLabel;
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldind_Ref),
                new(OpCodes.Ldnull),
                new(OpCodes.Call, Method(typeof(Object), "op_Inequality")),
                new(OpCodes.Brfalse_S, skipLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}