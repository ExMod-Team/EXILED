// -----------------------------------------------------------------------
// <copyright file="PlayerCommands.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Features;

    using RemoteAdmin;

    /// <summary>
    /// Player command related events.
    /// </summary>
    public static class PlayerCommands
    {
        /// <summary>
        /// Invoked before a Remote Admin command is executed.
        /// </summary>
        public static Event<ExecutingRemoteAdminCommandEventArgs> ExecutingRemoteAdminCommand { get; set; } = new();

        /// <summary>
        /// Invoked before a Client command is executed.
        /// </summary>
        public static Event<ExecutingClientCommandEventArgs> ExecutingClientCommand { get; set; } = new();

        /// <summary>
        /// Called before a Remote Admin command is executed.
        /// </summary>
        /// <param name="ev">The <see cref="ExecutingRemoteAdminCommandEventArgs"/> instance.</param>
        public static void OnExecutingRemoteAdminCommand(ExecutingRemoteAdminCommandEventArgs ev) => ExecutingRemoteAdminCommand.InvokeSafely(ev);

        /// <summary>
        /// Called before a Client command is executed.
        /// </summary>
        /// <param name="ev">The <see cref="ExecutingClientCommandEventArgs"/> instance.</param>
        public static void OnExecutingClientCommand(ExecutingClientCommandEventArgs ev) => ExecutingClientCommand.InvokeSafely(ev);
    }
}