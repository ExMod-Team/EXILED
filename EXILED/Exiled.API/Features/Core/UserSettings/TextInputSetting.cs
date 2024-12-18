// -----------------------------------------------------------------------
// <copyright file="TextInputSetting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.UserSettings
{
    using System;

    using Exiled.API.Interfaces;
    using global::UserSettings.ServerSpecific;
    using TMPro;

    /// <summary>
    /// Represents a text input setting.
    /// </summary>
    public class TextInputSetting : SettingBase, IWrapper<SSTextArea>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextInputSetting"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="SettingBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="foldoutMode"><inheritdoc cref="FoldoutMode"/></param>
        /// <param name="alignment"><inheritdoc cref="Alignment"/></param>
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        /// <param name="header"><inheritdoc cref="SettingBase.Header"/></param>
        /// <param name="onChanged"><inheritdoc cref="SettingBase.OnChanged"/></param>
        public TextInputSetting(
            int id,
            string label,
            SSTextArea.FoldoutMode foldoutMode = SSTextArea.FoldoutMode.NotCollapsable,
            TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft,
            string hintDescription = null,
            HeaderSetting header = null,
            Action<Player, SettingBase> onChanged = null)
            : base(new SSTextArea(id, label, foldoutMode, hintDescription, alignment), header, onChanged)
        {
            Base = (SSTextArea)base.Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextInputSetting"/> class.
        /// </summary>
        /// <param name="settingBase">A <see cref="SSTextArea"/> instance.</param>
        internal TextInputSetting(SSTextArea settingBase)
            : base(settingBase)
        {
            Base = settingBase;
        }

        /// <inheritdoc/>
        public new SSTextArea Base { get; }

        /// <summary>
        /// Gets or sets the text for the setting.
        /// </summary>
        public new string Label
        {
            get => Base.Label;
            set => Base.SendTextUpdate(value);
        }

        /// <summary>
        /// Gets or sets the foldout mode.
        /// </summary>
        public SSTextArea.FoldoutMode FoldoutMode
        {
            get => Base.Foldout;
            set => Base.Foldout = value;
        }

        /// <summary>
        /// Gets or sets the text alignment options.
        /// </summary>
        public TextAlignmentOptions Alignment
        {
            get => Base.AlignmentOptions;
            set => Base.AlignmentOptions = value;
        }

        /// <summary>
        /// Returns a representation of this <see cref="TextInputSetting"/>.
        /// </summary>
        /// <returns>A string in human-readable format.</returns>
        public override string ToString()
        {
            return base.ToString() + $" /{FoldoutMode}/ *{Alignment}*";
        }
    }
}