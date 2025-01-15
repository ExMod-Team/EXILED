// -----------------------------------------------------------------------
// <copyright file="SendingValidCommandEventArgs.cs" company="ExMod Team">
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
    /// Contains all information before a player sends the command.
    /// </summary>
    public class SendingValidCommandEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendingValidCommandEventArgs" /> class.
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
        public SendingValidCommandEventArgs(Player player, ICommand command, CommandType commandType, string query, string response)
        {
            Player = player;
            Command = command;
            Type = commandType;
            Query = query;
            Response = response;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player can send the command.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets or sets the response of the command. If this value is null, the response will stay unchanged.
        /// </summary>
        public string Response { get; set; }

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
