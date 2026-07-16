// -----------------------------------------------------------------------
// <copyright file="VisionInformationFix.cs" company="ExMod Team">
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

    using PlayerRoles.PlayableScps;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="VisionInformation.CheckIsInDarkness"/>.
    /// Fix for effects that blind the player are not perceived as blindness.
    /// Bug reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/3125).
    /// </summary>
    [HarmonyPatch(typeof(VisionInformation), nameof(VisionInformation.CheckIsInDarkness))]
    internal class VisionInformationFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder effectsController = generator.DeclareLocal(typeof(PlayerEffectsController));

            Label retTrue = generator.DefineLabel();
            Label @continue = generator.DefineLabel();

            newInstructions[0].labels.Add(@continue);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // effectsController = hub.playerEffectsController;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerEffectsController))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, effectsController),

                // if (effectsController.GetEffect<Flashed>().IsEnabled)
                //     return true;
                new(OpCodes.Callvirt, Method(typeof(PlayerEffectsController), nameof(PlayerEffectsController.GetEffect)).MakeGenericMethod(typeof(Flashed))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Flashed), nameof(Flashed.IsEnabled))),
                new(OpCodes.Brtrue_S, retTrue),

                // if (effectsController.GetEffect<SeveredEyes>().IsEnabled)
                //     return true;
                new(OpCodes.Ldloc_S, effectsController),
                new(OpCodes.Callvirt, Method(typeof(PlayerEffectsController), nameof(PlayerEffectsController.GetEffect)).MakeGenericMethod(typeof(SeveredEyes))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SeveredEyes), nameof(SeveredEyes.IsEnabled))),
                new(OpCodes.Brtrue_S, retTrue),

                // if (effectsController.GetEffect<Blindness>().Intensity >= Blindness.MaxHealableIntensity)
                //     return true;
                new(OpCodes.Ldloc_S, effectsController),
                new(OpCodes.Callvirt, Method(typeof(PlayerEffectsController), nameof(PlayerEffectsController.GetEffect)).MakeGenericMethod(typeof(Blindness))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Blindness), nameof(Blindness.Intensity))),
                new(OpCodes.Ldc_I4, (int)Blindness.MaxHealableIntensity),
                new(OpCodes.Bge_S, retTrue),

                new(OpCodes.Br_S, @continue),
                new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(retTrue),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}