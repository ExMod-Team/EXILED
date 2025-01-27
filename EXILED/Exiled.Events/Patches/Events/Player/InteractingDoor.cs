// -----------------------------------------------------------------------
// <copyright file="InteractingDoor.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DoorVariant.ServerInteract(ReferenceHub, byte)" />.
    /// Adds the <see cref="Handlers.Player.InteractingDoor" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.InteractingDoor))]
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal static class InteractingDoor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingDoorEventArgs));

            CodeInstruction[] interactingEvent = new CodeInstruction[]
            {
                // Player.Get(ply)
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new(OpCodes.Ldarg_0),

                // colliderId
                new(OpCodes.Ldarg_2),

                // IsAllowed
                new(OpCodes.Ldc_I4_1),

                // CanInteract
                new(OpCodes.Ldloc_1),

                // InteractingDoorEventArgs ev = new(Player.Get(ply), __instance, colliderId, false, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingDoorEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                // Handlers.Player.OnInteractingDoor(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingDoor))),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // if (!ev.CanInteract) return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.CanInteract))),
                new(OpCodes.Brfalse_S, retLabel),

                // CanInteract = ev.IsAllowed (Reminder this is done on purpose because we prefer than IDeniableEvent when cancel use CanInteract and not Interacting)
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))),
                new(OpCodes.Stloc_1),
            };

            int offset = 4;
            int index = newInstructions.FindLastIndex(x => x.Calls(Method(typeof(DoorLockUtils), nameof(DoorLockUtils.HasFlagFast), new System.Type[] { typeof(DoorLockMode), typeof(DoorLockMode) }))) + offset;
            newInstructions.InsertRange(index, interactingEvent);

            offset = 2;
            index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloc_0) + offset;
            newInstructions.InsertRange(index, interactingEvent);

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}