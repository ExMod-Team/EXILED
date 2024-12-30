// -----------------------------------------------------------------------
// <copyright file="RoundEnd.cs" company="ExMod Team">
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

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Server;

    using HarmonyLib;
    using PlayerRoles;

    using static HarmonyLib.AccessTools;
    using static RoundSummary;

    /// <summary>
    /// Patches <see cref="RoundSummary._ProcessServerSideCode()" />.
    /// Adds the <see cref="Handlers.Server.EndingRound" /> and <see cref="Handlers.Server.RoundEnded" /> event.
    /// Adds the <see cref="Round.IgnoredPlayers" /> Propperty.
    /// </summary>
    [HarmonyPatch]
    internal static class RoundEnd
    {
#pragma warning disable SA1600 // Elements should be documented
        public static Type PrivateType { get; internal set; }

        private static MethodInfo TargetMethod()
        {
            PrivateType = typeof(RoundSummary).GetNestedTypes(all)
                .FirstOrDefault(currentType => currentType.Name.Contains("_ProcessServerSideCode"));
            if (PrivateType == null)
                throw new Exception("State machine type for _ProcessServerSideCode not found.");
            MethodInfo moveNextMethod = PrivateType.GetMethod("MoveNext", all);

            if (moveNextMethod == null)
                throw new Exception("MoveNext method not found in the state machine type.");
            return moveNextMethod;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const string LeadingTeam = "<leadingTeam>5__9";
            const string NewList = "<newList>5__3";

            int offset = -1;
            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(PlayerRolesUtils), nameof(PlayerRolesUtils.GetTeam), new Type[] { typeof(ReferenceHub), }))) + offset;

            Label jmp = generator.DefineLabel();

            // if (Round.IgnoredPlayers.Contains(referencehub)
            //  goto jmp;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Call, PropertyGetter(typeof(Round), nameof(Round.IgnoredPlayers))),
                    new(OpCodes.Ldloc_S, 13),
                    new(OpCodes.Call, Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Contains))),
                    new(OpCodes.Brtrue_S, jmp),
                });

            offset = 4;
            index = newInstructions.FindIndex(x => x.Calls(Method(typeof(PlayerRolesUtils), nameof(PlayerRolesUtils.GetTeam), new Type[] { typeof(ReferenceHub), }))) + offset;

            newInstructions[index].labels.Add(jmp);

            // Get the whole leadingteam logic
            offset = -20;
            index = newInstructions.FindIndex(x => x.StoresField(Field(PrivateType, LeadingTeam))) + offset;
            int offset2 = 1;
            int index2 = newInstructions.FindLastIndex(x => x.StoresField(Field(PrivateType, LeadingTeam))) + offset2;
            List<CodeInstruction> leadingTeamLogic = newInstructions.GetRange(index, index2 - index);
            List<Label> moveLabel = newInstructions[index2].ExtractLabels();
            newInstructions.RemoveRange(index, index2 - index);

            // put the LeadingTeam logic before the event
            offset = -1;
            index = newInstructions.FindIndex(x => x.LoadsField(Field(typeof(RoundSummary), nameof(RoundSummary._roundEnded)))) + offset;
            newInstructions.InsertRange(index, leadingTeamLogic);

            // recorect the index because of the LeadingTeamLogic that got moved
            offset = -1;
            index = newInstructions.FindIndex(x => x.LoadsField(Field(typeof(RoundSummary), nameof(RoundSummary._roundEnded)))) + offset;
            LocalBuilder evEndingRound = generator.DeclareLocal(typeof(EndingRoundEventArgs));

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.LeadingTeam
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(moveLabel),
                    new(OpCodes.Ldfld, Field(PrivateType, LeadingTeam)),

                    // this.newList
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(PrivateType, NewList)),

                    // isForceEnd
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldfld, Field(typeof(RoundSummary), nameof(RoundSummary._roundEnded))),

                    // baseGameConditionsSatisfied
                    new(OpCodes.Ldloc_S, 6),

                    // EndingRoundEventArgs evEndingRound = new(RoundSummary.SumInfo_ClassList, bool, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EndingRoundEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Server.OnEndingRound(evEndingRound);
                    new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnEndingRound))),
                    new(OpCodes.Stloc_S, evEndingRound.LocalIndex),

                    // this._roundEnded = ev.IsForceEnded
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldloc_S, evEndingRound.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EndingRoundEventArgs), nameof(EndingRoundEventArgs.IsForceEnded))),
                    new(OpCodes.Stfld, Field(typeof(RoundSummary), nameof(RoundSummary._roundEnded))),

                    // flag = ev.IsAllowed
                    new(OpCodes.Ldloc_S, evEndingRound.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EndingRoundEventArgs), nameof(EndingRoundEventArgs.IsAllowed))),
                    new(OpCodes.Stloc_S, 6),

                    // this.LeadingTeam = ev.LeadingTeam
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, evEndingRound.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EndingRoundEventArgs), nameof(EndingRoundEventArgs.LeadingTeam))),
                    new(OpCodes.Stfld, Field(PrivateType, LeadingTeam)),
                });

            // Round.LastClassList = this.newList;
            offset = 1;
            index = newInstructions.FindIndex(x => x.opcode == OpCodes.Stfld && x.operand == (object)Field(typeof(SumInfo_ClassList), nameof(SumInfo_ClassList.warhead_kills))) + offset;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(PrivateType, NewList)),
                new(OpCodes.Call, PropertySetter(typeof(Round), nameof(Round.LastClassList))),
            });

            Label skip = generator.DefineLabel();

            offset = 7;
            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldstr && x.operand == (object)"auto_round_restart_time") + offset;

            LocalBuilder timeToRestartIndex = (LocalBuilder)newInstructions[index - 1].operand;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.leadingTeam
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(PrivateType, LeadingTeam)),

                    // this.newList
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(PrivateType, NewList)),

                    // timeToRestart
                    new(OpCodes.Ldloc_S, timeToRestartIndex),

                    // RoundEndedEventArgs evEndedRound = new(RoundSummary.LeadingTeam, RoundSummary.SumInfo_ClassList, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RoundEndedEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // Handlers.Server.OnRoundEnded(evEndedRound);
                    new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnRoundEnded))),

                    // timeToRestart = ev.TimeToRestart
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoundEndedEventArgs), nameof(RoundEndedEventArgs.TimeToRestart))),
                    new(OpCodes.Stloc_S, timeToRestartIndex),

                    // if (!ShowRoundSummary) goto skip;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoundEndedEventArgs), nameof(RoundEndedEventArgs.ShowRoundSummary))),
                    new(OpCodes.Brfalse_S, skip),
                });

            offset = 1;
            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Call && x.operand == (object)Method(typeof(RoundSummary), nameof(RoundSummary.RpcShowRoundSummary))) + offset;
            newInstructions[index].labels.Add(skip);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}