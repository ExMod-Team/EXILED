// -----------------------------------------------------------------------
// <copyright file="FixEffectOrder.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Items.Usables.Scp330;
    using PlayerRoles.PlayableScps.Scp049;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1402 // File may only contain a single type

    /// <summary>
    /// Patches <see cref="StatusEffectBase.ServerSetState(byte, float, bool)"/>.
    /// Fix than NW do not updated the EffectDuration before Intensity https://github.com/northwood-studios/LabAPI/issues/248.
    /// </summary>
    [HarmonyPatch(typeof(StatusEffectBase), nameof(StatusEffectBase.ServerSetState))]
    internal class FixEffectOrder
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Find the setter call index
            int offset = -2;
            int intensityCallIndex = newInstructions.FindIndex(ci => ci.Calls(PropertySetter(typeof(StatusEffectBase), nameof(StatusEffectBase.Intensity)))) + offset;

            // Extract: ldarg.0, ldarg.1, call set_Intensity
            List<CodeInstruction> intensityBlock = newInstructions.GetRange(intensityCallIndex, 3);

            // Remove it from original location
            newInstructions.RemoveRange(intensityCallIndex, 3);
            newInstructions[intensityCallIndex].WithLabels(intensityBlock[0].ExtractLabels());

            // Find ServerChangeDuration call
            int serverChangeIndex = newInstructions.FindIndex(ci => ci.Calls(Method(typeof(StatusEffectBase), nameof(StatusEffectBase.ServerChangeDuration))));

            // Insert AFTER ServerChangeDuration
            newInstructions.InsertRange(serverChangeIndex + 1, intensityBlock);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches Method that should use <see cref="StatusEffectBase.ServerSetState(byte, float, bool)"/>.
    /// Fix than NW do not updated the EffectDuration before Intensity https://github.com/northwood-studios/LabAPI/issues/248.
    /// </summary>
    [HarmonyPatch]
    internal class FixEffectOrder2
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return Method(typeof(CandyYellow), nameof(CandyYellow.ServerApplyEffects));
            yield return Method(typeof(Scp049AttackAbility), nameof(Scp049AttackAbility.ServerProcessCmd));
            yield return Method(typeof(SugarCrave), nameof(SugarCrave.Disabled));
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int intensityCallIndex = newInstructions.FindIndex(ci => ci.Calls(PropertySetter(typeof(StatusEffectBase), nameof(StatusEffectBase.Intensity))));

            int offset = 6;
            if (newInstructions[intensityCallIndex - offset].opcode == OpCodes.Dup)
            {
                // specific to Yellow candy that get it's Instance from a OpCodes.Dup
                newInstructions.RemoveAt(intensityCallIndex);
                newInstructions.RemoveAt(intensityCallIndex - offset);
            }
            else
            {
                newInstructions.RemoveRange(intensityCallIndex, 2);
            }

            int serverChangeIndex = newInstructions.FindIndex(ci => ci.Calls(Method(typeof(StatusEffectBase), nameof(StatusEffectBase.ServerChangeDuration))));

            newInstructions[serverChangeIndex] = new CodeInstruction(OpCodes.Callvirt, Method(typeof(StatusEffectBase), nameof(StatusEffectBase.ServerSetState)));
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}