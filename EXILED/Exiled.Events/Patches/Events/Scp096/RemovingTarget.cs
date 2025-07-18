// -----------------------------------------------------------------------
// <copyright file="RemovingTarget.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp096;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp096;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp096TargetsTracker.RemoveTarget(ReferenceHub)" /> and <see cref="Scp096TargetsTracker.ClearAllTargets()" />.
    /// Adds the <see cref="Scp096.RemovingTarget" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp096), nameof(Handlers.Scp096.RemovingTarget))]
    [HarmonyPatch(typeof(Scp096TargetsTracker))]
    internal static class RemovingTarget
    {
        [HarmonyPatch(nameof(Scp096TargetsTracker.RemoveTarget))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retFalseLabel = generator.DefineLabel();
            Label runLabel = generator.DefineLabel();

            // make game check contains instead of removing then forcibly running our event (so no spam event calls and is still deniable)
            newInstructions.Find(instruction => instruction.Calls(Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Remove)))).operand = Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Contains));

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) - 1;

            newInstructions[index].WithLabels(retFalseLabel);

            index -= 1;

            // integrate our condition into first if statement
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // integrate our condition into first if statement
                    new(OpCodes.Brfalse, retFalseLabel),

                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<Scp096Role>), nameof(StandardSubroutine<Scp096Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(target)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // RemovingTargetEventArgs ev = new(scp096, target, isAllowed)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovingTargetEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp096.OnRemovingTarget(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnRemovingTarget))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingTargetEventArgs), nameof(RemovingTargetEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue, runLabel),
                });

            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + 1;

            // if allowed, remove target
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]).WithLabels(runLabel),
                new(OpCodes.Ldfld, Field(typeof(Scp096TargetsTracker), nameof(Scp096TargetsTracker.Targets))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Remove))),
                new(OpCodes.Pop),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        [HarmonyPatch(nameof(Scp096TargetsTracker.ClearAllTargets))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            object continueLabel = newInstructions.Find(instruction => instruction.opcode == OpCodes.Br_S).operand;

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_1) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(base.Owner)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<Scp096Role>), nameof(StandardSubroutine<Scp096Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(target)
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // RemovingTargetEventArgs ev = new(scp096, target, isAllowed)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovingTargetEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp096.OnRemovingTarget(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnRemovingTarget))),

                    // if (!ev.IsAllowed)
                    //   continue;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingTargetEventArgs), nameof(RemovingTargetEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}