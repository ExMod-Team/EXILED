// -----------------------------------------------------------------------
// <copyright file="Fix106RegenerationWithScp244.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using CustomPlayerEffects;
    using HarmonyLib;
    using InventorySystem.Items.Usables.Scp244.Hypothermia;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp106;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="Hypothermia.Update()"/> delegate.
    /// Fix than SCP-106 regenerates slower in SCP-244 even if they are in stalk.
    /// Bug reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/367).
    /// </summary>
    [HarmonyPatch(typeof(Hypothermia), nameof(Hypothermia.Update))]
    internal class Fix106RegenerationWithScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder scp106Role = generator.DeclareLocal(typeof(Scp106Role));
            Label continueLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindLastIndex(x => x.operand == (object)Method(typeof(SpawnProtected), nameof(SpawnProtected.CheckPlayer))) + offset;

            Label skip = (Label)newInstructions[index].operand;

            index += offset;

            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(index, new[]
            {
                // Scp106Role scp106Role = base.Hub.roleManager.CurrentRole as Scp106Role;
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(StatusEffectBase), nameof(StatusEffectBase.Hub))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.roleManager))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.CurrentRole))),
                new CodeInstruction(OpCodes.Isinst, typeof(Scp106Role)),
                new CodeInstruction(OpCodes.Stloc_S, scp106Role.LocalIndex),

                // if (scp106Role is null) goto continueLabel
                new CodeInstruction(OpCodes.Ldloc_S, scp106Role.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, continueLabel),

                // if (!scp106Role.IsSubmerged) goto skip
                new CodeInstruction(OpCodes.Ldloc_S, scp106Role.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Scp106Role), nameof(Scp106Role.IsSubmerged))),
                new CodeInstruction(OpCodes.Brtrue_S, skip),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
