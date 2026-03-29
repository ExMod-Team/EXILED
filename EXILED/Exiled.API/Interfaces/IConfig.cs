// -----------------------------------------------------------------------
// <copyright file="IConfig.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using System.ComponentModel;

    /// <summary>
    /// Defines the contract for basic config features.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled.
        /// </summary>
        [Description("Whether this plugin is enabled.")]
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether debug messages should be displayed in the console.
        /// </summary>
        [Description("Whether debug messages should be shown in the console.")]
        bool Debug { get; set; }
    }
}