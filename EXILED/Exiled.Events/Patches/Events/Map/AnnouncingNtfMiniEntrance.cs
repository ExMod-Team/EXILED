// -----------------------------------------------------------------------
// <copyright file="AnnouncingNtfMiniEntrance.cs" company="ExMod Team">
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
    using Handlers;
    using HarmonyLib;
    using Respawning.Announcements;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="NtfMiniwaveAnnouncement.CreateAnnouncementString"/>
    /// Adds the <see cref="Map.AnnouncingNtfEntrance" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.AnnouncingNtfEntrance))]
    [HarmonyPatch(typeof(NtfMiniwaveAnnouncement), nameof(NtfMiniwaveAnnouncement.CreateAnnouncementString))]
    internal static class AnnouncingNtfMiniEntrance
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(AnnouncingNtfEntranceEventArgs));

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // WaveAnnouncementBase
                    new(OpCodes.Ldarg_0),

                    // scpsLeft
                    new(OpCodes.Ldloc_0),

                    // null
                    new(OpCodes.Ldnull),

                    // 0
                    new(OpCodes.Ldc_I4_0),

                    // AnnouncingNtfEntranceEventArgs ev = new(this, scpsLeft, null, 0);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingNtfEntranceEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingNtfEntrance))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingNtfEntranceEventArgs), nameof(AnnouncingNtfEntranceEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),

                    // scpsLeft = ev.ScpsLeft;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingNtfEntranceEventArgs), nameof(AnnouncingNtfEntranceEventArgs.ScpsLeft))),
                    new(OpCodes.Stloc_0),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}