// -----------------------------------------------------------------------
// <copyright file="CreditTags.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.CreditTags.Enums;
    using Exiled.CreditTags.Events;
    using Exiled.CreditTags.Features;

    using PlayerEvents = Exiled.Events.Handlers.Player;

    /// <summary>
    /// Handles credits for Exiled Devs, Contributors and Plugin devs.
    /// </summary>
    public sealed class CreditTags : Plugin<Config>
    {
        private static readonly CreditTags Singleton = new();

        private CreditsHandler handler;

        private CreditTags()
        {
        }

        /// <summary>
        /// Gets a static reference to this class.
        /// </summary>
        public static CreditTags Instance => Singleton;

        /// <inheritdoc/>
        public override string Prefix { get; } = "exiled_credits";

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of Exiled Credit ranks.
        /// </summary>
        internal Dictionary<RankType, Rank> Ranks { get; } = new()
        {
            [RankType.Dev] = new Rank("Exiled Developer", "aqua", "00FFFF"),
            [RankType.Contributor] = new Rank("Exiled Contributor", "magenta", "FF0090"),
            [RankType.PluginDev] = new Rank("Exiled Plugin Developer", "crimson", "DC143C"),
            [RankType.TournamentParticipant] = new Rank("Exiled Tournament Participant", "pink", "FF96DE"),
            [RankType.TournamentChampion] = new Rank("Exiled Tournament Champion", "deep_pink", "FF1493"),
            [RankType.Donator] = new Rank("EXILED Supporter", "army_green", "4B5320"),
        };

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            DatabaseHandler.UpdateData();
            RefreshHandler();
            AttachHandler();

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            UnattachHandler();

            base.OnDisabled();
        }

        // returns true if the player was in the cache
        internal bool ShowCreditTag(Player player, bool force = false)
        {
            if (player.DoNotTrack && !Instance.Config.IgnoreDntFlag && !force)
                return false;

            if (!DatabaseHandler.TryGetRank(player.UserId, out RankType cachedRank))
                return false;

            ShowRank(cachedRank);
            return true;

            void ShowRank(RankType rank)
            {
                bool canReceiveCreditBadge = force ||
                                             (((string.IsNullOrEmpty(player.RankName) &&
                                                string.IsNullOrEmpty(player.ReferenceHub.serverRoles.HiddenBadge)) ||
                                               Config.BadgeOverride) && player.GlobalBadge is null);
                bool canReceiveCreditCustomInfo = string.IsNullOrEmpty(player.CustomInfo) || Config.CustomPlayerInfoOverride;

                if (!Ranks.TryGetValue(rank, out Rank value))
                    return;

                switch (Config.Mode)
                {
                    case InfoSide.Badge when canReceiveCreditBadge:
                        SetCreditBadge(player, value);
                        break;
                    case InfoSide.CustomPlayerInfo when canReceiveCreditCustomInfo:
                        SetCreditCustomInfo(player, value);
                        break;
                    case InfoSide.FirstAvailable:
                        if (canReceiveCreditBadge)
                            SetCreditBadge(player, value);
                        else if (canReceiveCreditCustomInfo)
                            SetCreditCustomInfo(player, value);
                        break;
                }
            }
        }

        private void SetCreditBadge(Player player, Rank value)
        {
            player.RankName = value.Name;
            player.RankColor = value.Color;
            Log.Debug($"Updated {player.Nickname} badge to {value.Name}.");
        }

        private void SetCreditCustomInfo(Player player, Rank value)
        {
            player.CustomInfo = $"<color=#{value.HexValue}>{value.Name}</color>";
            Log.Debug($"Updated {player.Nickname} CustomInfo to {value.Name}.");
        }

        private void RefreshHandler() => handler = new CreditsHandler();

        private void AttachHandler() => PlayerEvents.Verified += handler.OnPlayerVerify;

        private void UnattachHandler() => PlayerEvents.Verified -= handler.OnPlayerVerify;
    }
}