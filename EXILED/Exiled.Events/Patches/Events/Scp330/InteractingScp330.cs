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

            int offset = 2;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(Scp330Candies), nameof(Scp330Candies.GetRandom)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // ply
                    new CodeInstruction(OpCodes.Ldarg_1),

                    // usage
                    new(OpCodes.Ldloc_1),

                    // shouldPlaySound
                    new(OpCodes.Ldloc_2),

                    // shouldSever
                    new(OpCodes.Ldloc_3),

                    // candyKindID
                    new(OpCodes.Ldloc_S, 4),

                    // InteractingScp330EventArgs ev = new(ReferenceHub, int, bool, bool, CandyKindID)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingScp330EventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Scp330.OnInteractingScp330(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnInteractingScp330))),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // shouldPlaySound = ev.ShouldPlaySound
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.ShouldPlaySound))),
                    new(OpCodes.Stloc, 2),

                    // shouldSever = ev.ShouldSever
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.ShouldSever))),
                    new(OpCodes.Stloc, 3),

                    // candyKindID = ev.Candy
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.Candy))),
                    new(OpCodes.Stloc_S, 4),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}