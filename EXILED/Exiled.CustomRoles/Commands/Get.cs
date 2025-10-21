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
    using System.Linq;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.CustomRoles.API;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Permissions.Extensions;

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
            try
            {
                Player? playerSender = sender as Player;
                if (!playerSender.CheckPermission("customroles.get"))
                {
                    response = "Permission Denied, required: customroles.get";
                    return false;
                }

                List<Player> targets = new();

                if (arguments.Count == 0)
                {
                    if (playerSender == null)
                    {
                        response = "You can't check your customroles if you're not connected to the server.";
                        return false;
                    }

                    targets.Add(Player.Get(playerSender.ReferenceHub));
                }
                else
                {
                    string identifier = string.Join(" ", arguments);

                    switch (identifier.ToLower())
                    {
                        case "*":
                        case "all":
                            targets.AddRange(Player.List);
                            break;

                        default:
                            IEnumerable<Player> foundPlayers = Player.GetProcessedData(arguments, 0);
                            if (foundPlayers.IsEmpty())
                            {
                                response = "No players found! Try using player ID or UserID.";
                                return false;
                            }

                            targets.AddRange(foundPlayers);
                            break;
                    }
                }

                StringBuilder builder = StringBuilderPool.Pool.Get();

                foreach (Player target in targets)
                {
                    CustomRole role = target.GetCustomRoles().FirstOrDefault();
                    if (role is null)
                    {
                        builder.AppendLine($"{target.Nickname.PadRight(30)} | None");
                    }
                    else
                    {
                        builder.AppendLine($"{target.Nickname.PadRight(30)} | {role.Name} [{role.Id}]");
                    }
                }

                string formattedList = new StringBuilder()
                    .AppendLine("===== Custom Roles =====")
                    .Append(builder.ToString())
                    .AppendLine("========================")
                    .ToString();

                response = formattedList;
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                response = "An error occurred while executing the command.";
                return false;
            }
        }
    }
}