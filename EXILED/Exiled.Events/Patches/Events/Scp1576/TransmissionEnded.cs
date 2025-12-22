// -----------------------------------------------------------------------
// <copyright file="TransmissionEnded.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp1576
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp1576;
    using HarmonyLib;
    using InventorySystem.Items.Usables.Scp1576;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp1576Item.ServerStopTransmitting"/> to add <see cref="Handlers.Scp1576.TransmissionEnded"/> event
    /// </summary>
    [EventPatch(typeof(Handlers.Scp1576), nameof(Handlers.Scp1576.TransmissionEnded))]
    [HarmonyPatch(typeof(InventorySystem.Items.Usables.Scp1576.Scp1576Item), nameof(Scp1576Item.ServerStopTransmitting))]
    public class TransmissionEnded
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
                List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

                int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ret);

                newInstructions.InsertRange(
                    index,
                    [

                        // Player.Get(base.Owner)
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Scp1576Item), nameof(Scp1576Item.Owner))),
                        new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(Player.Get), [typeof(ReferenceHub)])),

                        // this
                        new CodeInstruction(OpCodes.Ldarg_0),

                        // ServerStopTransmittingEventArgs ev = new(Player, this)
                        new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(TransmissionEndedEventArgs))[0]),

                        // OnTransmisionEnded(ev)
                        new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp1576), nameof(Handlers.Scp1576.OnTransmisionEnded)))
                    ]);

                foreach (CodeInstruction instruction in newInstructions)
                    yield return instruction;

                ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}