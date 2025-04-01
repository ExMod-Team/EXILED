// -----------------------------------------------------------------------
// <copyright file="PlayerHandler.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.CustomItems.Events
{
    using Sexiled.API.Extensions;
    using Sexiled.API.Features;
    using Sexiled.CustomItems.API.Features;
    using Sexiled.Events.EventArgs.Player;

    /// <summary>
    /// Handles Player events for the CustomItem API.
    /// </summary>
    internal sealed class PlayerHandler
    {
        /// <inheritdoc cref="ChangingItemEventArgs"/>
        public void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;
            if (CustomItem.TryGet(ev.Item, out CustomItem? newItem) && (newItem?.ShouldMessageOnGban ?? false))
            {
                SpectatorCustomNickname(ev.Player, $"{ev.Player.CustomName} (CustomItem: {newItem.Name})");
            }
            else if (ev.Player != null && CustomItem.TryGet(ev.Player.CurrentItem, out _))
            {
                SpectatorCustomNickname(ev.Player, ev.Player.HasCustomName ? ev.Player.CustomName : string.Empty);
            }
        }

        private void SpectatorCustomNickname(Player player, string itemName)
        {
            foreach (Player spectator in Player.List)
                spectator.SendFakeSyncVar(player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), itemName);
        }
    }
}