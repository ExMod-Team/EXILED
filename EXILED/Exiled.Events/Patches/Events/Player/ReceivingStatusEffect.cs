// -----------------------------------------------------------------------
// <copyright file="ReceivingStatusEffect.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using CustomPlayerEffects;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="StatusEffectBase.ForceIntensity"/> method.
    /// Adds the <see cref="Handlers.Player.ReceivingEffect"/> event and fix NW Duration not being correctly set to 0 when effect is Reset.
    /// </summary>
    // [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ReceivingEffect))]
    [HarmonyPatch(typeof(StatusEffectBase), nameof(StatusEffectBase.ForceIntensity))]
    internal static class ReceivingStatusEffect
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // remove "if (this._intensity == value) return;"
            // to make than ChangingDuration still called the event.
            newInstructions.RemoveRange(0, 5);

            LocalBuilder ev = generator.DeclareLocal(typeof(ReceivingEffectEventArgs));

            Label returnLabel = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                    // value
                    new(OpCodes.Ldarg_1),

                    // this._intensity
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(StatusEffectBase), nameof(StatusEffectBase._intensity))),

                    // this._timeLeft
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(StatusEffectBase), nameof(StatusEffectBase._timeLeft))),

                    // ReceivingEventArgs ev = new(Player, StatusEffectBase, byte, byte, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReceivingEffectEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnReceivingEffect))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // value = ev.State
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.Intensity))),
                    new(OpCodes.Starg_S, 1),

                    // this._timeLeft = ev.Duration
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.Duration))),
                    new(OpCodes.Stfld, Field(typeof(StatusEffectBase), nameof(StatusEffectBase._timeLeft))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches the <see cref="StatusEffectBase.Intensity"/> Propperty setter.
    /// Fix than above patched would not be called by modifying duration of the Effect.
    /// </summary>
    // [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ReceivingEffect))]
    [HarmonyPatch(typeof(StatusEffectBase), nameof(StatusEffectBase.Intensity), MethodType.Setter)]
    internal static class FixDurationNotCallingReceivingStatusEffect
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // remove "if (value > this._intensity) return;"
            // to make than ChangingDuration still called the event.
            newInstructions.RemoveRange(0, 4);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}