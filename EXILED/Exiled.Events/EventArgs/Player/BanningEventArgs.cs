// -----------------------------------------------------------------------
// <copyright file="BanningEventArgs.cs" company="ExMod Team">
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
    /// Contains all information before banning a player from the server.
    /// </summary>
    public class BanningEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BanningEventArgs" /> class.
        /// </summary>
        /// <param name="target">The ban target.</param>
        /// <param name="issuer">The ban issuer.</param>
        /// <param name="commandSender">The ban command sender.</param>
        /// <param name="duration">The ban seconds duration.</param>
        /// <param name="reason">The ban reason.</param>
        /// <param name="fullMessage">The ban full message.</param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public BanningEventArgs(Player target, Player issuer, ICommandSender commandSender, long duration, string reason, string fullMessage, bool isAllowed = true)
        {
            Target = target;
            Player = issuer ?? Server.Host;
            CommandSender = commandSender;
            Reason = reason;
            FullMessage = fullMessage;
            IsAllowed = isAllowed;
            Duration = duration;
        }

        /// <summary>
        /// Gets or sets the ban target.
        /// </summary>
        public Player Target
        {
            get;
            set
            {
                if (value is null || field == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans && field is not null)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name, $" changed the banned player from user {field.Nickname} ({field.UserId}) to {value.Nickname} ({value.UserId})");

                field = value;
            }
        }

        /// <summary>
        /// Gets or sets the ban reason.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets the full ban message.
        /// </summary>
        public string FullMessage => field + Reason;

        /// <summary>
        /// Gets or sets a value indicating whether the target is banned.
        /// </summary>
        public bool IsAllowed
        {
            get;
            set
            {
                if (field == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name, $" {(value ? "allowed" : "denied")} banning user with ID: {Target.UserId}");

                field = value;
            }
        }

        /// <summary>
        /// Gets or sets the ban issuer.
        /// </summary>
        public Player Player
        {
            get;
            set
            {
                if (value is null || field == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans && field is not null)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name, $" changed the ban issuer from user {field.Nickname} ({field.UserId}) to {value.Nickname} ({value.UserId})");

                field = value;
            }
        }

        /// <summary>
        /// Gets the command sender.
        /// </summary>
        public ICommandSender CommandSender { get; }

        /// <summary>
        /// Gets or sets the ban duration.
        /// </summary>
        public long Duration
        {
            get;
            set
            {
                if (field == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name, $" changed Ban duration: {field} to {value} for ID: {Target.UserId}");

                field = value;
            }
        }

        /// <summary>
        /// Logs the ban, anti-backdoor protection from malicious plugins.
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