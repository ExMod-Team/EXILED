// -----------------------------------------------------------------------
// <copyright file="InteractingScp330.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
    using InventorySystem.Items;

#pragma warning disable SA1402
#pragma warning disable SA1313

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp330;
    using HarmonyLib;
    using Interactables.Interobjects;
    using InventorySystem;
    using InventorySystem.Items.Usables.Scp330;
    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches the <see cref="Scp330Interobject.ServerInteract(ReferenceHub, byte)" /> method to add the
    /// <see cref="Handlers.Scp330.InteractingScp330" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp330), nameof(Handlers.Scp330.InteractingScp330))]
    [HarmonyPatch(typeof(Scp330Interobject), nameof(Scp330Interobject.ServerInteract))]
    public static class InteractingScp330
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingScp330EventArgs));

            // Remove original "No scp can touch" logic.
            newInstructions.RemoveRange(0, 3);

            // Find ServerProcessPickup, insert before it.
            int offset = -3;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(ply)
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // num2
                    new(OpCodes.Ldloc_1),

                    // InteractingScp330EventArgs ev = new(Player, int)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingScp330EventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Scp330.OnInteractingScp330(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnInteractingScp330))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // ldarg.1 to player
                    new(OpCodes.Ldarg_1),

                    // ev.Candy
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.Candy))),

                    // bag
                    new(OpCodes.Ldloca_S, 2),

                    // ServerProcessPickup(ReferenceHub, CandyKindID, Scp330Bag)
                    new CodeInstruction(OpCodes.Call, Method(typeof(InteractingScp330), nameof(ServerProcessPickup), new[] { typeof(ReferenceHub), typeof(CandyKindID), typeof(Scp330Bag).MakeByRefType() })),
                });

            int remove_offset = -3;
            int remove_index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup)))) + remove_offset;

            // Remove original add candy logic
            newInstructions.RemoveRange(remove_index, 4);

            /* next code will used to override sound rpc check by EXILED
             * old:
             *   if (args.PlaySound)
             * new:
             *   if (args.PlaySound | ev.PlaySound)
             */
            offset = 1;
            index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(PropertyGetter(typeof(PlayerInteractScp330Event), nameof(PlayerInteractScp330Event.PlaySound)))) + offset;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // load ev.ShouldPlaySound and or operation with nw property.
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.ShouldPlaySound))),
                    new(OpCodes.Or),
                });
            /* next code will used to override Sever check by EXILED
             * old:
             *   if (args.AllowPunishment && uses >= 2)
             * new:
             *   if (args.AllowPunishment && ev.ShouldSever)
             */

            // set `notSeverLabel`
            offset = -1;
            index = newInstructions.FindLastIndex(
                instruction => instruction.LoadsField(Field(typeof(Scp330Interobject), nameof(Scp330Interobject._previousUses)))) + offset;

            Label notSeverLabel = newInstructions[index].labels[0];

            offset = 2;
            index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(PropertyGetter(typeof(PlayerInteractScp330Event), nameof(PlayerInteractScp330Event.AllowPunishment)))) + offset;

            // remove `uses >= 2` check, to override that by ev.ShouldSever
            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (!ev.ShouldSever)
                    //    goto shouldNotSever;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.ShouldSever))),
                    new(OpCodes.Brfalse_S, notSeverLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool ServerProcessPickup(ReferenceHub player, CandyKindID candy, out Scp330Bag bag)
        {
            if (!Scp330Bag.TryGetBag(player, out bag))
            {
                player.inventory.ServerAddItem(ItemType.SCP330, ItemAddReason.AdminCommand);

                if (!Scp330Bag.TryGetBag(player, out bag))
                    return false;

                bag.Candies = new List<CandyKindID> { candy };
                bag.ServerRefreshBag();

                return true;
            }

            bool result = bag.TryAddSpecific(candy);

            if (bag.AcquisitionAlreadyReceived)
                bag.ServerRefreshBag();

            return result;
        }
    }
}