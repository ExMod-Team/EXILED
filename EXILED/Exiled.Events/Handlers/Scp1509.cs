// -----------------------------------------------------------------------
// <copyright file="Scp1509.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp1509;
    using Exiled.Events.Features;
#pragma warning disable SA1623

    /// <summary>
    /// SCP-1509 related events.
    /// </summary>
    public static class Scp1509
    {
        /// <summary>
        /// Invoked before player is resurrected.
        /// </summary>
        public static Event<ResurrectingEventArgs> Resurrecting { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-1509 melee attack is triggered.
        /// </summary>
        public static Event<TriggeringAttackEventArgs> TriggeringAttack { get; set; } = new();

        /// <summary>
        /// Called before SCP-1509 melee attack is triggered.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringAttackEventArgs"/> instance.</param>
        public static void OnTriggeringAttack(TriggeringAttackEventArgs ev) => TriggeringAttack.InvokeSafely(ev);

        /// <summary>
        /// Called before player is resurrected.
        /// </summary>
        /// <param name="ev">The <see cref="ResurrectingEventArgs"/> instance.</param>
        public static void OnResurrecting(ResurrectingEventArgs ev) => Resurrecting.InvokeSafely(ev);
    }
}