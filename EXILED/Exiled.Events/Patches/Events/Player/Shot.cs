// -----------------------------------------------------------------------
// <copyright file="Shot.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1402
#pragma warning disable SA1649
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Firearms.Modules.Misc;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="HitscanHitregModuleBase.ServerApplyDestructibleDamage" />.
    /// Adds the <see cref="Handlers.Player.Shot" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Shot))]
    [HarmonyPatch(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.ServerApplyDestructibleDamage))]
    internal static class ShotTarget
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldloc_2);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ShotEventArgs));

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this
                    new(OpCodes.Ldarg_0),

                    // target.Raycast.Hit
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(DestructibleHitPair), nameof(DestructibleHitPair.Raycast))),
                    new(OpCodes.Ldfld, Field(typeof(HitRayPair), nameof(HitRayPair.Hit))),

                    // this.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.Firearm))),

                    // destructible
                    new(OpCodes.Ldloc_2),

                    // damage
                    new(OpCodes.Ldloc_0),

                    // ShotEventArgs ev = new ShotEventArgs(this, hitInfo, firearm, component, damage);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShotEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShot))),

                    // if (!ev.CanHurt) num = 0;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanHurt))),
                    new(OpCodes.Brtrue, continueLabel),

                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Stloc_0),

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldarg_0);
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // if (!ev.CanSpawnImpactEffects) return;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanSpawnImpactEffects))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="HitscanHitregModuleBase.ServerAppendPrescan" />.
    /// Adds the <see cref="Handlers.Player.Shot" /> event.
    /// </summary>
    [HarmonyPatch(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.ServerAppendPrescan))]
    internal static class ShotMiss
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            CodeInstruction[] eventInstructions =
            {
                // hitInfo
                new(OpCodes.Ldloc_1),

                // this.Firearm
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.Firearm))),

                // (IDestructible)null
                new(OpCodes.Ldnull),

                // 0f
                new(OpCodes.Ldc_R4, 0f),

                // ShotEventArgs = new(this, hitInfo, this.Firearm, (IDestructible)null, 0f)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShotEventArgs))[0]),

                // Handlers.Player.OnShot(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShot))),
            };

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Brtrue_S) + 1;
            newInstructions.InsertRange(index, new CodeInstruction[] { new(OpCodes.Ldarg_0), }.AddRangeToArray(eventInstructions));

            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldarg_2);
            newInstructions.InsertRange(index, new[] { new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]) }.AddRangeToArray(eventInstructions));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
