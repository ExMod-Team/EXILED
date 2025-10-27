// -----------------------------------------------------------------------
// <copyright file="CompletingObjective.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Patches.Events.Map;
    using HarmonyLib;
    using PlayerRoles;
    using Respawning;
    using Respawning.Objectives;
    using Respawning.Waves;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FactionObjectiveBase.ServerSendUpdate"/>
    /// to add <see cref="Handlers.Server.CompletingObjective"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.CompletingObjective))]
    [HarmonyPatch(typeof(FactionObjectiveBase), nameof(FactionObjectiveBase.ServerSendUpdate))]
    internal class CompletingObjective
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retModLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // this
                new(OpCodes.Ldarg_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // CompletingObjectiveEventArgs ev = new(this, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(CompletingObjectiveEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Server.OnCompletingObjective(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnCompletingObjective))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ObjectiveCompletingEventArgs), nameof(ObjectiveCompletingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retModLabel),
            });

            newInstructions.InsertRange(newInstructions.Count, new CodeInstruction[]
            {
                // faction
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(retModLabel),
                new (OpCodes.Call, Method(typeof(CompletingObjective), nameof(RevertAllValue))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void RevertAllValue(FactionObjectiveBase factionObjectiveBase)
        {
            if (factionObjectiveBase is IFootprintObjective footprintObjective)
            {
                FactionInfluenceManager.Remove(footprintObjective.ObjectiveFootprint.AchievingPlayer.RoleType.GetFaction(), -footprintObjective.ObjectiveFootprint.InfluenceReward);
                foreach (TimeBasedWave wave in WaveManager.Waves.Cast<TimeBasedWave>())
                {
                    if (wave.TargetFaction == footprintObjective.ObjectiveFootprint.AchievingPlayer.RoleType.GetFaction() && wave.ReceiveObjectiveRewards)
                    {
                        wave.Timer.AddTime(-footprintObjective.ObjectiveFootprint.TimeReward);
                    }
                }
            }
        }
    }
}