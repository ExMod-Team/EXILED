// -----------------------------------------------------------------------
// <copyright file="TriggeringTesla.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;

    using static HarmonyLib.AccessTools;

    using BaseTeslaGate = TeslaGate;

    /// <summary>
    /// Patches <see cref="TeslaGateController.FixedUpdate" />.
    /// Adds the <see cref="Handlers.Player.TriggeringTesla" /> event.
    /// </summary>
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TriggeringTesla
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            const int offset = 1;

            // remove the reference hub Foreach
            int index = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.AllHubs))));

            newInstructions.RemoveRange(index, newInstructions.FindIndex(i => i.opcode == OpCodes.Endfinally) + offset - index);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // baseTeslaGate
                    new CodeInstruction(OpCodes.Ldloc_1),

                    // inIdleRange
                    new CodeInstruction(OpCodes.Ldloca_S, 2),

                    // isTriggerable
                    new CodeInstruction(OpCodes.Ldloca_S, 3),

                    // referenceHub
                    new CodeInstruction(OpCodes.Ldloca_S, 4),

                    // referenceHub2
                    new CodeInstruction(OpCodes.Ldloca_S, 5),

                    // TriggeringTesla.TriggeringTeslaEvent(BaseTeslaGate baseTeslaGate, ref bool inIdleRange, ref bool isTriggerable)
                    new CodeInstruction(OpCodes.Call, Method(typeof(TriggeringTesla), nameof(TriggeringTeslaEvent), new[] { typeof(BaseTeslaGate), typeof(bool).MakeByRefType(), typeof(bool).MakeByRefType(), typeof(ReferenceHub).MakeByRefType(), typeof(ReferenceHub).MakeByRefType(), })),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void TriggeringTeslaEvent(BaseTeslaGate baseTeslaGate, ref bool inIdleRange, ref bool isTriggerable, ref ReferenceHub referenceHub, ref ReferenceHub referenceHub2)
        {
            TeslaGate teslaGate = TeslaGate.Get(baseTeslaGate);

            foreach (Player player in ReferenceHub.AllHubs.Select(Player.Get))
            {
                if (player is null || !teslaGate.CanBeIdle(player))
                    continue;

                TriggeringTeslaEventArgs ev = new(player, teslaGate);

                Handlers.Player.OnTriggeringTesla(ev);

                if (ev.DisableTesla)
                {
                    isTriggerable = false;
                    inIdleRange = false;
                    break;
                }

                if (!ev.IsAllowed)
                    continue;

                if (ev.IsTriggerable && !isTriggerable && !teslaGate.IsShocking)
                {
                    isTriggerable = ev.IsTriggerable;
                    PlayerTriggeringTeslaEventArgs playerTriggeringTeslaEventArgs = new(player.ReferenceHub, teslaGate.Base);
                    PlayerEvents.OnTriggeringTesla(playerTriggeringTeslaEventArgs);
                    if (!playerTriggeringTeslaEventArgs.IsAllowed)
                    {
                        isTriggerable = false;
                    }
                    else
                    {
                        referenceHub2 = player.ReferenceHub;
                    }
                }

                if (ev.IsInIdleRange && !inIdleRange)
                {
                    inIdleRange = ev.IsInIdleRange;
                    PlayerIdlingTeslaEventArgs playerIdlingTeslaEventArgs = new(player.ReferenceHub, teslaGate.Base);
                    PlayerEvents.OnIdlingTesla(playerIdlingTeslaEventArgs);
                    if (!playerIdlingTeslaEventArgs.IsAllowed)
                    {
                        inIdleRange = false;
                    }
                    else
                    {
                        referenceHub = player.ReferenceHub;
                    }
                }
            }
        }
    }
}
