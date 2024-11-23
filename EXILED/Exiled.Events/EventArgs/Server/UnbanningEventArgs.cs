// -----------------------------------------------------------------------
// <copyright file="UnbanningEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
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
        /// <param name="id"><inheritdoc cref="TargetId"/></param>
        /// <param name="banType"><inheritdoc cref="BanType"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UnbanningEventArgs(string id, BanHandler.BanType banType, bool isAllowed = true)
        {
            TargetId = id;
            BanType = banType;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the target player id.
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// Gets the ban type.
        /// </summary>
        public BanHandler.BanType BanType { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}