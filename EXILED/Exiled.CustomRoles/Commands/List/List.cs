// -----------------------------------------------------------------------
// <copyright file="List.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands.List
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The command to list all registered roles.
    /// </summary>
    internal sealed class List : ParentCommand
    {
        private List()
        {
            LoadGeneratedCommands();
        }

        /// <summary>
        /// Gets the <see cref="List"/> command instance.
        /// </summary>
        public static List Instance { get; } = new();

        /// <inheritdoc/>
        public override string Command { get; } = "list";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = { "l" };

        /// <inheritdoc/>
        public override string Description { get; } = "Gets a list of all currently registered custom roles.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Registered.Instance);
            RegisterCommand(new Abilities());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.IsEmpty() && TryGetCommand(Registered.Instance.Command, out ICommand command))
            {
                command.Execute(arguments, sender, out response);
                response += $"\nTo view all abilities registered use command: {string.Join(" ", arguments.Array)} abilities";
                return true;
            }

            response = "Invalid subcommand! Available: registered, abilities";
            return false;
        }
    }
}