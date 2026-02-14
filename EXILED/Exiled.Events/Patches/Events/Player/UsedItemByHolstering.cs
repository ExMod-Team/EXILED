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

    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.Usables;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Consumable.OnHolstered" />.
    /// Adds an alternative trigger of the <see cref="Handlers.Player.UsedItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsedItem))]
    [HarmonyPatch(typeof(Consumable), nameof(Consumable.OnHolstered))]
    public class UsedItemByHolstering
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder handler = generator.DeclareLocal(typeof(PlayerHandler));
            LocalBuilder curr = generator.DeclareLocal(typeof(CurrentlyUsedItem));

            Label retLabel = generator.DefineLabel();

            // add retLabel to return
            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            // before ServerRemoveSelf, 2 instructions behind the return instruction
            newInstructions.InsertRange(newInstructions.Count - 1 - 2, new CodeInstruction[]
            {
                // if (!UsableItemsController.Handlers.TryGetValue(Owner, out PlayerHandler handler) return;
                new(OpCodes.Ldsfld, Field(typeof(UsableItemsController), nameof(UsableItemsController.Handlers))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Consumable), nameof(Consumable.Owner))),
                new(OpCodes.Ldloca_S, handler),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ReferenceHub, PlayerHandler>), nameof(Dictionary<ReferenceHub, PlayerHandler>.TryGetValue))),
                new(OpCodes.Brfalse_S, retLabel),

                // CurrentlyUsedItem curr = handler.CurrentUsable;
                new(OpCodes.Ldloc_S, handler),
                new(OpCodes.Ldfld, Field(typeof(PlayerHandler), nameof(PlayerHandler.CurrentUsable))),
                new(OpCodes.Stloc_S, curr),

                // if (curr.ItemSerial == 0) return;
                new(OpCodes.Ldloc_S, curr),
                new(OpCodes.Ldfld, Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.ItemSerial))),
                new(OpCodes.Brfalse_S, retLabel),

                // this check is to not call the event if the trigger was from UsableItemsController (the standard trigger for this event), contact @Someone on discord for more details
                // if (Time.timeSinceLevelLoad >= currentUsable.StartTime + (currentUsable.Item.UseTime / cons.ItemTypeId.GetSpeedMultiplier(hub))) return;
                new(OpCodes.Call, PropertyGetter(typeof(Time), nameof(Time.timeSinceLevelLoad))),
                new(OpCodes.Ldloc_S, curr),
                new(OpCodes.Ldfld, Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.StartTime))),
                new(OpCodes.Ldloc_S, curr),
                new(OpCodes.Ldfld, Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.Item))),
                new(OpCodes.Ldfld, Field(typeof(UsableItem), nameof(UsableItem.UseTime))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Consumable), nameof(Consumable.ItemTypeId))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Consumable), nameof(Consumable.Owner))),
                new(OpCodes.Call, Method(typeof(UsableItemModifierEffectExtensions), nameof(UsableItemModifierEffectExtensions.GetSpeedMultiplier))),
                new(OpCodes.Div),
                new(OpCodes.Add),
                new(OpCodes.Bgt_Un_S, retLabel),

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