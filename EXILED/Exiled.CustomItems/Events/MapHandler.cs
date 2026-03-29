// -----------------------------------------------------------------------
// <copyright file="MapHandler.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Events
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using MEC;

    /// <summary>
    /// Event Handlers for the CustomItem API.
    /// </summary>
    internal sealed class MapHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.WaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            Timing.CallDelayed(2, () => // The delay is necessary because the generation of the lockers takes time, due to the way they are made in the base game.
            {
                foreach (CustomItem customItem in CustomItem.Registered)
                {
                    try
                    {
                        customItem?.SpawnAll();
                    }
                    catch (System.Exception e)
                    {
                        Log.Error($"There was an error while spawning the custom item '{customItem?.Name}' ({customItem?.Id}) | {e.Message}");
                    }
                }
            });
        }
    }
}