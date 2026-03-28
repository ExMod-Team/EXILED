// -----------------------------------------------------------------------
// <copyright file="IRagdollEvent.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using API.Features;

    /// <summary>
    /// Event args used for all <see cref="API.Features.Ragdoll" /> related events.
    /// </summary>
    public interface IRagdollEvent : IExiledEvent
    {
        /// <summary>
        /// Gets the <see cref="API.Features.Ragdoll" /> triggering the event.
        /// </summary>
        public Ragdoll Ragdoll { get; }
    }
}