// -----------------------------------------------------------------------
// <copyright file="Emotion.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="EmotionSync.ServerSetEmotionPreset"/>.
    /// Adds the <see cref="Handlers.Player.ChangingEmotion" /> event and
    /// <see cref="Handlers.Player.ChangedEmotion" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingEmotion))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangedEmotion))]
    [HarmonyPatch(typeof(EmotionSync), nameof(EmotionSync.ServerSetEmotionPreset))]
    internal static class Emotion
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingEmotionEventArgs));

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldsfld);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // ChangingEmotionEventArgs ev = new(hub, preset, EmotionSync.GetEmotionPreset(hub), true);
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(EmotionSync), nameof(EmotionSync.GetEmotionPreset))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingEmotionEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev),

                // Handlers.Player.OnChangingEmotion(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingEmotion))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingEmotionEventArgs), nameof(ChangingEmotionEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, ret),

                // preset = ev.EmotionPresetTypeNew
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingEmotionEventArgs), nameof(ChangingEmotionEventArgs.NewEmotionPresetType))),
                new(OpCodes.Starg_S, 1),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // ChangedEmotionEventArgs ev = new(hub, EmotionSync.GetEmotionPreset(hub));
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(EmotionSync), nameof(EmotionSync.GetEmotionPreset))),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangedEmotionEventArgs))[0]),

                // Handlers.Player.OnChangedEmotion(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangedEmotion))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}