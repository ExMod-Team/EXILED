// -----------------------------------------------------------------------
// <copyright file="IdlingTesla.cs" company="ExMod Team">
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
    /// Adds the <see cref="Handlers.Player.IdlingTesla" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.IdlingTesla))]
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class IdlingTesla
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(IdlingTeslaEventArgs));

            int offset = 4;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(TeslaGate), nameof(TeslaGate.IsInIdleRange), new[] { typeof(ReferenceHub) }))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(allHub);
                new(OpCodes.Ldloc_S, 7),
                new(OpCodes.Call, Method(typeof(Exiled.API.Features.Player), nameof(Exiled.API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // TeslaGate.Get(allGate);
                new(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(Exiled.API.Features.TeslaGate), nameof(Exiled.API.Features.TeslaGate.Get), new[] { typeof(TeslaGate) })),

                // IdlingTeslaEventArgs ev = new(Player, TeslaGate);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(IdlingTeslaEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnIdlingTeslaTesla(ev);
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnIdlingTesla))),
            });

            int labApiIsAllowedIndex = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(
                typeof(LabApi.Events.Arguments.PlayerEvents.PlayerIdlingTeslaEventArgs),
                nameof(LabApi.Events.Arguments.PlayerEvents.PlayerIdlingTeslaEventArgs.IsAllowed)))) + 1;

            newInstructions.InsertRange(labApiIsAllowedIndex, new CodeInstruction[]
            {
                // if (e.IsAllowed && ev.IsAllowed)
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(IdlingTeslaEventArgs), nameof(IdlingTeslaEventArgs.IsAllowed))),
                new(OpCodes.And),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}