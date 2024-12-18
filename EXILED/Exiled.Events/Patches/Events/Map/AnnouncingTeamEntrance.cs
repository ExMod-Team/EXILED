// -----------------------------------------------------------------------
// <copyright file="AnnouncingTeamEntrance.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using Respawning.Announcements;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="WaveAnnouncementBase.PlayAnnouncement"/> to prevent cassie from playing empty string.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.AnnouncingNtfEntrance))]
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.AnnouncingChaosEntrance))]
    [HarmonyPatch(typeof(WaveAnnouncementBase), nameof(WaveAnnouncementBase.PlayAnnouncement))]
    internal static class AnnouncingTeamEntrance
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instruction, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instruction);

            Label returnLabel = generator.DefineLabel();

            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldsfld);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // if (stringReturn == "")
                //     return;
                new(OpCodes.Ldloc_S, 4),
                new(OpCodes.Ldstr, string.Empty),
                new(OpCodes.Ceq),
                new(OpCodes.Brtrue_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}