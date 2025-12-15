// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTermination.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.Handlers;
    using Footprinting;
    using global::Cassie;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    using Map = Exiled.Events.Handlers.Map;
    using Player = API.Features.Player;

    /// <summary>
    /// Patches
    /// <see cref="CassieScpTerminationAnnouncement.OnStartedPlaying()" />.
    /// Adds the <see cref="Map.AnnouncingScpTermination" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.AnnouncingScpTermination))]
    [HarmonyPatch(typeof(CassieScpTerminationAnnouncement), nameof(CassieScpTerminationAnnouncement.OnStartedPlaying))]
    internal static class AnnouncingScpTermination
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder cause = generator.DeclareLocal(typeof(string));
            LocalBuilder enumerator = generator.DeclareLocal(typeof(IEnumerator<Footprint>));

            ExceptionBlock beginTry = new(ExceptionBlockType.BeginExceptionBlock);
            ExceptionBlock beginFinally = new(ExceptionBlockType.BeginFinallyBlock);
            ExceptionBlock endFinally = new(ExceptionBlockType.EndExceptionBlock);

            Label ret = generator.DefineLabel();
            Label entryLabel = generator.DefineLabel();
            Label loopLabel = generator.DefineLabel();
            Label leaveLabel = generator.DefineLabel();
            Label endFinallyLabel = generator.DefineLabel();

            int offset = -1;
            int index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(CassieScpTerminationAnnouncement), nameof(CassieScpTerminationAnnouncement._announcementTts)))) + offset;

            newInstructions.RemoveRange(index, 2);
            newInstructions.Insert(index, new CodeInstruction(OpCodes.Ldloc_S, cause));

            newInstructions[0].WithLabels(leaveLabel);

            newInstructions.InsertRange(
                0,
                new[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CassieScpTerminationAnnouncement), nameof(CassieScpTerminationAnnouncement._announcementTts))),
                new(OpCodes.Stloc_S, cause),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(CassieScpTerminationAnnouncement), nameof(CassieScpTerminationAnnouncement.Victims))),
                new(OpCodes.Callvirt, Method(typeof(IEnumerable<Footprint>), nameof(IEnumerable<Footprint>.GetEnumerator))),
                new(OpCodes.Stloc_S, enumerator),

                // start of try
                new CodeInstruction(OpCodes.Br_S, entryLabel).WithBlocks(beginTry),

                // start of loop
                new CodeInstruction(OpCodes.Ldloc_S, enumerator).WithLabels(loopLabel),
                new(OpCodes.Callvirt, PropertyGetter(typeof(IEnumerator<Footprint>), nameof(IEnumerator<Footprint>.Current))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(Footprint) })),
                new(OpCodes.Ldloc_S, cause),
                new(OpCodes.Newobj, Constructor(typeof(AnnouncingScpTerminationEventArgs), new[] { typeof(Player), typeof(string) })),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingScpTermination))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.TerminationCause))),
                new(OpCodes.Stloc_S, cause),

                // entry point
                new CodeInstruction(OpCodes.Ldloc_S, enumerator).WithLabels(entryLabel),
                new(OpCodes.Callvirt, Method(typeof(IEnumerator), nameof(IEnumerator.MoveNext))),
                new(OpCodes.Brtrue_S, loopLabel),

                // end of loop
                new(OpCodes.Leave, leaveLabel),

                // begin finally
                new CodeInstruction(OpCodes.Ldloc_S, enumerator).WithBlocks(beginFinally),
                new(OpCodes.Brfalse, endFinallyLabel),
                new(OpCodes.Ldloc_S, enumerator),
                new(OpCodes.Callvirt, Method(typeof(IDisposable), nameof(IDisposable.Dispose))),

                // end of finally
                new CodeInstruction(OpCodes.Endfinally).WithLabels(endFinallyLabel).WithBlocks(endFinally),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}