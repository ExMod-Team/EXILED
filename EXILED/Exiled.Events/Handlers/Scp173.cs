// -----------------------------------------------------------------------
// <copyright file="Scp173.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Scp173;
    using Exiled.Events.Features;

    /// <summary>
    /// SCP-173 related events.
    /// </summary>
    public static class Scp173
    {
        /// <summary>
        /// Invoked before players near SCP-173 blink.
        /// </summary>
        public static Event<BlinkingEventArgs> Blinking { get; set; } = new();

        /// <summary>
        /// Invoked before server handle SCP-173 blink network message.
        /// </summary>
        public static Event<BlinkingRequestEventArgs> BlinkingRequest { get; set; } = new();

        /// <summary>
        /// Invoked before a tantrum is placed.
        /// </summary>
        public static Event<PlacingTantrumEventArgs> PlacingTantrum { get; set; } = new();

        /// <summary>
        /// Invoked before using breakneck speeds.
        /// </summary>
        public static Event<UsingBreakneckSpeedsEventArgs> UsingBreakneckSpeeds { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-173 is observed.
        /// </summary>
        public static Event<BeingObservedEventArgs> BeingObserved { get; set; } = new();

        /// <summary>
        /// Invoked before a player starts looking at SCP-173.
        /// </summary>
        public static Event<AddingObserverEventArgs> AddingObserver { get; set; } = new();

        /// <summary>
        /// Invoked after a player stops looking at SCP-173.
        /// </summary>
        public static Event<RemovedObserverEventArgs> RemovedObserver { get; set; } = new();

        /// <summary>
        /// Called before players near SCP-173 blink.
        /// </summary>
        /// <param name="ev">The <see cref="BlinkingEventArgs" /> instance.</param>
        public static void OnBlinking(BlinkingEventArgs ev) => Blinking.InvokeSafely(ev);

        /// <summary>
        /// Called before server handle SCP-173 blink network message.
        /// </summary>
        /// <param name="ev">The <see cref="BlinkingRequestEventArgs" /> instance.</param>
        public static void OnBlinkingRequest(BlinkingRequestEventArgs ev) => BlinkingRequest.InvokeSafely(ev);

        /// <summary>
        /// Called before a tantrum is placed.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingTantrumEventArgs" /> instance.</param>
        public static void OnPlacingTantrum(PlacingTantrumEventArgs ev) => PlacingTantrum.InvokeSafely(ev);

        /// <summary>
        /// Called before a using breakneck speeds.
        /// </summary>
        /// <param name="ev">The <see cref="UsingBreakneckSpeedsEventArgs" /> instance.</param>
        public static void OnUsingBreakneckSpeeds(UsingBreakneckSpeedsEventArgs ev) => UsingBreakneckSpeeds.InvokeSafely(ev);

        /// <summary>
        /// Called before Scp 173 is observed.
        /// </summary>
        /// <param name="ev">The <see cref="BeingObservedEventArgs" /> instance.</param>
        public static void OnBeingObserved(BeingObservedEventArgs ev) => BeingObserved.InvokeSafely(ev);

        /// <summary>
        /// Called before player starts looking at SCP-173.
        /// </summary>
        /// <param name="ev">The <see cref="AddingObserverEventArgs" /> instance.</param>
        public static void OnAddingObserver(AddingObserverEventArgs ev) => AddingObserver.InvokeSafely(ev);

        /// <summary>
        /// Called after a player stops looking at SCP-173.
        /// </summary>
        /// <param name="ev">The <see cref="AddingObserverEventArgs" /> instance.</param>
        public static void OnRemovedObserver(RemovedObserverEventArgs ev) => RemovedObserver.InvokeSafely(ev);
    }
}
