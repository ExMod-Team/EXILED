// -----------------------------------------------------------------------
// <copyright file="RoleSyncExtraFunctionality.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.NetworkMessages;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FpcServerPositionDistributor.WriteAll"/> to make the Exiled Fake Role API works for niche cases.
    /// </summary>
    [HarmonyPatch(typeof(FpcServerPositionDistributor), nameof(FpcServerPositionDistributor.WriteAll))]
    internal class RoleSyncExtraFunctionality
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label eventLabel = generator.DefineLabel();

            int offset = 3;
            int index = newInstructions.FindIndex(x => x.Calls(PropertyGetter(typeof(NetworkBehaviour), nameof(NetworkBehaviour.netId)))) + offset;

            Label oldTarget = (Label)newInstructions[index].operand;
            newInstructions[index].operand = eventLabel;

            offset = 3;
            index = newInstructions.FindIndex(x => x.opcode == OpCodes.Isinst && x.OperandIs(typeof(IFpcRole))) + offset;

            newInstructions[index].operand = eventLabel;

            offset = 0;
            index = newInstructions.FindLastIndex(x => x.labels.Contains(oldTarget)) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new(OpCodes.Br_S, oldTarget),

                new CodeInstruction(OpCodes.Ldloc_S, 5).WithLabels(eventLabel),
                new CodeInstruction(OpCodes.Ldarg_0),

                new(OpCodes.Call, Method(typeof(RoleSyncExtraFunctionality), nameof(HandleEvent))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void HandleEvent(ReferenceHub target, ReferenceHub viewer)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();

            RoleTypeId role = target.GetRoleId();
            RoleTypeId fakeRole = Handlers.Internal.Round.OnRoleSyncEvent(target, viewer, role, writer);

            // largely copy pasted from FpcServerPositionDistributor.SendRole
            if (target.roleManager.PreviouslySentRole.TryGetValue(viewer.netId, out role) && role == fakeRole)
                return;

            bool fakeOverwatch = FpcServerPositionDistributor.IsDistributionActive(fakeRole);
            viewer.connectionToClient.Send(new RoleSyncInfo(target, fakeOverwatch ? RoleTypeId.Overwatch : fakeRole, viewer, writer));
            target.roleManager.PreviouslySentRole[viewer.netId] = fakeRole;
        }
    }
}