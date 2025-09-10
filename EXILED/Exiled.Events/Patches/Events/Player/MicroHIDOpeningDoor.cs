// -----------------------------------------------------------------------
// <copyright file="MicroHIDOpeningDoor.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using InventorySystem.Items.MicroHID.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventorySystem.Items.MicroHID.Modules.ChargeFireModeModule.HandlePotentialDoor"/>.
    /// Adds the <see cref="MicroHIDOpeningDoor" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.MicroHIDOpeningDoor))]
    [HarmonyPatch(typeof(ChargeFireModeModule), nameof(ChargeFireModeModule.HandlePotentialDoor))]
    internal static class MicroHIDOpeningDoor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 0;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Brtrue_S) + offset;

            LocalBuilder result = generator.DeclareLocal(typeof(bool));

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // bool result = onStack
                    new(OpCodes.Stloc_S, result.LocalIndex),

                    // this.MicroHid
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ChargeFireModeModule), nameof(ChargeFireModeModule.MicroHid))),

                    // breakableDoor
                    new(OpCodes.Ldloc_0),

                    // result
                    new(OpCodes.Ldloc_S, result.LocalIndex),

                    // MicroHIDOpeningDoorEventArgs ev = new(MicroHIDItem, DoorVariant, ValueOnStack);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(MicroHIDOpeningDoorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnMicroHIDOpeningDoor(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnMicroHIDOpeningDoor))),

                    // !ev.IsAllowed (leave on stack)
                    new(OpCodes.Callvirt, PropertyGetter(typeof(MicroHIDOpeningDoorEventArgs), nameof(MicroHIDOpeningDoorEventArgs.IsAllowed))),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
