// -----------------------------------------------------------------------
// <copyright file="OwnerEscapingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.EventArgs
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;

    using PlayerRoles;

    using Respawning;

    /// <summary>
    /// Contains all information of a <see cref="CustomItem"/> before a <see cref="Player"/> escapes.
    /// </summary>
    public class OwnerEscapingEventArgs : EscapingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerEscapingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="ev">The <see cref="EscapingEventArgs"/> instance.</param>
        public OwnerEscapingEventArgs(Item item, EscapingEventArgs ev)
            : base(ev.Player, ev.NewRole, ev.EscapeScenario)
        {
            Item = item;
        }

        /// <summary>
        /// Gets the item in the player's inventory.
        /// </summary>
        public Item Item { get; }
    }
}