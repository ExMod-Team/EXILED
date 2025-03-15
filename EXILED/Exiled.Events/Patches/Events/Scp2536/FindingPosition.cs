// -----------------------------------------------------------------------
// <copyright file="FindingPosition.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp2536
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Christmas.Scp2536;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp244;
    using Exiled.Events.EventArgs.Scp2536;
    using HarmonyLib;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp2536Controller.CanFindPosition"/>
    /// to add <see cref="Handlers.Scp2536.FindingPosition"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.FindingPosition))]
    [HarmonyPatch(typeof(Scp2536Controller), nameof(Scp2536Controller.CanFindPosition))]
    internal class FindingPosition
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(FindingPositionEventArgs));

            int offset = 0;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_S && x.operand is LocalBuilder { LocalIndex: 5 }) + offset;

            Label continueLabel = (Label)newInstructions[index - 1].operand;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(target);
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // scp2536Spawnpoint
                    new(OpCodes.Ldloc_S, 4),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // FindingPositionEventArgs ev = new(Player, Scp2536Spawnpoint, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FindingPositionEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp2536.OnFindingPosition(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.OnFindingPosition))),

                    // if (!ev.IsAllowed) continue;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FindingPositionEventArgs), nameof(FindingPositionEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // scp2536Spawnpoint = ev.Spawnpoint;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FindingPositionEventArgs), nameof(FindingPositionEventArgs.Spawnpoint))),
                    new(OpCodes.Stloc_S, 4),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}