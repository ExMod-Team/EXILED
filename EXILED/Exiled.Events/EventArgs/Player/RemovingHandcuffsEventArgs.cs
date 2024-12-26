// -----------------------------------------------------------------------
// <copyright file="RemovingHandcuffsEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Enums;
    using API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before freeing a handcuffed player.
    /// </summary>
    public class RemovingHandcuffsEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovingHandcuffsEventArgs" /> class.
        /// </summary>
        /// <param name="cuffer">The cuffer player.</param>
        /// <param name="target">The target player to be uncuffed.</param>
        /// <param name="uncuffReason">The reason of removing handcuffs.</param>
        /// <param name="isAllowed">Indicates whether the event can be executed.</param>
        public RemovingHandcuffsEventArgs(Player cuffer, Player target, UncuffReason uncuffReason, bool isAllowed = true)
        {
            Player = cuffer;
            Target = target;
            UncuffReason = uncuffReason;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the target player to be cuffed.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can be handcuffed. Denying the event will only have an effect when <see cref="UncuffReason" /> is <see cref="UncuffReason.Player" />  until next major update.
        /// </summary>
        /// TODO: Update docs and patches
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the cuffer player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the reason of removing handcuffs.
        /// </summary>
        public UncuffReason UncuffReason { get; }
    }
}