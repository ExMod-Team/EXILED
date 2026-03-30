// -----------------------------------------------------------------------
// <copyright file="SavingByAntiScp207.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CustomPlayerEffects;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AntiScp207.GetDamageModifier"/>.
    /// Adds the <see cref="Handlers.Player.SavingByAntiScp207"/> event before the player is saved from Anti-SCP-207 damage.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SavingByAntiScp207))]
    [HarmonyPatch(typeof(AntiScp207), nameof(AntiScp207.GetDamageModifier))]
    internal class SavingByAntiScp207
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label skipLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(SavingByAntiScp207EventArgs));

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_1);

            List<Label> mainLogicLabels = newInstructions[index].ExtractLabels();

            newInstructions[index].WithLabels(skipLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this.Hub
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(mainLogicLabels),
                new(OpCodes.Call, PropertyGetter(typeof(StatusEffectBase), nameof(StatusEffectBase.Hub))),

                // damageAmount
                new(OpCodes.Ldarg_1),

                // handler
                new(OpCodes.Ldarg_2),

                // hitboxType
                new(OpCodes.Ldarg_3),

                // SavingByAntiScp207EventArgs ev = new SavingByAntiScp207EventArgs(ReferenceHub, float, DamageHandlerBase, HitboxType)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SavingByAntiScp207EventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnSavingByAntiScp207(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSavingByAntiScp207))),

                // if (!ev.IsAllowed)
                // return 1f;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SavingByAntiScp207EventArgs), nameof(SavingByAntiScp207EventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, skipLabel),

                new(OpCodes.Ldc_R4, 1f),
                new(OpCodes.Ret),
            });

            const int Offset = 1;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldloc_1) + Offset;

            newInstructions.RemoveAt(index);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // module.CurValue = ev.DeathSaveHealth;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SavingByAntiScp207EventArgs), nameof(SavingByAntiScp207EventArgs.DeathSaveHealth))),
            });

            for (int i = 0; i < newInstructions.Count; i++)
            {
                yield return newInstructions[i];
            }

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
