// -----------------------------------------------------------------------
// <copyright file="Scp079Recontain.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic.Scp079API
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp079;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079Recontainer.OnServerRoleChanged(ReferenceHub, PlayerRoles.RoleTypeId, PlayerRoles.RoleChangeReason)"/>.
    /// Adds the <see cref="Exiled.Events.Config.RecontainScp079IfNoScpsLeft" /> support.
    /// </summary>
    [HarmonyPatch(typeof(Scp079Recontainer), nameof(Scp079Recontainer.OnServerRoleChanged))]
    internal class Scp079Recontain
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // if (!Events.Instance.Config.ShouldScp079RecontainedWhenNoScps)
                    //     return;
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Config), nameof(Exiled.Events.Config.RecontainScp079IfNoScpsLeft))),
                    new(OpCodes.Brfalse_S, ret),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}