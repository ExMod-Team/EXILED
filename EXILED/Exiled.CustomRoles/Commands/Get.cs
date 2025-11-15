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
        public string Command { get; } = "get";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; } = "Gets the specified player(s)' current custom role(s).";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customroles.get"))
            {
                response = "Permission Denied, required: customroles.get";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                if (arguments.Count > 0 || !Player.TryGet(sender, out Player player))
                {
                    response = $"Player not found: {arguments.ElementAtOrDefault(0)}";
                    return false;
                }

                players.AddItem(player);
            }

            StringBuilder builder = StringBuilderPool.Pool.Get();

            builder.AppendLine("===== Custom Roles =====");

            foreach (Player target in players)
            {
                ReadOnlyCollection<CustomRole> role = target.GetCustomRoles();
                if (role.IsEmpty())
                {
                    builder.AppendLine($"{target.DisplayNickname.PadRight(30)} | None");
                }
                else
                {
                    builder.AppendLine($"{target.DisplayNickname.PadRight(30)} ({target.Id}) | {string.Join("-", role.ToString())}]");
                }
            }

            builder.AppendLine("========================");

            response = StringBuilderPool.Pool.ToStringReturn(builder);
            return true;
        }
    }
}