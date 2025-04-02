// -----------------------------------------------------------------------
// <copyright file="RoleAppearance.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles;
    using PlayerRoles.SpawnData;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoleSyncInfo.Write(Mirror.NetworkWriter)"/> to implement <see cref="Role.GlobalAppearance"/>, <see cref="Role.TeamAppearances"/> and <see cref="Role.IndividualAppearances"/>.
    /// </summary>
    [HarmonyPatch(typeof(RoleSyncInfo), nameof(RoleSyncInfo.Write))]
    internal class RoleAppearance
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(codeInstructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder role = generator.DeclareLocal(typeof(Role));

            Label skipEvent = generator.DefineLabel();
            Label skip = generator.DefineLabel();
            Label skip2 = generator.DefineLabel();

            int offset = -2;
            int index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(RoleSyncInfo), nameof(RoleSyncInfo._targetRole)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(_targetNetId);
                    // if (player == null)
                    //     skip;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(RoleSyncInfo), nameof(RoleSyncInfo._targetNetId))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(uint) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, skipEvent),

                    // if (_targetNetId == _receiverNetId)
                    //     skip;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(RoleSyncInfo), nameof(RoleSyncInfo._targetNetId))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(RoleSyncInfo), nameof(RoleSyncInfo._receiverNetId))),
                    new(OpCodes.Beq_S, skipEvent),

                    // role = player.Role;
                    // if (role == null)
                    //     skip;
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, role.LocalIndex),
                    new(OpCodes.Brfalse_S, skip),

                    // _targetRole = role.GetAppearanceForPlayer(Player.Get(this._receiverHub));
                    new(OpCodes.Ldarg_0),

                    new(OpCodes.Ldloc_S, role.LocalIndex),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(RoleSyncInfo), nameof(RoleSyncInfo._receiverNetId))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(uint) })),
                    new(OpCodes.Call, Method(typeof(RoleExtensions), nameof(RoleExtensions.GetAppearanceForPlayer))),
                    new(OpCodes.Stfld, Field(typeof(RoleSyncInfo), nameof(RoleSyncInfo._targetRole))),

                    new CodeInstruction(OpCodes.Nop).WithLabels(skip),

                    // SendingRoleEventArgs ev = new(player, _receiverNetId, _targetRole);
                    // Player.OnSendingRole(ev);
                    // roleType = ev.RoleType;
                    new(OpCodes.Ldarg_0),

                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(RoleSyncInfo), nameof(RoleSyncInfo._receiverNetId))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(RoleSyncInfo), nameof(RoleSyncInfo._targetRole))),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingRoleEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSendingRole))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingRoleEventArgs), nameof(SendingRoleEventArgs.RoleType))),
                    new(OpCodes.Stfld, Field(typeof(RoleSyncInfo), nameof(RoleSyncInfo._targetRole))),

                    new CodeInstruction(OpCodes.Nop).WithLabels(skipEvent),
                });

            offset = -2;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(IPublicSpawnDataWriter), nameof(IPublicSpawnDataWriter.WritePublicSpawnData)))) + offset;

            Label cnt = (Label)newInstructions[index - 1].operand;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (role == null)
                    //     skip;
                    new CodeInstruction(OpCodes.Ldloc_S, role.LocalIndex),
                    new(OpCodes.Brfalse_S, skip2),

                    // SendMessage(player.Role, writer, _targetRole)
                    // skip original nw code;
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(RoleSyncInfo), nameof(RoleSyncInfo._targetRole))),

                    new(OpCodes.Call, Method(typeof(RoleAppearance), nameof(RoleAppearance.SendMessage))),
                    new(OpCodes.Br_S, cnt),

                    new CodeInstruction(OpCodes.Nop).WithLabels(skip2),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void SendMessage(Role role, NetworkWriter writer, RoleTypeId appearance)
        {
            Role appearancedRole = role.Type == appearance ? role : Role.Create(appearance.GetRoleBase());

            appearancedRole.SendAppearanceSpawnMessage(writer, role.Base);
        }
    }
}