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

    using Scp49FinishingSenseEvent;

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

            newInstructions.InsertRange(0,
            [

                // Player scp049 = Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), [typeof(ReferenceHub)])),

                // Player target = Player.Get(this.Target);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), [typeof(ReferenceHub)])),

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
            ]);

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
            
            // if index cant be found, exit the patch
            if (index < 0)
            {
                Log.Error("FinishingSenseEvent2 error: Scp049SenseAbility.Cooldown not found, patch failed.");
                foreach (var instruction in instructions)
                    yield return instruction;

                yield break;
            }

            newInstructions.InsertRange(index,
            [

                // Player scp049 = Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), [typeof(ReferenceHub)])),

                // Player target = Player.Get(this.Target);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), [typeof(ReferenceHub)])),

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
                new(OpCodes.Call, PropertyGetter(typeof(FinishingSenseEventArgs), nameof(FinishingSenseEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                // return;
                new(OpCodes.Ret),

                // continue label
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            ]);

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
}
