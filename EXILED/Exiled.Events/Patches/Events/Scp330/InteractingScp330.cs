// -----------------------------------------------------------------------
// <copyright file="InteractingScp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp330;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using Interactables.Interobjects;
    using InventorySystem.Items.Usables.Scp330;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches the <see cref="Scp330Interobject.ServerInteract(ReferenceHub, byte)" /> method to add the
    /// <see cref="Scp330.InteractingScp330" /> event.
    /// </summary>
    [EventPatch(typeof(Scp330), nameof(Scp330.InteractingScp330))]
    [HarmonyPatch(typeof(Scp330Interobject), nameof(Scp330Interobject.ServerInteract))]
    public static class InteractingScp330
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label shouldNotSever = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();
            Label enableEffectLabel = generator.DefineLabel();

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
                    new(OpCodes.Ldloc_2),

                    // InteractingScp330EventArgs ev = new(Player, int)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingScp330EventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Scp330.OnInteractingScp330(ev)
                    new(OpCodes.Call, Method(typeof(Scp330), nameof(Scp330.OnInteractingScp330))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            // This is to find the location of RpcMakeSound to remove the original code and add a new sever logic structure (Start point)
            int addShouldSeverOffset = -1;
            int addShouldSeverIndex = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(Scp330Interobject), nameof(Scp330Interobject.RpcMakeSound)))) + addShouldSeverOffset;

            int serverEffectLocationStart = -1;
            int enableEffect = newInstructions.FindLastIndex(
                instruction => instruction.LoadsField(Field(typeof(ReferenceHub), nameof(ReferenceHub.playerEffectsController)))) + serverEffectLocationStart;
            newInstructions[enableEffect].WithLabels(enableEffectLabel);
            newInstructions.InsertRange(
                addShouldSeverIndex,
                new[]
                {
                    // if (!ev.ShouldSever)
                    //    goto shouldNotSever;
                    new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.ShouldSever))),
                    new(OpCodes.Brfalse, shouldNotSever),
                    new(OpCodes.Br, enableEffectLabel),
                });

            // This will let us jump to the taken candies code and lock until ldarg_0, meaning we allow base game logic handle candy adding.
            int addTakenCandiesOffset = -1;
            int addTakenCandiesIndex = newInstructions.FindLastIndex(
                instruction => instruction.LoadsField(Field(typeof(Scp330Interobject), nameof(Scp330Interobject._takenCandies)))) + addTakenCandiesOffset;

            newInstructions[addTakenCandiesIndex].WithLabels(shouldNotSever);
            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}