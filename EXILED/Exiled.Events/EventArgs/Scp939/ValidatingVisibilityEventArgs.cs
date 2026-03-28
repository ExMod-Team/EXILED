// -----------------------------------------------------------------------
// <copyright file="ValidatingVisibilityEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;

    using Exiled.API.Enums;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information before SCP-939 sees the player.
    /// </summary>
    public class ValidatingVisibilityEventArgs : IScp939Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingVisibilityEventArgs" /> class.
        /// </summary>
        /// <param name="state">
        /// <inheritdoc cref="TargetVisibilityState"/>
        /// </param>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        /// The target being shown to SCP-939.
        /// </param>
        public ValidatingVisibilityEventArgs(Scp939VisibilityState state, ReferenceHub player, ReferenceHub target)
        {
            Player = Player.Get(player);
            Scp939 = Player.Role.As<Scp939Role>();
            Target = Player.Get(target);
            TargetVisibilityState = state;
            IsAllowed = TargetVisibilityState is not(Scp939VisibilityState.NotSeen or Scp939VisibilityState.None);
            IsLateSeen = TargetVisibilityState is Scp939VisibilityState.SeenByRange;
        }

        /// <summary>
        /// Gets the player who's being shown to SCP-939.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }

        /// <summary>
        /// Gets the info about base-game vision information.
        /// </summary>
        public Scp939VisibilityState TargetVisibilityState { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the SCP-939 will detect that vision as <see cref="Scp939VisibilityState.SeenByRange"/>.
        /// </summary>
        /// <remarks>
        /// Works only when <see cref="IsAllowed"/> = <see langword="true"/>, and makes player visible to SCP-939 for a while, after it's out of range.
        /// </remarks>
        public bool IsLateSeen { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}
