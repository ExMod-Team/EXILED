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

            Label continueLabel = generator.DefineLabel();

            int offset = -1;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(NetworkBehaviour), nameof(NetworkBehaviour.isLocalPlayer)))) + offset;

            // set label for code right after OnAdded/OnItemAdded, to skip that part for ammo
            Label afterAmmoLabel = newInstructions[index].labels[0];

            offset = -2;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ItemBase), nameof(ItemBase.OnAdded)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // CallBefore(itemBase, pickup)
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Ldarg_S, 4),
                    new(OpCodes.Call, Method(typeof(FixOnAddedBeingCallAfterOnRemoved), nameof(FixOnAddedBeingCallAfterOnRemoved.CallBefore))),

                    // if (itemBase is not AmmoItem)
                    //     skip;
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Isinst, typeof(AmmoItem)),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // call help method for inverse call
                    // InverseCall(itemBase, inv._hub, pickup)
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Ldarg_S, 4),
                    new(OpCodes.Call, Method(typeof(FixOnAddedBeingCallAfterOnRemoved), nameof(FixOnAddedBeingCallAfterOnRemoved.InverseCall))),

                    // move after basegame OnAdded/OnItemAdded
                    new(OpCodes.Br_S, afterAmmoLabel),

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void InverseCall(ItemBase item, ReferenceHub referenceHub, ItemPickupBase pickup)
        {
            Exiled.API.Extensions.ReflectionExtensions.InvokeStaticEvent(typeof(InventoryExtensions), nameof(InventoryExtensions.OnItemAdded), new object[] { referenceHub, item, pickup });
            item.OnAdded(pickup);
        }

        private static void CallBefore(ItemBase itemBase, ItemPickupBase pickupBase)
        {
            Item item = Item.Get(itemBase);
            Pickup pickup = Pickup.Get(pickupBase);
            item.ReadPickupInfoBefore(pickup);
        }
    }
}
