// -----------------------------------------------------------------------
// <copyright file="SettingBase.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.UserSettings
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Exiled.API.Features.Pools;
    using Exiled.API.Interfaces;
    using global::UserSettings.ServerSpecific;

    /// <summary>
    /// A base class for all Server Specific Settings.
    /// </summary>
    public class SettingBase : TypeCastObject<SettingBase>, IWrapper<ServerSpecificSettingBase>
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> that contains <see cref="SettingBase"/> that were received by a players.
        /// </summary>
        internal static readonly Dictionary<Player, List<SettingBase>> ReceivedSettings = new();

        /// <summary>
        /// A collection that contains all settings that were sent to clients.
        /// </summary>
        internal static readonly List<SettingBase> Settings = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingBase"/> class.
        /// </summary>
        /// <param name="settingBase">A <see cref="ServerSpecificSettingBase"/> instance.</param>
        /// <param name="header"><inheritdoc cref="Header"/></param>
        /// <param name="onChanged"><inheritdoc cref="OnChanged"/></param>
        internal SettingBase(ServerSpecificSettingBase settingBase, HeaderSetting header, Action<Player, SettingBase> onChanged)
        {
            Base = settingBase;

            Header = header;
            OnChanged = onChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingBase"/> class.
        /// </summary>
        /// <param name="settingBase"><inheritdoc cref="Base"/></param>
        internal SettingBase(ServerSpecificSettingBase settingBase)
        {
            Base = settingBase;

            if (OriginalDefinition != null)
            {
                Header = OriginalDefinition.Header;
                OnChanged = OriginalDefinition.OnChanged;
                Label = OriginalDefinition.Label;
                HintDescription = OriginalDefinition.HintDescription;
            }
        }

        /// <summary>
        /// Gets the list of all synced settings.
        /// </summary>
        public static IReadOnlyDictionary<Player, ReadOnlyCollection<SettingBase>> SyncedList
            => new ReadOnlyDictionary<Player, ReadOnlyCollection<SettingBase>>(ReceivedSettings.ToDictionary(x => x.Key, x => x.Value.AsReadOnly()));

        /// <summary>
        /// Gets the list of settings that were used as a prefabs.
        /// </summary>
        public static IReadOnlyCollection<SettingBase> List => Settings;

        /// <summary>
        /// Gets or sets the predicate for syncing this setting when a player joins.
        /// </summary>
        public static Predicate<Player> SyncOnJoin { get; set; }

        /// <inheritdoc/>
        public ServerSpecificSettingBase Base { get; }

        /// <summary>
        /// Gets or sets the id of this setting.
        /// </summary>
        public int Id
        {
            get => Base.SettingId;
            set => Base.SetId(value, string.Empty);
        }

        /// <summary>
        /// Gets or sets the label of this setting.
        /// </summary>
        public string Label
        {
            get => Base.Label;
            set => Base.Label = value;
        }

        /// <summary>
        /// Gets or sets the description of this setting.
        /// </summary>
        public string HintDescription
        {
            get => Base.HintDescription;
            set => Base.HintDescription = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the setting receives updates from server (and client stops sending updates).
        /// </summary>
        /// <remarks>
        /// Useful for displaying information, you cannot receive updates from a setting with this enabled.
        /// </remarks>
        public bool IsServerOnly
        {
            get => Base.IsServerOnly;
            set => Base.IsServerOnly = value;
        }

        /// <summary>
        /// Gets or sets a value controlling if this setting is shared across servers with <b>same Ip address!</b>. Default value is 255.
        /// </summary>
        /// <remarks>
        /// Settings with this value between 0 and 20 will store the servers Ip (or other unique identifier) instead of port, meaning the client treats a setting between 1.1.1.1:7777 and 1.1.1.1:7778 the same.
        /// <br/>
        /// <br/>
        /// If this value is above 20, the aforementioned behavior will not occur and the setting will behave as normal.
        /// </remarks>
        public byte CollectionId
        {
            get => Base.CollectionId;
            set => Base.CollectionId = value;
        }

        /// <summary>
        /// Gets the response mode of this setting.
        /// </summary>
        public ServerSpecificSettingBase.UserResponseMode ResponseMode => Base.ResponseMode;

        /// <summary>
        /// Gets the setting that was sent to players.
        /// </summary>
        /// <remarks>Can be <c>null</c> if this <see cref="SettingBase"/> is a prefab.</remarks>
        public SettingBase OriginalDefinition => Settings.Find(x => x.Id == Id);

        /// <summary>
        /// Gets or sets the header of this setting.
        /// </summary>
        /// <remarks>Can be <c>null</c>.</remarks>
        public HeaderSetting Header { get; set; }

        /// <summary>
        /// Gets or sets the action to be executed when this setting is changed.
        /// </summary>
        public Action<Player, SettingBase> OnChanged { get; set; }

        /// <summary>
        /// Tries to get the setting with the specified id.
        /// </summary>
        /// <param name="player">Player who has received the setting.</param>
        /// <param name="id">Id of the setting.</param>
        /// <param name="setting">A <see cref="SettingBase"/> instance if found. Otherwise, <c>null</c>.</param>
        /// <typeparam name="T">Type of the setting.</typeparam>
        /// <returns><c>true</c> if the setting was found, <c>false</c> otherwise.</returns>
        public static bool TryGetSetting<T>(Player player, int id, out T setting)
            where T : SettingBase
        {
            setting = null;

            if (!ReceivedSettings.TryGetValue(player, out List<SettingBase> list))
                return false;

            setting = (T)list.FirstOrDefault(x => x.Id == id);
            return setting != null;
        }

        /// <summary>
        /// Tries to get the setting with the specified id.
        /// </summary>
        /// <param name="player">Player who has received the setting.</param>
        /// <param name="id">Id of the setting.</param>
        /// <param name="setting">A <see cref="SettingBase"/> instance if found. Otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the setting was found, <c>false</c> otherwise.</returns>
        public static bool TryGetSetting(Player player, int id, out SettingBase setting) => TryGetSetting<SettingBase>(player, id, out setting);

        /// <summary>
        /// Creates a new instance of this setting.
        /// </summary>
        /// <param name="settingBase">A <see cref="ServerSpecificSettingBase"/> instance.</param>
        /// <returns>A new instance of this setting.</returns>
        /// <remarks>
        /// This method is used only to create a new instance of <see cref="SettingBase"/> from an existing <see cref="ServerSpecificSettingBase"/> instance.
        /// New setting won't be synced with players.
        /// </remarks>
        public static SettingBase Create(ServerSpecificSettingBase settingBase) => settingBase switch
        {
            SSButton button => new ButtonSetting(button),
            SSDropdownSetting dropdownSetting => new DropdownSetting(dropdownSetting),
            SSTextArea textArea => new TextInputSetting(textArea),
            SSGroupHeader header => new HeaderSetting(header),
            SSKeybindSetting keybindSetting => new KeybindSetting(keybindSetting),
            SSTwoButtonsSetting twoButtonsSetting => new TwoButtonsSetting(twoButtonsSetting),
            SSPlaintextSetting plainTextSetting => new UserTextInputSetting(plainTextSetting),
            SSSliderSetting sliderSetting => new SliderSetting(sliderSetting),
            _ => new SettingBase(settingBase)
        };

        /// <summary>
        /// Creates a new instance of this setting.
        /// </summary>
        /// <param name="settingBase">A<see cref="ServerSpecificSettingBase"/> instance.</param>
        /// <typeparam name="T">Type of the setting.</typeparam>
        /// <returns>A new instance of this setting.</returns>
        /// <remarks>
        /// This method is used only to create a new instance of <see cref="SettingBase"/> from an existing <see cref="ServerSpecificSettingBase"/> instance.
        /// New setting won't be synced with players.
        /// </remarks>
        public static T Create<T>(ServerSpecificSettingBase settingBase)
            where T : SettingBase => Create(settingBase) as T;

        /// <summary>
        /// Syncs setting with all players.
        /// </summary>
        public static void SendToAll() => ServerSpecificSettingsSync.SendToAll();

        /// <summary>
        /// Syncs setting with all players according to the specified predicate.
        /// </summary>
        /// <param name="predicate">A requirement to meet.</param>
        public static void SendToAll(Func<Player, bool> predicate)
        {
            foreach (Player player in Player.List)
            {
                if (predicate(player))
                    SendToPlayer(player);
            }
        }

        /// <summary>
        /// Syncs setting with the specified target.
        /// </summary>
        /// <param name="player">Target player.</param>
        public static void SendToPlayer(Player player) => ServerSpecificSettingsSync.SendToPlayer(player.ReferenceHub);

        /// <summary>
        /// Syncs specific settings with the specified target.
        /// </summary>
        /// <param name="player">Target player.</param>
        /// <param name="settings">Settings to send to the player.</param>
        public static void SendToPlayer(Player player, IEnumerable<SettingBase> settings) =>
            ServerSpecificSettingsSync.SendToPlayer(player.ReferenceHub, settings.Select(setting => setting.Base).ToArray());

        /// <summary>
        /// Registers all settings from the specified collection.
        /// </summary>
        /// <param name="settings">A collection of settings to register.</param>
        /// <param name="predicate">A requirement to meet when sending settings to players.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="SettingBase"/> instances that were successfully registered.</returns>
        /// <remarks>This method is used to sync new settings with players.</remarks>
        public static IEnumerable<SettingBase> Register(IEnumerable<SettingBase> settings, Func<Player, bool> predicate = null)
        {
            IEnumerable<IGrouping<HeaderSetting, SettingBase>> grouped = settings.Where(s => s != null).GroupBy(s => s.Header);

            List<SettingBase> result = new();

            // Group settings by headers
            foreach (IGrouping<HeaderSetting, SettingBase> grouping in grouped)
            {
                if (grouping.Key != null)
                    result.Add(grouping.Key);

                result.AddRange(grouping);
            }

            ServerSpecificSettingsSync.DefinedSettings = (ServerSpecificSettingsSync.DefinedSettings ?? Array.Empty<ServerSpecificSettingBase>()).Concat(result.Select(s => s.Base)).ToArray();
            Settings.AddRange(result);

            if (predicate == null)
                SendToAll();
            else
                SendToAll(predicate);

            return result;
        }

        /// <summary>
        /// Registers all settings from the specified collection to player.
        /// </summary>
        /// <param name="player">A player that will receive settings.</param>
        /// <param name="settings">A collection of settings to register.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="SettingBase"/> instances that were successfully registered.</returns>
        /// <remarks>This method is used to sync new settings with players.</remarks>
        public static IEnumerable<SettingBase> Register(Player player, IEnumerable<SettingBase> settings)
        {
            IEnumerable<IGrouping<HeaderSetting, SettingBase>> grouped = settings.Where(s => s != null).GroupBy(s => s.Header);

            List<SettingBase> result = new();

            // Group settings by headers
            foreach (IGrouping<HeaderSetting, SettingBase> grouping in grouped)
            {
                if (grouping.Key != null)
                    result.Add(grouping.Key);

                result.AddRange(grouping);
            }

            ServerSpecificSettingsSync.DefinedSettings = (ServerSpecificSettingsSync.DefinedSettings ?? Array.Empty<ServerSpecificSettingBase>()).Concat(result.Select(s => s.Base)).ToArray();
            Settings.AddRange(result);

            SendToPlayer(player);

            return result;
        }

        /// <summary>
        /// Removes settings from players.
        /// </summary>
        /// <param name="predicate">Determines which players will receive this update.</param>
        /// <param name="settings">Settings to remove. If <c>null</c>, all settings will be removed.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="SettingBase"/> instances that were successfully removed.</returns>
        /// <remarks>This method is used to unsync settings from players. Using it with <see cref="Register(IEnumerable{SettingBase},Func{Player,bool})"/> provides an opportunity to update synced settings.</remarks>
        public static IEnumerable<SettingBase> Unregister(Func<Player, bool> predicate = null, IEnumerable<SettingBase> settings = null)
        {
            List<ServerSpecificSettingBase> list = ListPool<ServerSpecificSettingBase>.Pool.Get(ServerSpecificSettingsSync.DefinedSettings);
            List<SettingBase> list2 = new((settings ?? Settings).Where(setting => list.Remove(setting.Base)));

            ServerSpecificSettingsSync.DefinedSettings = list.ToArray();

            if (predicate == null)
                SendToAll();
            else
                SendToAll(predicate);

            ListPool<ServerSpecificSettingBase>.Pool.Return(list);

            return list2;
        }

        /// <summary>
        /// Removes settings from players.
        /// </summary>
        /// <param name="player">Determines which player will receive this update.</param>
        /// <param name="settings">Settings to remove. If <c>null</c>, all settings will be removed.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="SettingBase"/> instances that were successfully removed.</returns>
        /// <remarks>This method is used to unsync settings from players. Using it with <see cref="Register(Player,IEnumerable{SettingBase})"/> provides an opportunity to update synced settings.</remarks>
        public static IEnumerable<SettingBase> Unregister(Player player, IEnumerable<SettingBase> settings = null)
        {
            List<ServerSpecificSettingBase> list = ListPool<ServerSpecificSettingBase>.Pool.Get(ServerSpecificSettingsSync.DefinedSettings);
            List<SettingBase> list2 = new((settings ?? Settings).Where(setting => list.Remove(setting.Base)));

            ServerSpecificSettingsSync.DefinedSettings = list.ToArray();

            SendToPlayer(player);

            ListPool<ServerSpecificSettingBase>.Pool.Return(list);

            return list2;
        }

        /// <summary>
        /// Sends an updated label and hint to clients.
        /// </summary>
        /// <param name="label"><inheritdoc cref="Label"/></param>
        /// <param name="hint"><inheritdoc cref="Hint"/></param>
        /// <param name="overrideValue">If false, sends fake values.</param>
        /// <param name="filter">Who to send the update to.</param>
        public void UpdateLabelAndHint(string label, string hint, bool overrideValue = true, Predicate<Player> filter = null)
        {
            filter ??= _ => true;
            Base.SendUpdate(label, hint, overrideValue, hub => filter(Player.Get(hub)));
        }

        /// <summary>
        /// Returns a string representation of this <see cref="SettingBase"/>.
        /// </summary>
        /// <returns>A string in human-readable format.</returns>
        public override string ToString()
        {
            return $"{Id} ({Label}) [{HintDescription}] {{{ResponseMode}}} ^{Header}^";
        }

        /// <summary>
        /// Internal method that fires when a setting is updated.
        /// </summary>
        /// <param name="hub"><see cref="ReferenceHub"/> that has updates the setting.</param>
        /// <param name="settingBase">A new updated setting.</param>
        internal static void OnSettingUpdated(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            if (!Player.TryGet(hub, out Player player) || hub.IsHost)
                return;

            SettingBase setting;

            if (!ReceivedSettings.TryGetValue(player, out List<SettingBase> list))
            {
                setting = Create(settingBase);
                ReceivedSettings.Add(player, new() { setting });

                if (setting.Is(out ButtonSetting _))
                    goto invoke;

                return;
            }

            if (!list.Exists(x => x.Id == settingBase.SettingId))
            {
                setting = Create(settingBase);
                list.Add(setting);

                if (setting.Is(out ButtonSetting _))
                    goto invoke;

                return;
            }

            setting = list.Find(x => x.Id == settingBase.SettingId);

            invoke:

            if (setting.OriginalDefinition == null)
            {
                Settings.Add(Create(settingBase.OriginalDefinition));
            }

            setting.OriginalDefinition?.OnChanged?.Invoke(player, setting);
        }
    }
}
