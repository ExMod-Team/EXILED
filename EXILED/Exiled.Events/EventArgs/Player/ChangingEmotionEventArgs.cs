// -----------------------------------------------------------------------
// <copyright file="ChangingEmotionEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;

    /// <summary>
    /// Contains all the information before the player's emotion changes.
    /// </summary>
    public class ChangingEmotionEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingEmotionEventArgs"/> class.
        /// </summary>
        /// <param name="hub"><inheritdoc cref="Player"/></param>
        /// <param name="newEmotionPresetType"><inheritdoc cref="NewEmotionPresetType"/></param>
        /// <param name="oldEmotionPresetType"><inheritdoc cref="OldEmotionPresetType"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingEmotionEventArgs(ReferenceHub hub, EmotionPresetType newEmotionPresetType, EmotionPresetType oldEmotionPresetType, bool isAllowed = true)
        {
            Player = Player.Get(hub);
            NewEmotionPresetType = newEmotionPresetType;
            OldEmotionPresetType = oldEmotionPresetType;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the old player's emotion.
        /// </summary>
        public EmotionPresetType OldEmotionPresetType { get; }

        /// <summary>
        /// Gets or sets the new player's emotion.
        /// </summary>
        public EmotionPresetType NewEmotionPresetType { get; set; }

        /// <inheritdoc/>
        public Player Player { get; }
    }
}