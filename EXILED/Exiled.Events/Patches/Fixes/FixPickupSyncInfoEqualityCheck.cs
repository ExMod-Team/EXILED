// -----------------------------------------------------------------------
// <copyright file="FixPickupSyncInfoEqualityCheck.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Footprinting;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items.Firearms.Ammo;
    using InventorySystem.Items.Pickups;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PickupSyncInfo.Equals(PickupSyncInfo)"/> method.
    /// Fix than NW don't check <see cref="PickupSyncInfo.Serial"/> for unequality.
    /// </summary>
    [HarmonyPatch(typeof(PickupSyncInfo), nameof(PickupSyncInfo.Equals), new[] { typeof(PickupSyncInfo) })]
    internal class FixPickupSyncInfoEqualityCheck
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 0;
            int index = newInstructions.FindLastIndex(instruction => instruction.LoadsConstant(0)) + offset;

            Label label = newInstructions[index].labels[0];

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(PickupSyncInfo), nameof(PickupSyncInfo.Serial))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(PickupSyncInfo), nameof(PickupSyncInfo.Serial))),
                    new(OpCodes.Bne_Un_S, label),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
