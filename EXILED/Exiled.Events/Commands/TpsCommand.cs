// -----------------------------------------------------------------------
// <copyright file="TpsCommand.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands
{
    using System;

    using CommandSystem;
    using Exiled.API.Features;

    /// <summary>
    /// Command for showing current server TPS.
    /// </summary>
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class TpsCommand : ICommand
    {
        /// <inheritdoc />
        public string Command { get; } = "tps";

        /// <inheritdoc />
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc />
        public string Description { get; } = "Shows the current TPS of the server";

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            foreach (string data in ServerStatic.PermissionsHandler._config.GetStringList("AdditionalGroups"))
            {
                Log.Info(data);
            }

            response = "uwu";
            return true;
        }
    }
}