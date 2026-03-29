// -----------------------------------------------------------------------
// <copyright file="ShowingHitMarker.cs" company="ExMod Team">
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

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="Hitmarker.SendHitmarkerDirectly(ReferenceHub, float, bool, HitmarkerType)"/> method.
    /// Adds the <see cref="Handlers.Player.ShowingHitMarker"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ShowingHitMarker))]
    [HarmonyPatch(typeof(Hitmarker), nameof(Hitmarker.SendHitmarkerDirectly), typeof(ReferenceHub), typeof(float), typeof(bool), typeof(HitmarkerType))]
    internal static class ShowingHitMarker
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ShowingHitMarkerEventArgs));
            Label continueLabel = generator.DefineLabel();

            int offset = 1;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // ShowingHitMarkerEventArgs ev = new(hub, size, playAudio, hitmarkerType)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShowingHitMarkerEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnShowingHitMarker(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShowingHitMarker))),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ShowingHitMarkerEventArgs), nameof(ShowingHitMarkerEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                new(OpCodes.Ret),

                // size = ev.Size;
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ShowingHitMarkerEventArgs), nameof(ShowingHitMarkerEventArgs.Size))),
                new(OpCodes.Starg_S, 1),

                // playAudio = ev.ShouldPlayAudio;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ShowingHitMarkerEventArgs), nameof(ShowingHitMarkerEventArgs.ShouldPlayAudio))),
                new(OpCodes.Starg_S, 2),

                // hitmarkerType = ev.HitmarkerType;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ShowingHitMarkerEventArgs), nameof(ShowingHitMarkerEventArgs.HitmarkerType))),
                new(OpCodes.Starg_S, 3),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
