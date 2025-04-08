﻿// -----------------------------------------------------------------------
// <copyright file="SpawningRoomConnector.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using MapGeneration.RoomConnectors;
    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="WaveUpdateMessage.ServerSendUpdate"/>.
    /// Adds the <see cref="Handlers.Map.SpawningTeamVehicle"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.SpawningRoomConnector))]
    [HarmonyPatch(typeof(RoomConnectorDistributorSettings), nameof(RoomConnectorDistributorSettings.TryGetTemplate))]
    internal static class SpawningRoomConnector
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningRoomConnectorEventArgs));

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                // type
                new CodeInstruction(OpCodes.Ldarg_0),

                // SpawningRoomConnectorEventArgs ev = new SpawningRoomConnectorEventArgs(type)
                new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(SpawningRoomConnectorEventArgs), new[] { typeof(SpawnableRoomConnectorType) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Map.OnSpawningRoomConnector(ev);
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Handlers.Map), nameof(Handlers.Map.OnSpawningRoomConnector))),

                // if (!ev.IsAllowed)
                //    return;
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(SpawningRoomConnectorEventArgs), nameof(SpawningRoomConnectorEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, retLabel),

                // type = ev.ConnectorType
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(SpawningRoomConnectorEventArgs), nameof(SpawningRoomConnectorEventArgs.ConnectorType))),
                new CodeInstruction(OpCodes.Starg_S, 0),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}