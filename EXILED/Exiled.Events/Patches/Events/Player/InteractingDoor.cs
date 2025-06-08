// -----------------------------------------------------------------------
// <copyright file="InteractingDoor.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Attributes;
    using EventArgs.Player;
    using HarmonyLib;
    using Interactables.Interobjects.DoorUtils;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DoorVariant.ServerInteract(ReferenceHub, byte)" />.
    /// Adds the <see cref="Handlers.Player.InteractingDoor" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Player.InteractingDoor))]
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal static class InteractingDoor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder labEvent = generator.DeclareLocal(typeof(PlayerInteractedDoorEventArgs));

            Label exiledEvContinue = generator.DefineLabel();
            Label labEvContinue = generator.DefineLabel();

            int offset = -3;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(DoorVariant), nameof(DoorVariant.AllowInteracting)))) + offset;

            Label elseStatement = newInstructions[index].labels[0];

            offset = -1;
            index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(DoorVariant), nameof(DoorVariant._remainingDeniedCooldown)))) + offset;

            Label bypassLock = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            newInstructions[index].labels.Add(bypassLock);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // pluginRequestSent = true (which prevent the second event call)
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Stloc_2),

                    // Player.Get(ply)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // doorVariant
                    new(OpCodes.Ldarg_0),

                    // colliderId
                    new(OpCodes.Ldarg_2),

                    // false (canOpen is false because the door is locked, but still calling the event)
                    new(OpCodes.Ldc_I4_0),

                    // ev = new InteractingDoorEventArgs(Player, DoorVariant, byte)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingDoorEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // Player.OnInteractingDoor(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingDoor))),

                    // if (ev.CanInteract) goto continueLabel
                    // else return
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.CanInteract))),
                    new(OpCodes.Brtrue_S, exiledEvContinue),
                    new(OpCodes.Pop),
                    new(OpCodes.Ret),

                    // canOpen = ev.IsAllowed
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))).WithLabels(exiledEvContinue),
                    new(OpCodes.Stloc_0),

                    // hub
                    new(OpCodes.Ldarg_1),

                    // doorVariant
                    new(OpCodes.Ldarg_0),

                    // canOpen
                    new(OpCodes.Ldloc_0),

                    // labEvent = new PlayerInteractingDoorEventArgs(ReferenceHub, DoorVariant, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlayerInteractingDoorEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, labEvent.LocalIndex),

                    // PlayerEvents.OnInteractingDoor(labEvent)
                    new(OpCodes.Call, Method(typeof(PlayerEvents), nameof(PlayerEvents.OnInteractingDoor))),

                    // if (!labEvent.IsAllowed) goto bypassLock
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerInteractingDoorEventArgs), nameof(PlayerInteractingDoorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, bypassLock),

                    // canOpen = ev.CanOpen
                    new(OpCodes.Ldloc_S, labEvent.LocalIndex),
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlayerInteractingDoorEventArgs), nameof(PlayerInteractingDoorEventArgs.CanOpen))).WithLabels(labEvContinue),
                    new(OpCodes.Stloc_0),
                    new(OpCodes.Br_S, elseStatement),
                });

            offset = -3;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerInteractingDoorEventArgs))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(ply)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // doorVariant
                    new(OpCodes.Ldarg_0),

                    // colliderId
                    new(OpCodes.Ldarg_2),

                    // canOpen
                    new(OpCodes.Ldloc_0),

                    // ev = new InteractingDoorEventArgs(Player, DoorVariant, byte)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingDoorEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // Player.OnInteractingDoor(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingDoor))),

                    // if (ev.CanInteract) goto continueLabel
                    // else return
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.CanInteract))),
                    new(OpCodes.Brtrue_S, continueLabel),
                    new(OpCodes.Pop),
                    new(OpCodes.Ret),

                    // canOpen = ev.IsAllowed
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))).WithLabels(continueLabel),
                    new(OpCodes.Stloc_0),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="DoorVariant.TryResolveLock" />.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Player.InteractingDoor))]
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.TryResolveLock))]
#pragma warning disable SA1402
    internal static class ChangeNWLogic
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -3;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerInteractingDoorEventArgs))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldc_I4_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ret),
                });

            // placing index after new added instructions
            index += 2;
            newInstructions.RemoveRange(index, newInstructions.Count - index);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}