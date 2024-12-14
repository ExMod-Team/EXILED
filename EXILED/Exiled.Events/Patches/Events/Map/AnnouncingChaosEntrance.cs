// -----------------------------------------------------------------------
// <copyright file="AnnouncingChaosEntrance.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using Respawning.Announcements;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ChaosWaveAnnouncement.CreateAnnouncementString"/> and <see cref="ChaosMiniwaveAnnouncement.CreateAnnouncementString"/>
    /// to add <see cref="Handlers.Map.AnnouncingChaosEntrance"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.AnnouncingChaosEntrance))]
    [HarmonyPatch(typeof(ChaosWaveAnnouncement), nameof(ChaosWaveAnnouncement.CreateAnnouncementString))]
    [HarmonyPatch(typeof(ChaosMiniwaveAnnouncement), nameof(ChaosMiniwaveAnnouncement.CreateAnnouncementString))]
    internal static class AnnouncingChaosEntrance
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instruction, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instruction);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),

                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingChaosEntranceEventArgs))[0]),
                new(OpCodes.Dup),

                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnAnnouncingChaosEntrance))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingChaosEntranceEventArgs), nameof(AnnouncingChaosEntranceEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}