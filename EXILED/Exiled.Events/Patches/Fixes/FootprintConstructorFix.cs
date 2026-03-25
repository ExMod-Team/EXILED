// -----------------------------------------------------------------------
// <copyright file="FootprintConstructorFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Footprinting;
    using HarmonyLib;
    using LiteNetLib;
    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Footprint(ReferenceHub)"/> constructor.
    /// Fixes an issue where calling the constructor after a player disconnects throws an error.
    /// </summary>
    // [HarmonyPatch(typeof(Footprint), MethodType.Constructor, typeof(ReferenceHub))]
    public class FootprintConstructorFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldnull);

            Label nullIpLabel = newInstructions[index].labels.First();

            index -= 4;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldsfld, Field(typeof(Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorServer), nameof(Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorServer.Peers))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.connectionToClient))),
                new(OpCodes.Ldfld, Field(typeof(NetworkConnectionToClient), nameof(NetworkConnectionToClient.connectionId))),
                new(OpCodes.Callvirt, Method(typeof(ConcurrentDictionary<int, NetPeer>), nameof(ConcurrentDictionary<int, NetPeer>.ContainsKey))),
                new(OpCodes.Brfalse_S, nullIpLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}