// -----------------------------------------------------------------------
// <copyright file="Scp1576TransmissionEnded.cs" company="ExMod Team">
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
    using InventorySystem.Items.Usables.Scp1576;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp1576Item.ServerStopTransmitting"/> to add <see cref="Handlers.Player.Scp1576TransmissionEnded"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Scp1576TransmissionEnded))]
    [HarmonyPatch(typeof(InventorySystem.Items.Usables.Scp1576.Scp1576Item), nameof(Scp1576Item.ServerStopTransmitting))]
    public class Scp1576TransmissionEnded
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
                List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

                int index = newInstructions.Count - 1;

                newInstructions.InsertRange(
                    index,
                    new[]
                    {
                        // Player.Get(base.Owner)
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Scp1576Item), nameof(Scp1576Item.Owner))),
                        new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                        // this
                        new CodeInstruction(OpCodes.Ldarg_0),

                        // Scp1576TransmissionEndedEventArgs ev = new(Player, this)
                        new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(Scp1576TransmissionEndedEventArgs))[0]),

                        // OnScp1576TransmissionEnded(ev)
                        new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnScp1576TransmissionEnded))),
                    });

                for (int i = 0; i < newInstructions.Count; i++)
                    yield return newInstructions[i];

                ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}