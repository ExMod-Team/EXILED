// -----------------------------------------------------------------------
// <copyright file="MapHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Events
{
    using Exiled.CustomItems.API.Features;

    /// <summary>
    /// Event Handlers for the CustomItem API.
    /// </summary>
    internal sealed class MapHandler
    {
        /// <summary>
        /// Handle spawning Custom Items.
        /// </summary>
        public void OnMapGenerated()
        {
            foreach (CustomItem customItem in CustomItem.Registered)
                customItem?.SpawnAll();
        }
    }
}