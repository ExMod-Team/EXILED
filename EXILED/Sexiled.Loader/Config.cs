// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Loader
{
    using System;
    using System.ComponentModel;
    using System.IO;

    using API.Enums;
    using API.Interfaces;
    using Sexiled.API.Features;
    using YamlDotNet.Core;

    /// <summary>
    /// The configs of the loader.
    /// </summary>
    public sealed class Config : IConfig
    {
        /// <inheritdoc />
        [Description("Whether SEXILED is enabled on this server.")]
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        [Description("Whether debug messages should be shown.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether outdated Sexiled versions should be loaded.
        /// </summary>
        [Description("Indicates whether outdated Sexiled versions should be loaded.")]
        public bool ShouldLoadOutdatedSexiled { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether outdated plugins should be loaded.
        /// </summary>
        [Description("Indicates whether outdated plugins should be loaded.")]
        public bool ShouldLoadOutdatedPlugins { get; set; } = true;

        /// <summary>
        /// Gets or sets the Sexiled directory path from which plugins will be loaded.
        /// </summary>
        [Description("The Sexiled directory path from which plugins will be loaded.")]
        public string SexiledDirectoryPath { get; set; } = Path.Combine(Paths.AppData, "EXILED");

        /// <summary>
        /// Gets or sets the environment type.
        /// </summary>
        [Description("The working environment type (Development, Testing, Production, Ptb, ProductionDebug).")]
        public EnvironmentType Environment { get; set; } = EnvironmentType.Production;

        /// <summary>
        /// Gets or sets the config files distribution type.
        /// </summary>
        [Description("The config files distribution type (Default, Separated)")]
        public ConfigType ConfigType { get; set; } = ConfigType.Default;

        /// <summary>
        /// Gets or sets the quotes wrapper type.
        /// </summary>
        [Description("Indicates in which quoted strings in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal).")]
        public ScalarStyle ScalarStyle { get; set; } = ScalarStyle.SingleQuoted;

        /// <summary>
        /// Gets or sets the quotes wrapper type.
        /// </summary>
        [Description("Indicates in which quoted strings with multiline in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal).")]
        public ScalarStyle MultiLineScalarStyle { get; set; } = ScalarStyle.Literal;

        /// <summary>
        /// Gets or sets a value indicating whether testing releases have to be downloaded.
        /// </summary>
        [Description("Indicates whether testing releases have to be downloaded.")]
        public bool ShouldDownloadTestingReleases { get; set; } = false;

        /// <summary>
        /// Gets or sets which assemblies should be excluded from the update.
        /// </summary>
        [Description("Indicates which assemblies should be excluded from the updater.")]
        public string[] ExcludeAssemblies { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets a value indicating whether Sexiled should auto-update itself as soon as a new release is available.
        /// </summary>
        [Description("Indicates whether Sexiled should auto-update itself as soon as a new release is available.")]
        public bool EnableAutoUpdates { get; set; } = true;
    }
}