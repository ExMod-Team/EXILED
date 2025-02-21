// -----------------------------------------------------------------------
// <copyright file="Parent.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomUnits.Commands
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The main parent command.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Parent : ParentCommand
    {
        /// <inheritdoc/>
        public override string Command { get; } = "customroles";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = { "cr", "crs" };

        /// <inheritdoc/>
        public override string Description { get; } = string.Empty;

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommand! Available: give, info, list";
            return false;
        }
    }
}