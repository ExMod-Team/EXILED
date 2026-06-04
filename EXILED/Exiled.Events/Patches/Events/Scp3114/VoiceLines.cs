// -----------------------------------------------------------------------
// <copyright file="VoiceLines.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp3114;

    using static HarmonyLib.AccessTools;
    using static PlayerRoles.PlayableScps.Scp3114.Scp3114VoiceLines;

    /// <summary>
    ///     Patches <see cref="Scp3114VoiceLines.ServerPlayConditionally" />.
    ///     Adds the <see cref="Handlers.Scp3114.VoiceLines" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.VoiceLines))]
    [HarmonyPatch(typeof(Scp3114VoiceLines), nameof(Scp3114VoiceLines.ServerPlayConditionally))]
    internal class VoiceLines
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(VoiceLinesEventArgs));

            int offset = 3;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(VoiceLinesDefinition), nameof(VoiceLinesDefinition.TryDrawNext)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                // voiceLinesDefinition
                new(OpCodes.Ldloc_0),

                // clipId
                new(OpCodes.Ldloc_2),

                // true
                new(OpCodes.Ldc_I4_1),

                // VoiceLinesEventArgs ev = new VoiceLinesEventArgs(Scp3114VoiceLines, VoiceLinesDefinition, byte, bool);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(VoiceLinesEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp3114.OnVoiceLines(ev);
                new(OpCodes.Call, Method(typeof(Scp3114), nameof(Scp3114.OnVoiceLines))),

                // if(!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(VoiceLinesEventArgs), nameof(VoiceLinesEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // voiceLinesDefinition = ev.VoiceLine;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(VoiceLinesEventArgs), nameof(VoiceLinesEventArgs.VoiceLine))),
                new(OpCodes.Stloc_S, 0),

                // clipId = ev.ClipId;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(VoiceLinesEventArgs), nameof(VoiceLinesEventArgs.ClipId))),
                new(OpCodes.Stloc_S, 2),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}