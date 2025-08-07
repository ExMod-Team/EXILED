// -----------------------------------------------------------------------
// <copyright file="ReservedSlotPatch.cs" company="ExMod Team">
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

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ReservedSlot.HasReservedSlot(string)" />.
    /// Adds the <see cref="Player.ReservedSlot" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.ReservedSlot))]
    [HarmonyPatch(typeof(ReservedSlot), nameof(ReservedSlot.HasReservedSlot))]
    internal static class ReservedSlotPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label callExiledEvent = generator.DefineLabel();

            for (int i = 0; i < newInstructions.Count; i++)
            {
                if (newInstructions[i].opcode == OpCodes.Ret)
                {
                    newInstructions[i] = new(OpCodes.Br, callExiledEvent);
                }
            }

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                // flag is already loaded
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(callExiledEvent),

                // ReservedSlotCheckEventArgs ev = new(flag, userid);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReservedSlotsCheckEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Player.OnReservedSlot(ev);
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnReservedSlot))),

                // return ev.IsAllowed
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReservedSlotsCheckEventArgs), nameof(ReservedSlotsCheckEventArgs.IsAllowed))),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}