// -----------------------------------------------------------------------
// <copyright file="InspectingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp1509
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Scp1509;

    /// <summary>
    /// Contains all information before SCP-1509 is inspected.
    /// </summary>
    public class InspectingEventArgs : IItemEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectingEventArgs"/> class.
        /// </summary>
        /// <param name="scp1509"><inheritdoc cref="Scp1509"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InspectingEventArgs(Scp1509Item scp1509, bool isAllowed = true)
        {
            Scp1509 = Item.Get<Scp1509>(scp1509);
            Player = Scp1509.Owner;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <inheritdoc />
        public Item Item => Scp1509;

        /// <summary>
        /// Gets the SCP-1509 instance.
        /// </summary>
        public Scp1509 Scp1509 { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}