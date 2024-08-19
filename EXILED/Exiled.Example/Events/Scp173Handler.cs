// -----------------------------------------------------------------------
// <copyright file="Scp173Handler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Scp173;

    /// <summary>
    /// Handles Scp-173 events.
    /// </summary>
    public class Scp173Handler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Scp173.OnScp173BeingObserved(Scp173BeingObservedEventArgs)" />.
        public void Scp173BeingObserved(Scp173BeingObservedEventArgs ev)
        {
            Log.Info($"Target: {ev.Target.ToString()}");
            Log.Info($"173: {ev.Player}");
            ev.IsAllowed = false;
        }
    }
}