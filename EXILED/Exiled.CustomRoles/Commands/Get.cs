// -----------------------------------------------------------------------
// <copyright file="Get.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands
{
    using System;
    using System.Linq;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.CustomRoles.API;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;

    /// <summary>
    /// The command to get player's current custom role.
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
        public string Description { get; } = "Gets the specified player's current custom role.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("customroles.get"))
                {
                    response = "Permission Denied, required: customroles.get";
                    return false;
                }

                if (sender is not PlayerCommandSender playerSender)
                {
                    response = "Command can be used in game only.";
                    return false;
                }

                Player target;

                if (arguments.Count == 0)
                {
                    target = Player.Get(playerSender.ReferenceHub);
                }
                else
                {
                    string identifier = string.Join(" ", arguments);
                    target = Player.List.FirstOrDefault(p =>
                        p.Nickname.Equals(identifier, StringComparison.OrdinalIgnoreCase));

                    if (target is null)
                    {
                        response = $"No player found with nickname \"{identifier}\".";
                        return false;
                    }
                }

                CustomRole role = target.GetCustomRoles().FirstOrDefault();

                if (role is null)
                {
                    response = $"{target.Nickname} has no active custom role.";
                    return true;
                }

                StringBuilder builder = StringBuilderPool.Pool.Get();
                builder.AppendLine($"{target.Nickname}'s custom role:");
                builder.Append("- ").Append(role.Name).Append(" [").Append(role.Id).Append(']');

                response = StringBuilderPool.Pool.ToStringReturn(builder);
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