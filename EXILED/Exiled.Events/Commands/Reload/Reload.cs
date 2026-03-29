// -----------------------------------------------------------------------
// <copyright file="Reload.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The reload command.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Reload : ParentCommand
    {
        /// <inheritdoc/>
        public override string Command { get; } = "reload";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = new[] { "rld" };

        /// <inheritdoc/>
        public override string Description { get; } = "Reload plugins, configs, gameplay configs, remote admin configs, translations, permissions or all of them.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please, specify a valid subcommand! Available ones: all, plugins, gameplay, configs, remoteadmin, translations, permissions";
            return false;
        }
    }
}