// -----------------------------------------------------------------------
// <copyright file="RecontainingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains information before SCP-079 gets recontained.
    /// </summary>
    public class RecontainingEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecontainingEventArgs" /> class.
        /// </summary>
        /// <param name="recontainer">The <see cref="BreakableWindow"/> instance.</param>
        public RecontainingEventArgs(BreakableWindow recontainer)
        {
            Player = Player.Get(recontainer.LastAttacker.Hub);
            IsAutomatic = recontainer.LastAttacker.IsSet;
        }

        /// <summary>
        /// Gets the Player that started the recontainment process.<br></br>
        /// Can be null if <see cref="IsAutomatic"/> is true.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether the recontained has been made automatically or by triggering the proccess.
        /// </summary>
        public bool IsAutomatic { get; }
    }
}
