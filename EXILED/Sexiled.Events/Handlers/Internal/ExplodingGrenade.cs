// -----------------------------------------------------------------------
// <copyright file="ExplodingGrenade.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.Handlers.Internal
{
    using Sexiled.API.Features.Pickups;
    using Sexiled.Events.EventArgs.Map;

    /// <summary>
    /// Handles <see cref="Map.ChangedIntoGrenade"/> event.
    /// </summary>
    internal static class ExplodingGrenade
    {
        /// <inheritdoc cref="Map.OnChangedIntoGrenade(ChangedIntoGrenadeEventArgs)" />
        public static void OnChangedIntoGrenade(ChangedIntoGrenadeEventArgs ev)
        {
            ev.Pickup.WriteProjectileInfo(ev.Projectile);
        }
    }
}
