// -----------------------------------------------------------------------
// <copyright file="EConfig.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Config
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The config command.
    /// </summary>
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class EConfig : ParentCommand
    {
        /// <inheritdoc/>
        public override string Command { get; } = "econfig";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = new[] { "ecfg" };

        /// <inheritdoc/>
        public override string Description { get; } = "Changes from one config distribution to another one.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please, specify a valid subcommand! Available ones: merge, split";
            return false;
        }
    }
}