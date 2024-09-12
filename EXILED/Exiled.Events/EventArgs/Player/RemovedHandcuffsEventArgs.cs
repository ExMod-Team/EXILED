// -----------------------------------------------------------------------
// <copyright file="RemovedHandcuffsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Enums;
    using API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after freeing a handcuffed player.
    /// </summary>
    public class RemovedHandcuffsEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovedHandcuffsEventArgs" /> class.
        /// </summary>
        /// <param name="cuffer">The cuffer player.</param>
        /// <param name="target">The target player was uncuffed.</param>
        /// <param name="uncuffReason">The reason for removing the handcuffs.</param>
        public RemovedHandcuffsEventArgs(Player cuffer, Player target, UncuffReason uncuffReason)
        {
            Player = cuffer;
            Target = target;
            UncuffReason = uncuffReason;
        }

        /// <summary>
        /// Gets the  target player to be cuffed.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the cuffer player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the reason for removing handcuffs.
        /// </summary>
        public UncuffReason UncuffReason { get; }
    }
}
