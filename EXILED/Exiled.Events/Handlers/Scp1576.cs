// -----------------------------------------------------------------------
// <copyright file="Scp1576.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp1576;
    using Exiled.Events.Features;

    /// <summary>
    /// Handles Scp1576 related events.
    /// </summary>
    public class Scp1576
    {
        /// <summary>
        /// Invoked after transmission has ended.
        /// </summary>
        public static Event<TransmissionEndedEventArgs> TransmissionEnded { get; set; } = new Event<TransmissionEndedEventArgs>();

        /// <summary>
        /// Called after the transmission has ended.
        /// </summary>
        /// <param name="ev">The <see cref="TransmissionEndedEventArgs"/> instance.</param>
        public static void OnTransmisionEnded(TransmissionEndedEventArgs ev) => TransmissionEnded.InvokeSafely(ev);
    }
}