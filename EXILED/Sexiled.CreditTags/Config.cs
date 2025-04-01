// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.CreditTags
{
    using System.ComponentModel;

    using Sexiled.API.Interfaces;
    using Sexiled.CreditTags.Enums;

    /// <inheritdoc />
    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        public bool Debug { get; set; }

        [Description("Info side - Badge, CustomPlayerInfo, FirstAvailable")]
        public InfoSide Mode { get; private set; } = InfoSide.FirstAvailable;

        [Description("Overrides badge if exists")]
        public bool BadgeOverride { get; private set; } = false;

        [Description("Overrides Custom Player Info if exists")]
        public bool CustomPlayerInfoOverride { get; private set; } = false;

        [Description("Whether the plugin should ignore a player's DNT flag. By default (false), players with DNT flag will not be checked for credit tags.")]
        public bool IgnoreDntFlag { get; private set; } = false;
    }
}