// -----------------------------------------------------------------------
// <copyright file="ExecutingRemoteAdminCommandEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a Remote Admin command is executed.
    /// </summary>
    public class ExecutingRemoteAdminCommandEventArgs : Exiled.Events.EventArgs.Interfaces.IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutingRemoteAdminCommandEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="command"><inheritdoc cref="Command"/></param>
        /// <param name="arguments"><inheritdoc cref="Arguments"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ExecutingRemoteAdminCommandEventArgs(API.Features.Player player, string command, string[] arguments, bool isAllowed = true)
        {
            Player = player;
            Command = command;
            Arguments = arguments;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Exiled.API.Features.Player Player { get; }

        /// <summary>
        /// Gets the name of the Remote Admin command being executed.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Gets the array of arguments provided with the command.
        /// </summary>
        public string[] Arguments { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the command execution is allowed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}