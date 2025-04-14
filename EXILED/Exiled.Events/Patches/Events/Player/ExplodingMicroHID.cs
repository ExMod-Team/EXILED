// -----------------------------------------------------------------------
// <copyright file="ExplodingMicroHID.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.MicroHID.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ChargeFireModeModule.ServerExplode"/>.
    /// Adds the <see cref="Handlers.Player.ExplodingMicroHID" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ExplodingMicroHID))]
    [HarmonyPatch(typeof(ChargeFireModeModule), nameof(ChargeFireModeModule.ServerExplode))]
    internal static class ExplodingMicroHID
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -2;
            int index = newInstructions.FindIndex(x => x.StoresField(Field(typeof(ChargeFireModeModule), nameof(ChargeFireModeModule._alreadyExploded)))) + offset;

            Label retLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this.MicroHid;
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(ChargeFireModeModule), nameof(ChargeFireModeModule.MicroHid))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ExplodingMicroHIDEventArgs ev = new(MicroHIDItem, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExplodingMicroHIDEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnExplodingMicroHID(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnExplodingMicroHID))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingMicroHIDEventArgs), nameof(ExplodingMicroHIDEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}