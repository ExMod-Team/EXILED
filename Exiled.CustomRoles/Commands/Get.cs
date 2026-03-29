// -----------------------------------------------------------------------
// <copyright file="Get.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.CustomRoles.API;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Permissions.Extensions;
    using HarmonyLib;

    /// <summary>
    /// The command to get specified player(s) current custom roles.
    /// </summary>
    internal sealed class Get : ICommand
    {
        private Get()
        {
        }

        /// <summary>
        /// Gets the <see cref="Get"/> command instance.
        /// </summary>
        public static Get Instance { get; } = new();

        /// <inheritdoc/>
        public string Command => "get";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Gets the specified player(s)' current custom role(s).";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customroles.get"))
            {
                response = "Permission Denied, required: customroles.get";
                return false;
            }

            List<Player> players = ListPool<Player>.Pool.Get();

            if (arguments.Count > 0)
            {
                string identifier = string.Join(" ", arguments);

                switch (identifier.ToLower())
                {
                    case "*":
                    case "all":
                        players.AddRange(Player.List);
                        break;
                    default:
                        players.AddRange(Player.GetProcessedData(arguments));
                        break;
                }

                if (players.IsEmpty())
                {
                    if (arguments.Count > 0 || !Player.TryGet(sender, out Player player))
                    {
                        response = $"Player not found: {identifier}";
                        return false;
                    }

                    players.Add(player);
                }
            }
            else
            {
                response = "get <Nickname/PlayerID/UserID/all/*>";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Pool.Get();

            builder.AppendLine();
            builder.AppendLine("====================== Custom Roles ======================");

            foreach (Player target in players)
            {
                ReadOnlyCollection<CustomRole> roles = target.GetCustomRoles();
                builder.Append((target.DisplayNickname + (target.HasCustomName ? $" ({target.Nickname})" : string.Empty)).PadRight(30 + (target.HasCustomName ? 23 : 0)));
                builder.Append(" ");
                builder.Append($"({target.Id})".PadRight(5));
                if (roles.IsEmpty())
                {
                    builder.AppendLine(" | No Custom Role");
                }
                else
                {
                    // builder.Append($" | [{string.Join(", ", roles.Select(role => $"<color={role.Role.GetColor().ToHex()}>{role}</color>"))}]");
                    builder.Append(" | [");
                    builder.Append(string.Join(", ", roles.Select(role => $"<color={role.Role.GetColor().ToHex()}>{role}</color>")));
                    builder.AppendLine("]");
                }
            }

            builder.AppendLine("==========================================================");

            ListPool<Player>.Pool.Return(players);

            response = StringBuilderPool.Pool.ToStringReturn(builder);
            return true;
        }
    }
}