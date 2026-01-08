// -----------------------------------------------------------------------
// <copyright file="Generating.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using LabApi.Events.Arguments.ServerEvents;
    using MapGeneration;
    using MapGeneration.Holidays;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="SeedSynchronizer.Awake" />.
    /// Adds the <see cref="Handlers.Map.Generating" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.Generating))]
    [HarmonyPatch(typeof(SeedSynchronizer), nameof(SeedSynchronizer.Awake))]
    public class Generating
    {
        private static readonly List<SpawnableRoom> Candidates = new();
        private static readonly List<AtlasZoneGenerator.SpawnedRoomData> Spawned = new();

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label skipLabel = generator.DefineLabel();

            // Label to LabAPI's ev.IsAllowed = false branch
            Label notAllowedLabel = newInstructions.FindLast(x => x.opcode == OpCodes.Ldstr).labels.First();

            Label continueEventLabel = generator.DefineLabel();

            LocalBuilder lcz = generator.DeclareLocal(typeof(LczFacilityLayout));
            LocalBuilder hcz = generator.DeclareLocal(typeof(HczFacilityLayout));
            LocalBuilder ez = generator.DeclareLocal(typeof(EzFacilityLayout));

            LocalBuilder ev = generator.DeclareLocal(typeof(GeneratingEventArgs));

            Label lczLabel1 = generator.DefineLabel();
            Label lczLabel2 = generator.DefineLabel();

            Label hczLabel1 = generator.DefineLabel();
            Label hczLabel2 = generator.DefineLabel();

            Label ezLabel1 = generator.DefineLabel();
            Label ezLabel2 = generator.DefineLabel();

            LocalBuilder newSeed = generator.DeclareLocal(typeof(int));

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_1);

            /*
             * To summarize this transpiler:
             *
             * if (!TryDetermineLayouts(ev2.Seed, out LczFacilityLayout lcz, out HczFacilityLayout hcz, out EzFacilityLayout ez)
             *  goto skipEvent;
             *
             * ev = new GeneratingEventArgs(ev2.Seed, lcz, hcz, ez);
             * Handlers.Map.OnGenerating(ev);
             *
             * if (!ev.IsAllowed)
             *  goto "Map generation cancelled by a plugin." debug statement;
             *
             * if (ev2.Seed != ev.Seed)
             * {
             *  ev2.Seed = ev.Seed;
             *  goto skipEvent;
             * }
             *
             * int newSeed = GenerateSeed(lcz == ev.LczLayout ? LczFacilityLayout.Unknown : ev.LczLayout, hcz == ev.HczLayout ? HczFacilityLayout.Unknown : ev.HczLayout, ez == ev.EzLayout ? EzFacilityLayout.Unknown : ev.EzLayout);
             * if (newSeed == -1)
             *  goto skipEvent;
             * ev2.Seed = newSeed;
             */

            newInstructions.InsertRange(index, new[]
            {
                // ev.Seed (from LabAPI event)
                new CodeInstruction(OpCodes.Ldloc_1).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(MapGeneratingEventArgs), nameof(MapGeneratingEventArgs.Seed))),

                // TryDetermineLayouts(ev.Seed, out lcz, out hcz, out ez)
                new(OpCodes.Ldloca_S, lcz),
                new(OpCodes.Ldloca_S, hcz),
                new(OpCodes.Ldloca_S, ez),
                new(OpCodes.Call, Method(typeof(Generating), nameof(TryDetermineLayouts))),

                // if (false) skip our code;
                new(OpCodes.Brfalse_S, skipLabel),

                // ev.Seed (from LabAPI event) again
                new CodeInstruction(OpCodes.Ldloc_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(MapGeneratingEventArgs), nameof(MapGeneratingEventArgs.Seed))),

                // new GeneratingEventArgs(ev.Seed, lcz, hcz, ez);
                new(OpCodes.Ldloc_S, lcz),
                new(OpCodes.Ldloc_S, hcz),
                new(OpCodes.Ldloc_S, ez),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(GeneratingEventArgs))[0]),

                // Dup on stack
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                // Call OnGenerating
                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnGenerating))),

                // save ev
                new(OpCodes.Stloc_S, ev),

                // if (!ev.IsAllowed) goto "Map generation cancelled by a plugin." debug statement;
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, notAllowedLabel),

                // if (ev.Seed (LabAPI) != ev.Seed (ours))
                new(OpCodes.Ldloc_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(MapGeneratingEventArgs), nameof(MapGeneratingEventArgs.Seed))),
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.Seed))),
                new(OpCodes.Beq_S, continueEventLabel),

                // {
                // ev.Seed (LabAPI) = ev.Seed (ours)
                new(OpCodes.Ldloc_1),
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.Seed))),
                new(OpCodes.Callvirt, PropertySetter(typeof(MapGeneratingEventArgs), nameof(MapGeneratingEventArgs.Seed))),

                // skip other event code;
                new(OpCodes.Br_S, skipLabel),

                // }
                // load (lcz == ev.LczLayout ? LczFacilityLayout.Unknown : ev.LczLayout) onto the stack
                new CodeInstruction(OpCodes.Ldloc_S, lcz).WithLabels(continueEventLabel),
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.LczLayout))),
                new(OpCodes.Beq_S, lczLabel1),

                // ev.LczLayout
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.LczLayout))),
                new(OpCodes.Ldind_I4),
                new(OpCodes.Br_S, lczLabel2),

                // LczFacilityLayout.Unknown
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(lczLabel1),

                // load (hcz == ev.HczLayout ? HczFacilityLayout.Unknown : ev.HczLayout) onto the stack
                new CodeInstruction(OpCodes.Ldloc_S, hcz).WithLabels(lczLabel2),
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.HczLayout))),
                new(OpCodes.Beq_S, hczLabel1),

                // ev.HczLayout
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.HczLayout))),
                new(OpCodes.Ldind_I4),
                new(OpCodes.Br_S, hczLabel2),

                // HczFacilityLayout.Unknown
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(hczLabel1),

                // load (ez == ev.EzLayout ? EzFacilityLayout.Unknown : ev.EzLayout) onto the stack
                new CodeInstruction(OpCodes.Ldloc_S, ez).WithLabels(hczLabel2),
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.EzLayout))),
                new(OpCodes.Beq_S, ezLabel1),

                // ev.EzLayout
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.EzLayout))),
                new(OpCodes.Ldind_I4),
                new(OpCodes.Br_S, ezLabel2),

                // EzFacilityLayout.Unknown
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(ezLabel1),

                // int newSeed = GenerateSeed(those 3 ternary values);
                new CodeInstruction(OpCodes.Call, Method(typeof(Generating), nameof(GenerateSeed))).WithLabels(ezLabel2),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, newSeed),

                // if (newSeed == -1) skip other event code;
                new(OpCodes.Ldc_I4_M1),
                new(OpCodes.Beq_S, skipLabel),

                // ev.Seed (LabAPI) = newSeed;
                new(OpCodes.Ldloc_1),
                new(OpCodes.Ldloc_S, newSeed),
                new(OpCodes.Callvirt, PropertySetter(typeof(MapGeneratingEventArgs), nameof(MapGeneratingEventArgs.Seed))),
            });

            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_1);

            newInstructions[index].WithLabels(skipLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        // generates a seed for target layouts
        private static int GenerateSeed(LczFacilityLayout lcz, HczFacilityLayout hcz, EzFacilityLayout ez)
        {
            if (lcz is LczFacilityLayout.Unknown && hcz is HczFacilityLayout.Unknown && ez is EzFacilityLayout.Unknown)
                return -1;

            System.Random seedGenerator = new();

            int best = -1;
            int bestMatches = 0;

            for (int i = 0; i < 1000; i++)
            {
                int matches = 0;

                int seed = seedGenerator.Next(1, int.MaxValue);

                if (!TryDetermineLayouts(seed, out LczFacilityLayout currLcz, out HczFacilityLayout currHcz, out EzFacilityLayout currEz))
                {
                    break;
                }

                if (lcz is LczFacilityLayout.Unknown || currLcz == lcz)
                    matches++;
                if (hcz is HczFacilityLayout.Unknown || currHcz == hcz)
                    matches++;
                if (ez is EzFacilityLayout.Unknown || currEz == ez)
                    matches++;

                if (matches > bestMatches)
                {
                    best = seed;
                    bestMatches = matches;
                }

                if (bestMatches is 3)
                    break;
            }

            return best;
        }

        /// <summary>
        /// Determines what layouts will be generated from a seed, code comes from interpreting <see cref="AtlasZoneGenerator.Generate"/> and sub-methods.
        /// </summary>
        /// <param name="seed">The seed to find the layouts of.</param>
        /// <param name="lcz">The Light Containment Zone layout of the seed.</param>
        /// <param name="hcz">The Heavy Containment Zone layout of the seed.</param>
        /// <param name="ez">The Entrance Zone layout of the seed.</param>
        /// <returns>Whether the method executed correctly.</returns>
        private static bool TryDetermineLayouts(int seed, out LczFacilityLayout lcz, out HczFacilityLayout hcz, out EzFacilityLayout ez)
        {
            // (Surface gen + PD gen)
            const int ExcludedZoneGeneratorCount = 2;

            lcz = LczFacilityLayout.Unknown;
            hcz = HczFacilityLayout.Unknown;
            ez = EzFacilityLayout.Unknown;

            System.Random rng = new(seed);
            try
            {
                ZoneGenerator[] gens = SeedSynchronizer._singleton._zoneGenerators;
                for (int i = 0; i < gens.Length - ExcludedZoneGeneratorCount; i++)
                {
                    ZoneGenerator generator = gens[i];

                    switch (generator)
                    {
                        // EntranceZoneGenerator should be the last zone generator
                        case EntranceZoneGenerator ezGen:
                            if (i != gens.Length - 1 - ExcludedZoneGeneratorCount)
                            {
                                return false;
                            }

                            ez = (EzFacilityLayout)(rng.Next(ezGen.Atlases.Length) + 1);
                            break;
                        case AtlasZoneGenerator gen:
                            int layout = rng.Next(gen.Atlases.Length);

                            if (gen is LightContainmentZoneGenerator)
                                lcz = (LczFacilityLayout)(layout + 1);
                            else
                                hcz = (HczFacilityLayout)(layout + 1);

                            // rng needs to be called the same amount as during map gen for next zone generator.
                            // this block of code picks what rooms to generate.
                            Texture2D tex = gen.Atlases[layout];
                            AtlasInterpretation[] interpretations = MapAtlasInterpreter.Singleton.Interpret(tex, rng);
                            RandomizeInterpreted(rng, interpretations);
                            foreach (AtlasInterpretation interpretation in interpretations)
                                FakeSpawn(gen, interpretation, rng);

                            // this block "generates" them and accounts for duplicates and other things.
                            break;
                        default:
                            Log.Error($"{typeof(Generating).FullName}.{nameof(TryDetermineLayouts)}: Found non AtlasZoneGenerator [{generator}] in SeedSynchronizer._singleton._zoneGenerators!");
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }

            bool error = false;
            if (lcz is LczFacilityLayout.Unknown)
            {
                Log.Error($"{typeof(Generating).FullName}.{nameof(TryDetermineLayouts)}: Failed to find layout for LCZ for seed {seed}");
                error = true;
            }

            if (hcz is HczFacilityLayout.Unknown)
            {
                Log.Error($"{typeof(Generating).FullName}.{nameof(TryDetermineLayouts)}: Failed to find layout for HCZ for seed {seed}");
                error = true;
            }

            if (ez is EzFacilityLayout.Unknown)
            {
                Log.Error($"{typeof(Generating).FullName}.{nameof(TryDetermineLayouts)}: Failed to find layout for EZ for seed {seed}");
                error = true;
            }

            return !error;
        }

        /// <summary>
        /// Copied from <see cref="AtlasZoneGenerator.RandomizeInterpreted"/>, I changed some variable names to better reflect what is actually happening.
        /// </summary>
        /// <param name="rng">The <see cref="System.Random"/> instance.</param>
        /// <param name="interpretations">The layout rooms to iterate over.</param>
        private static void RandomizeInterpreted(System.Random rng, AtlasInterpretation[] interpretations)
        {
            int length = interpretations.Length;
            while (length > 1)
            {
                --length;
                int random = rng.Next(length + 1);
                ref AtlasInterpretation current = ref interpretations[length];
                ref AtlasInterpretation randomPick = ref interpretations[random];
                AtlasInterpretation randomPickValue = interpretations[random];
                AtlasInterpretation currentValue = interpretations[length];
                current = randomPickValue;
                randomPick = currentValue;
            }
        }

        private static void FakeSpawn(AtlasZoneGenerator generator, AtlasInterpretation interpretation, System.Random rng)
        {
            Candidates.Clear();
            float chanceMultiplier = 0F;
            bool flag = interpretation.SpecificRooms.Length != 0;
            foreach (SpawnableRoom room in generator.CompatibleRooms)
            {
                SpawnableRoom spawnableRoom = room;
                if (spawnableRoom.HolidayVariants.TryGetResult(out SpawnableRoom result))
                {
                    spawnableRoom = result;
                }

                int count = PreviouslySpawnedCount(spawnableRoom);
                if (flag != spawnableRoom.SpecialRoom || (flag && !interpretation.SpecificRooms.Contains(spawnableRoom.Room.Name)) || spawnableRoom.Room.Shape != interpretation.RoomShape || count >= spawnableRoom.MaxAmount)
                    continue;

                if (count < spawnableRoom.MinAmount)
                {
                    Spawned.Add(new AtlasZoneGenerator.SpawnedRoomData
                    {
                        ChosenCandidate = spawnableRoom,
                        Instance = null!,
                        Interpretation = interpretation,
                    });

                    return;
                }

                chanceMultiplier += GetChanceWeight(interpretation.Coords, spawnableRoom);
                Candidates.Add(spawnableRoom);
            }

            double random = rng.NextDouble() * chanceMultiplier;
            float chance = 0F;
            foreach (SpawnableRoom room in Candidates)
            {
                chance += GetChanceWeight(interpretation.Coords, room);
                if (random > chance)
                    continue;

                Spawned.Add(new AtlasZoneGenerator.SpawnedRoomData
                {
                    ChosenCandidate = room,
                    Instance = null!,
                    Interpretation = interpretation,
                });

                return;
            }
        }

        private static float GetChanceWeight(Vector2Int coords, SpawnableRoom candidate)
        {
            Vector2Int up = coords + Vector2Int.up;
            Vector2Int down = coords + Vector2Int.down;
            Vector2Int left = coords + Vector2Int.left;
            Vector2Int right = coords + Vector2Int.right;
            float chance = candidate.ChanceMultiplier;

            foreach (AtlasZoneGenerator.SpawnedRoomData spawnedRoomData in Spawned)
            {
                if (spawnedRoomData.ChosenCandidate != candidate)
                    continue;

                Vector2Int candidateCoords = spawnedRoomData.Interpretation.Coords;
                if (candidateCoords == up || candidateCoords == down || candidateCoords == left || candidateCoords == right)
                    chance *= candidate.AdjacentChanceMultiplier;
            }

            return chance;
        }

        private static int PreviouslySpawnedCount(SpawnableRoom candidate) => Spawned.Count(spawnedRoomData => spawnedRoomData.ChosenCandidate == candidate);
    }
}