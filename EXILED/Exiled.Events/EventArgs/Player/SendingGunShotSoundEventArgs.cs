// -----------------------------------------------------------------------
// <copyright file="SendingGunShotSoundEventArgs.cs" company="ExMod Team">
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
    /// Contains all information before a gun shot sound is sent to players.
    /// </summary>
    public class SendingGunShotSoundEventArgs : IPlayerEvent, IDeniableEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendingGunShotSoundEventArgs"/> class.
        /// </summary>
        /// <param name="firearm">The internal firearm instance.</param>
        /// <param name="audioIndex">The index of the audio clip to be played.</param>
        /// <param name="mixerChannel">The audio mixer channel.</param>
        /// <param name="range">The audible range of the sound.</param>
        /// <param name="pitch">The pitch of the sound.</param>
        public SendingGunShotSoundEventArgs(InventorySystem.Items.Firearms.Firearm firearm, int audioIndex, MixerChannel mixerChannel, float range, float pitch)
        {
            Firearm = Item.Get<Firearm>(firearm);
            Player = Firearm.Owner;
            Range = range;
            Pitch = pitch;
            AudioIndex = audioIndex;
            MixerChannel = mixerChannel;
            SendingPosition = Player.Position;
        }

        /// <summary>
        /// Gets the player who is shooting.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the firearm being fired.
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
        /// Gets or sets the max audible distance of the gunshot sound.
        /// </summary>
        public float Range { get; set; }

        /// <summary>
        /// Gets or sets the pitch of the gunshot sound.
        /// </summary>
        public float Pitch { get; set; }

        /// <summary>
        /// Gets or sets the world position from where the sound will originate.
        /// </summary>
        public Vector3 SendingPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the gun shot sound should be sent.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
