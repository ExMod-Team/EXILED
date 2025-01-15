// -----------------------------------------------------------------------
// <copyright file="SentValidCommandEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using PluginAPI.Enums;
    using RemoteAdmin;

    /// <summary>
    /// Contains all information after a player sends the command.
    /// </summary>
    public class SentValidCommandEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SentValidCommandEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="command">
        /// <inheritdoc cref="Command" />
        /// </param>
        /// <param name="commandType">
        /// <inheritdoc cref="Type" />
        /// </param>
        /// <param name="query">
        /// <inheritdoc cref="Query" />
        /// </param>
        /// <param name="response">
        /// <inheritdoc cref="Response" />
        /// </param>
        /// <param name="result">
        /// <inheritdoc cref="Result" />
        /// </param>
        public SentValidCommandEventArgs(Player player, ICommand command, CommandType commandType, string query, string response, bool result)
        {
            Player = player;
            Command = command;
            Type = commandType;
            Query = query;
            Response = response;
            Result = result;
        }

        /// <summary>
        /// Gets a value indicating whether the command succeeded.
        /// </summary>
        public bool Result { get; }

        /// <summary>
        /// Gets the response of the command.
        /// </summary>
        public string Response { get; }

        /// <summary>
        /// Gets the player who is sending the command.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the command query.
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// Gets the command type.
        /// </summary>
        public CommandType Type { get; }

        /// <summary>
        /// Gets the command interface.
        /// </summary>
        public ICommand Command { get; }
    }
}
