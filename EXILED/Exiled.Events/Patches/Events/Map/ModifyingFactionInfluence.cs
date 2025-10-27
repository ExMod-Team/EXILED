// -----------------------------------------------------------------------
// <copyright file="ModifyingFactionInfluence.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;

    using Handlers;

    using HarmonyLib;

    using MapGeneration.Distributors;
    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FactionInfluenceManager.Set(PlayerRoles.Faction, float)" />.
    /// Adds the <see cref="Map.ModifyingFactionInfluence" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.ModifyingFactionInfluence))]
    [HarmonyPatch(typeof(FactionInfluenceManager), nameof(FactionInfluenceManager.Set))]
    internal static class ModifyingFactionInfluence
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retModLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ModifyingFactionInfluenceEventArgs));

            // ModifyingFactionInfluenceEventArgs ev = new(this, true);
            //
            // Map.OnGeneratorActivated(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // faction
                    new(OpCodes.Ldarg_0),

                    // influence
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ModifyingFactionInfluenceEventArgs ev = new(Team, float, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ModifyingFactionInfluenceEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Map.OnGeneratorActivated(ev)
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnModifyingFactionInfluence))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ModifyingFactionInfluenceEventArgs), nameof(ModifyingFactionInfluenceEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}