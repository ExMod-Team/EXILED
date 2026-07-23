// -----------------------------------------------------------------------
// <copyright file="BaseCheckPatch.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Features.MultipleGroups
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CommandSystem;
    using Exiled.API.Features;
    using HarmonyLib;
    using Query;
    using RemoteAdmin;

    /// <summary>
    /// Patches base-game method to implement multiple groups check per user.
    /// </summary>
    [HarmonyPatch]
    internal static class BaseCheckPatch
    {
        private static bool Check(ICommandSender sender, PlayerPermissions[] permissions)
        {
            if (sender is not CommandSender commandSender)
                return false;

            if (commandSender.FullPermissions || sender is ServerConsoleSender || commandSender == Server.Host.Sender)
            {
                return true;
            }

            if (sender is PlayerCommandSender || sender is QueryCommandSender)
            {
                if (!Player.TryGet(sender, out Player player))
                    return false;

                if (!MultiGroup.Permissions.TryGetValue(player.UserId, out string[] data))
                    return false;

                foreach (string groupName in data)
                {
                    UserGroup group = ServerStatic.PermissionsHandler.GetGroup(groupName);
                    if (group == null)
                    {
                        Log.Error($"Invalid group name: {groupName}");
                        continue;
                    }

                    if (PermissionsHandler.IsPermitted(group.Permissions, permissions))
                        return true;
                }
            }

            return false;
        }

        private static bool Check(ICommandSender sender, PlayerPermissions permissions) => Check(sender, new[] { permissions });

        [HarmonyPatch(typeof(Misc), nameof(Misc.CheckPermission), typeof(ICommandSender), typeof(PlayerPermissions))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            yield return new(OpCodes.Ldarg_0);
            yield return new(OpCodes.Ldarg_1);
            yield return new(OpCodes.Call, AccessTools.Method(typeof(BaseCheckPatch), nameof(Check), new[] { typeof(ICommandSender),  typeof(PlayerPermissions) }));
            yield return new(OpCodes.Ret);
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Misc), nameof(Misc.CheckPermission), typeof(ICommandSender), typeof(PlayerPermissions[]))]
        private static IEnumerable<CodeInstruction> TranspilerMore(IEnumerable<CodeInstruction> instructions)
        {
            yield return new(OpCodes.Ldarg_0);
            yield return new(OpCodes.Ldarg_1);
            yield return new(OpCodes.Call, AccessTools.Method(typeof(BaseCheckPatch), nameof(Check), new[] { typeof(ICommandSender),  typeof(PlayerPermissions[]) }));
            yield return new(OpCodes.Ret);
        }
    }
}