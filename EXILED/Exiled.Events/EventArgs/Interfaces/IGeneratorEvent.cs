// -----------------------------------------------------------------------
// <copyright file="IGeneratorEvent.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using API.Features;

    /// <summary>
    /// Event args used for all <see cref="API.Features.Generator" /> related events.
    /// </summary>
    public interface IGeneratorEvent : IExiledEvent
    {
        /// <summary>
        /// Gets the <see cref="API.Features.Generator" /> triggering the event.
        /// </summary>
        public Generator Generator { get; }
    }
}