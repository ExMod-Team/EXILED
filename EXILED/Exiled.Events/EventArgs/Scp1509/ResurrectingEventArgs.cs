// -----------------------------------------------------------------------
// <copyright file="ResurrectingEventArgs.cs" company="ExMod Team">
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
    using PlayerRoles;

    /// <summary>
    /// Contains all information before player is resurrected.
    /// </summary>
    public class ResurrectingEventArgs : IItemEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResurrectingEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="victim"><inheritdoc cref="Victim"/></param>
        /// <param name="newRole"><inheritdoc cref="NewRole"/></param>
        /// <param name="scp1509"><inheritdoc cref="Scp1509"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ResurrectingEventArgs(Player target, Player victim, RoleTypeId newRole, Scp1509Item scp1509, bool isAllowed = true)
        {
            Target = target;
            Victim = victim;
            NewRole = newRole;
            Scp1509 = Item.Get<Scp1509>(scp1509);
            Player = Scp1509.Owner;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the role which will be set to the <see cref="Target"/> after resurrection.
        /// </summary>
        public RoleTypeId NewRole { get; set; }

        /// <summary>
        /// Gets the target of resurrection.
        /// </summary>
        public Player Target { get; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the victim of this kill.
        /// </summary>
        public Player Victim { get; }

        /// <summary>
        /// Gets the SCP-1509 instance.
        /// </summary>
        public Scp1509 Scp1509 { get; }

        /// <inheritdoc/>
        public Item Item => Scp1509;

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}