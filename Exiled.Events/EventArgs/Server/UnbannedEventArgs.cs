// -----------------------------------------------------------------------
// <copyright file="UnbannedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after a player gets unbanned.
    /// </summary>
    public class UnbannedEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnbannedEventArgs"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="TargetId"/></param>
        /// <param name="banType"><inheritdoc cref="BanType"/></param>
        public UnbannedEventArgs(string id, BanHandler.BanType banType)
        {
            TargetId = id;
            BanType = banType;
        }

        /// <summary>
        /// Gets or sets the target player id.
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// Gets the ban type.
        /// </summary>
        public BanHandler.BanType BanType { get; }
    }
}