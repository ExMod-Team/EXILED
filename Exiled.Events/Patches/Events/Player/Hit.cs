// -----------------------------------------------------------------------
// <copyright file="Hit.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.PlayableScps.Subroutines;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches ScpAttackAbilityBase.ServerPerformAttack
    /// to add <see cref="Handlers.Player.Hit" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Hit))]
    [HarmonyPatch(typeof(ScpAttackAbilityBase<ZombieRole>), nameof(ScpAttackAbilityBase<ZombieRole>.ServerPerformAttack))]
    public class Hit
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ret);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(base.Owner);
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<ZombieRole>), nameof(StandardSubroutine<ZombieRole>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this.LastAttackResult
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpAttackAbilityBase<ZombieRole>), nameof(ScpAttackAbilityBase<ZombieRole>.LastAttackResult))),

                    // this.DetectedPlayers
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(ScpAttackAbilityBase<ZombieRole>), nameof(ScpAttackAbilityBase<ZombieRole>.DetectedPlayers))),

                    // new(ReferenceHub, AttackResult, HashSet<ReferenceHub>)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(HitEventArgs))[0]),

                    // Handlers.Player.OnHit(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHit))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}