// -----------------------------------------------------------------------
// <copyright file="JailbirdChangingWearStateEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Jailbird;

    /// <summary>
    /// Contains all information before a <see cref="JailbirdItem"/> changes its state.
    /// </summary>
    public class JailbirdChangingWearStateEventArgs : IItemEvent, IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JailbirdChangingWearStateEventArgs"/> class.
        /// </summary>
        /// <param name="jailbird">The Jailbird item whose state is changing.</param>
        /// <param name="newWearState">The <see cref="JailbirdWearState"/> the Jailbird is attempting to switch to.</param>
        /// <param name="oldWearState">The current <see cref="JailbirdWearState"/> the Jailbird is at.</param>
        public JailbirdChangingWearStateEventArgs(InventorySystem.Items.ItemBase jailbird, JailbirdWearState newWearState, JailbirdWearState oldWearState)
        {
            Jailbird = Item.Get<Jailbird>(jailbird);
            Player = Jailbird.Owner;
            NewWearState = newWearState;
            OldWearState = oldWearState;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who owns the <see cref="API.Features.Items.Jailbird"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Jailbird"/> that atempted to change its state.
        /// </summary>
        public Jailbird Jailbird { get; }

        /// <summary>
        /// Gets or sets the new <see cref="JailbirdWearState"/> of the Jailbird.
        /// </summary>
        public JailbirdWearState NewWearState { get; set; }

        /// <summary>
        /// Gets the old <see cref="JailbirdWearState"/> of the Jailbird.
        /// </summary>
        public JailbirdWearState OldWearState { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Item"/> associated with the Jailbird.
        /// </summary>
        public Item Item => Jailbird;

        /// <summary>
        /// Gets or sets a value indicating whether the Jailbird is allowed to change its <see cref="JailbirdWearState"/>.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
