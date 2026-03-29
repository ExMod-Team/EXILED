// -----------------------------------------------------------------------
// <copyright file="PluginManager.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.PluginManager
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The plugin manager.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PluginManager : ParentCommand
    {
        /// <inheritdoc/>
        public override string Command { get; } = "pluginmanager";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = new[] { "plymanager", "plmanager", "pmanager", "plym" };

        /// <inheritdoc/>
        public override string Description { get; } = "Manage plugin. Enable, disable and show plugins.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please, specify a valid subcommand! Available ones: enable, disable, show, patches";
            return false;
        }
    }
}
