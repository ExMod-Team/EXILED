// -----------------------------------------------------------------------
// <copyright file="FixExplosionArmor.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
#pragma warning disable SA1402 // File may only contain a single type

namespace Exiled.Events.Patches.Fixes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    using Exiled.API.Features.Pools;

    using Footprinting;

    using HarmonyLib;

    using InventorySystem.Items.Armor;

    using PlayerStatsSystem;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ExplosionDamageHandler"/> cttor.
    /// Fix Explosion damage calculation using Attacker BodyArmor instead of the one from Victim.
    /// Bug was already reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/work_items/3243).
    /// </summary>
    [HarmonyPatch(typeof(ExplosionDamageHandler), MethodType.Constructor, new Type[] { typeof(Footprint), typeof(Vector3), typeof(float), typeof(int), typeof(ExplosionType) })]

    internal class FixExplosionArmor
    {
#pragma warning disable SA1600 // Elements should be documented
        internal static readonly ConditionalWeakTable<ExplosionDamageHandler, StrongBox<int>> Memory = new();
#pragma warning restore SA1600 // Elements should be documented

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // remove the "if (armorPenetration == 0) return;"
            newInstructions.RemoveRange(0, 5);

            int offset = 1;
            int index = newInstructions.FindLastIndex(x => x.StoresField(Field(typeof(ExplosionDamageHandler), nameof(ExplosionDamageHandler._serverLogsText)))) + offset;

            newInstructions.InsertRange(index, new List<CodeInstruction>()
            {
                // this.Damage = damage;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Callvirt, PropertySetter(typeof(StandardDamageHandler), nameof(StandardDamageHandler.Damage))),

                // FixExplosionArmor.Memory.Add(this, new StrongBox(value))
                new(OpCodes.Ldsfld, Field(typeof(FixExplosionArmor), nameof(FixExplosionArmor.Memory))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_S, 4),
                new(OpCodes.Newobj, DeclaredConstructor(typeof(StrongBox<int>), new Type[] { typeof(int), }, false)),
                new(OpCodes.Callvirt, Method(typeof(ConditionalWeakTable<ExplosionDamageHandler, StrongBox<int>>), nameof(ConditionalWeakTable<ExplosionDamageHandler, StrongBox<int>>.Add))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="ExplosionDamageHandler.ApplyDamage"/>.
    /// Fix Explosion damage calculation using Attacker BodyArmor instead of the one from Victim.
    /// Bug was already reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/work_items/3243).
    /// </summary>
    [HarmonyPatch(typeof(ExplosionDamageHandler), nameof(ExplosionDamageHandler.ApplyDamage))]
    internal class FixExplosionArmor2
    {
#pragma warning disable SA1600 // Elements should be documented
        internal static float HelperMethod(ExplosionDamageHandler @this, ReferenceHub ply)
#pragma warning restore SA1600 // Elements should be documented
        {
            if (ply.inventory.TryGetBodyArmor(out BodyArmor bodyArmor) && FixExplosionArmor.Memory.TryGetValue(@this, out StrongBox<int> armorPenetration))
            {
                return BodyArmorUtils.ProcessDamage(bodyArmor.VestEfficacy, @this.Damage, armorPenetration.Value);
            }

            return @this.Damage;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(0, new List<CodeInstruction>()
            {
                // base.Damage = FixExplosionArmor2.HelperMethod(this, ply);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(FixExplosionArmor2), nameof(FixExplosionArmor2.HelperMethod))),
                new(OpCodes.Callvirt, PropertySetter(typeof(StandardDamageHandler), nameof(StandardDamageHandler.Damage))),
            });
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}