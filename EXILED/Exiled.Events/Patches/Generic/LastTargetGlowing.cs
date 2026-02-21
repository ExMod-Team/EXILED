// -----------------------------------------------------------------------
// <copyright file="LastTargetGlowing.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.HumanTracker;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="LastHumanTracker.Network_lastTargetPos"/> setter to implement <see cref="Config.CanLastHumanGlow"/>.
    /// </summary>
    [HarmonyPatch(typeof(LastHumanTracker), nameof(LastHumanTracker.Network_lastTargetPos), MethodType.Setter)]
    public class LastTargetGlowing
    {
        private static readonly Vector3? FakePosition = new(999f, 999f, 999f);

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            newInstructions[0].labels.Add(continueLabel);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // if (!ExiledEvents.Instance.Config.LastHumanGlowing)
                    //    goto continueLabel;
                    new(OpCodes.Call, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Plugin<Config>), nameof(Plugin<Config>.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanLastHumanGlow))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // value = FakeVector;
                    new(OpCodes.Ldsfld, AccessTools.Field(typeof(LastTargetGlowing), nameof(FakePosition))),
                    new(OpCodes.Starg_S, 1),
                });

            for (int z = 0; z < newInstructions.Count; ++z)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
