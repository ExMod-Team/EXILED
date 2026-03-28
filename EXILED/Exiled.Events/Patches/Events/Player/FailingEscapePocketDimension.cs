// -----------------------------------------------------------------------
// <copyright file="FailingEscapePocketDimension.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PocketDimensionTeleport.Kill(PocketDimensionTeleport, ReferenceHub)" />.
    /// Adds the <see cref="Handlers.Player.FailingEscapePocketDimension" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.FailingEscapePocketDimension))]
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.Kill))]
    internal static class FailingEscapePocketDimension
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // pocketDimensionTeleport
                    new CodeInstruction(OpCodes.Ldarg_0),

                    // referenceHub
                    new CodeInstruction(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // FailingEscapePocketDimensionEventArgs ev = new(PocketDimensionTeleport, ReferenceHub, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FailingEscapePocketDimensionEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnFailingEscapePocketDimension(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnFailingEscapePocketDimension))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FailingEscapePocketDimensionEventArgs), nameof(FailingEscapePocketDimensionEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}