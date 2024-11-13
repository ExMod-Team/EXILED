// -----------------------------------------------------------------------
// <copyright file="UpdateScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp244;

    using Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp244;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244DeployablePickup.UpdateRange" /> to add missing event handler to the
    /// <see cref="Scp244.OpeningScp244" />.
    /// </summary>
    [EventPatch(typeof(Scp244), nameof(Scp244.OpeningScp244))]
    [HarmonyPatch(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.UpdateRange))]
    internal static class UpdateScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            var continueLabel = generator.DefineLabel();

            var offset = 2;
            var index = newInstructions.FindIndex(
                instruction => instruction.LoadsField(Field(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup._activationDot)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this
                    new(OpCodes.Ldarg_0),

                    // OpeningScp244EventArgs ev = new(Scp244DeployablePickup)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(OpeningScp244EventArgs))[0]),
                    new(OpCodes.Dup),

                    // Scp244.OnOpeningScp244(ev)
                    new(OpCodes.Call, Method(typeof(Scp244), nameof(Scp244.OnOpeningScp244))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(OpeningScp244EventArgs), nameof(OpeningScp244EventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, continueLabel),
                });

            offset = -2;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Stopwatch), nameof(Stopwatch.Restart)))) + offset;

            newInstructions[index].WithLabels(continueLabel);

            for (var z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}