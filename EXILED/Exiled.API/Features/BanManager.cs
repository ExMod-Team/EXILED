// -----------------------------------------------------------------------
// <copyright file="BanManager.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;

    /// <summary>
    /// Useful class to manage player bans.
    /// </summary>
    public static class BanManager
    {
        /// <summary>
        /// Bans an offline player.
        /// </summary>
        /// <param name="banType">Type of the ban (UserID/IP).</param>
        /// <param name="id">The UserID or IP address to ban.</param>
        /// <param name="reason">The ban reason.</param>
        /// <param name="duration">A <see cref="TimeSpan"/>representing the duration.</param>
        /// <param name="issuer">The Nickname of the ban issuer.</param>
        /// <returns>Whether the ban was successful.</returns>
        public static bool OfflineBanPlayer(BanHandler.BanType banType, string id, string reason, TimeSpan duration, string issuer = "SERVER CONSOLE") => OfflineBanPlayer(banType, id, reason, duration.TotalSeconds, issuer);

        /// <summary>
        /// Bans an offline player.
        /// </summary>
        /// <param name="banType">Type of the ban (UserID/IP).</param>
        /// <param name="id">The UserID or IP address to ban.</param>
        /// <param name="reason">The ban reason.</param>
        /// <param name="duration">Duration in seconds.</param>
        /// <param name="issuer">The Nickname of the ban issuer.</param>
        /// <returns>Whether the ban was successful.</returns>
        public static bool OfflineBanPlayer(BanHandler.BanType banType, string id, string reason, double duration, string issuer = "SERVER CONSOLE")
        {
            BanDetails details = new()
            {
                OriginalName = "Unknown - offline ban",
                Id = id,
                IssuanceTime = DateTime.UtcNow.Ticks,
                Expires = DateTime.UtcNow.AddSeconds(duration).Ticks,
                Reason = reason,
                Issuer = issuer,
            };
            return BanHandler.IssueBan(details, banType);
        }

        /// <summary>
        /// Unbans a player.
        /// </summary>
        /// <param name="banType">Type of the ban (UserID/IP).</param>
        /// <param name="id">The UserID or IP address to ban.</param>\
        public static void UnbanPlayer(BanHandler.BanType banType, string id) => BanHandler.RemoveBan(id, banType);
    }
}