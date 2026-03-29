// -----------------------------------------------------------------------
// <copyright file="TwoButtonsSetting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.UserSettings
{
    using System;

    using Exiled.API.Interfaces;
    using global::UserSettings.ServerSpecific;

    /// <summary>
    /// Represents a two-button setting.
    /// </summary>
    public class TwoButtonsSetting : SettingBase, IWrapper<SSTwoButtonsSetting>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TwoButtonsSetting"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="SettingBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="firstOption"><inheritdoc cref="FirstOption"/></param>
        /// <param name="secondOption"><inheritdoc cref="SecondOption"/></param>
        /// <param name="defaultIsSecond"><inheritdoc cref="IsSecondDefault"/></param>
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        /// <param name="header"><inheritdoc cref="SettingBase.Header"/></param>
        /// <param name="onChanged"><inheritdoc cref="SettingBase.OnChanged"/></param>
        [Obsolete("Will be removed in Exiled 10 in favour of ctor with more params.")]
        public TwoButtonsSetting(int id, string label, string firstOption, string secondOption, bool defaultIsSecond, string hintDescription, HeaderSetting header, Action<Player, SettingBase> onChanged)
            : base(new SSTwoButtonsSetting(id, label, firstOption, secondOption, defaultIsSecond, hintDescription), header, onChanged)
        {
            Base = (SSTwoButtonsSetting)base.Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoButtonsSetting"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="SettingBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="firstOption"><inheritdoc cref="FirstOption"/></param>
        /// <param name="secondOption"><inheritdoc cref="SecondOption"/></param>
        /// <param name="defaultIsSecond"><inheritdoc cref="IsSecondDefault"/></param>
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        /// <param name="collectionId"><inheritdoc cref="SettingBase.CollectionId"/></param>
        /// <param name="isServerOnly"><inheritdoc cref="SettingBase.IsServerOnly"/></param>
        /// <param name="header"><inheritdoc cref="SettingBase.Header"/></param>
        /// <param name="onChanged"><inheritdoc cref="SettingBase.OnChanged"/></param>
        public TwoButtonsSetting(int id, string label, string firstOption, string secondOption, bool defaultIsSecond = false, string hintDescription = "", byte collectionId = byte.MaxValue, bool isServerOnly = false, HeaderSetting header = null, Action<Player, SettingBase> onChanged = null)
            : base(new SSTwoButtonsSetting(id, label, firstOption, secondOption, defaultIsSecond, hintDescription, collectionId, isServerOnly), header, onChanged)
        {
            Base = (SSTwoButtonsSetting)base.Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoButtonsSetting"/> class.
        /// </summary>
        /// <param name="settingBase">A <see cref="SSTwoButtonsSetting"/> instance.</param>
        internal TwoButtonsSetting(SSTwoButtonsSetting settingBase)
            : base(settingBase)
        {
            Base = settingBase;

            if (OriginalDefinition != null && OriginalDefinition.Is(out TwoButtonsSetting setting))
            {
                FirstOption = setting.FirstOption;
                SecondOption = setting.SecondOption;
            }
        }

        /// <inheritdoc/>
        public new SSTwoButtonsSetting Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the second option is chosen.
        /// </summary>
        public bool IsSecond
        {
            get => Base.SyncIsB;
            set => Base.SyncIsB = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the first option is chosen.
        /// </summary>
        public bool IsFirst
        {
            get => Base.SyncIsA;
            set => Base.SyncIsB = !value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the second option is default.
        /// </summary>
        public bool IsSecondDefault
        {
            get => Base.DefaultIsB;
            set => Base.DefaultIsB = value;
        }

        /// <summary>
        /// Gets or sets a label for the first option.
        /// </summary>
        public string FirstOption
        {
            get => Base.OptionA;
            set => Base.OptionA = value;
        }

        /// <summary>
        /// Gets or sets a label for the second option.
        /// </summary>
        public string SecondOption
        {
            get => Base.OptionB;
            set => Base.OptionB = value;
        }

        /// <summary>
        /// Sends updated values to clients.
        /// </summary>
        /// <param name="firstOption"><inheritdoc cref="FirstOption"/></param>
        /// <param name="secondOption"><inheritdoc cref="SecondOption"/></param>
        /// <param name="overrideValue">If false, sends fake values.</param>
        /// <param name="filter">Who to send the update to.</param>
        public void UpdateSetting(string firstOption, string secondOption, bool overrideValue = true, Predicate<Player> filter = null)
        {
            filter ??= _ => true;
            Base.SendTwoButtonUpdate(firstOption, secondOption, overrideValue, hub => filter(Player.Get(hub)));
        }

        /// <summary>
        /// If setting is server only, sends updated values to clients.
        /// </summary>
        /// <param name="isSecond"><inheritdoc cref="IsSecond"/></param>
        /// <param name="overrideValue">If false, sends fake values.</param>
        /// <param name="filter">Who to send the update to.</param>
        public void UpdateValue(bool isSecond, bool overrideValue = true, Predicate<Player> filter = null)
        {
            filter ??= _ => true;
            Base.SendValueUpdate(isSecond, overrideValue, hub => filter(Player.Get(hub)));
        }

        /// <summary>
        /// Returns a representation of this <see cref="ButtonSetting"/>.
        /// </summary>
        /// <returns>A string in human-readable format.</returns>
        public override string ToString()
        {
            return base.ToString() + $" /{FirstOption}/ *{SecondOption}* +{IsSecondDefault}+ '{IsFirst}'";
        }
    }
}