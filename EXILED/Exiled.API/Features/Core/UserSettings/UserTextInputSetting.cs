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
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        [Obsolete("Will be removed in Exiled 10 in favour of ctor with more params.")]
        public UserTextInputSetting(int id, string label, string placeHolder, int characterLimit, TMP_InputField.ContentType contentType, string hintDescription)
            : this(new SSPlaintextSetting(id, label, placeHolder, characterLimit, contentType, hintDescription))
        {
            Base = (SSPlaintextSetting)base.Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTextInputSetting"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="SettingBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="placeHolder"><inheritdoc cref="PlaceHolder"/></param>
        /// <param name="characterLimit"><inheritdoc cref="CharacterLimit"/></param>
        /// <param name="contentType"><inheritdoc cref="ContentType"/></param>
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        /// <param name="collectionId"><inheritdoc cref="SettingBase.CollectionId"/></param>
        /// <param name="isServerOnly"><inheritdoc cref="SettingBase.IsServerOnly"/></param>
        /// <param name="header"><inheritdoc cref="SettingBase.Header"/></param>
        /// <param name="onChanged"><inheritdoc cref="SettingBase.OnChanged"/></param>
        public UserTextInputSetting(int id, string label, string placeHolder = "", int characterLimit = 64, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard, string hintDescription = null, byte collectionId = byte.MaxValue, bool isServerOnly = false, HeaderSetting header = null, Action<Player, SettingBase> onChanged = null)
            : base(new SSPlaintextSetting(id, label, placeHolder, characterLimit, contentType, hintDescription, collectionId, isServerOnly), header, onChanged)
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
        /// Requests clients TextInputs to be cleared.
        /// </summary>
        /// <param name="filter">Who to send the request to.</param>
        public void RequestClear(Predicate<Player> filter = null)
        {
            filter ??= _ => true;
            Base.SendClearRequest(hub => filter(Player.Get(hub)));
        }

        /// <summary>
        /// Sends updated values to clients.
        /// </summary>
        /// <param name="placeholder"><inheritdoc cref="PlaceHolder"/></param>
        /// <param name="characterLimit"><inheritdoc cref="CharacterLimit"/></param>
        /// <param name="contentType"><inheritdoc cref="ContentType"/></param>
        /// <param name="overrideValue">If false, sends fake values.</param>
        /// <param name="filter">Who to send the update to.</param>
        public void UpdateSetting(string placeholder, ushort characterLimit, TMP_InputField.ContentType contentType, bool overrideValue = true, Predicate<Player> filter = null)
        {
            filter ??= _ => true;
            Base.SendPlaintextUpdate(placeholder, characterLimit, contentType, overrideValue, hub => filter(Player.Get(hub)));
        }

        /// <summary>
        /// If setting is server only, sends updated values to clients.
        /// </summary>
        /// <param name="value"><inheritdoc cref="Text"/></param>
        /// <param name="overrideValue">If false, sends fake values.</param>
        /// <param name="filter">Who to send the update to.</param>
        public void UpdateValue(string value, bool overrideValue = true, Predicate<Player> filter = null)
        {
            filter ??= _ => true;
            Base.SendValueUpdate(value, overrideValue, hub => filter(Player.Get(hub)));
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