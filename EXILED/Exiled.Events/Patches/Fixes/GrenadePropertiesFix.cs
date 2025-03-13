// -----------------------------------------------------------------------
// <copyright file="GrenadePropertiesFix.cs" company="ExMod Team">
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

    using API.Features.Items;
    using API.Features.Pickups.Projectiles;
    using API.Features.Pools;

    using Exiled.API.Features.Pickups;

    using HarmonyLib;

    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Log = API.Features.Log;

    /// <summary>
    /// Patches <see cref="ThrowableItem.ServerThrow(float, float, Vector3, Vector3)"/> to fix all grenade properties.
    /// </summary>
    [HarmonyPatch(typeof(ThrowableItem), nameof(ThrowableItem.ServerThrow), typeof(float), typeof(float), typeof(Vector3), typeof(Vector3))]
    internal static class GrenadePropertiesFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = -1;
            int index = newInstructions.FindLastIndex(i => i.Calls(PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner)))) + offset;

            // if (Item.Get(this) is not Throwable throwable)
            // {
            //     Log.Error("Item is not Throwable, should never happen");
            //     return;
            // }
            // ((Projectile)Pickup.Get(projectile)).ReadThrowableItemInfo(throwable)
            newInstructions.InsertRange(index, new[]
            {
                // ((Projectile)Pickup.Get(projectile)).ReadThrowableItemInfo(throwable)
                new CodeInstruction(OpCodes.Dup),
                new(OpCodes.Call, GetDeclaredMethods(typeof(Pickup)).First(x => !x.IsGenericMethod && x.Name is nameof(Pickup.Get) && x.GetParameters().Length is 1 && x.GetParameters()[0].ParameterType == typeof(ItemPickupBase))),
                new(OpCodes.Isinst, typeof(Projectile)),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, GetDeclaredMethods(typeof(Item)).First(x => !x.IsGenericMethod && x.Name is nameof(Item.Get) && x.GetParameters().Length is 1 && x.GetParameters()[0].ParameterType == typeof(ItemBase))),
                new(OpCodes.Castclass, typeof(Throwable)),
                new(OpCodes.Callvirt, Method(typeof(Projectile), nameof(Projectile.ReadThrowableItemInfo))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void NotifyWrongType(Item item)
        {
            Log.Warn($"Item is not Throwable, should never happen: '{item}'");
        }
    }
}
