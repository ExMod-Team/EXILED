// -----------------------------------------------------------------------
// <copyright file="IScp1344Event.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.EventArgs.Interfaces
{
    using Sexiled.API.Features.Items;

    /// <summary>
    /// Event args used for all <see cref="API.Features.Items.Scp1344" /> related events.
    /// </summary>
    public interface IScp1344Event : IItemEvent
    {
        /// <summary>
        /// Gets the <see cref="API.Features.Items.Scp1344" /> triggering the event.
        /// </summary>
        public Scp1344 Scp1344 { get; }
    }
}