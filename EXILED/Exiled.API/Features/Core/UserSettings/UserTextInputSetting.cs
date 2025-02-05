// -----------------------------------------------------------------------
// <copyright file="UserTextInputSetting.cs" company="ExMod Team">
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
    /// Represents a user text input setting.
    /// </summary>
    public class UserTextInputSetting : SettingBase, IWrapper<SSPlaintextSetting>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserTextInputSetting"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="SettingBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="placeHolder"><inheritdoc cref="PlaceHolder"/></param>
        /// <param name="characterLimit"><inheritdoc cref="CharacterLimit"/></param>
        /// <param name="contentType"><inheritdoc cref="ContentType"/></param>
        /// /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        public UserTextInputSetting(int id, string label, string placeHolder = "", int characterLimit = 64, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard, string hintDescription = null)
            : this(new SSPlaintextSetting(id, label, placeHolder, characterLimit, contentType, hintDescription))
        {
            Base = (SSPlaintextSetting)base.Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTextInputSetting"/> class.
        /// </summary>
        /// <param name="settingBase">A <see cref="SSPlaintextSetting"/> instance.</param>
        internal UserTextInputSetting(SSPlaintextSetting settingBase)
            : base(settingBase)
        {
            Base = settingBase;
        }

        /// <inheritdoc/>
        public new SSPlaintextSetting Base { get; }

        /// <summary>
        /// Gets the value of the text entered by a  client.
        /// </summary>
        public string Text => Base.SyncInputText;

        /// <summary>
        /// Gets or sets a value indicating the placeholder shown within the PlainTextSetting.
        /// </summary>
        public string PlaceHolder
        {
            get => Base.Placeholder;
            set => Base.Placeholder = value;
        }

        /// <summary>
        /// Gets or sets a value indicating the type of content within the PlainTextSetting.
        /// </summary>
        public TMP_InputField.ContentType ContentType
        {
            get => Base.ContentType;
            set => Base.ContentType = value;
        }

        /// <summary>
        /// Gets or sets a value indicating the max number of characters in the PlainTextSetting.
        /// </summary>
        public int CharacterLimit
        {
            get => Base.CharacterLimit;
            set => Base.CharacterLimit = value;
        }

        /// <summary>
        /// Returns a representation of this <see cref="UserTextInputSetting"/>.
        /// </summary>
        /// <returns>A string in human-readable format.</returns>
        public override string ToString()
        {
            return base.ToString() + $" /{Text}/ *{ContentType}* +{CharacterLimit}+";
        }
    }
}