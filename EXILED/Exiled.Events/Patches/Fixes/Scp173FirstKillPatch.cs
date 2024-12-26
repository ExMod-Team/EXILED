// -----------------------------------------------------------------------
// <copyright file="Scp173FirstKillPatch.cs" company="ExMod Team">
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
    using PlayerRoles.PlayableScps.Scp173;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp173SnapAbility.TryHitTarget(Transform, out ReferenceHub)" /> to fix https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/143 bug.
    /// </summary>
    [HarmonyPatch(typeof(Scp173SnapAbility), nameof(Scp173SnapAbility.TryHitTarget))]
    internal static class Scp173FirstKillPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldc_I4_0);
            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // if (hitboxIdentity.TargetHub.playerEffectController.GetEffect<SpawnProtected>().IsEnabled) return false;
                new(OpCodes.Ldloc_2),
                new(OpCodes.Callvirt, PropertyGetter(typeof(HitboxIdentity), nameof(HitboxIdentity.TargetHub))),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerEffectsController))),
                new(OpCodes.Callvirt, Method(typeof(PlayerEffectsController), nameof(PlayerEffectsController.GetEffect), generics: new[] { typeof(SpawnProtected) })),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StatusEffectBase), nameof(StatusEffectBase.IsEnabled))),
                new(OpCodes.Brfalse_S, continueLabel),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
