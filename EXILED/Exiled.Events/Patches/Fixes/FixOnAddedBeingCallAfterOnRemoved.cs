// -----------------------------------------------------------------------
// <copyright file="FixOnAddedBeingCallAfterOnRemoved.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;

    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms.Ammo;
    using InventorySystem.Items.Pickups;

    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerAddItem"/>.
    /// Fix than NW call <see cref="InventoryExtensions.OnItemRemoved"/> before <see cref="InventoryExtensions.OnItemAdded"/> for AmmoItem.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
    internal class FixOnAddedBeingCallAfterOnRemoved
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ItemBase), nameof(ItemBase.OnAdded)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // CallBefore(itemBase, pickup)
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Ldarg_S, 4),
                    new(OpCodes.Call, Method(typeof(FixOnAddedBeingCallAfterOnRemoved), nameof(FixOnAddedBeingCallAfterOnRemoved.CallBefore))),
                });

            /*
                // Modify this
                itemBase2.OnAdded(pickup);
                Action<ReferenceHub, ItemBase, ItemPickupBase> onItemAdded = InventoryExtensions.OnItemAdded;
                if (onItemAdded != null)
                {
                    onItemAdded(inv._hub, itemBase2, pickup);
                }
                // To this
                Action<ReferenceHub, ItemBase, ItemPickupBase> onItemAdded = InventoryExtensions.OnItemAdded;
                if (onItemAdded != null)
                {
                    onItemAdded(inv._hub, itemBase2, pickup);
                }
                itemBase2.OnAdded(pickup);
            */
            int opCodesToMove = 3;
            offset = -2;

            int indexOnAdded = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ItemBase), nameof(ItemBase.OnAdded)))) + offset;
            offset = 1;

            int indexInvoke = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Action<ReferenceHub, ItemBase, ItemPickupBase>), nameof(Action<ReferenceHub, ItemBase, ItemPickupBase>.Invoke)))) + offset;

            // insert new OnAdded before the Event InventoryExtensions.OnItemAdded
            newInstructions.InsertRange(indexInvoke, newInstructions.GetRange(indexOnAdded, opCodesToMove));

            // move Label to not skip the OnAdded
            newInstructions[indexInvoke].MoveLabelsFrom(newInstructions[indexInvoke + opCodesToMove]);

            // remove the old OnAdded
            newInstructions.RemoveRange(indexOnAdded, opCodesToMove);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void CallBefore(ItemBase itemBase, ItemPickupBase pickupBase)
        {
            Item item = Item.Get(itemBase);
            Pickup pickup = Pickup.Get(pickupBase);
            item.ReadPickupInfoBefore(pickup);
        }
    }
}
