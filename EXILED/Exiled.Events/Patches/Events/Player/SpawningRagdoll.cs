// -----------------------------------------------------------------------
// <copyright file="SpawningRagdoll.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles.Ragdolls;

    using PlayerStatsSystem;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = Handlers.Player;

    /// <summary>
    /// Patches <see cref="RagdollManager.ServerSpawnRagdoll(ReferenceHub, DamageHandlerBase)" />.
    /// <br>Adds the <see cref="Player.SpawningRagdoll" /> and <see cref="Player.SpawnedRagdoll"/> events.</br>
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.SpawningRagdoll))]
    [EventPatch(typeof(Player), nameof(Player.SpawnedRagdoll))]
    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
    internal static class SpawningRagdoll
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label cnt = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningRagdollEventArgs));

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(PropertySetter(typeof(BasicRagdoll), nameof(BasicRagdoll.NetworkInfo)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // ragdoll.NetworkInfo
                new(OpCodes.Ldloc_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(BasicRagdoll), nameof(BasicRagdoll.NetworkInfo))),

                // true
                new(OpCodes.Ldc_I4_1),

                // SpawningRagdollEventArgs ev = new(ragdoll, RagdollInfo, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningRagdollEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Player.OnSpawningRagdoll(ev)
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnSpawningRagdoll))),

                // if (!ev.IsAllowed) {
                //     Object.Destroy(gameObject);
                //     return null;
                // }
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, cnt),
            });

            newInstructions.InsertRange(newInstructions.Count - 2, new CodeInstruction[]
            {
                // ev.Player
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Player))),

                // Ragdoll::Get(basicRagdoll)
                new(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(Ragdoll), nameof(Ragdoll.Get), new[] { typeof(BasicRagdoll) })),

                // ev.Info
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Info))),

                // ev.DamageHandlerBase
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.DamageHandlerBase))),

                // Player::OnSpawnedRagdoll(new SpawnedRagdollEventArgs(ev.Player, Ragdoll::Get(basicRagdoll), ev.Info, ev.DamageHandlerBase))
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawnedRagdollEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnSpawnedRagdoll))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
