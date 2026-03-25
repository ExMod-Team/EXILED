// -----------------------------------------------------------------------
// <copyright file="RoundStarting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Threading.Tasks;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Commands.Reload;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;
    using static RoundSummary;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.Init()" />.
    /// Adds the <see cref="Handlers.Server.RoundStarting" /> event.
    /// </summary>
    [HarmonyPatch]
    internal class RoundStarting
    {
        #pragma warning disable SA1600 // Elements should be documented
        public static Type PrivateType { get; internal set; }

        private static MethodInfo TargetMethod()
        {
            PrivateType = typeof(CharacterClassManager).GetNestedTypes(all)
                .FirstOrDefault(currentType => currentType.Name.Contains("Init"));
            if (PrivateType == null)
                throw new Exception("State machine type for Init not found.");
            MethodInfo moveNextMethod = PrivateType.GetMethod("MoveNext", all);

            if (moveNextMethod == null)
                throw new Exception("MoveNext method not found in the state machine type.");
            return moveNextMethod;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
    }
