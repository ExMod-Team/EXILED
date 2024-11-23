// -----------------------------------------------------------------------
// <copyright file="Strangling.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp3114;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp3114Strangle.ProcessAttackRequest"/> to add the <see cref="Scp3114.Strangling"/> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Strangling))]
    [HarmonyPatch(typeof(Scp3114Strangle), nameof(Scp3114Strangle.ProcessAttackRequest))]
    internal class Strangling
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instruction, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instruction);

            Label retLabel = newInstructions[newInstructions.Count - 2].labels[0];
            Label jumpLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(StranglingEventArgs));

            const int offset = -1;
            int index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(ReferenceHub), nameof(ReferenceHub.playerEffectsController)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this.Owner
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Strangle), nameof(Scp3114Strangle.Owner))),

                // target
                new(OpCodes.Ldloc_0),

                // StranglingEventArgs ev = new(ReferenceHub, ReferenceHub);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StranglingEventArgs))[0]),
                new(OpCodes.Dup),

                // Scp3114.OnStrangling(ev);
                new(OpCodes.Call, Method(typeof(Scp3114), nameof(Scp3114.OnStrangling))),

                // if (ev.IsAllowed) goto jumpLabel;
                new(OpCodes.Callvirt, PropertyGetter(typeof(StranglingEventArgs), nameof(StranglingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, jumpLabel),

                // return strangleTarget = null;
                new(OpCodes.Ldloca_S, 4),
                new(OpCodes.Initobj, typeof(Scp3114Strangle.StrangleTarget?)),
                new(OpCodes.Ldloc_S, 4),
                new(OpCodes.Stloc_S, 4),
                new(OpCodes.Leave, retLabel),

                // jump:
                new CodeInstruction(OpCodes.Nop).WithLabels(jumpLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}