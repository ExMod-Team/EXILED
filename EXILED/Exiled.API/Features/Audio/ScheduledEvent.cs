// -----------------------------------------------------------------------
// <copyright file="ScheduledEvent.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio
{
    using System;

    /// <summary>
    /// Represents a time-based action for audio playback.
    /// </summary>
    public class ScheduledEvent : IComparable<ScheduledEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledEvent"/> class.
        /// </summary>
        /// <param name="time">The exact time in seconds to trigger the action.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="id">The optional unique identifier for the event. If null, a random GUID will be generated automatically.</param>
        public ScheduledEvent(double time, Action action, string id = null)
        {
            Time = time;
            Action = action;
            Id = id ?? Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the specific time in seconds at which the event should trigger.
        /// </summary>
        public double Time { get; }

        /// <summary>
        /// Gets the action to be invoked when the specified time is reached.
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// Gets the unique identifier for this time event.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Compares this instance to another <see cref="ScheduledEvent"/> based on their trigger times.
        /// </summary>
        /// <param name="other">The other <see cref="ScheduledEvent"/> to compare to.</param>
        /// <returns>A value that indicates the relative order of the events being compared.</returns>
        public int CompareTo(ScheduledEvent other) => Time.CompareTo(other.Time);
    }
}