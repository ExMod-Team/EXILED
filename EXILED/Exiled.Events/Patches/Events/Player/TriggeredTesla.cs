// -----------------------------------------------------------------------
// <copyright file="TriggeredTesla.cs" company="ExMod Team">
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
    using Handlers;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="TeslaGateController.FixedUpdate" />.
    /// Adds the <see cref="Handlers.Player.TriggeredTesla" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.TriggeredTesla))]
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TriggeredTesla
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(TeslaGate), nameof(TeslaGate.ServerSideCode)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
                {
                    // Player.Get(hub2);
                    new(OpCodes.Ldloc_S, 5),
                    new(OpCodes.Call, Method(typeof(Exiled.API.Features.Player), nameof(Exiled.API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // TeslaGate.Get(allGate);
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Exiled.API.Features.TeslaGate), nameof(Exiled.API.Features.TeslaGate.Get), new[] { typeof(TeslaGate) })),

                    // TriggeredTeslaEventArgs ev = new(Player, TeslaGate);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TriggeredTeslaEventArgs))[0]),

                    // Handlers.Player.OnTriggeredTesla(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnTriggeredTesla))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}