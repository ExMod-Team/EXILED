// -----------------------------------------------------------------------
// <copyright file="CustomRankDetailData.cs" company="ExMod Team">
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
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch for storing rank from custom keycards.
    /// </summary>
    [HarmonyPatch(typeof(CustomRankDetail))]
    public class CustomRankDetailData
    {
        [HarmonyPatch(nameof(CustomRankDetail.WriteNewItem))]
        private static IEnumerable<CodeInstruction> Transpiler1(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(CustomKeycard), nameof(CustomKeycard.RankDict))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(KeycardItem), nameof(KeycardItem.ItemSerial))),
                new(OpCodes.Ldsfld, Field(typeof(CustomRankDetail), nameof(CustomRankDetail._index))),

                new(OpCodes.Call, Method(typeof(Mathf), nameof(Mathf.Abs))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CustomRankDetail), nameof(CustomRankDetail._options))),
                new(OpCodes.Ldlen),
                new(OpCodes.Conv_I4),
                new(OpCodes.Rem),
                new(OpCodes.Conv_U1),

                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, byte>), "set_Item")),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        [HarmonyPatch(nameof(CustomRankDetail.WriteNewPickup))]
        private static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(CustomKeycard), nameof(CustomKeycard.RankDict))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(KeycardPickup), nameof(KeycardPickup.ItemId))),
                new(OpCodes.Ldfld, Field(typeof(ItemIdentifier), nameof(ItemIdentifier.SerialNumber))),
                new(OpCodes.Ldsfld, Field(typeof(CustomRankDetail), nameof(CustomRankDetail._index))),

                new(OpCodes.Call, Method(typeof(Mathf), nameof(Mathf.Abs))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CustomRankDetail), nameof(CustomRankDetail._options))),
                new(OpCodes.Ldlen),
                new(OpCodes.Conv_I4),
                new(OpCodes.Rem),
                new(OpCodes.Conv_U1),

                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, byte>), "set_Item")),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}