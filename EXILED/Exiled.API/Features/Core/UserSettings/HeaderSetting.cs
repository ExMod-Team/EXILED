// -----------------------------------------------------------------------
// <copyright file="HeaderSetting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.UserSettings
{
    using Exiled.API.Interfaces;
    using global::UserSettings.ServerSpecific;

    /// <summary>
    /// Represents a header setting.
    /// </summary>
    public class HeaderSetting : SettingBase, IWrapper<SSGroupHeader>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderSetting"/> class.
        /// </summary>
        /// <param name="name"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        /// <param name="paddling"><inheritdoc cref="ReducedPaddling"/></param>
        public HeaderSetting(string name, string hintDescription = "", bool paddling = false)
            : this(new SSGroupHeader(name, paddling, hintDescription))
        {
            Base = (SSGroupHeader)base.Base;

            Base.SetId(null, name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderSetting"/> class.
        /// </summary>
        /// <param name="settingBase">A <see cref="SSGroupHeader"/> instance.</param>
        internal HeaderSetting(SSGroupHeader settingBase)
            : base(settingBase)
        {
            Base = settingBase;
            Base.SetId(null, settingBase.Label);
        }

        /// <inheritdoc/>
        public new SSGroupHeader Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to reduce padding.
        /// </summary>
        public bool ReducedPaddling
        {
            get => Base.ReducedPadding;
            set => Base.ReducedPadding = value;
        }

        /// <summary>
        /// Returns a representation of this <see cref="HeaderSetting"/>.
        /// </summary>
        /// <returns>A string in human-readable format.</returns>
        public override string ToString()
        {
            return base.ToString() + $" /{ReducedPaddling}/";
        }
    }
}