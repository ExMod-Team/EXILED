// -----------------------------------------------------------------------
// <copyright file="KeybindSetting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.UserSettings
{
    using System;

    using Exiled.API.Interfaces;
    using global::UserSettings.ServerSpecific;
    using UnityEngine;

    /// <summary>
    /// Represents a keybind setting.
    /// </summary>
    public class KeybindSetting : SettingBase, IWrapper<SSKeybindSetting>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeybindSetting"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="SettingBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="suggested"><inheritdoc cref="KeyCode"/></param>
        /// <param name="preventInteractionOnGUI"><inheritdoc cref="PreventInteractionOnGUI"/></param>
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        /// <param name="header"><inheritdoc cref="SettingBase.Header"/></param>
        /// <param name="onChanged"><inheritdoc cref="SettingBase.OnChanged"/></param>
        [Obsolete("This method will be removed next major version because of a new feature. Use the constructor with \"allowSpectator\" instead.")]
        public KeybindSetting(int id, string label, KeyCode suggested, bool preventInteractionOnGUI, string hintDescription, HeaderSetting header, Action<Player, SettingBase> onChanged)
            : base(new SSKeybindSetting(id, label, suggested, preventInteractionOnGUI, false, hintDescription), header, onChanged)
        {
            Base = (SSKeybindSetting)base.Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeybindSetting"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="SettingBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="suggested"><inheritdoc cref="KeyCode"/></param>
        /// <param name="preventInteractionOnGUI"><inheritdoc cref="PreventInteractionOnGUI"/></param>
        /// <param name="allowSpectatorTrigger"><inheritdoc cref="AllowSpectatorTrigger"/></param>
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        /// <param name="header"><inheritdoc cref="SettingBase.Header"/></param>
        /// <param name="onChanged"><inheritdoc cref="SettingBase.OnChanged"/></param>
        public KeybindSetting(int id, string label, KeyCode suggested, bool preventInteractionOnGUI = false, bool allowSpectatorTrigger = false, string hintDescription = "", HeaderSetting header = null, Action<Player, SettingBase> onChanged = null)
            : base(new SSKeybindSetting(id, label, suggested, preventInteractionOnGUI, allowSpectatorTrigger, hintDescription), header, onChanged)
        {
            Base = (SSKeybindSetting)base.Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeybindSetting"/> class.
        /// </summary>
        /// <param name="settingBase">A <see cref="SSKeybindSetting"/> instance.</param>
        internal KeybindSetting(SSKeybindSetting settingBase)
            : base(settingBase)
        {
            Base = settingBase;
        }

        /// <inheritdoc/>
        public new SSKeybindSetting Base { get; }

        /// <summary>
        /// Gets a value indicating whether the key is pressed.
        /// </summary>
        public bool IsPressed => Base.SyncIsPressed;

        /// <summary>
        /// Gets or sets a value indicating whether the interaction is prevented while player is in RA, Settings etc.
        /// </summary>
        public bool PreventInteractionOnGUI
        {
            get => Base.PreventInteractionOnGUI;
            set => Base.PreventInteractionOnGUI = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the interaction is prevented in spectator roles.
        /// </summary>
        public bool AllowSpectatorTrigger
        {
            get => Base.AllowSpectatorTrigger;
            set => Base.AllowSpectatorTrigger = value;
        }

        /// <summary>
        /// Gets or sets the assigned key.
        /// </summary>
        public KeyCode KeyCode
        {
            get => Base.SuggestedKey;
            set => Base.SuggestedKey = value;
        }

        /// <summary>
        /// Returns a representation of this <see cref="KeybindSetting"/>.
        /// </summary>
        /// <returns>A string in human-readable format.</returns>
        public override string ToString()
        {
            return base.ToString() + $" /{IsPressed}/ *{KeyCode}* +{PreventInteractionOnGUI}+";
        }
    }
}