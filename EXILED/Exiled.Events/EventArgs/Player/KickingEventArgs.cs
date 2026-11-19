// -----------------------------------------------------------------------
// <copyright file="KickingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System.Reflection;

    using CommandSystem;

    using Exiled.API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information before kicking a player from the server.
    /// </summary>
    public class KickingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KickingEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        /// The target of the kick.
        /// </param>
        /// <param name="issuer">
        /// The issuer of the kick.
        /// </param>
        /// <param name="commandSender">
        /// The <see cref="ICommandSender"/> of the Issuer.
        /// </param>
        /// <param name="reason">
        /// The reason for the kick.
        /// </param>
        /// <param name="prefix">
        /// The text prefixed in every kick message.
        /// </param>
        /// <param name="isAllowed">
        /// Whether the kick is allowed.
        /// </param>
        public KickingEventArgs(Player target, Player issuer, ICommandSender commandSender, string reason, string prefix, bool isAllowed = true)
        {
            Target = target;
            Player = issuer ?? Server.Host;
            CommandSender = commandSender;
            Reason = reason;
            FullMessage = prefix;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the kick target.
        /// </summary>
        public Player Target
        {
            get;
            set
            {
                if (value is null || field == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans && field is not null)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name, $" changed the kicked player from user {field.Nickname} ({field.UserId}) to {value.Nickname} ({value.UserId})");

                field = value;
            }
        }

        /// <summary>
        /// Gets or sets the kick reason.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets the full kick message.
        /// </summary>
        public string FullMessage => field + Reason;

        /// <summary>
        /// Gets or sets a value indicating whether action is taken against the target.
        /// </summary>
        public bool IsAllowed
        {
            get;
            set
            {
                if (field == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name, $" {(value ? "allowed" : "denied")} kicking user with ID: {Target.UserId}");

                field = value;
            }
        }

        /// <summary>
        /// Gets or sets the kick issuer.
        /// </summary>
        public Player Player
        {
            get;
            set
            {
                if (value is null || field == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans && field is not null)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name, $" changed the kick issuer from user {field.Nickname} ({field.UserId}) to {value.Nickname} ({value.UserId})");

                field = value;
            }
        }

        /// <summary>
        /// Gets the command sender.
        /// </summary>
        public ICommandSender CommandSender { get; }

        /// <summary>
        /// Logs the kick, anti-backdoor protection from malicious plugins.
        /// </summary>
        /// <param name="assemblyName">The name of the calling assembly.</param>
        /// <param name="message">The message to be logged.</param>
        private static void LogBanChange(string assemblyName, string message)
        {
            if (assemblyName != "Exiled.Events")
            {
                lock (ServerLogs.LockObject)
                {
                    Log.Warn($"[ANTI-BACKDOOR]: {assemblyName} {message} - {TimeBehaviour.FormatTime("yyyy-MM-dd HH:mm:ss.fff zzz")}");
                }
            }

            ServerLogs._state = ServerLogs.LoggingState.Write;
        }
    }
}