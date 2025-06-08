// -----------------------------------------------------------------------
// <copyright file="UsingRadioBattery.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Radio;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RadioItem.Update" />.
    /// Adds the <see cref="Handlers.Player.UsingRadioBattery" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsingRadioBattery))]
    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.Update))]
    internal static class UsingRadioBattery
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(UsingRadioBatteryEventArgs));

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_1) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this
                    new(OpCodes.Ldarg_0),

                    // num
                    new(OpCodes.Ldloc_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // UsingRadioBatteryEventArgs ev = new(RadioItem, ReferenceHub, float, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingRadioBatteryEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnUsingRadioBattery(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingRadioBattery))),

                    // num = ev.Drain;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingRadioBatteryEventArgs), nameof(UsingRadioBatteryEventArgs.Drain))),
                    new(OpCodes.Stloc_1),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingRadioBatteryEventArgs), nameof(UsingRadioBatteryEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}