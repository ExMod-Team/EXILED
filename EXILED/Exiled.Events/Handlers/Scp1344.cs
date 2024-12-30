// -----------------------------------------------------------------------
// <copyright file="Scp1344.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Scp1344;
    using Exiled.Events.Features;

    /// <summary>
    /// SCP-1344 related events.
    /// </summary>
    public static class Scp1344
    {
        /// <summary>
        /// Invoked  before SCP-1344 status changing.
        /// </summary>
        public static Event<ChangingStatusEventArgs> ChangingStatus { get; set; } = new();

        /// <summary>
        /// Invoked after SCP-1344 status changing.
        /// </summary>
        public static Event<ChangedStatusEventArgs> ChangedStatus { get; set; } = new();

        /// <summary>
        /// Invoked after trying deactivating SCP-1344.
        /// </summary>
        public static Event<TryingDeactivatingEventArgs> TryingDeactivating { get; set; } = new();

        /// <summary>
        /// Invoked before deactivating SCP-1344.
        /// </summary>
        public static Event<DeactivatingEventArgs> Deactivating { get; set; } = new();

        /// <summary>
        /// Invoked after deactivating SCP-1344.
        /// </summary>
        public static Event<DeactivatedEventArgs> Deactivated { get; set; } = new();

        /// <summary>
        /// Called after deactivating SCP-1344.
        /// </summary>
        /// <param name="ev">The <see cref="DeactivatingEventArgs"/> instance.</param>
        public static void OnDeactivated(DeactivatedEventArgs ev) => Deactivated.InvokeSafely(ev);

        /// <summary>
        /// Called before deactivating SCP-1344.
        /// </summary>
        /// <param name="ev">The <see cref="DeactivatingEventArgs"/> instance.</param>
        public static void OnDeactivating(DeactivatingEventArgs ev) => Deactivating.InvokeSafely(ev);

        /// <summary>
        /// Called after trying deactivating SCP-1344.
        /// </summary>
        /// <param name="ev">The <see cref="TryingDeactivatingEventArgs"/> instance.</param>
        public static void OnTryingDeactivating(TryingDeactivatingEventArgs ev) => TryingDeactivating.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-1344 status changing.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingStatusEventArgs"/> instance.</param>
        public static void OnChangingStatus(ChangingStatusEventArgs ev) => ChangingStatus.InvokeSafely(ev);

        /// <summary>
        /// Called after SCP-1344 status changing.
        /// </summary>
        /// <param name="ev">The <see cref="ChangedStatusEventArgs"/> instance.</param>
        public static void OnChangedStatus(ChangedStatusEventArgs ev) => ChangedStatus.InvokeSafely(ev);
    }
}
