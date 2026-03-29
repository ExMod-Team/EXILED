// -----------------------------------------------------------------------
// <copyright file="IScp127Event.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    /// <summary>
    /// Represents all events related to SCP-127.
    /// </summary>
    public interface IScp127Event : IItemEvent
    {
        /// <summary>
        /// Gets the SCP-127 instance, related to this event.
        /// </summary>
        public API.Features.Items.Scp127 Scp127 { get; }
    }
}