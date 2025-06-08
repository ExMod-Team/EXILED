// ----------------DissolveMatPool-------------------------------------------------------
// <copyright file="DisruptorFiringEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features.Pickups;
    using Interfaces;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information before a pickup <see cref="ItemType.ParticleDisruptor"/> shoot while on the ground.
    /// </summary>
    public class DisruptorFiringEventArgs : IDeniableEvent, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisruptorFiringEventArgs"/> class.
        /// </summary>
        /// <param name="disruptor"><inheritdoc cref="Pickup"/></param>
        /// <param name="attacker"><inheritdoc cref="Attacker"/></param>
        /// <param name="state"><inheritdoc cref="State"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DisruptorFiringEventArgs(Pickup disruptor, API.Features.Player attacker, DisruptorActionModule.FiringState state, bool isAllowed = true)
        {
            Pickup = disruptor;
            Attacker = attacker;
            State = state;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or Sets a value indicating whether the disruptor shoot and the ground.
        /// <remarks>The client will still see all effects, like sounds and shoot.</remarks>
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or Sets whether is the attacker.
        /// </summary>
        public API.Features.Player Attacker { get; set; }

        /// <summary>
        /// Gets the state of the weapon.
        /// </summary>
        public DisruptorActionModule.FiringState State { get; }

        /// <summary>
        /// Gets the pickup who shot the bullet.
        /// </summary>
        public Pickup Pickup { get; }
    }
}