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
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pools;

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

            Label nonAmmoLabel = generator.DefineLabel();
            Label eventNullLabel = generator.DefineLabel();
            Label runEventLabel = generator.DefineLabel();

            int offset = -1;
            int index = newInstructions.FindLastIndex(x => x.Calls(PropertyGetter(typeof(NetworkBehaviour), nameof(NetworkBehaviour.isLocalPlayer)))) + offset;
            Label skipLabel = newInstructions[index].labels.First();

            offset = -2;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ItemBase), nameof(ItemBase.OnAdded)))) + offset;

            newInstructions[index].labels.Add(nonAmmoLabel);

            /*
                // Modify this
                itemBase2.OnAdded(pickup);
                Action<ReferenceHub, ItemBase, ItemPickupBase> onItemAdded = InventoryExtensions.OnItemAdded;
                if (onItemAdded != null)
                {
                    onItemAdded(inv._hub, itemBase2, pickup);
                }

                // To this
                if (type.IsAmmo())
                {
                  Action<ReferenceHub, ItemBase, ItemPickupBase> onItemAdded = InventoryExtensions.OnItemAdded;
                  if (onItemAdded != null)
                    onItemAdded(inv._hub, itemInstance, pickup);

                  itemInstance.OnAdded(pickup);
                }
                else
                {
                  itemInstance.OnAdded(pickup);

                  Action<ReferenceHub, ItemBase, ItemPickupBase> onItemAdded = InventoryExtensions.OnItemAdded;
                  if (onItemAdded != null)
                    onItemAdded(inv._hub, itemInstance, pickup);
                }
            */

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // CallBefore(itemBase, pickup)
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldarg_S, 4),
                    new(OpCodes.Call, Method(typeof(FixOnAddedBeingCallAfterOnRemoved), nameof(FixOnAddedBeingCallAfterOnRemoved.CallBefore))),

                    // if (!type.IsAmmo())
                    //     goto default behavior
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.IsAmmo))),
                    new(OpCodes.Brfalse_S, nonAmmoLabel),

                    // Event first
                    new(OpCodes.Ldsfld, Field(typeof(InventoryExtensions), nameof(InventoryExtensions.OnItemAdded))),
                    new(OpCodes.Dup),
                    new(OpCodes.Brtrue_S, runEventLabel),

                    // skip if null
                    new(OpCodes.Pop),
                    new(OpCodes.Br_S, eventNullLabel),

                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(runEventLabel),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldarg_S, 4),
                    new(OpCodes.Callvirt, Method(typeof(Action<ReferenceHub, ItemBase, ItemPickupBase>), nameof(Action<ReferenceHub, ItemBase, ItemPickupBase>.Invoke))),

                    // THEN OnAdded
                    new CodeInstruction(OpCodes.Ldloc_1).WithLabels(eventNullLabel),
                    new(OpCodes.Ldarg_S, 4),
                    new(OpCodes.Callvirt, Method(typeof(ItemBase), nameof(ItemBase.OnAdded))),

                    new(OpCodes.Br_S, skipLabel),
                });

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
