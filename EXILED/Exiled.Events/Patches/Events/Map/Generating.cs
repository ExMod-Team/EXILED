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
    using System.Diagnostics;
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

        /// <summary>
        /// Determines what layouts will be generated from a seed, code comes from interpreting <see cref="AtlasZoneGenerator.Generate"/> and sub-methods.
        /// </summary>
        /// <param name="seed">The seed to find the layouts of.</param>
        /// <param name="lcz">The Light Containment Zone layout of the seed.</param>
        /// <param name="hcz">The Heavy Containment Zone layout of the seed.</param>
        /// <param name="ez">The Entrance Zone layout of the seed.</param>
        /// <returns>Whether the method executed correctly.</returns>
        internal static bool TryDetermineLayouts(int seed, out LczFacilityLayout lcz, out HczFacilityLayout hcz, out EzFacilityLayout ez)
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
                    Spawned.Clear();
                    ZoneGenerator generator = gens[i];

                    switch (generator)
                    {
                        // EntranceZoneGenerator should be the last zone generator
                        case EntranceZoneGenerator ezGen:
                            if (i != gens.Length - 1 - ExcludedZoneGeneratorCount)
                            {
                                Log.Error("EntranceZoneGenerator was not in expected index!");
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

                            // debugs here are good for determining if the wrong # of rng.Next();'s are called.
                            Log.Debug(interpretations.Length);

                            // this block "generates" them and accounts for duplicates and other things.
                            for (int j = 0; j < interpretations.Length; j++)
                            {
                                Log.Debug(interpretations[j].ToString());
                                FakeSpawn(gen, interpretations[j], rng);
                            }

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

            LocalBuilder newSeed = generator.DeclareLocal(typeof(int));

            int offset = -2;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Stloc_1) + offset;

            /*
             * To summarize this transpiler:
             *
             * if (!TryDetermineLayouts(SeedSynchronizer.Seed, out LczFacilityLayout lcz, out HczFacilityLayout hcz, out EzFacilityLayout ez)
             *  goto skipEvent;
             *
             * ev = new GeneratingEventArgs(SeedSynchronizer.Seed, lcz, hcz, ez);
             * Handlers.Map.OnGenerating(ev);
             *
             * if (!ev.IsAllowed)
             *  goto "Map generation cancelled by a plugin." debug statement;
             *
             * if (SeedSynchronizer.Seed != ev.Seed)
             * {
             *  SeedSynchronizer.Seed = ev.Seed;
             *  goto skipEvent;
             * }
             *
             * int newSeed = GenerateSeed(ev.TargetLczLayout, ev.TargetHczLayout, ev.TargetEzLayout);
             * if (newSeed == -1)
             *  goto skipEvent;
             * SeedSynchronizer.Seed = newSeed;
            */

            newInstructions.InsertRange(index, new[]
            {
                // SeedSynchronizer.Seed
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(SeedSynchronizer), nameof(SeedSynchronizer.Seed))).MoveLabelsFrom(newInstructions[index]),

                // TryDetermineLayouts(ev.Seed, out lcz, out hcz, out ez)
                new(OpCodes.Ldloca_S, lcz),
                new(OpCodes.Ldloca_S, hcz),
                new(OpCodes.Ldloca_S, ez),
                new(OpCodes.Call, Method(typeof(Generating), nameof(TryDetermineLayouts))),

                // if (false) skip our code;
                new(OpCodes.Brfalse_S, skipLabel),

                // SeedSynchronizer.Seed again
                new(OpCodes.Call, PropertyGetter(typeof(SeedSynchronizer), nameof(SeedSynchronizer.Seed))),

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

                // if (SeedSynchronizer.Seed != ev.Seed)
                new(OpCodes.Call, PropertyGetter(typeof(SeedSynchronizer), nameof(SeedSynchronizer.Seed))),
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.Seed))),
                new(OpCodes.Beq_S, continueEventLabel),

                // {
                // SeedSynchronizer.Seed = ev.Seed
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.Seed))),
                new(OpCodes.Call, PropertySetter(typeof(SeedSynchronizer), nameof(SeedSynchronizer.Seed))),

                // skip other event code;
                new(OpCodes.Br_S, skipLabel),

                // }
                new CodeInstruction(OpCodes.Ldloc_S, ev).WithLabels(continueEventLabel),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.TargetLczLayout))),

                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.TargetHczLayout))),

                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratingEventArgs), nameof(GeneratingEventArgs.TargetEzLayout))),

                // int newSeed = GenerateSeed(ev.TargetLczLayout, ev.TargetHczLayout, ev.TargetEzLayout);
                new(OpCodes.Call, Method(typeof(Generating), nameof(GenerateSeed))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, newSeed),

                // if (newSeed == -1) skip other event code;
                new(OpCodes.Ldc_I4_M1),
                new(OpCodes.Beq_S, skipLabel),

                // SeedSynchronizer.Seed = newSeed;
                new(OpCodes.Ldloc_S, newSeed),
                new(OpCodes.Call, PropertySetter(typeof(SeedSynchronizer), nameof(SeedSynchronizer.Seed))),
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

            Stopwatch debug = new();
            debug.Start();

            int i = 0;

            try
            {
                // TODO: optimize, increase max iterations, and calculate probability of failure.
                for (i = 0; i < 1000; i++)
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
                    {
                        if (lcz != currLcz)
                        {
                            Log.Error($"{typeof(Generating).FullName}.{nameof(GenerateSeed)}: A logical error occured processing {seed}. Data: {matches}, {bestMatches}, {currLcz}, {currHcz}, {currEz}");
                        }

                        if (hcz != currHcz)
                        {
                            Log.Error($"{typeof(Generating).FullName}.{nameof(GenerateSeed)}: A logical error occured processing {seed}. Data: {matches}, {bestMatches}, {currLcz}, {currHcz}, {currEz}");
                        }

                        if (ez != currEz)
                        {
                            Log.Error($"{typeof(Generating).FullName}.{nameof(GenerateSeed)}: A logical error occured processing {seed}. Data: {matches}, {bestMatches}, {currLcz}, {currHcz}, {currEz}");
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            debug.Stop();
            Log.Debug($"Attempted {i} seeds in {debug.Elapsed.TotalSeconds}");

            return best;
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

        private static void FakeSpawn(AtlasZoneGenerator gen, AtlasInterpretation interpretation, System.Random rng)
        {
            Candidates.Clear();
            float chanceMultiplier = 0F;
            bool flag = interpretation.SpecificRooms.Length != 0;
            foreach (SpawnableRoom room in gen.CompatibleRooms)
            {
                SpawnableRoom spawnableRoom = room;
                if (spawnableRoom.HolidayVariants.TryGetResult(out SpawnableRoom result))
                {
                    spawnableRoom = result;
                }

                int count = Spawned.Count(spawned => spawned.ChosenCandidate == spawnableRoom);
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
    }
}