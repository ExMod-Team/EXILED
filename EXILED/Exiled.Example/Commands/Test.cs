// -----------------------------------------------------------------------
// <copyright file="Test.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Commands
{
    using System;

    using CommandSystem;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// This is an example of how commands should be made.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Test : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "test";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "t" };

        /// <inheritdoc/>
        public string Description { get; } = "A simple test command.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!Enum.TryParse(arguments.At(0), out FirearmType itemType))
            {
                response = "notitem type.";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float pitch))
            {
                response = "not index.";
                return false;
            }

            if (!byte.TryParse(arguments.At(2), out byte clipIndex))
            {
                response = "not index.";
                return false;
            }

            player.PlayGunSound(itemType, pitch, clipIndex);

            response = $"{player.Nickname} sent the command!";

            // Return true if the command was executed successfully; otherwise, false.
            return true;
        }
    }
}