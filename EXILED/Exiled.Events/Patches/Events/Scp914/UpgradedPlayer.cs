// -----------------------------------------------------------------------
// <copyright file="UpgradedPlayer.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp914;
    using global::Scp914;
    using HarmonyLib;
    using Mono.Cecil.Cil;
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using OpCode = System.Reflection.Emit.OpCode;
    using Scp914 = Handlers.Scp914;

    /// <summary>
    /// Patches <see cref="Scp914Upgrader.ProcessPlayer" />
    /// to add the <see cref="Scp914.UpgradedInventoryItem" /> event.
    /// </summary>
    [EventPatch(typeof(Scp914), nameof(Scp914.UpgradedInventoryItem))]
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPlayer))]
    internal static class UpgradedPlayer
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Stloc_S && x.operand is LocalBuilder lb && lb.LocalIndex == 11) + 1;

            LocalBuilder curSetting = generator.DeclareLocal(typeof(Scp914KnobSetting));
            List<Label> label = newInstructions[index].ExtractLabels();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // setting = curSetting
                    new(OpCodes.Ldloc_S, curSetting.LocalIndex),
                    new(OpCodes.Starg_S, 3),

                    // Player.Get(ply)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // itemBase
                    new(OpCodes.Ldloc_S, 7),

                    // setting
                    new(OpCodes.Ldarg_S, 3),

                    // resultingItems
                    new(OpCodes.Ldloc_S, 11),
                    new(OpCodes.Ldfld, Field(typeof(Scp914Result), nameof(Scp914Result.ResultingItems))),

                    // UpgradedInventoryItemEventArgs ev = new(player, itemBase, setting, resultingItems)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradedInventoryItemEventArgs))[0]),

                    // Handlers.Scp914.OnUpgradedInventoryItem(ev);
                    new(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnUpgradedInventoryItem))),
                });

            newInstructions[index].WithLabels(label);

            foreach (CodeInstruction t in newInstructions)
                yield return t;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}