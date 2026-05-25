// -----------------------------------------------------------------------
// <copyright file="ChangingWearablesEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.Wearables;

    using Unity.Collections.LowLevel.Unsafe;

    /// <summary>
    /// Contains all information before new information about wearables is sent to clients.
    /// </summary>
    public class ChangingWearablesEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingWearablesEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="newWearables"><inheritdoc cref="NewWearables"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingWearablesEventArgs(Player player, WearableElements newWearables, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;

            WearableElementType exiledFlags = WearableElementType.None;

            if (newWearables.HasFlag(WearableElements.Armor) && WearableSync.PayloadWriter.buffer.Length is 1)
            {
                ItemType armor = (ItemType)UnsafeUtility.As<byte, sbyte>(ref WearableSync.PayloadWriter.buffer[0]);
                exiledFlags = armor.GetWearableElementType();
            }

            NewWearables = (WearableElementType)newWearables | exiledFlags;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets new wearables that'll be displayed on <see cref="Player"/>.
        /// </summary>
        public WearableElementType NewWearables { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}