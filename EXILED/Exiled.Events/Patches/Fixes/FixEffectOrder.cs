// -----------------------------------------------------------------------
// <copyright file="FixEffectOrder.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="StatusEffectBase.ServerSetState(byte, float, bool)"/> delegate.
    /// Fix than NW do not updated the EffectDuration before Intensity https://github.com/northwood-studios/LabAPI/issues/248.
    /// </summary>
    [HarmonyPatch(typeof(StatusEffectBase), nameof(StatusEffectBase.ServerSetState))]
    internal class FixEffectOrder
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Find the setter call index
            int intensityCallIndex = newInstructions.FindIndex(ci => ci.Calls(PropertySetter(typeof(StatusEffectBase), nameof(StatusEffectBase.Intensity))));

            // Extract: ldarg.0, ldarg.1, call set_Intensity
            List<CodeInstruction> intensityBlock = newInstructions.GetRange(intensityCallIndex - 2, 3);

            // Remove it from original location
            newInstructions.RemoveRange(intensityCallIndex - 2, 3);

            // Find ServerChangeDuration call
            int serverChangeIndex = newInstructions.FindIndex(ci => ci.Calls(Method(typeof(StatusEffectBase), nameof(StatusEffectBase.ServerChangeDuration))));

            // Insert AFTER ServerChangeDuration
            newInstructions.InsertRange(serverChangeIndex + 1, intensityBlock);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
