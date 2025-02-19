// -----------------------------------------------------------------------
// <copyright file="RemoteAdminNpcCommandAddToDictionaryFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using CommandSystem.Commands.RemoteAdmin.Dummies;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using GameCore;
    using HarmonyLib;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Fix to add <see cref="Npc"/> created via RA to the <see cref="Npc.List"/>.
    /// </summary>
    [HarmonyPatch(typeof(SpawnDummyCommand), nameof(SpawnDummyCommand.Execute))]
    internal static class RemoteAdminNpcCommandAddToDictionaryFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            MethodBase method = Method(typeof(DummyUtils), nameof(DummyUtils.SpawnDummy));

            // call ReferenceHub GameCore.DummyUtils::SpawnDummy(string)
            int index = newInstructions.FindIndex(instruction =>
                                        instruction.operand == (object)method) + 1;

            LocalBuilder npc = generator.DeclareLocal(typeof(Npc));

            // pop
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                    {
                        // Npc::.ctor(ReferenceHub)
                        new(OpCodes.Newobj,   Constructor(typeof(Npc), new[] { typeof(ReferenceHub) })),
                        new(OpCodes.Stloc_S,  npc.LocalIndex),

                        // Player.Dictionary.get_Dictionary
                        new(OpCodes.Call,     PropertyGetter(typeof(Player), nameof(Player.Dictionary))),
                        new(OpCodes.Ldloc_S,  npc.LocalIndex),

                        // Player::GameObject.get_GameObject
                        new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.GameObject))),
                        new(OpCodes.Ldloc_S,  npc.LocalIndex),

                        // Player.Dictionary.Add(GameObject, ReferenceHub)
                        new(OpCodes.Callvirt, Method(typeof(Dictionary<GameObject, Player>), nameof(Dictionary<GameObject, Player>.Add))),
                    });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
