// -----------------------------------------------------------------------
// <copyright file="UsedItemByHolstering.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.Usables;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Consumable.OnHolstered" />.
    /// Adds an alternative trigger of the <see cref="Handlers.Player.UsedItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsedItem))]
    [HarmonyPatch(typeof(Consumable), nameof(Consumable.OnHolstered))]
    public class UsedItemByHolstering
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // after ServerRemoveSelf, which lines up with before the ret
            newInstructions.InsertRange(newInstructions.Count - 2, new CodeInstruction[]
            {
                // this.Owner
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Consumable), nameof(Consumable.Owner))),

                // this (Consumable inherits UsableItem)
                new(OpCodes.Ldarg_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // OnUsedItem(new UsedItemEventArgs(ReferenceHub, UsableItem, bool));
                new(OpCodes.Newobj, Constructor(typeof(UsedItemEventArgs), new[] { typeof(ReferenceHub), typeof(UsableItem), typeof(bool) })),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsedItem))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}