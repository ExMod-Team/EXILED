// -----------------------------------------------------------------------
// <copyright file="UnbanningEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before player is unbanned.
    /// </summary>
    public class UnbanningEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnbanningEventArgs"/> class.
        /// </summary>
        /// <param name="banDetails"><inheritdoc cref="BanDetails"/></param>
        /// <param name="banType"><inheritdoc cref="BanType"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UnbanningEventArgs(string banDetails, BanHandler.BanType banType, bool isAllowed = true)
        {
            BanDetails = BanHandler.ProcessBanItem(banDetails, banType);
            BanType = banType;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the ban details.
        /// </summary>
        public BanDetails BanDetails { get; set; }

        /// <summary>
        /// Gets the ban type.
        /// </summary>
        public BanHandler.BanType BanType { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}