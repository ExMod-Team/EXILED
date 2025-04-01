// -----------------------------------------------------------------------
// <copyright file="IScp330Event.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.EventArgs.Interfaces
{
    using Sexiled.API.Features.Items;

    /// <summary>
    /// Event args used for all <see cref="API.Features.Items.Scp330" /> related events.
    /// </summary>
    public interface IScp330Event : IItemEvent
    {
        /// <summary>
        /// Gets the <see cref="API.Features.Items.Scp330" /> triggering the event.
        /// </summary>
        public Scp330 Scp330 { get; }
    }
}