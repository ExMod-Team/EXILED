// -----------------------------------------------------------------------
// <copyright file="IPickupEvent.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.EventArgs.Interfaces
{
    using Sexiled.API.Features.Pickups;

    /// <summary>
    /// Event args used for all <see cref="API.Features.Pickups.Pickup" /> related events.
    /// </summary>
    public interface IPickupEvent : ISexiledEvent
    {
        /// <summary>
        /// Gets the <see cref="API.Features.Pickups.Pickup" /> triggering the event.
        /// </summary>
        public Pickup Pickup { get; }
    }
}