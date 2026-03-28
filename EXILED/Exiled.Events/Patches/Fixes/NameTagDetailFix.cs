// -----------------------------------------------------------------------
// <copyright file="NameTagDetailFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;
    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Fixes NameTagDetail not using custom value if Custom Keycard was spawned as pickup.
    /// </summary>
    [HarmonyPatch(typeof(NametagDetail), nameof(NametagDetail.WriteNewPickup))]
    public class NameTagDetailFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label label = generator.DefineLabel();

            newInstructions[0].WithLabels(label);
            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // if (pickup.ItemId.TypeId.IsCustomKeycard)
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(KeycardPickup), nameof(KeycardPickup.ItemId))),
                new(OpCodes.Ldfld, Field(typeof(ItemIdentifier), nameof(ItemIdentifier.TypeId))),
                new(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.IsCustomKeycard))),
                new(OpCodes.Brfalse, label),

                // writer.WriteString(_customNametag);
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldsfld, Field(typeof(NametagDetail), nameof(NametagDetail._customNametag))),
                new(OpCodes.Call, Method(typeof(NetworkWriterExtensions), nameof(NetworkWriterExtensions.WriteString))),
                new(OpCodes.Ret),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}