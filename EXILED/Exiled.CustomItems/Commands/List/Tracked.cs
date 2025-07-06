// -----------------------------------------------------------------------
// <copyright file="Tracked.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands.List
{
    using System;
    using System.Linq;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pools;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

    using RemoteAdmin;

    /// <inheritdoc/>
    internal sealed class Tracked : ICommand
    {
        private Tracked()
        {
        }

        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static Tracked Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "insideinventories";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "ii", "inside", "inv", "inventories" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets a list of custom items actually inside of players' inventories.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.list.insideinventories") && sender is PlayerCommandSender playerSender && !playerSender.FullPermissions)
            {
                response = "Permission Denied, required: customitems.list.insideinventories";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "list insideinventories";
                return false;
            }

            StringBuilder message = StringBuilderPool.Pool.Get();

            int count = 0;

            foreach (Player player in Player.List)
            {
                foreach (Item item in player.Items)
                {
                    if (CustomItem.TryGet(item, out CustomItem? customItem))
                    {
                        message.AppendLine()
                            .Append('[').Append(customItem!.Id).Append(". ").Append(customItem.Name).Append(" (").Append(customItem.Type).Append(')') // this is unreadable, I don't know why you would write it like this but whatever - Bonjemus
                            .Append(" {").Append(item.Serial).AppendLine("}]").AppendLine();
                        count++;
                    }
                }
            }

            if (message.Length == 0)
                message.Append("There are no custom items inside inventories.");
            else
                message.Insert(0, Environment.NewLine + "[Custom items inside inventories (" + count + ")]" + Environment.NewLine);

            response = StringBuilderPool.Pool.ToStringReturn(message);
            return true;
        }
    }
}