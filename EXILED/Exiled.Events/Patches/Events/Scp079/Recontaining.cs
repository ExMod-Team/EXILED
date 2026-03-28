// -----------------------------------------------------------------------
// <copyright file="Recontaining.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp079;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079Recontainer.Recontain" />.
    /// Adds the <see cref="Scp079.Recontaining" /> event.
    /// </summary>
    [EventPatch(typeof(Scp079), nameof(Scp079.Recontaining))]
    [HarmonyPatch(typeof(Scp079Recontainer), nameof(Scp079Recontainer.Recontain))]
    internal class Recontaining
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(RecontainingEventArgs));

            Label returnLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + 1;

            newInstructions.InsertRange(index, new[]
            {
                // RecontainingEventArgs ev = new(this._activatorGlass)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldfld, Field(typeof(Scp079Recontainer), nameof(Scp079Recontainer._activatorGlass))),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RecontainingEventArgs))[0]),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Scp079.OnRecontaining(ev)
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnRecontaining))),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(RecontainingEventArgs), nameof(RecontainingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
