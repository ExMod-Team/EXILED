// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events
{
    using System.ComponentModel;

    using API.Interfaces;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        public bool Debug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether events are only patched if they have delegates subscribed to them.
        /// </summary>
        [Description("Indicates whether events are patched only if they have delegates subscribed to them")]
        public bool UseDynamicPatching { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the name tracking (invisible EXILED version string added to the end of the server name) is enabled or not.
        /// </summary>
        [Description("Indicates whether the name tracking (invisible EXILED version string added to the end of the server name) is enabled or not")]
        public bool IsNameTrackingEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether keycard throw can affect basic doors.
        /// </summary>
        [Description("Indicates whether thrown keycards can affect doors that don't require any permissions")]
        public bool CanKeycardThrowAffectDoors { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether configs has to be reloaded every time a round restarts.
        /// </summary>
        [Description("Indicates whether configs have to be reloaded every round restart")]
        public bool ShouldReloadConfigsAtRoundRestart { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether translations has to be reloaded every time a round restarts.
        /// </summary>
        [Description("Indicates whether translations has to be reloaded every round restart")]
        public bool ShouldReloadTranslationsAtRoundRestart { get; set; } = false;

        /// <summary>
        /// Gets a value indicating whether bans should be logged.
        /// </summary>
        [Description("Indicates whether bans should be logged")]
        public bool ShouldLogBans { get; private set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to log RA commands.
        /// </summary>
        [Description("Whether to log RA commands.")]
        public bool LogRaCommands { get; set; } = true;
    }
}
