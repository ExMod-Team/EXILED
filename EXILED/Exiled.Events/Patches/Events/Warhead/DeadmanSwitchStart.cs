// -----------------------------------------------------------------------
// <copyright file="DeadmanSwitchStart.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Warhead;
    using Handlers;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AlphaWarheadController.Detonate" />
    /// to add  <see cref="Warhead.DeadmanSwitchInitiating"/> events.
    /// </summary>
    [EventPatch(typeof(Warhead), nameof(Warhead.DeadmanSwitchInitiating))]
    [HarmonyPatch(typeof(DeadmanSwitch), nameof(DeadmanSwitch.InitiateProtocol))]
    internal static class DeadmanSwitchStart
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(DetonatingEventArgs));

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // DeadmanSwitchInitiatingEventArgs ev = new();
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DeadmanSwitchInitiatingEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Warhead.OnDeadmanSwitchInitiating(ev);
                new(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnDeadmanSwitchInitiating))),

                // if (ev.IsAllowed)
                //    goto retLabel;
                new(OpCodes.Callvirt, PropertyGetter(typeof(DeadmanSwitchInitiatingEventArgs), nameof(DeadmanSwitchInitiatingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}