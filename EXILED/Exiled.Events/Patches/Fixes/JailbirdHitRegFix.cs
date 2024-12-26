// -----------------------------------------------------------------------
// <copyright file="JailbirdHitRegFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using HarmonyLib;
    using InventorySystem.Items.Jailbird;
    using Mirror;
    using Utils.Networking;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="JailbirdHitreg.ServerAttack(bool, NetworkReader)" />.
    /// </summary>
    [HarmonyPatch(typeof(JailbirdHitreg), nameof(JailbirdHitreg.ServerAttack))]
    internal static class JailbirdHitRegFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(ReferenceHubReaderWriter), nameof(ReferenceHubReaderWriter.TryReadReferenceHub)))) - 2;

            int breakIndex = newInstructions.FindIndex(i => i.Calls(Method(typeof(JailbirdHitreg), nameof(JailbirdHitreg.DetectDestructibles)))) - 1;
            Label breakLabel = generator.DefineLabel();
            newInstructions[breakIndex].WithLabels(breakLabel);

            List<Label> loopBeginingLabels = newInstructions[index].ExtractLabels();

            newInstructions.InsertRange(index, new[]
            {
                // reader.Remaining
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(NetworkReader), nameof(NetworkReader.Remaining))),

                // if (reader.Remaining == 0)
                //     break;
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue, breakLabel),
            });

            newInstructions[index].WithLabels(loopBeginingLabels);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
