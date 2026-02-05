// -----------------------------------------------------------------------
// <copyright file="SpawningRoomConnector.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using MapGeneration.RoomConnectors;

#pragma warning disable SA1402 // File may only contain a single type

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using MapGeneration;
    using MapGeneration.RoomConnectors.Spawners;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoomConnectorSpawnpointBase.Spawn"/>.
    /// Adds the <see cref="Handlers.Map.OnSpawningRoomConnector"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.SpawningRoomConnector))]
    [HarmonyPatch(typeof(RoomConnectorSpawnpointBase), nameof(RoomConnectorSpawnpointBase.Spawn))]
    internal static class SpawningRoomConnector
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // this
                new(OpCodes.Ldarg_0),

                // type
                new(OpCodes.Ldarg_1),

                // SpawningRoomConnectorEventArgs ev = new SpawningRoomConnectorEventArgs(RoomConnectorSpawnpointBase, SpawnableRoomConnectorType)
                new(OpCodes.Newobj, Constructor(
                    typeof(SpawningRoomConnectorEventArgs),
                    new[] { typeof(RoomConnectorSpawnpointBase), typeof(SpawnableRoomConnectorType) })),
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                // Handlers.Map.OnSpawningRoomConnector(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnSpawningRoomConnector))),

                // type = ev.ConnectorType
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRoomConnectorEventArgs), nameof(SpawningRoomConnectorEventArgs.ConnectorType))),
                new(OpCodes.Starg_S, 1),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRoomConnectorEventArgs), nameof(SpawningRoomConnectorEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            int earlyRetIndex = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret);
            newInstructions[earlyRetIndex].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="SeedSynchronizer.GenerateLevel"/>.
    /// Adds the <see cref="Handlers.Map.OnSpawningRoomConnector"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.SpawningRoomConnector))]
    [HarmonyPatch(typeof(SeedSynchronizer), nameof(SeedSynchronizer.GenerateLevel))]
    internal static class SpawningRoomConnectorFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label skip = generator.DefineLabel();

            // steal the code to put it later in the code:
            // RoomConnectorSpawnpointBase.SetupAllRoomConnectors();
            // ServerEvents.OnMapGenerated(new MapGeneratedEventArgs(SeedSynchronizer.Seed));
            const int lengthOfCodeToMove = 4;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(RoomConnectorSpawnpointBase), nameof(RoomConnectorSpawnpointBase.SetupAllRoomConnectors))));
            List<Label> labels = newInstructions[index].ExtractLabels();
            List<CodeInstruction> codeInstructionsCopy = newInstructions.GetRange(index, lengthOfCodeToMove);
            newInstructions.RemoveRange(index, lengthOfCodeToMove);
            newInstructions[index].labels.AddRange(labels);

            index = newInstructions.FindIndex(x => x.OperandIs(Field(typeof(SeedSynchronizer), nameof(SeedSynchronizer.OnGenerationStage))));
            const int lengthOfCodeAdded = 3;
            newInstructions.InsertRange(index, new List<CodeInstruction>()
            {
                // if (mapGenerationPhase == MapGenerationPhase.ParentRoomRegistration)
                //     // The moved code
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Bne_Un_S, skip),
            });

            newInstructions.InsertRange(index + lengthOfCodeAdded, codeInstructionsCopy);
            newInstructions[index + lengthOfCodeAdded + codeInstructionsCopy.Count].labels.Add(skip);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}