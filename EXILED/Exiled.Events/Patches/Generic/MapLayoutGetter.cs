// -----------------------------------------------------------------------
// <copyright file="MapLayoutGetter.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="MapGeneration.AtlasZoneGenerator"/> to set the different layout properties in <see cref="Map"/>.
    /// </summary>
    [HarmonyPatch(typeof(MapGeneration.AtlasZoneGenerator), nameof(MapGeneration.AtlasZoneGenerator.Generate))]
    public class MapLayoutGetter
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder local = generator.DeclareLocal(typeof(int));

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldelem_Ref);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Stloc_S, local),
                new(OpCodes.Ldloc_S, local),
            });

            index += 3;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Dup),
                new(OpCodes.Ldloc_S, local),
                new(OpCodes.Call, Method(typeof(MapLayoutGetter), nameof(SetLayout))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void SetLayout(Texture2D tex, int index)
        {
            switch (tex.name.Substring(0, 3))
            {
                case "LC_":
                    if (index > 5)
                        Log.Warn($"Unknown layout: {tex}");

                    Map.LczLayout = (LczFacilityLayout)(index + 1);
                    return;
                case "HC_":
                    if (index > 10)
                        Log.Warn($"Unknown layout: {tex}");

                    Map.HczLayout = (HczFacilityLayout)(index + 1);
                    return;
                case "EZ_":
                    if (index > 5)
                        Log.Warn($"Unknown layout: {tex}");

                    Map.EzLayout = (EzFacilityLayout)(index + 1);
                    return;
                default:
                    Log.Warn($"Failed to parse layout name: {tex.name}!");
                    return;
            }
        }
    }
}