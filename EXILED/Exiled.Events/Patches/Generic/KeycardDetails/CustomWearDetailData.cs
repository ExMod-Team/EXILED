// -----------------------------------------------------------------------
// <copyright file="CustomWearDetailData.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic.KeycardDetails
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Items.Keycards;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch for storing wear from custom keycards.
    /// </summary>
    [HarmonyPatch(typeof(CustomWearDetail))]
    public class CustomWearDetailData
    {
        [HarmonyPatch(nameof(CustomWearDetail.WriteNewItem))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler1(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(CustomKeycard), nameof(CustomKeycard.WearDict))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(KeycardItem), nameof(KeycardItem.ItemSerial))),
                new(OpCodes.Ldsfld, Field(typeof(CustomWearDetail), nameof(CustomWearDetail._customWearLevel))),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, byte>), "set_Item")),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        [HarmonyPatch(nameof(CustomWearDetail.WriteNewPickup))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(CustomKeycard), nameof(CustomKeycard.WearDict))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(KeycardPickup), nameof(KeycardPickup.ItemId))),
                new(OpCodes.Ldfld, Field(typeof(ItemIdentifier), nameof(ItemIdentifier.SerialNumber))),
                new(OpCodes.Ldsfld, Field(typeof(CustomWearDetail), nameof(CustomWearDetail._customWearLevel))),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, byte>), "set_Item")),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}