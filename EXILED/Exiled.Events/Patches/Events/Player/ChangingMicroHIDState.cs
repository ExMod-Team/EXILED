// -----------------------------------------------------------------------
// <copyright file="ChangingMicroHIDState.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features.Items;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.MicroHID;
    using InventorySystem.Items.MicroHID.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CycleController.Phase"/>.
    /// Adds the <see cref="Handlers.Player.ChangingMicroHIDState"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingMicroHIDState))]
    [HarmonyPatch(typeof(CycleController), nameof(CycleController.Phase), MethodType.Setter)]
    internal static class ChangingMicroHIDState
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label pickupLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            LocalBuilder item = generator.DeclareLocal(typeof(MicroHIDItem));

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // MicroHIDItem item = Item.Get(this._prevItem);
                // if (item is null)
                //     pickup code;
                // else
                //     itemCode;
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldfld, Field(typeof(CycleController), nameof(CycleController._prevItem))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, item.LocalIndex),
                new(OpCodes.Brfalse_S, pickupLabel),

                // bool allowed = CallItemEvent(microHidItem, phase)
                new(OpCodes.Ldloc_S, item.LocalIndex),
                new(OpCodes.Ldarga_S, 1),
                new(OpCodes.Call, Method(typeof(ChangingMicroHIDState), nameof(ChangingMicroHIDState.CallItemEvent))),

                new CodeInstruction(OpCodes.Br_S, continueLabel),

                // bool allowed = CallPickupEvent(Serial, phase)
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(pickupLabel),
                new(OpCodes.Ldfld, Field(typeof(CycleController), nameof(CycleController.Serial))),
                new(OpCodes.Ldarga_S, 1),
                new(OpCodes.Call, Method(typeof(ChangingMicroHIDState), nameof(ChangingMicroHIDState.CallPickupEvent))),

                // if (!allowed)
                //    return;
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel).WithLabels(continueLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool CallItemEvent(MicroHIDItem microHIDItem, ref MicroHidPhase phase)
        {
            ChangingMicroHIDStateEventArgs ev = new(microHIDItem, phase);
            Handlers.Player.OnChangingMicroHIDState(ev);
            phase = ev.NewPhase;
            return ev.IsAllowed;
        }

        private static bool CallPickupEvent(ushort serial, ref MicroHidPhase phase)
        {
            if (!MicroHIDPickup.PickupsBySerial.TryGetValue(serial, out MicroHIDPickup pickup))
                return true;

            ChangingMicroHIDPickupStateEventArgs ev = new(pickup, phase);
            Handlers.Item.OnChangingMicroHIDPickupState(ev);
            phase = ev.NewPhase;
            return ev.IsAllowed;
        }
    }
}
