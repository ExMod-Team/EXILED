// -----------------------------------------------------------------------
// <copyright file="EscapingPocketDimension.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp106;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="PocketDimensionTeleport.Exit(PocketDimensionTeleport, ReferenceHub)"/> method.
    /// Adds the <see cref="Handlers.Player.EscapingPocketDimension"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.EscapingPocketDimension))]
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.Exit))]
    internal static class EscapingPocketDimension
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(EscapingPocketDimensionEventArgs));

            Label ret = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // pocketDimensionTeleport
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                    // referenceHub
                    new CodeInstruction(OpCodes.Ldarg_1),

                    // Scp106PocketExitFinder.GetBestExitPosition(fpcRole)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Scp106PocketExitFinder), nameof(Scp106PocketExitFinder.GetBestExitPosition), new[] { typeof(IFpcRole) })),

                    // EscapingPocketDimensionEventArgs ev = new(PocketDimensionTeleport, Player, Vector3)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapingPocketDimensionEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnEscapingPocketDimension(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscapingPocketDimension))),

                    // if (ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingPocketDimensionEventArgs), nameof(EscapingPocketDimensionEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });

            offset = -2;
            index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.ServerOverridePosition)))) + offset;

            // Replaces "fpcRole.FpcModule.ServerOverridePosition(Scp106PocketExitFinder.GetBestExitPosition(fpcRole))"
            // with "fpcRole.FpcModule.ServerOverridePosition(ev.TeleportPosition)"
            newInstructions.RemoveRange(index, 2);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingPocketDimensionEventArgs), nameof(EscapingPocketDimensionEventArgs.TeleportPosition))),
            });
            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}