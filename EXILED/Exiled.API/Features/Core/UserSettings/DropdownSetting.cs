// -----------------------------------------------------------------------
// <copyright file="DropdownSetting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.UserSettings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Interfaces;
    using global::UserSettings.ServerSpecific;

    /// <summary>
    /// Represents a dropdown setting.
    /// </summary>
    public class DropdownSetting : SettingBase, IWrapper<SSDropdownSetting>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropdownSetting"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="SettingBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="options"><inheritdoc cref="Options"/></param>
        /// <param name="defaultOptionIndex"><inheritdoc cref="DefaultOptionIndex"/></param>
        /// <param name="dropdownEntryType"><inheritdoc cref="DropdownType"/></param>
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        /// <param name="header"><inheritdoc cref="SettingBase.Header"/></param>
        /// <param name="onChanged"><inheritdoc cref="SettingBase.OnChanged"/></param>
        public DropdownSetting(
            int id,
            string label,
            IEnumerable<string> options,
            int defaultOptionIndex = 0,
            SSDropdownSetting.DropdownEntryType dropdownEntryType = SSDropdownSetting.DropdownEntryType.Regular,
            string hintDescription = null,
            HeaderSetting header = null,
            Action<Player, SettingBase> onChanged = null)
            : base(new SSDropdownSetting(id, label, options.ToArray(), defaultOptionIndex, dropdownEntryType, hintDescription), header, onChanged)
        {
            Base = (SSDropdownSetting)base.Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropdownSetting"/> class.
        /// </summary>
        /// <param name="settingBase">A <see cref="SSDropdownSetting"/> instance.</param>
        internal DropdownSetting(SSDropdownSetting settingBase)
            : base(settingBase)
        {
            Base = settingBase;

            if (OriginalDefinition != null && OriginalDefinition.Is(out DropdownSetting dropdown))
            {
                Options = dropdown.Options;
            }
        }

        /// <inheritdoc/>
        public new SSDropdownSetting Base { get; }

        /// <summary>
        /// Gets or sets a collection of all options in dropdown.
        /// </summary>
        public IEnumerable<string> Options
        {
            get => Base.Options;
            set => Base.Options = value.ToArray();
        }

        /// <summary>
        /// Gets or sets an index of default option.
        /// </summary>
        public int DefaultOptionIndex
        {
            get => Base.DefaultOptionIndex;
            set => Base.DefaultOptionIndex = value;
        }

        /// <summary>
        /// Gets or sets a default option.
        /// </summary>
        public string DefaultOption
        {
            get => Base.Options[DefaultOptionIndex];
            set => DefaultOptionIndex = Array.IndexOf(Base.Options, value);
        }

        /// <summary>
        /// Gets or sets a type of dropdown.
        /// </summary>
        public SSDropdownSetting.DropdownEntryType DropdownType
        {
            get => Base.EntryType;
            set => Base.EntryType = value;
        }

        /// <summary>
        /// Gets or sets an index of selected option.
        /// </summary>
        public int SelectedIndex
        {
            get => Base.SyncSelectionIndexRaw;
            set => Base.SyncSelectionIndexRaw = value;
        }

        /// <summary>
        /// Gets or sets a selected option.
        /// </summary>
        public string SelectedOption
        {
            get => Base.SyncSelectionText;
            set => SelectedIndex = Array.IndexOf(Base.Options, value);
        }

        /// <summary>
        /// Gets a string representation of this <see cref="DropdownSetting"/>.
        /// </summary>
        /// <returns>A string in human-readable format.</returns>
        public override string ToString()
        {
            return base.ToString() + $" ={DefaultOptionIndex}= -{SelectedIndex}- /{string.Join(";", Options)}/";
        }
    }
}