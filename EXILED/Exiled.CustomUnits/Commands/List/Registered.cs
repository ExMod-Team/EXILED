// -----------------------------------------------------------------------
// <copyright file="Registered.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomUnits.Commands.List
{
    using System;
    using System.Linq;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features.Pools;
    using Exiled.CustomUnits.API.Features;
    using Exiled.Permissions.Extensions;

    /// <inheritdoc />
    [CommandHandler(typeof(List))]
    internal sealed class Registered : ICommand
    {
        private Registered()
        {
        }

        /// <summary>
        /// Gets the instance of the <see cref="Registered"/>.
        /// </summary>
        public static Registered Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "registered";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "r" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets a list of registered custom roles.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customunits.list.registered"))
            {
                response = "Permission Denied, required: customunits.list.registered";
                return false;
            }

            if (CustomUnit.Registered.Count == 0)
            {
                response = "There are no custom units currently on this server.";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();

            builder.Append("[Registered custom units (").Append(CustomUnit.Registered.Count).AppendLine(")]");

            foreach (CustomUnit unit in CustomUnit.Registered.OrderBy(r => r.Id))
                builder.Append('[').Append(unit.Id).Append(". ").Append(unit.Name).AppendLine("]");

            response = StringBuilderPool.Pool.ToStringReturn(builder);
            return true;
        }
    }
}