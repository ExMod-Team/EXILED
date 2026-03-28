// -----------------------------------------------------------------------
// <copyright file="SingleUseKeycardRemainingUses.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Items.Keycards;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem.Items.Keycards;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="SingleUseKeycardItem.OnUsed"/>.
    /// </summary>
    [HarmonyPatch(typeof(SingleUseKeycardItem), nameof(SingleUseKeycardItem.OnUsed))]
    public class SingleUseKeycardRemainingUses
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder uses = generator.DeclareLocal(typeof(int));

            Label runLabel = generator.DefineLabel();

            // after the return
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + 1;

            // get the original jump label and set the jump to our new instructions
            object continueLabel = newInstructions[index - 2].operand;
            newInstructions[index - 2].operand = runLabel;

            newInstructions.InsertRange(index, new[]
            {
                // SingleUseKeycard.RemainingUses.TryGetValue(ItemSerial, out int uses);
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(SingleUseKeycard), nameof(SingleUseKeycard.RemainingUses))).WithLabels(runLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SingleUseKeycardItem), nameof(SingleUseKeycardItem.ItemSerial))),
                new(OpCodes.Ldloca, uses),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, int>), nameof(Dictionary<ushort, int>.TryGetValue))),

                // run default behavior (destroying keycard) if no stored uses are found.
                new(OpCodes.Brfalse, continueLabel),

                // uses--;
                new(OpCodes.Ldloc, uses),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Sub),
                new(OpCodes.Stloc, uses),

                // SingleUseKeycard.RemainingUses[ItemSerial] = uses;
                new(OpCodes.Call, PropertyGetter(typeof(SingleUseKeycard), nameof(SingleUseKeycard.RemainingUses))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SingleUseKeycardItem), nameof(SingleUseKeycardItem.ItemSerial))),
                new(OpCodes.Ldloc, uses),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, int>), "set_Item")),

                // if (uses < 1)
                //    run default behavior (destroying keycard)
                new(OpCodes.Ldloc, uses),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Blt, continueLabel),

                // Return;
                new(OpCodes.Ret),
            });
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}