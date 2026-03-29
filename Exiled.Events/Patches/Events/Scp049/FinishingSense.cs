// -----------------------------------------------------------------------
// <copyright file="FinishingSense.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp049;
    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp049SenseAbility.ServerLoseTarget" />.
    /// Adds the <see cref="Handlers.Scp049.FinishingSense" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.FinishingSense))]
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerLoseTarget))]
    internal class FinishingSense
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(FinishingSenseEventArgs));
            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // this.Owner
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Owner))),

                // this.Target
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),

                // Scp049SenseAbility.TargetLostCooldown
                new(OpCodes.Ldc_R8, (double)Scp049SenseAbility.TargetLostCooldown),

                // true (IsAllowed)
                new(OpCodes.Ldc_I4_1),

                // FinishingSenseEventArgs ev = new FinishingSenseEventArgs(ReferenceHub, ReferenceHub, double, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FinishingSenseEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp049.OnFinishingSense(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnFinishingSense))),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            // this.Cooldown.Trigger((double)Scp049SenseAbility.TargetLostCooldown) index
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldc_R8 && (double)i.operand == Scp049SenseAbility.TargetLostCooldown);

            // Replace "this.Cooldown.Trigger((double)Scp049SenseAbility.ReducedCooldown)" with "this.Cooldown.Trigger((double)ev.cooldowntime)"
            newInstructions.RemoveAt(index);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.CooldownTime))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Scp049SenseAbility.ServerProcessKilledPlayer" />.
    /// Adds the <see cref="Handlers.Scp049.FinishingSense" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.FinishingSense))]
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerProcessKilledPlayer))]
    internal class FinishingSense2
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(FinishingSenseEventArgs));

            // Continue label for isAllowed check
            Label retLabel = generator.DefineLabel();

            // this.Cooldown.Trigger(Scp049SenseAbility.BaseCooldown) index
            int offset = -2;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_R8 && (double)i.operand == Scp049SenseAbility.BaseCooldown) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this.Owner
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Owner))),

                // this.Target
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),

                // double CooldownTime = Scp049SenseAbility.BaseCooldown;
                new(OpCodes.Ldc_R8, (double)Scp049SenseAbility.BaseCooldown),

                // true (IsAllowed)
                new(OpCodes.Ldc_I4_1),

                // FinishingSenseEventArgs ev = new FinishingSenseEventArgs(ReferenceHub, ReferenceHub, double, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FinishingSenseEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp049.OnFinishingSense(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnFinishingSense))),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            // this.Cooldown.Trigger(Scp049SenseAbility.BaseCooldown) index
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldc_R8 && (double)i.operand == Scp049SenseAbility.BaseCooldown);

            newInstructions.RemoveAt(index);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.CooldownTime))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Scp049SenseAbility.ServerProcessCmd" />.
    /// Adds the <see cref="Handlers.Scp049.FinishingSense" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.FinishingSense))]
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerProcessCmd), new[] { typeof(Mirror.NetworkReader) })]
    internal class FinishingSense3
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(FinishingSenseEventArgs));
            LocalBuilder isAbilityActive = generator.DeclareLocal(typeof(bool));

            Label skipactivatingsense = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();
            Label allowed = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // isAbilityActive = this.HasTarget;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.HasTarget))),
                new(OpCodes.Stloc, isAbilityActive.LocalIndex),
            });

            // this.Cooldown.Trigger(2.5) index
            int offset = -1;
            int index = newInstructions.FindIndex(i =>
                i.opcode == OpCodes.Ldfld &&
                i.operand == (object)Field(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Cooldown))) + offset;

            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Skip if the ability is not active and this is an unsuccessful attempt
                new(OpCodes.Ldloc, isAbilityActive.LocalIndex),
                new(OpCodes.Brfalse_S, continueLabel),

                // this.Owner;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Owner))),

                // this.Target;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),

                // Scp049SenseAbility.AttemptFailCooldown;
                new(OpCodes.Ldc_R8, (double)Scp049SenseAbility.AttemptFailCooldown),

                // true (IsAllowed)
                new(OpCodes.Ldc_I4_1),

                // FinishingSenseEventArgs ev = new FinishingSenseEventArgs(ReferenceHub, ReferenceHub, double, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FinishingSenseEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp049.OnFinishingSense(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnFinishingSense))),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, allowed),

                // If not allowed, set has target to true so as not to break the sense ability
                // this.HasTarget = true;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Callvirt, PropertySetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.HasTarget))),

                // return;
                new(OpCodes.Ret),

                // this.Cooldown.Trigger(ev.cooldown.time)
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(allowed),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.CooldownTime))),
                new(OpCodes.Callvirt, Method(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger), new[] { typeof(double) })),

                new(OpCodes.Br_S, skipactivatingsense),
            });

            offset = -2;
            index = newInstructions.FindIndex(i =>
                i.opcode == OpCodes.Call &&
                i.operand == (object)Method(typeof(SubroutineBase), nameof(SubroutineBase.ServerSendRpc), new[] { typeof(bool) })) + offset;

            newInstructions[index].labels.Add(skipactivatingsense);

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
