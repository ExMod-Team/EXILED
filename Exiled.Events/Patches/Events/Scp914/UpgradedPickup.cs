// -----------------------------------------------------------------------
// <copyright file="UpgradedPickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp914;

    using global::Scp914;

    using Handlers;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp914Upgrader.ProcessPickup" />.
    /// Adds the <see cref="Scp914.UpgradedPickup" /> event.
    /// </summary>
    [EventPatch(typeof(Scp914), nameof(Scp914.UpgradedPickup))]
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPickup))]
    internal static class UpgradedPickup
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_2);

            List<Label> label = newInstructions[index].ExtractLabels();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // pickup
                    new(OpCodes.Ldarg_0),

                    // outputPos
                    new(OpCodes.Ldloc_1),

                    // knobSetting
                    new(OpCodes.Ldarg_2),

                    // resultingPickups
                    new(OpCodes.Ldloc_S, 4),
                    new(OpCodes.Ldfld, Field(typeof(Scp914Result), nameof(Scp914Result.ResultingPickups))),

                    // UpgradedPickupEventArgs ev = new(pickup, outputPos, knobSetting, resultingPickups)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradedPickupEventArgs))[0]),

                    // Handlers.Scp914.OnUpgradingPickup(ev);
                    new(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnUpgradedPickup))),
                });

            newInstructions[index].WithLabels(label);

            foreach (CodeInstruction t in newInstructions)
                yield return t;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}