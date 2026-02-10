// -----------------------------------------------------------------------
// <copyright file="ReceivingGunSoundEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using AudioPooling;

    using Exiled.Events.EventArgs.Interfaces;

    using UnityEngine;

    /// <summary>
    /// Contains all information before a player receive gun sound.
    /// </summary>
    public class ReceivingGunSoundEventArgs : IPlayerEvent, IDeniableEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingGunSoundEventArgs"/> class.
        /// </summary>
        /// <param name="hub">The referencehub who will receive gun sound.</param>
        /// <param name="firearm">The internal firearm instance.</param>
        /// <param name="audioIndex">The index of the audio clip to be played.</param>
        /// <param name="mixerChannel">The audio mixer channel.</param>
        /// <param name="range">The audible range of the sound.</param>
        /// <param name="pitch">The pitch of the sound.</param>
        public ReceivingGunSoundEventArgs(ReferenceHub hub, InventorySystem.Items.Firearms.Firearm firearm, int audioIndex, MixerChannel mixerChannel, float range, float pitch)
        {
            Player = Player.Get(hub);
            Firearm = Item.Get<Firearm>(firearm);
            Sender = Firearm.Owner;
            Range = range;
            Pitch = pitch;
            AudioIndex = audioIndex;
            MixerChannel = mixerChannel;
        }

        /// <summary>
        /// Gets the player who will receive gun sound.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player who owns the Firearm.
        /// </summary>
        public Player Sender { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the firearm that was the source of the sound.
        /// </summary>
        public Firearm Firearm { get; }

        /// <summary>
        /// Gets or sets the index of the audio clip to be played from the firearm's audio list.
        /// </summary>
        public int AudioIndex { get; set; }

        /// <summary>
        /// Gets or sets the mixer channel through which the sound will be played.
        /// </summary>
        public MixerChannel MixerChannel { get; set; }

        /// <summary>
        /// Gets or sets the max audible distance of the gun sound.
        /// </summary>
        public float Range { get; set; }

        /// <summary>
        /// Gets or sets the pitch of the gun sound.
        /// </summary>
        public float Pitch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the gun sound should be sent.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
