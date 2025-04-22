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
    using CommandSystem.Commands.RemoteAdmin;
    using Exiled.API.Features;

    /// <summary>
    /// The EXILED hub command.
    /// </summary>
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
            if (sender is not ServerConsoleSender || Environment.StackTrace.Contains(nameof(RconCommand)) || Environment.StackTrace.Contains($"{nameof(Server)}.{nameof(Server.ExecuteCommand)}"))
            {
                response = "EXILED hub can only be used directly through the game console!";
                return false;
            }

            response = "Please, specify a valid subcommand! Available ones: install";
            return false;
        }
    }
}