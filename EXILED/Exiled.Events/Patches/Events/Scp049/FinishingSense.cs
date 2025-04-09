// -----------------------------------------------------------------------
// <copyright file="FinishingSense.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{ 
    using API.Features;
    using API.Features.Pools;

    using HarmonyLib;
    using System.Reflection.Emit;
    using System.Collections.Generic;
    using static HarmonyLib.AccessTools;

    using PlayerRoles.Subroutines;
    using Exiled.Events.EventArgs.Scp049;
    using PlayerRoles.PlayableScps.Scp049;

    [EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.FinishingSense))]
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerLoseTarget))]
    internal class FinishingSense
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Declare local variable for FinishingSenseEventArgs
            LocalBuilder ev = generator.DeclareLocal(typeof(FinishingSenseEventArgs));

            // Contuinue label for isAllowed check
            Label continueLabel = generator.DefineLabel();

            // ReducedCooldown value double
            const double DefaultReducedCooldowntime = Scp049SenseAbility.ReducedCooldown;

            newInstructions.InsertRange(0, new CodeInstruction[]
            {

                // Player scp049 = Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player target = Player.Get(this.Target);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // double CooldownTime = 20;
                new(OpCodes.Ldc_R8, DefaultReducedCooldowntime),

                // true (IsAllowed)
                new(OpCodes.Ldc_I4_1),

                // FinishingSenseEventArgs ev = new FinishingSenseEventArgs(scp049, target ,cooldowntime, isallowed);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FinishingSenseEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp049.OnFinishingSense(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnFinishingSense))),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                // Return;
                new(OpCodes.Ret),

                // continue label
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            // this.Cooldown.Trigger(20.0) index
            int cooldownIndex = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldc_R8 && (double)i.operand == DefaultReducedCooldowntime);

            // Replace "this.Cooldown.Trigger(20.0)" with "this.Cooldown.Trigger((double)ev.cooldowntime)"
            newInstructions[cooldownIndex] = new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex);
            newInstructions.Insert(cooldownIndex + 1, new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.CooldownTime))));

            // Return the new instructions
            foreach (var newInstruction in newInstructions)
                yield return newInstruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    [EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.FinishingSense))]
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerProcessKilledPlayer))]
    internal class FinishingSense2
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Declare local variable for FinishingSenseEventArgs
            LocalBuilder ev2 = generator.DeclareLocal(typeof(FinishingSenseEventArgs));

            // Contuinue label for isAllowed check
            Label continueLabel = generator.DefineLabel();

            // BaseCoolDown value double
            const double defaultCooldowntime = Scp049SenseAbility.BaseCooldown;

            // this.Cooldown.Trigger(40.0) index
            int offset = -2;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_R8 && (double)i.operand == defaultCooldowntime) + offset;
            
            // Fail safe: if index cant be found, exit the patch
            if (index < 0)
            {
                Log.Error("FinishingSenseEvent2 error: Scp049SenseAbility.Cooldown not found, patch failed.");
                foreach (var instruction in instructions)
                    yield return instruction;

                yield break;
            }

            newInstructions.InsertRange(index, new CodeInstruction[]
            {

                // Player scp049 = Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player target = Player.Get(this.Target);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // double CooldownTime = 40;
                new CodeInstruction(OpCodes.Ldc_R8, defaultCooldowntime),

                // true (IsAllowed)
                new(OpCodes.Ldc_I4_1),

                // FinishingSenseEventArgs ev = new FinishingSenseEventArgs(scp049, target, cooldowntime, isAllowed);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FinishingSenseEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev2.LocalIndex),

                // Handlers.Scp049.OnFinishingSense(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnFinishingSense))),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Ldloc_S, ev2.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                // return;
                new(OpCodes.Ret),

                // continue label
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            // this.Cooldown.Trigger(40.0) index
            int cooldownIndex = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldc_R8 && (double)i.operand == defaultCooldowntime);

            // Replace "this.Cooldown.Trigger(40.0)" with "this.Cooldown.Trigger((double)ev.cooldowntime)"
            newInstructions[cooldownIndex] = new CodeInstruction(OpCodes.Ldloc, ev2.LocalIndex);
            newInstructions.Insert(cooldownIndex + 1, new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.CooldownTime))));
            
            // Return the new instructions
            foreach (var newInstruction in newInstructions)
                yield return newInstruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    [EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.FinishingSense))]
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerProcessCmd), new[] { typeof(Mirror.NetworkReader) })]
    internal class FinishingSense3
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Declare local variable for FinishingSenseEventArgs
            LocalBuilder ev3 = generator.DeclareLocal(typeof(FinishingSenseEventArgs));

            // Declare local variable for If Ability is active
            LocalBuilder IsAbilityActive = generator.DeclareLocal(typeof(bool));

            // Contuinue label for if ability is not active
            Label continueLabel = generator.DefineLabel();

            // Ret label for Exiting the code without breaking ActivatingSense patch
            Label Allowed = generator.DefineLabel();

            // BaseCoolDown value double
            const double DefaultFailCooldowntime = Scp049SenseAbility.AttemptFailCooldown;

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // To determine whether the ability is active, i.e. whether this is an unsuccessful attempt or a sense that is not allowed to end
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.HasTarget))),
                new(OpCodes.Stloc, IsAbilityActive.LocalIndex),
            });

            // this.Cooldown.Trigger(2.5) index
            MethodInfo triggerMethod = Method(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger));
            int offset = -3;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && i.operand is MethodInfo method && method == triggerMethod) + offset;

            // Fail safe: if index cant be found, exit the patch
            if (index < 0)
            {
                Log.Error("FinishingSenseEvent3 error: Scp049SenseAbility.AttemptFailCooldown not found, patch failed.");
                foreach (var instruction in instructions)
                    yield return instruction;

                yield break;
            }

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Skip if the ability is not active and this is an unsuccessful attempt
                new(OpCodes.Ldloc, IsAbilityActive.LocalIndex),
                new(OpCodes.Brfalse_S, continueLabel),

                // Player scp049 = Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player target = Player.Get(this.Target);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // double CooldownTime = 2.5;
                new CodeInstruction(OpCodes.Ldc_R8, DefaultFailCooldowntime),

                // true (IsAllowed)
                new(OpCodes.Ldc_I4_1),

                // FinishingSenseEventArgs ev = new FinishingSenseEventArgs(scp049, target, cooldowntime, isAllowed);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FinishingSenseEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev3.LocalIndex),

                // Handlers.Scp049.OnFinishingSense(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnFinishingSense))),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Ldloc_S, ev3.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, Allowed),

                // If not allowed, set hastarget to true so as not to break the sense ability
                // this.HasTarget = true;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Callvirt, PropertySetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.HasTarget))),

                // return;
                new(OpCodes.Pop),
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(Allowed),

                // this.Cooldown.Trigger(ev.cooldown.time)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, typeof(Scp049SenseAbility).GetField("Cooldown", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)),
                new CodeInstruction(OpCodes.Ldloc, ev3.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.CooldownTime))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger), new[] { typeof(double) })),


                // this.ServerSendRpc(true)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_1), // true
                new CodeInstruction(OpCodes.Call, Method(typeof(SubroutineBase), nameof(SubroutineBase.ServerSendRpc), new[] { typeof(bool) })),

                // return;
                new(OpCodes.Pop),
                new(OpCodes.Ret),


                // Continue  if the ability is not active and this is an unsuccessful attempt
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            
            });

            // Return the new instructions
            foreach (var newInstruction in newInstructions)
                yield return newInstruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
