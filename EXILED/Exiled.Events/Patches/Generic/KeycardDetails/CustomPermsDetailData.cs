// -----------------------------------------------------------------------
// <copyright file="CustomPermsDetailData.cs" company="ExMod Team">
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
    /// Patch for storing permission colors from custom keycards.
    /// </summary>
    [HarmonyPatch(typeof(CustomPermsDetail), nameof(CustomPermsDetail.WriteCustom))]
    public class CustomPermsDetailData
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(CustomKeycard), nameof(CustomKeycard.PermsColorDict))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(IIdentifierProvider), nameof(IIdentifierProvider.ItemId))),
                new(OpCodes.Ldfld, Field(typeof(ItemIdentifier), nameof(ItemIdentifier.SerialNumber))),
                new(OpCodes.Ldsfld, Field(typeof(CustomPermsDetail), nameof(CustomPermsDetail._customColor))),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, Color>), "set_Item")),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}