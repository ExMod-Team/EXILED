// -----------------------------------------------------------------------
// <copyright file="WaitingForPlayers.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using MEC;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="CharacterClassManager.Start" />.
    ///     Adds the <see cref="Handlers.Server.WaitingForPlayers" /> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.Start))]
    internal static class WaitingForPlayers
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Stsfld) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // WaitingForPlayers.HelpMethod();
                new(OpCodes.Call, Method(typeof(WaitingForPlayers), nameof(WaitingForPlayers.HelpMethod))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void HelpMethod() => Timing.CallDelayed(Timing.WaitForOneFrame, Handlers.Server.OnWaitingForPlayers);
    }
}