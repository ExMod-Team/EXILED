// -----------------------------------------------------------------------
// <copyright file="ExecutedCommandEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System.Collections.Generic;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after command being executed.
    /// </summary>
    public class ExecutedCommandEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutedCommandEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="command"><inheritdoc cref="Command"/></param>
        /// <param name="arguments"><inheritdoc cref="Arguments"/></param>">
        /// <param name="response"><inheritdoc cref="Response"/></param>
        public ExecutedCommandEventArgs(Player player, ICommand command, IEnumerable<string> arguments, string response)
        {
            Player = player;
            Command = command;
            Response = response;
            Arguments = arguments;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the executed command.
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// Gets the arguments of the executed command.
        /// </summary>
        public IEnumerable<string> Arguments { get; }

        /// <summary>
        /// Gets the response of the executed command.
        /// </summary>
        public string Response { get; }
    }
}