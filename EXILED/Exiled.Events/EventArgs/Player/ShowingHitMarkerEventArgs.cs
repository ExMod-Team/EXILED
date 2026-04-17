// -----------------------------------------------------------------------
// <copyright file="ShowingHitMarkerEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Interfaces;

    /// <summary>
    /// Contains all information before a hitmarker is show to player.
    /// </summary>
    public class ShowingHitMarkerEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowingHitMarkerEventArgs" /> class.
        /// </summary>
        /// <param name="hub">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="size">
        /// <inheritdoc cref="Size" />
        /// </param>
        /// <param name="shouldPlayAudio">
        /// <inheritdoc cref="ShouldPlayAudio" />
        /// </param>
        /// <param name="hitmarkerType">
        /// <inheritdoc cref="HitmarkerType" />
        /// </param>
        public ShowingHitMarkerEventArgs(ReferenceHub hub, float size, bool shouldPlayAudio, HitmarkerType hitmarkerType)
        {
            Player = Player.Get(hub);
            Size = size;
            ShouldPlayAudio = shouldPlayAudio;
            HitmarkerType = hitmarkerType;
        }

        /// <summary>
        /// Gets or sets the player that the hitmarker is being shown to.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Gets or sets the target size multiplier.
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the hitmarker sound effect should play.
        /// </summary>
        public bool ShouldPlayAudio { get; set; }

        /// <summary>
        /// Gets or sets a the type of the hitmarker.
        /// </summary>
        public HitmarkerType HitmarkerType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the hitmarker should be shown.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}