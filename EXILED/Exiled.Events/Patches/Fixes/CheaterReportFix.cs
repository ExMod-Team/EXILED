// -----------------------------------------------------------------------
// <copyright file="CheaterReportFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="CheaterReport.SubmitReport"/>.
    /// Fixes method not working.
    /// Bug reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/1814).
    /// </summary>
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.SubmitReport))]
    public class CheaterReportFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            CodeInstruction instruction = newInstructions.Find(instruction => instruction.opcode == OpCodes.Ldstr);

            // will notify devs of fix when NW fixes themselves, rather than another silent error
            if (instruction.operand is not "payload_json=")
            {
                Log.Error($"{typeof(CheaterReportFix).FullName} failed to fix {typeof(CheaterReport).FullName}.{nameof(CheaterReport.SubmitReport)}, please verify this fix is still required and fix it if so.");

                for (int index = 0; index < newInstructions.Count; index++)
                    yield return newInstructions[index];

                ListPool<CodeInstruction>.Pool.Return(newInstructions);
                yield break;
            }

            // easiest solution with minimal changes, but NW fixing it will make us cause issue.
            instruction.operand = string.Empty;

            for (int index = 0; index < newInstructions.Count; index++)
                yield return newInstructions[index];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}