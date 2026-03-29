// -----------------------------------------------------------------------
// <copyright file="RemovingTargetEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using API.Features;

    using Interfaces;

    using Scp096Role = API.Features.Roles.Scp096Role;

    /// <summary>
    /// Contains all information after removing a target from SCP-096.
    /// </summary>
    public class RemovingTargetEventArgs : IScp096Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovingTargetEventArgs" /> class.
        /// </summary>
        /// <param name="scp096">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        /// <inheritdoc cref="Target" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public RemovingTargetEventArgs(Player scp096, Player target, bool isAllowed = true)
        {
            Player = scp096;
            Scp096 = scp096.Role.As<Scp096Role>();
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Player" /> that is controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp096Role Scp096 { get; }

        /// <summary>
        /// Gets the <see cref="Player" /> being removed as a target.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the target is allowed to be removed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}