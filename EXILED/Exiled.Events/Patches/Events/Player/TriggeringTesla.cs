// -----------------------------------------------------------------------
// <copyright file="TriggeringTesla.cs" company="ExMod Team">
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
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="TeslaGateController.FixedUpdate" />.
    /// Adds the <see cref="Handlers.Player.TriggeringTesla" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.TriggeringTesla))]
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TriggeringTesla
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(TriggeringTeslaEventArgs));

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(PlayerEvents), nameof(PlayerEvents.OnIdlingTesla)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // TeslaGate.Get(allGate);
                new(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(API.Features.TeslaGate), nameof(API.Features.TeslaGate.Get), new[] { typeof(TeslaGate) })),

                // Player.Get(allHub);
                new(OpCodes.Ldloc_S, 7),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // TriggeringTeslaEventArgs ev = new(Player, TeslaGate);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TriggeringTeslaEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnTriggeringTesla(ev);
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnTriggeringTesla))),
            });

            int index1 = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(PlayerIdlingTeslaEventArgs), nameof(PlayerIdlingTeslaEventArgs.IsAllowed)))) + offset;

            newInstructions.InsertRange(index1, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.IsAllowed))),
                new(OpCodes.And),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.CanIdle))),
                new(OpCodes.And),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.DisableTesla))),
                new(OpCodes.Not),

                // if (e.IsAllowed && ev.IsAllowed && ev.CanIdle && !ev.DisableTesla)
                new(OpCodes.And),
            });

            int index2 = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(PlayerTriggeringTeslaEventArgs), nameof(PlayerTriggeringTeslaEventArgs.IsAllowed)))) + offset;

            newInstructions.InsertRange(index2, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.IsAllowed))),
                new(OpCodes.And),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.IsTriggerable))),
                new(OpCodes.And),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.DisableTesla))),
                new(OpCodes.Not),

                // if (e2.IsAllowed && ev.IsAllowed && ev.IsTriggerable && !ev.DisableTesla)
                new(OpCodes.And),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}