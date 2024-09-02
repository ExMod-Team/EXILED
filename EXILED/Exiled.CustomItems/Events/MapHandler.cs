// -----------------------------------------------------------------------
// <copyright file="MapHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Events
{
    using Exiled.CustomItems.API.Features;
    using MEC;

    /// <summary>
    /// Event Handlers for the CustomItem API.
    /// </summary>
    internal sealed class MapHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Map.Generated"/>
        public void OnMapGenerated()
        {
            Timing.CallDelayed(0.5f, () => // Delay its necessary for the spawnpoints of lockers and rooms to be generated.
            {
                foreach (CustomItem customItem in CustomItem.Registered)
                    customItem?.SpawnAll();
            });
        }
    }
}