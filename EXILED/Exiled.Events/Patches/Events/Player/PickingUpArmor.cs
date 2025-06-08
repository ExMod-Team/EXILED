// -----------------------------------------------------------------------
// <copyright file="PickingUpArmor.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Searching;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="ArmorSearchCompletor.Complete" /> method to add the
    /// <see cref="Handlers.Player.PickingUpItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.PickingUpItem))]
    [HarmonyPatch(typeof(ArmorSearchCompletor), nameof(ArmorSearchCompletor.Complete))]
    internal static class PickingUpArmor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = -5;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerPickingUpArmorEventArgs))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.Hub
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ArmorSearchCompletor), nameof(ArmorSearchCompletor.Hub))),

                    // this.TargetPickup
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(ArmorSearchCompletor), nameof(ArmorSearchCompletor.TargetPickup))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // PickingUpArmorEventArgs ev = new(ReferenceHub, ItemPickupBase, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PickingUpItemEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnPickingUpItem(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPickingUpItem))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpItemEventArgs), nameof(PickingUpItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}