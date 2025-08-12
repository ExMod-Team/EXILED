// -----------------------------------------------------------------------
// <copyright file="SliderSetting.cs" company="ExMod Team">
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
    /// Represents a slider setting.
    /// </summary>
    public class SliderSetting : SettingBase, IWrapper<SSSliderSetting>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SliderSetting"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="SettingBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="minValue"><inheritdoc cref="MinimumValue"/></param>
        /// <param name="maxValue"><inheritdoc cref="MaximumValue"/></param>
        /// <param name="defaultValue"><inheritdoc cref="DefaultValue"/></param>
        /// <param name="isInteger"><inheritdoc cref="IsInteger"/></param>
        /// <param name="stringFormat"><inheritdoc cref="StringFormat"/></param>
        /// <param name="displayFormat"><inheritdoc cref="DisplayFormat"/></param>
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        [Obsolete("Will be removed in Exiled 10 in favour of ctor with more params.")]
        public SliderSetting(int id, string label, float minValue, float maxValue, float defaultValue, bool isInteger, string stringFormat, string displayFormat, string hintDescription)
            : this(new SSSliderSetting(id, label, minValue, maxValue, defaultValue, isInteger, stringFormat, displayFormat, hintDescription))
        {
            Base = (SSSliderSetting)base.Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SliderSetting"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="SettingBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="minValue"><inheritdoc cref="MinimumValue"/></param>
        /// <param name="maxValue"><inheritdoc cref="MaximumValue"/></param>
        /// <param name="defaultValue"><inheritdoc cref="DefaultValue"/></param>
        /// <param name="isInteger"><inheritdoc cref="IsInteger"/></param>
        /// <param name="stringFormat"><inheritdoc cref="StringFormat"/></param>
        /// <param name="displayFormat"><inheritdoc cref="DisplayFormat"/></param>
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        /// <param name="collectionId"><inheritdoc cref="SettingBase.CollectionId"/></param>
        /// <param name="isServerOnly"><inheritdoc cref="SettingBase.IsServerOnly"/></param>
        /// <param name="header"><inheritdoc cref="SettingBase.Header"/></param>
        /// <param name="onChanged"><inheritdoc cref="SettingBase.OnChanged"/></param>
        public SliderSetting(int id, string label, float minValue, float maxValue, float defaultValue, bool isInteger = false, string stringFormat = "0.##", string displayFormat = "{0}", string hintDescription = null, byte collectionId = byte.MaxValue, bool isServerOnly = false, HeaderSetting header = null, Action<Player, SettingBase> onChanged = null)
            : base(new SSSliderSetting(id, label, minValue, maxValue, defaultValue, isInteger, stringFormat, displayFormat, hintDescription, collectionId, isServerOnly), header, onChanged)
        {
            Base = (SSSliderSetting)base.Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SliderSetting"/> class.
        /// </summary>
        /// <param name="settingBase">A <see cref="SSSliderSetting"/> instance.</param>
        internal SliderSetting(SSSliderSetting settingBase)
            : base(settingBase)
        {
            Base = settingBase;
        }

        /// <summary>
        /// Gets or sets the minimum value of the slider.
        /// </summary>
        public float MinimumValue
        {
            get => Base.MinValue;
            set => Base.MinValue = value;
        }

        /// <summary>
        /// Gets or sets the maximum value of the slider.
        /// </summary>
        public float MaximumValue
        {
            get => Base.MaxValue;
            set => Base.MaxValue = value;
        }

        /// <summary>
        /// Gets or sets the default value of the slider.
        /// </summary>
        public float DefaultValue
        {
            get => Base.DefaultValue;
            set => Base.DefaultValue = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the slider displays integers.
        /// </summary>
        public bool IsInteger
        {
            get => Base.Integer;
            set => Base.Integer = value;
        }

        /// <summary>
        /// Gets a value indicating whether the slider is currently being dragged.
        /// </summary>
        public bool IsBeingDragged => Base.SyncDragging;

        /// <summary>
        /// Gets a float that represents the current value of the slider.
        /// </summary>
        public float SliderValue => Base.Integer ? Base.SyncIntValue : Base.SyncFloatValue;

        /// <summary>
        /// Gets or sets the formatting used for the number in the slider.
        /// </summary>
        public string StringFormat
        {
            get => Base.ValueToStringFormat;
            set => Base.ValueToStringFormat = value;
        }

        /// <summary>
        /// Gets or sets the formatting used for the final display of the value of the slider.
        /// </summary>
        public string DisplayFormat
        {
            get => Base.FinalDisplayFormat;
            set => Base.FinalDisplayFormat = value;
        }

        /// <inheritdoc/>
        public new SSSliderSetting Base { get; }

        /// <summary>
        /// Sends updated values to clients.
        /// </summary>
        /// <param name="min"><inheritdoc cref="MinimumValue"/></param>
        /// <param name="max"><inheritdoc cref="MaximumValue"/></param>
        /// <param name="isInteger"><inheritdoc cref="IsInteger"/></param>
        /// <param name="stringFormat"><inheritdoc cref="StringFormat"/></param>
        /// <param name="displayFormat"><inheritdoc cref="DisplayFormat"/></param>
        /// <param name="overrideValue">If false, sends fake values.</param>
        /// <param name="filter">Who to send the update to.</param>
        public void UpdateSetting(float min, float max, bool isInteger, string stringFormat, string displayFormat, bool overrideValue = true, Predicate<Player> filter = null)
        {
            filter ??= _ => true;
            Base.SendSliderUpdate(min, max, isInteger, stringFormat, displayFormat, overrideValue, hub => filter(Player.Get(hub)));
        }

        /// <summary>
        /// If setting is server only, sends updated values to clients.
        /// </summary>
        /// <param name="value"><inheritdoc cref="SliderValue"/></param>
        /// <param name="overrideValue">If false, sends fake values.</param>
        /// <param name="filter">Who to send the update to.</param>
        public void UpdateValue(float value, bool overrideValue = true, Predicate<Player> filter = null)
        {
            filter ??= _ => true;
            Base.SendValueUpdate(value, overrideValue, hub => filter(Player.Get(hub)));
        }

        /// <summary>
        /// Returns a representation of this <see cref="SliderSetting"/>.
        /// </summary>
        /// <returns>A string in human-readable format.</returns>
        public override string ToString()
        {
            return base.ToString() + $" /{MinimumValue}/ *{MaximumValue}* +{DefaultValue}+ '{SliderValue}'";
        }
    }
}