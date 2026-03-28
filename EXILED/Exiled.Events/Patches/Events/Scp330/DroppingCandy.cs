// -----------------------------------------------------------------------
// <copyright file="DroppingCandy.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp330;

    using Handlers;

    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp330;

    using Mirror;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches the <see cref="Scp330NetworkHandler.ServerDropCandy" /> method to add the
    /// <see cref="Scp330.DroppingScp330" /> event.
    /// </summary>
    [EventPatch(typeof(Scp330), nameof(Scp330.DroppingScp330))]
    [HarmonyPatch(typeof(Scp330NetworkHandler), nameof(Scp330NetworkHandler.ServerDropCandy))]
    internal static class DroppingCandy
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(DroppingScp330EventArgs));

            const int offset = -1;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(ItemBase), nameof(ItemBase.OwnerInventory)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // scp330Bag
                    new(OpCodes.Ldarg_0),

                    // candyKindID = GetCandyID(bag, index)
                    new(OpCodes.Dup),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(DroppingCandy), nameof(DroppingCandy.GetCandyID))),

                    // DroppingScp330EventArgs ev = new(Player, Scp330Bag, CandyKindID)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingScp330EventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Scp330.OnDroppingScp330(ev)
                    new(OpCodes.Call, Method(typeof(Scp330), nameof(Scp330.OnDroppingScp330))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingScp330EventArgs), nameof(DroppingScp330EventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Brfalse_S, returnLabel),

                    // index = GetCandyID(bag, candyKindID)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingScp330EventArgs), nameof(DroppingScp330EventArgs.Candy))),
                    new(OpCodes.Call, Method(typeof(DroppingCandy), nameof(DroppingCandy.GetIDCandy))),
                    new(OpCodes.Starg_S, 1),

                    // if (index == -1)
                    //    return;
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldc_I4_M1),
                    new(OpCodes.Beq, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static CandyKindID GetCandyID(Scp330Bag scp330Bag, int index)
        {
            if (index < 0 || index > scp330Bag.Candies.Count)
                return CandyKindID.None;
            return scp330Bag.Candies[index];
        }

        private static int GetIDCandy(Scp330Bag scp330Bag, CandyKindID candyKindId)
        {
            return scp330Bag.Candies.FindIndex(x => candyKindId == x);
        }
    }
}