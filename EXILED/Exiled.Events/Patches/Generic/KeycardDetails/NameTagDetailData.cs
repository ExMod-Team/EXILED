// -----------------------------------------------------------------------
// <copyright file="NameTagDetailData.cs" company="ExMod Team">
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
    using Exiled.Events.Patches.Fixes;
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch for storing name tags from custom keycards.
    /// </summary>
    /// <remarks>There's a fix for <see cref="NametagDetail.WriteNewPickup"/> (<see cref="NameTagDetailFix"/>) so I set the priority of this patch 1 lower than default to avoid errors.</remarks>
    [HarmonyPatch(typeof(NametagDetail))]
    [HarmonyPriority(399)]
    public class NameTagDetailData
    {
        [HarmonyPatch(nameof(NametagDetail.WriteNewItem))]
        private static IEnumerable<CodeInstruction> Transpiler1(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(CustomKeycard), nameof(CustomKeycard.NameTagDict))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(KeycardItem), nameof(KeycardItem.ItemSerial))),
                new(OpCodes.Ldsfld, Field(typeof(NametagDetail), nameof(NametagDetail._customNametag))),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, string>), "set_Item")),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        [HarmonyPatch(nameof(NametagDetail.WriteNewPickup))]
        private static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(CustomKeycard), nameof(CustomKeycard.NameTagDict))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(KeycardPickup), nameof(KeycardPickup.ItemId))),
                new(OpCodes.Ldfld, Field(typeof(ItemIdentifier), nameof(ItemIdentifier.SerialNumber))),
                new(OpCodes.Ldsfld, Field(typeof(NametagDetail), nameof(NametagDetail._customNametag))),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, string>), "set_Item")),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}