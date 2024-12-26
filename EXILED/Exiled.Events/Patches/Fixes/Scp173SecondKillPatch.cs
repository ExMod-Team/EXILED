// -----------------------------------------------------------------------
// <copyright file="Scp173SecondKillPatch.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using CustomPlayerEffects;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp173;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp173TeleportAbility.ServerProcessCmd(NetworkReader)" /> to fix https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/143 bug.
    /// </summary>
    [HarmonyPatch(typeof(Scp173TeleportAbility), nameof(Scp173TeleportAbility.ServerProcessCmd))]
    internal static class Scp173SecondKillPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            int offset = -5;
            int index = newInstructions.FindIndex(x => x.Is(OpCodes.Callvirt, Method(typeof(MovementTracer), nameof(MovementTracer.GenerateBounds)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // if (hub.playerEffectController.GetEffect<SpawnProtected>().IsEnabled) return;
                new CodeInstruction(OpCodes.Ldloc_S, 5).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerEffectsController))),
                new(OpCodes.Callvirt, Method(typeof(PlayerEffectsController), nameof(PlayerEffectsController.GetEffect), generics: new[] { typeof(SpawnProtected) })),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StatusEffectBase), nameof(StatusEffectBase.IsEnabled))),
                new(OpCodes.Brtrue_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
