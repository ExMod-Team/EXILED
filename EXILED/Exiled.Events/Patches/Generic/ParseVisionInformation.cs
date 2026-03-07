// -----------------------------------------------------------------------
// <copyright file="ParseVisionInformation.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using HarmonyLib;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp096;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;
    using Scp096Role = API.Features.Roles.Scp096Role;

    /// <summary>
    /// Patches <see cref="Scp096TargetsTracker.IsObservedBy(ReferenceHub)"/>.
    /// Adds the <see cref="Scp096Role.TurnedPlayers"/> support.
    /// </summary>
    [HarmonyPatch(typeof(Scp096TargetsTracker), nameof(Scp096TargetsTracker.IsObservedBy))]
    internal static class ParseVisionInformation
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            Label returnLabel = generator.DefineLabel();

            newInstructions[0].labels.Add(continueLabel);

            // if (!Scp096Role.TurnedPlayers.Contains(Player.Get(referenceHub)))
            //      return false;
            newInstructions.InsertRange(
                0,
                new[]
                {
                    // if (!Scp096Role.TurnedPlayers.Contains(Player.Get(referenceHub)))
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp096Role), nameof(Scp096Role.TurnedPlayers))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // return false;
                    new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(returnLabel),
                    new(OpCodes.Ret),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}