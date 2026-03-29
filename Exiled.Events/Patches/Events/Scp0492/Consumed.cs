// -----------------------------------------------------------------------
// <copyright file="Consumed.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp0492
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp0492;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.Subroutines;
    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ZombieConsumeAbility.ServerComplete"/>
    /// to add <see cref="Handlers.Scp0492.ConsumedCorpse" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp0492), nameof(Handlers.Scp0492.ConsumedCorpse))]
    [HarmonyPatch(typeof(ZombieConsumeAbility), nameof(ZombieConsumeAbility.ServerComplete))]
    public class Consumed
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ConsumedCorpseEventArgs));

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // base.Owner
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<ZombieRole>), nameof(StandardSubroutine<ZombieRole>.Owner))),

                    // base.CurRagdoll
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(RagdollAbilityBase<ZombieRole>), nameof(RagdollAbilityBase<ZombieRole>.CurRagdoll))),

                    // ConsumingCorpseEventArgs ev = new(ReferenceHub, Ragdoll)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ConsumedCorpseEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp049.OnConsumedCorpse(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp0492), nameof(Handlers.Scp0492.OnConsumedCorpse))),
                });

            // replace "Scp0492ConsumingCorpseEventArgs(base.Owner, base.CurRagdoll, 100f);" with "Scp0492ConsumingCorpseEventArgs(base.Owner, base.CurRagdoll, ev.ConsumeHeal);"
            int offset = 0;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldc_R4 && (float)x.operand == ZombieConsumeAbility.ConsumeHeal) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.ConsumeHeal
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ConsumedCorpseEventArgs), nameof(ConsumedCorpseEventArgs.ConsumeHeal))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
