// -----------------------------------------------------------------------
// <copyright file="IUsableEvent.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.EventArgs.Interfaces
{
    using Sexiled.API.Features.Items;

    /// <summary>
    /// Event args used for all <see cref="API.Features.Items.Usable" /> related events.
    /// </summary>
    public interface IUsableEvent : IItemEvent
    {
        /// <summary>
        /// Gets the <see cref="API.Features.Items.Usable" /> triggering the event.
        /// </summary>
        public Usable Usable { get; }
    }
}