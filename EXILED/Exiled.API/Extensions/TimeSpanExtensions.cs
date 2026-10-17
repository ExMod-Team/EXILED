// -----------------------------------------------------------------------
// <copyright file="TimeSpanExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;

    /// <summary>
    /// A set of extensions for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Converts a TimeSpan object to a human-readable format.
        /// </summary>
        /// <param name="timeSpan"><see cref="TimeSpan"/> object.</param>
        /// <returns>A <see cref="TimeSpan"/> object in string representation.</returns>
        public static string ToHumanReadable(this TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
                return "-" + timeSpan.Negate().ToHumanReadable();

            int days = timeSpan.Days;
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if (days > 0)
                return $"{days}:{hours:D2}:{minutes:D2}:{seconds:D2}";
            else if (hours > 0)
                return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
            else
                return $"{minutes}:{seconds:D2}";
        }
    }
}
