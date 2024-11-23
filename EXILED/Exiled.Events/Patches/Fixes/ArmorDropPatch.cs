// -----------------------------------------------------------------------
// <copyright file="ArmorDropPatch.cs" company="ExMod Team">
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
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Armor;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="BodyArmorUtils.RemoveEverythingExceedingLimits(Inventory, BodyArmor, bool, bool)"/> to fix https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/230 bug.
    /// </summary>
    [HarmonyPatch(typeof(BodyArmorUtils), nameof(BodyArmorUtils.RemoveEverythingExceedingLimits))]
    internal static class ArmorDropPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            int continueIndex = newInstructions.FindIndex(x => x.Is(OpCodes.Call, Method(typeof(Dictionary<ushort, ItemBase>.Enumerator), nameof(Dictionary<ushort, ItemBase>.Enumerator.MoveNext)))) - 1;
            newInstructions[continueIndex].WithLabels(continueLabel);

            // before: if (keyValuePair.Value.Category != ItemCategory.Armor)
            // after: if (keyValuePair.Value.Category != ItemCategory.Armor && keyValuePair.Value.Category != ItemCategory.None)
            int index = newInstructions.FindIndex(x => x.Is(OpCodes.Ldc_I4_S, 9));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // && keyValuePair.Value.Category != ItemCategory.None)
                new(OpCodes.Ldloca_S, 4),
                new(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<ushort, ItemBase>), nameof(KeyValuePair<ushort, ItemBase>.Value))),
                new(OpCodes.Ldfld, Field(typeof(ItemBase), nameof(ItemBase.Category))),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Beq_S, continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}