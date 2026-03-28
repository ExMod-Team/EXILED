// -----------------------------------------------------------------------
// <copyright file="AnnouncingChaosEntrance.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using Respawning.Announcements;
    using Subtitles;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ChaosWaveAnnouncement.CreateAnnouncement"/> and <see cref="ChaosMiniwaveAnnouncement.CreateAnnouncement"/>
    /// to add <see cref="Handlers.Map.AnnouncingChaosEntrance"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.AnnouncingChaosEntrance))]
    [HarmonyPatch]
    internal static class AnnouncingChaosEntrance
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return Method(typeof(ChaosWaveAnnouncement), nameof(ChaosWaveAnnouncement.CreateAnnouncement));
            yield return Method(typeof(ChaosMiniwaveAnnouncement), nameof(ChaosMiniwaveAnnouncement.CreateAnnouncement));
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instruction, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instruction);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(AnnouncingChaosEntranceEventArgs));

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Pop) + 1;

            newInstructions.InsertRange(index, new[]
            {
                // WaveAnnouncementBase
                new(OpCodes.Ldarg_0),

                // builder
                new(OpCodes.Ldarg_1),

                // new AnnouncingChaosEntranceEventArgs(WaveAnnouncementBase, StringBuilder)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingChaosEntranceEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Map.OnAnnouncingChaosEntrance(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnAnnouncingChaosEntrance))),

                // if (!ev.IsAllowed)
                //     builder.Clear();
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingChaosEntranceEventArgs), nameof(AnnouncingChaosEntranceEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, Method(typeof(StringBuilder), nameof(StringBuilder.Clear))),
                new(OpCodes.Pop),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Call, Method(typeof(Array), nameof(Array.Empty), new Type[] { }, new[] { typeof(SubtitlePart) })),
                new(OpCodes.Stind_Ref),
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}