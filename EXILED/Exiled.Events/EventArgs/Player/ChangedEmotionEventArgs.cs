// -----------------------------------------------------------------------
// <copyright file="ChangedEmotionEventArgs.cs" company="ExMod Team">
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
    /// Contains all the information after the player's emotion.
    /// </summary>
    public class ChangedEmotionEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedEmotionEventArgs"/> class.
        /// </summary>
        /// <param name="hub"><inheritdoc cref="Player"/></param>
        /// <param name="emotionPresetType"><inheritdoc cref="EmotionPresetType"/></param>
        public ChangedEmotionEventArgs(ReferenceHub hub, EmotionPresetType emotionPresetType)
        {
            Player = Player.Get(hub);
            EmotionPresetType = emotionPresetType;
        }

        /// <summary>
        /// Gets the player's emotion.
        /// </summary>
        public EmotionPresetType EmotionPresetType { get; }

        /// <inheritdoc/>
        public Player Player { get; }
    }
}