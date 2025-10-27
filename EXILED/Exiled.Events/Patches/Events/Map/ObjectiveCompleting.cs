// -----------------------------------------------------------------------
// <copyright file="ObjectiveCompleting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.API.Extensions;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using Handlers;
    using HarmonyLib;
    using PlayerRoles;
    using Respawning;
    using Respawning.Objectives;
    using Respawning.Waves;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FactionInfluenceManager.Set(PlayerRoles.Faction, float)" />.
    /// Adds the <see cref="Map.ObjectiveCompleting" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.ObjectiveCompleting))]
    [HarmonyPatch(typeof(FactionObjectiveBase), nameof(FactionObjectiveBase.ServerSendUpdate))]
    internal static class ObjectiveCompleting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retModLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ObjectiveCompletingEventArgs));

            // ObjectiveCompletingEventArgs ev = new(this, true);
            //
            // Map.OnObjectiveCompleting(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // this
                new(OpCodes.Ldarg_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // ObjectiveCompletingEventArgs ev = new(FactionObjectiveBase, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ObjectiveCompletingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Map.OnObjectiveCompleting(ev)
                new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnObjectiveCompleting))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ObjectiveCompletingEventArgs), nameof(ObjectiveCompletingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retModLabel),
            });

            newInstructions.InsertRange(newInstructions.Count, new CodeInstruction[]
            {
                // faction
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(retModLabel),
                new (OpCodes.Call, Method(typeof(ObjectiveCompleting), nameof(ObjectiveCompleting.RevertAllValue))),
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