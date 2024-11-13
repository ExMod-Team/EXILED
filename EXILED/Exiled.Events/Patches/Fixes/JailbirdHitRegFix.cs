// -----------------------------------------------------------------------
// <copyright file="JailbirdHitRegFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
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
            var newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            var index = newInstructions.FindIndex(i => i.Calls(Method(typeof(ReferenceHubReaderWriter), nameof(ReferenceHubReaderWriter.TryReadReferenceHub)))) - 2;

            var breakIndex = newInstructions.FindIndex(i => i.Calls(Method(typeof(JailbirdHitreg), nameof(JailbirdHitreg.DetectDestructibles)))) - 1;
            var breakLabel = generator.DefineLabel();
            newInstructions[breakIndex].WithLabels(breakLabel);

            var loopBeginingLabels = newInstructions[index].ExtractLabels();

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

            foreach (var instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
