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
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            // the instruction that sends subtitles is called before stringReturn is created (and thus checked) so we need to move it so that empty (or disallowed) message's subtitles are not sent.
            // this removes the Ldarg_0 and the CallVirt
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(WaveAnnouncementBase), nameof(WaveAnnouncementBase.SendSubtitles))));
            CodeInstruction sendSubtitlesInstruction = newInstructions[index];
            newInstructions.RemoveRange(index - 2, 3);

            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldsfld);

            newInstructions.InsertRange(index, new[]
            {
                // if (stringReturn == "")
                //     return;
                new(OpCodes.Ldloc_S, 4),
                new(OpCodes.Ldstr, string.Empty),
                new(OpCodes.Ceq),
                new(OpCodes.Brtrue_S, returnLabel),

                // send subtitles before cassie message, but after our check.
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                sendSubtitlesInstruction,
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}