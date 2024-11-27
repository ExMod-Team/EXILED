// -----------------------------------------------------------------------
// <copyright file="RespawningTeam.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using PlayerRoles;
    using Respawning;
    using Respawning.NamingRules;
    using Respawning.Waves;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    // This event should be renaned to SpawningWave, might be a good time to do it now since it's a breaking change

    /// <summary>
    /// Patch the <see cref="WaveSpawner.SpawnWave" />.
    /// Adds the <see cref="Server.RespawningTeam" /> event.
    /// </summary>
    [EventPatch(typeof(Server), nameof(Server.RespawningTeam))]
    [HarmonyPatch(typeof(WaveSpawner), nameof(WaveSpawner.SpawnWave))]
    internal static class RespawningTeam
    {
        /*private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(SpawnableWaveBase), nameof(SpawnableWaveBase.PopulateQueue)))) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(RespawningTeamEventArgs));

            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // GetPlayers(list);
                    new CodeInstruction(OpCodes.Ldloc_2).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetPlayers))),

                    // maxWaveSize
                    new(OpCodes.Ldloc_2),

                    // this.NextKnownTeam
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // RespawningTeamEventArgs ev = new(players, num, this.NextKnownTeam)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RespawningTeamEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Server.OnRespawningTeam(ev)
                    new(OpCodes.Call, Method(typeof(Server), nameof(Server.OnRespawningTeam))),

                    // if (ev.IsAllowed)
                    //    goto continueLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // this.NextKnownTeam == null
                    //    return;
                    new(OpCodes.Ldnull),
                    new(OpCodes.Stsfld, Field(typeof(WaveManager), nameof(WaveManager._nextWave))),
                    new(OpCodes.Ret),

                    // load "ev" four times
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // num = ev.MaximumRespawnAmount
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.MaximumRespawnAmount))),
                    new(OpCodes.Stloc_S, 4),

                    // list = GetHubs(ev.Players)
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.Players))),
                    new(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetHubs))),
                    new(OpCodes.Stloc_1),

                    // queueToFill = ev.SpawnQueue;
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.SpawnQueue))),
                    new(OpCodes.Call, Method(typeof(RespawningTeam), nameof(RefillQueue))),

                    // wave = ev.NextKnownTeam;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.NextKnownTeam))),
                    new(OpCodes.Starg_S, 1),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static List<Player> GetPlayers(List<ReferenceHub> hubs) => hubs.Select(Player.Get).ToList();

        private static List<ReferenceHub> GetHubs(List<Player> players) => players.Select(player => player.ReferenceHub).ToList();

        private static void RefillQueue(Queue<RoleTypeId> newQueue)
        {
            WaveSpawner.SpawnQueue.Clear();
            foreach (RoleTypeId role in newQueue)
                WaveSpawner.SpawnQueue.Enqueue(role);
        }*/

        private static bool Prefix(SpawnableWaveBase wave)
        {
            List<ReferenceHub> playerList = ReferenceHub.AllHubs.Where(WaveSpawner.CheckSpawnable).OrderByDescending(hub => WaveSpawner.CalculatePriority(hub, spawningTeam)).ToList<ReferenceHub>();

            RespawningTeamEventArgs ev = new()
        }
    }
}