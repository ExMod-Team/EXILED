// -----------------------------------------------------------------------
// <copyright file="UnbannedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    /// <summary>
    /// Contains all information after a player gets unbanned.
    /// </summary>
    public class UnbannedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnbannedEventArgs"/> class.
        /// </summary>
        /// <param name="details"><inheritdoc cref="BanDetails"/></param>
        /// <param name="banType"><inheritdoc cref="BanType"/></param>
        public UnbannedEventArgs(string details, BanHandler.BanType banType)
        {
            BanDetails = BanHandler.ProcessBanItem(details, banType);
            BanType = banType;
        }

        /// <summary>
        /// Gets the ban details.
        /// </summary>
        public BanDetails BanDetails { get; }

        /// <summary>
        /// Gets the ban type.
        /// </summary>
        public BanHandler.BanType BanType { get; }
    }
}