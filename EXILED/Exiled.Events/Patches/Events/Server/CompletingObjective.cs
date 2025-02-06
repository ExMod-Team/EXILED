// -----------------------------------------------------------------------
// <copyright file="CompletingObjective.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;
    using HarmonyLib;
    using Respawning.Objectives;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FactionObjectiveBase.ServerSendUpdate"/>
    /// to add <see cref="Handlers.Server.CompletingObjective"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.CompletingObjective))]
    [HarmonyPatch(typeof(FactionObjectiveBase), nameof(FactionObjectiveBase.ServerSendUpdate))]
    internal class CompletingObjective
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // this
                new(OpCodes.Ldarg_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // CompletingObjectiveEventArgs ev = new(this, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(CompletingObjectiveEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Server.OnCompletingObjective(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnCompletingObjective))),

                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(CompletingObjectiveEventArgs), nameof(CompletingObjectiveEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}