// -----------------------------------------------------------------------
// <copyright file="DamagingDoor.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using Handlers;

    using HarmonyLib;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="BreakableDoor.ServerDamage" />.
    /// Adds the <see cref="Player.DamagingDoor" /> event and fix https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/817 in the EventArgs.
    /// </summary>
    [HarmonyPatch(typeof(BreakableDoor), nameof(BreakableDoor.ServerDamage))]
    internal static class DamagingDoor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(DamagingDoorEventArgs));

            Label ret = generator.DefineLabel();
            int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            newInstructions[index].labels.Add(ret);

            offset = -3;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_1) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                    // damage
                    new(OpCodes.Ldarg_1),

                    // DoorDamageType
                    new(OpCodes.Ldarg_2),

                    // attacker
                    new(OpCodes.Ldarg_3),

                    // DamagingDoorEventArgs ev = new(doorVariant, this, doorDamageType, Footprint);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DamagingDoorEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnDamagingDoor(ev);
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnDamagingDoor))),

                    // if (!ev.IsAllowed)
                    //    return false;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingDoorEventArgs), nameof(DamagingDoorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, ret),

                    // damage = ev.Handler.Damage;
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingDoorEventArgs), nameof(DamagingDoorEventArgs.Damage))),
                    new(OpCodes.Starg_S, 1),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}