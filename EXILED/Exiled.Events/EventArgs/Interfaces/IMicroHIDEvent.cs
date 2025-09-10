// -----------------------------------------------------------------------
// <copyright file="IMicroHIDEvent.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using API.Features.Items;

    /// <summary>
    /// Event args used for all <see cref="API.Features.Items.MicroHid" /> related events.
    /// </summary>
    public interface IMicroHIDEvent : IItemEvent
    {
        /// <summary>
        /// Gets the <see cref="API.Features.Items.MicroHid" /> triggering the event.
        /// </summary>
        public MicroHid MicroHID { get; }
    }
}