// -----------------------------------------------------------------------
// <copyright file="SendingCassieMessage.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Cassie
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Cassie;
    using global::Cassie;
    using Handlers;
    using HarmonyLib;
    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="global::Cassie.CassieAnnouncementDispatcher.PlayNewAnnouncement(CassieAnnouncement)" />.
    /// Adds the <see cref="Cassie.SendingCassieMessage" /> event.
    /// </summary>
    [EventPatch(typeof(Cassie), nameof(Cassie.SendingCassieMessage))]
    [HarmonyPatch(typeof(CassieAnnouncementDispatcher), nameof(CassieAnnouncementDispatcher.PlayNewAnnouncement))]
    internal static class SendingCassieMessage
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label skipLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            newInstructions[0].WithLabels(skipLabel);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Isinst, typeof(CassieScpTerminationAnnouncement)),
                    new(OpCodes.Brtrue_S, skipLabel),

                    new(OpCodes.Ldarg_0),

                    // isAllowed
                    new(OpCodes.Ldc_I4_1),

                    // SendingCassieMessageEventArgs ev = new SendingCassieMessageEventArgs(CassieAnnouncement, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingCassieMessageEventArgs)).Single(ctor => ctor.GetParameters().Length == 2)),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // Cassie.OnSendingCassieMessage(ev);
                    new(OpCodes.Call, Method(typeof(Cassie), nameof(Cassie.OnSendingCassieMessage))),

                    // annc = ev.Announcement
                    new(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.Announcement))),
                    new(OpCodes.Starg_S, 0),

                    // if (!IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}