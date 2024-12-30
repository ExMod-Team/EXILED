// -----------------------------------------------------------------------
// <copyright file="ServerSideDancesPatch.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.API.Features;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.Subroutines;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Scp3114Role = API.Features.Roles.Scp3114Role;

    /// <summary>
    /// Patches the <see cref="Scp3114Dance.DanceVariant"/>.
    /// Fix that the game doesn't write this.
    /// </summary>
    [HarmonyPatch(typeof(Scp3114Dance), nameof(Scp3114Dance.ServerWriteRpc))]
    internal class ServerSideDancesPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label skip = generator.DefineLabel();

            newInstructions.Add(new CodeInstruction(OpCodes.Ret));
            newInstructions[newInstructions.Count - 1].labels.Add(skip);

            int offset = 4;
            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(SubroutineBase), nameof(SubroutineBase.ServerWriteRpc)))) + offset;

            newInstructions.InsertRange(index, new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Br_S, skip),
            });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void Postfix(ref Scp3114Dance __instance, NetworkWriter writer)
        {
            Player player = Player.Get(__instance.Owner);

            Scp3114Role role = player.Role as Scp3114Role;

            if (player != null && role.DanceType != API.Enums.DanceType.None)
            {
                writer.WriteByte((byte)role.DanceType);
                return;
            }

            writer.WriteByte((byte)Random.Range(0, 255));
            return;
        }
    }
}
