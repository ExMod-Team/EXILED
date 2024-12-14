// -----------------------------------------------------------------------
// <copyright file="SelectingRespawnTeam.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;

    using HarmonyLib;

    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="WaveManager"/> to add the <see cref="Handlers.Server.SelectingRespawnTeam"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.SelectingRespawnTeam))]
    [HarmonyPatch(typeof(WaveManager), nameof(WaveManager.InitiateRespawn))]
    internal static class SelectingRespawnTeam
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            LocalBuilder ev = generator.DeclareLocal(typeof(SelectingRespawnTeamEventArgs));

            Label returnLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_I4_1);

            newInstructions.InsertRange(index, new[]
            {
                // SelectingRespawnTeamEventArgs ev = new(dominatingTeam);
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SelectingRespawnTeamEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev),

                // Handlers.Server.OnSelectingRespawnTeam(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnSelectingRespawnTeam))),

                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(SelectingRespawnTeamEventArgs), nameof(SelectingRespawnTeamEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // SpawnableWaveBase = ev.Wave;
                new(OpCodes.Ldloc_S, ev),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SelectingRespawnTeamEventArgs), nameof(SelectingRespawnTeamEventArgs.Wave))),
                new(OpCodes.Starg_S, 0),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}