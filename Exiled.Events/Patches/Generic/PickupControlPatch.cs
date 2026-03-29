// -----------------------------------------------------------------------
// <copyright file="PickupControlPatch.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features.Pickups;
    using API.Features.Pools;

    using Exiled.API.Features.Items;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using MapGeneration.Distributors;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1402 /// File may only contain a single type.
    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerCreatePickup(Inventory, ItemBase, PickupSyncInfo?, bool, Action{ItemPickupBase})"/> to save scale for pickups and control <see cref="Pickup.IsSpawned"/> property.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup), typeof(ItemBase), typeof(PickupSyncInfo), typeof(Vector3), typeof(Quaternion), typeof(bool), typeof(Action<ItemPickupBase>))]
    internal static class PickupControlPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_S) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // pickup = Pickup.Get(pickupBase);
                new(OpCodes.Ldloc_0),
                new(OpCodes.Call, GetDeclaredMethods(typeof(Pickup)).First(x => !x.IsGenericMethod && x.Name is nameof(Pickup.Get) && x.GetParameters().Length is 1 && x.GetParameters()[0].ParameterType == typeof(ItemPickupBase))),

                // Item.Get(itemBase);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, GetDeclaredMethods(typeof(Item)).First(x => !x.IsGenericMethod && x.Name is nameof(Item.Get) && x.GetParameters().Length is 1 && x.GetParameters()[0].ParameterType == typeof(ItemBase))),

                // pickup.ReadItemInfo(item);
                new(OpCodes.Callvirt, Method(typeof(Pickup), nameof(Pickup.ReadItemInfo))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="ItemDistributor.SpawnPickup"/> to control <see cref="Pickup.IsSpawned"/> property for delayed spawned pickup.
    /// </summary>
    [HarmonyPatch(typeof(ItemDistributor), nameof(ItemDistributor.SpawnPickup))]
    internal static class TriggerPickupControlPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // Pickup.Get(pickupBase)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, GetDeclaredMethods(typeof(Pickup)).First(x => !x.IsGenericMethod && x.Name is nameof(Pickup.Get) && x.GetParameters().Length is 1 && x.GetParameters()[0].ParameterType == typeof(ItemPickupBase))),
                new(OpCodes.Pop),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
