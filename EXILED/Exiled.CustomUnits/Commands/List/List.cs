// -----------------------------------------------------------------------
// <copyright file="List.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomUnits.Commands.List
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The command to list all registered roles.
    /// </summary>
    internal sealed class List : ParentCommand
    {
        /// <inheritdoc/>
        public override string Command { get; } = "list";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = { "l" };

        /// <inheritdoc/>
        public override string Description { get; } = "Gets a list of all currently registered custom roles.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.IsEmpty() && TryGetCommand(Registered.Instance.Command, out ICommand command))
            {
                command.Execute(arguments, sender, out response);
                return true;
            }

            response = "Invalid subcommand! Available: registered";
            return false;
        }
    }
}