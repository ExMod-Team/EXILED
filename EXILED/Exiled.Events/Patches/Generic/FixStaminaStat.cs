// -----------------------------------------------------------------------
// <copyright file="FixStaminaStat.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.CustomStats;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl;
    using PlayerStatsSystem;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Fix for <see cref="CustomStaminaStat"/>.
    /// </summary>
    [HarmonyPatch(typeof(FpcStateProcessor), nameof(FpcStateProcessor.UpdateMovementState))]
    internal static class FixStaminaStat
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // replace "1f" const to "this._stat.MaxValue"
            int offset = 0;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldc_R4 && (float)i.operand == 1f) + offset;

            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this._stat.MaxValue
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(FpcStateProcessor), nameof(FpcStateProcessor._stat))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StaminaStat), nameof(StaminaStat.MaxValue))),
            });

            // replace all "Mathf.Clamp01(value)" to "Mathf.Clamp(value, 0, this._stat.MaxValue)"
            System.Reflection.MethodInfo clamp01Method = Method(typeof(Mathf), nameof(Mathf.Clamp01));
            System.Reflection.MethodInfo clampMethod = Method(typeof(Mathf), nameof(Mathf.Clamp), new Type[] { typeof(float), typeof(float), typeof(float), });
            for (int i = 0; i < newInstructions.Count; i++)
            {
                CodeInstruction codeInstruction = newInstructions[i];
                if (codeInstruction.operand == (object)clamp01Method)
                {
                    newInstructions.InsertRange(i, new CodeInstruction[]
                    {
                        // 0f
                        new(OpCodes.Ldc_R4, 0f),

                        // this._stat.MaxValue
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldfld, Field(typeof(FpcStateProcessor), nameof(FpcStateProcessor._stat))),
                        new(OpCodes.Callvirt, PropertyGetter(typeof(StaminaStat), nameof(StaminaStat.MaxValue))),
                    });
                    codeInstruction.operand = clampMethod;
                }
            }

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}