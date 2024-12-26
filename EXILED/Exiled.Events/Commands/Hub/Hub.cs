// -----------------------------------------------------------------------
// <copyright file="Hub.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Hub
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The EXILED hub command.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Hub : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hub"/> class.
        /// </summary>
        public Hub()
        {
            LoadGeneratedCommands();
        }

        /// <inheritdoc/>
        public override string Command { get; } = "hub";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public override string Description { get; } = "The EXILED hub command.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Install.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please, specify a valid subcommand! Available ones: install";
            return false;
        }
    }
}