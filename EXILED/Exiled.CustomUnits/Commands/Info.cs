// -----------------------------------------------------------------------
// <copyright file="Info.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomUnits.Commands
{
    using System;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features.Pools;
    using Exiled.CustomUnits.API.Features;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The command to view info about a specific role.
    /// </summary>
    [CommandHandler(typeof(Parent))]
    internal sealed class Info : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "info";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "i" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets more information about the specified custom role.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customunits.info"))
            {
                response = "Permission Denied, required: customunits.info";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "info [Custom unit name/Custom unit ID]";
                return false;
            }

            if ((!(uint.TryParse(arguments.At(0), out uint id) && CustomUnit.TryGet(id, out CustomUnit? unit)) && !CustomUnit.TryGet(arguments.At(0), out unit)) || unit is null)
            {
                response = $"{arguments.At(0)} is not a valid custom unit.";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();

            builder.Append("<color=#E6AC00>-</color> <color=#00D639>").Append(unit.Name)
                .Append("</color> <color=#05C4E8>(").Append(unit.Id).Append(")</color>")
                .Append("- ").AppendLine(unit.Description).AppendLine();

            response = StringBuilderPool.Pool.ToStringReturn(builder);
            return true;
        }
    }
}