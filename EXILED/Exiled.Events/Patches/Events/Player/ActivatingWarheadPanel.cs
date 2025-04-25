// -----------------------------------------------------------------------
// <copyright file="ActivatingWarheadPanel.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="PlayerInteract.UserCode_CmdSwitchAWButton" />.
    /// Adds the <see cref="Handlers.Player.ActivatingWarheadPanel" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ActivatingWarheadPanel))]
    [HarmonyPatch(typeof(PlayerInteract), nameof(AlphaWarheadActivationPanel.ServerInteractKeycard))]
    internal static class ActivatingWarheadPanel
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            Label ev = generator.DefineLabel();
            Label cardCheck = generator.DefineLabel();

            int offset = 2;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(DoorPermissionsPolicy), nameof(DoorPermissionsPolicy.CheckPermissions)))) + offset;

            newInstructions.RemoveRange(index, 17);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(this._hub);
                    new(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // flag
                    new(OpCodes.Ldloc_0),

                    // ActivatingWarheadPanekEventArgs ev = new(player, flag);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingWarheadPanelEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnActivatingWarheadPanel(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnActivatingWarheadPanel))),

                    // if (!ev.IsAllowed)
                    //      return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingWarheadPanelEventArgs), nameof(ActivatingWarheadPanelEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ret),

                    // flag = true;
                    new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(continueLabel),
                    new(OpCodes.Stloc_1),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}