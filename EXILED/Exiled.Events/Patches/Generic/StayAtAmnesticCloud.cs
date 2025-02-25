// -----------------------------------------------------------------------
// <copyright file="StayAtAmnesticCloud.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp939;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;
    using Scp939Role = API.Features.Roles.Scp939Role;

    /// <summary>
    /// Patches <see cref="PlayerRoles.PlayableScps.Scp939.Scp939AmnesticCloudInstance.OnStay(ReferenceHub)"/>.
    /// <see cref="Config.TutorialAffectedByScp939AmnesticCloud"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp939AmnesticCloudInstance), nameof(Scp939AmnesticCloudInstance.OnStay))]
    internal static class StayAtAmnesticCloud
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            Label returnLabel = generator.DefineLabel();

            // Second check pointer
            // We use it to pass execution
            // to the second check if the first check fails,
            // otherwise the second check won't be executed
            Label secondCheckPointer = generator.DefineLabel();

            newInstructions[0].WithLabels(continueLabel);

            // if (referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial && ExiledEvents.Instance.Config.TutorialAffectedByScp939AmnesticCloud
            // || Scp939Role.TurnedPlayers.Contains(Player.Get(referenceHub)))
            //      return;
            newInstructions.InsertRange(
                0,
                new[]
                {
                    // if ((referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial &&
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.roleManager))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.CurrentRole))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                    new(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                    new(OpCodes.Bne_Un_S, secondCheckPointer),

                    // ExiledEvents.Instance.Config.TutorialAffectedByScp939AmnesticCloud)
                    new(OpCodes.Call, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Plugin<Config>), nameof(Plugin<Config>.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.TutorialAffectedByScp939AmnesticCloud))),
                    new(OpCodes.Brtrue_S, returnLabel),

                    // || Scp939Role.TurnedPlayers.Contains(Player.Get(referenceHub))
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp939Role), nameof(Scp939Role.TurnedPlayers))).WithLabels(secondCheckPointer),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // return;
                    new CodeInstruction(OpCodes.Ret).WithLabels(returnLabel),
                });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}