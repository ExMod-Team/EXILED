// -----------------------------------------------------------------------
// <copyright file="SendingCassieMessage.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Cassie
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Cassie;

    using Handlers;

    using HarmonyLib;
    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RespawnEffectsController.PlayCassieAnnouncement(string, bool, bool, bool, string)" />.
    /// Adds the <see cref="Cassie.SendingCassieMessage" /> event.
    /// </summary>
    [EventPatch(typeof(Cassie), nameof(Cassie.SendingCassieMessage))]
    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.PlayCassieAnnouncement))]
    internal static class SendingCassieMessage
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // words
                    new(OpCodes.Ldarg_0),

                    // makeHold
                    new(OpCodes.Ldarg_1),

                    // makeNoise
                    new(OpCodes.Ldarg_2),

                    // customAnnouncement
                    new(OpCodes.Ldarg_3),

                    // customSubtitles
                    new(OpCodes.Ldarg_S, 4),

                    // isAllowed
                    new(OpCodes.Ldc_I4_1),

                    // SendingCassieMessageEventArgs ev = new SendingCassieMessageEventArgs(string, bool, bool, bool, string, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingCassieMessageEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // Cassie.OnSendingCassieMessage(ev);
                    new(OpCodes.Call, Method(typeof(Cassie), nameof(Cassie.OnSendingCassieMessage))),

                    // words = ev.Words
                    new(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.Words))),
                    new(OpCodes.Starg_S, 0),

                    // makeHold = ev.MakeHold
                    new(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.MakeHold))),
                    new(OpCodes.Starg_S, 1),

                    // makeNoise = ev.MakeNoise
                    new(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.MakeNoise))),
                    new(OpCodes.Starg_S, 2),

                    // customAnnouncement = ev.IsCustomAnnouncement
                    new(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.IsCustomAnnouncement))),
                    new(OpCodes.Starg_S, 3),

                    // customSubtitles = ev.CustomSubtitles
                    new(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.CustomSubtitles))),
                    new(OpCodes.Starg_S, 4),

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