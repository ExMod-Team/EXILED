// -----------------------------------------------------------------------
// <copyright file="ChangingMicroHIDStateEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using API.Features;
    using API.Features.Items;
    using Interfaces;
    using InventorySystem.Items.MicroHID;
    using InventorySystem.Items.MicroHID.Modules;

    /// <summary>
    /// Contains all information before MicroHID state is changed.
    /// </summary>
    public class ChangingMicroHIDStateEventArgs : IDeniableEvent, IItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingMicroHIDStateEventArgs" /> class.
        /// </summary>
        /// <param name="microHID">
        /// <inheritdoc cref="MicroHID" />
        /// </param>
        /// <param name="newPhase">
        /// <inheritdoc cref="NewPhase" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingMicroHIDStateEventArgs(Item microHID, MicroHidPhase newPhase, bool isAllowed = true)
        {
            MicroHID = microHID.As<MicroHid>();
            NewPhase = newPhase;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the MicroHID instance.
        /// </summary>
        public MicroHid MicroHID { get; }

        /// <summary>
        /// Gets or sets the new MicroHID state.
        /// </summary>
        public MicroHidPhase NewPhase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the MicroHID state can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Item Item => MicroHID;
    }
}