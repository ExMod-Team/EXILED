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

    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Server;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.Init()" />.
    /// Adds the <see cref="Handlers.Server.RoundStarting" /> event.
    /// Fixes the event for LabAPI too.
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

            const string TimeLeft = "<timeLeft>5__3";
            const string OriginalTimeLeft = "<originalTimeLeft>5__2";
            const string TopPlayer = "<topPlayers>5__4";

            LocalBuilder ev = generator.DeclareLocal(typeof(RoundStartingEventArgs));
            Label continueLabel = generator.DefineLabel();
            Label checkSkip = generator.DefineLabel();

            int index = newInstructions.FindLastIndex(x => x.Calls(Method(typeof(CharacterClassManager), nameof(CharacterClassManager.ForceRoundStart))));

            // we will call ForceRoundStart ourselves to check its result
            newInstructions.RemoveRange(index, 2);

            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(index, new[]
            {
                // this.TimeLeft
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(PrivateType, TimeLeft)),

                // this.OriginalTimeLeft
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(PrivateType, OriginalTimeLeft)),

                // this.TopPlayer
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(PrivateType, TopPlayer)),

                // playerCount
                new(OpCodes.Ldloc_2),

                // RoundStartingEventArgs ev = new(short, short, int, int)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RoundStartingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Server.OnRoundStarting(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnRoundStarting))),

                // this.TimeLeft = ev.TimeLeft
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(RoundStartingEventArgs), nameof(RoundStartingEventArgs.TimeLeft))),
                new(OpCodes.Stfld, Field(PrivateType, TimeLeft)),

                // this.OriginalTimeLeft = ev.OriginalTimeLeft
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(RoundStartingEventArgs), nameof(RoundStartingEventArgs.OriginalTimeLeft))),
                new(OpCodes.Stfld, Field(PrivateType, OriginalTimeLeft)),

                // this.TopPlayer = ev.TopPlayer
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(RoundStartingEventArgs), nameof(RoundStartingEventArgs.TopPlayer))),
                new(OpCodes.Stfld, Field(PrivateType, TopPlayer)),

                // if (!ev.IsAllowed)
                //    goto checkSkip; (checkSkip is where we see if we need to change timeLeft so the round doesn't start from stupid NW stuff)
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(RoundStartingEventArgs), nameof(RoundStartingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, checkSkip),

                // try start the round because it was allowed.
                new(OpCodes.Call, Method(typeof(CharacterClassManager), nameof(CharacterClassManager.ForceRoundStart))),

                // round has started, we don't care about timeLeft because of the Timer check later in the method.
                new(OpCodes.Brtrue_S, continueLabel),

                // if (timeLeft != -1)
                //     goto continueLabel; (timeLeft isn't -1, so we can just let the method execute)
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(checkSkip),
                new(OpCodes.Ldfld, Field(PrivateType, TimeLeft)),

                new(OpCodes.Ldc_I4_M1),
                new(OpCodes.Bne_Un_S, continueLabel),

                // timeLeft = 0; (so we delay round start 1s)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Stfld, Field(PrivateType, TimeLeft)),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
    }
