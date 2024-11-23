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

    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp330;

    using Mirror;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches the <see cref="Scp330NetworkHandler.ServerSelectMessageReceived(NetworkConnection, SelectScp330Message)" /> method to add the
    /// <see cref="Scp330.DroppingScp330" /> event.
    /// </summary>
    [EventPatch(typeof(Scp330), nameof(Scp330.DroppingScp330))]
    [HarmonyPatch(typeof(Scp330NetworkHandler), nameof(Scp330NetworkHandler.ServerSelectMessageReceived))]
    internal static class DroppingCandy
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(DroppingScp330EventArgs));

            const int offset = -1;
            int index = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(ReferenceHub), nameof(ReferenceHub.inventory)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // scp330Bag
                    new(OpCodes.Ldloc_1),

                    // Load candy bag array to get CandyKindID
                    new(OpCodes.Dup),
                    new(OpCodes.Ldfld, Field(typeof(Scp330Bag), nameof(Scp330Bag.Candies))),
                    new(OpCodes.Ldarg_1),

                    // Grab candy ID from network message, and pass it to event args.
                    new(OpCodes.Ldfld, Field(typeof(SelectScp330Message), nameof(SelectScp330Message.CandyID))),
                    new(OpCodes.Ldelem_U1),

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
                });

            // Set our location of previous owner
            int jumpOverOffset = 1;
            int jumpOverIndex = newInstructions.FindLastIndex(
                instruction => instruction.StoresField(Field(typeof(ItemPickupBase), nameof(ItemPickupBase.PreviousOwner)))) + jumpOverOffset;

            int candyKindIdIndex = 4;

            newInstructions.InsertRange(
                jumpOverIndex,
                new[]
                {
                    // candyKindID = ev.Candy, save locally.
                    new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingScp330EventArgs), nameof(DroppingScp330EventArgs.Candy))),
                    new(OpCodes.Stloc, candyKindIdIndex),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}