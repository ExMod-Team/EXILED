// -----------------------------------------------------------------------
// <copyright file="ElevatorSequencesUpdated.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;

    using Interactables.Interobjects;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ElevatorChamber.CurSequence" />'s setter.
    /// Adds the <see cref="Handlers.Map.ElevatorSequencesUpdated" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.ElevatorSequencesUpdated))]
    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.CurSequence), MethodType.Setter)]
    internal static class ElevatorSequencesUpdated
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // After the value is set, we find the last line to trigger it
            int lastindex = newInstructions.Count - 1;

            newInstructions.InsertRange(lastindex, new CodeInstruction[]
            {
                    // this.ElevatorChamber
                    new(OpCodes.Ldarg_0),

                    // this.ElevatorChamber.CurSequence
                    new(OpCodes.Ldarg_1),

                    // ElevatorSequencesUpdatedEventArgs ev = new(ElevatorChamber, ElevatorSequence)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ElevatorSequencesUpdatedEventArgs))[0]),

                    // Handlers.Map.OnElevatorSequencesUpdated(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnElevatorSequencesUpdated))),
            });

            foreach (CodeInstruction newcode in newInstructions)
                    yield return newcode;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
