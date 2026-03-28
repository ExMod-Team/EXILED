// -----------------------------------------------------------------------
// <copyright file="UsingAndCancellingItemUse.cs" company="ExMod Team">
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
    using CustomPlayerEffects;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Usables;
    using LabApi.Events.Arguments.PlayerEvents;
    using Utils.Networking;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="UsableItemsController.ServerReceivedStatus" />.
    /// Adds the <see cref="Handlers.Player.UsingItem" /> event,
    /// <see cref="Handlers.Player.CancellingItemUse" /> event and
    /// <see cref="Handlers.Player.CancelledItemUse" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.CancellingItemUse))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsingItem))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.CancelledItemUse))]
    [HarmonyPatch(typeof(UsableItemsController), nameof(UsableItemsController.ServerReceivedStatus))]
    internal static class UsingAndCancellingItemUse
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder evUsingItemEventArgs = generator.DeclareLocal(typeof(UsingItemEventArgs));
            LocalBuilder evCancellingItemUseEventArgs = generator.DeclareLocal(typeof(CancellingItemUseEventArgs));

            int offset = 3;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(UsableItemModifierEffectExtensions), nameof(UsableItemModifierEffectExtensions.GetSpeedMultiplier)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // referenceHub
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),

                    // usableItem
                    new(OpCodes.Ldloc_1),

                    // cooldown
                    new(OpCodes.Ldloc_S, 4),

                    // UsingItemEventArgs ev = new(Player, UsableItem, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, evUsingItemEventArgs.LocalIndex),

                    // Handlers.Player.OnUsingItem(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingItem))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingItemEventArgs), nameof(UsingItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // cooldown = ev.Cooldown
                    new(OpCodes.Ldloc_S, evUsingItemEventArgs.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingItemEventArgs), nameof(UsingItemEventArgs.Cooldown))),
                    new(OpCodes.Stloc_S, 4),
                });

            offset = -2;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerCancellingUsingItemEventArgs))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // referenceHub
                    new CodeInstruction(OpCodes.Ldloc_0),

                    // usableItem
                    new(OpCodes.Ldloc_1),

                    // CancellingItemUseEventArgs ev = new(Player, UsableItem)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(CancellingItemUseEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, evCancellingItemUseEventArgs.LocalIndex),

                    // Handlers.Player.OnCancellingItemUse(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnCancellingItemUse))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CancellingItemUseEventArgs), nameof(CancellingItemUseEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            // right before the LabAPI event "new PlayerCancelledUsingItemEventArgs(...)"
            offset = -2;
            index = newInstructions.FindLastIndex(
                instruction => instruction.opcode == OpCodes.Newobj) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // referenceHub
                    new CodeInstruction(OpCodes.Ldloc_0),

                    // usableItem
                    new(OpCodes.Ldloc_1),

                    // CancelledItemUseEventArgs ev = new(ReferenceHub, UsableItem)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(CancelledItemUseEventArgs))[0]),

                    // Handlers.Player.OnCancelledItemUse(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnCancelledItemUse))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}