// -----------------------------------------------------------------------
// <copyright file="ExecutingCommandEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System;
    using System.Collections.Generic;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before command being executed.
    /// </summary>
    public class ExecutingCommandEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutingCommandEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="command"><inheritdoc cref="Command"/></param>
        /// <param name="arguments"><inheritdoc cref="Arguments"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ExecutingCommandEventArgs(Player player, ICommand command, ArraySegment<string> arguments, bool isAllowed = true)
        {
            Player = player;
            Command = command;
            Arguments = arguments;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets a command that is going to be executed.
        /// </summary>
        /// <remarks>Can be <see langword="null"/>.</remarks>
        public ICommand Command { get; internal set; }

        /// <summary>
        /// Gets a collection of arguments for this command.
        /// </summary>
        public IEnumerable<string> Arguments { get; }
    }
}