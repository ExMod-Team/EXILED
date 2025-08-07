// -----------------------------------------------------------------------
// <copyright file="TogglingNoClip.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using Exiled.API.Features.Roles;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.NetworkMessages;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// patches <see cref="FpcNoclip.IsActive" /> to add the
    /// <see cref="Handlers.Player.TogglingNoClip" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.TogglingNoClip))]
    [HarmonyPatch(typeof(FpcNoclipToggleMessage), nameof(FpcNoclipToggleMessage.ProcessMessage))]
    internal static class TogglingNoClip
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(TogglingNoClipEventArgs));

            int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // steal all TogglingNoClipEventArgs.ctor argument(LabApi.Events.Arguments.PlayerEvents.PlayerTogglingNoclipEventArgs)
                    // ReferenceHub & bool
                    // true
                    new(OpCodes.Ldc_I4_1),

                    // TogglingNoClipEventArgs ev = new(ReferenceHub, bool, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingNoClipEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnTogglingNoClip(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTogglingNoClip))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingNoClipEventArgs), nameof(TogglingNoClipEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // referenceHub
                    new(OpCodes.Ldloc_0),

                    // ev.IsEnabled
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingNoClipEventArgs), nameof(TogglingNoClipEventArgs.IsEnabled))),

                    // Give Back to LabAPI Event
                });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}