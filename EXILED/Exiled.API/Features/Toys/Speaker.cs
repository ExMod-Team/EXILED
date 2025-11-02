// -----------------------------------------------------------------------
// <copyright file="Speaker.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System.Collections.Generic;

    using AdminToys;
    using Enums;
    using Exiled.API.Interfaces;
    using UnityEngine;
    using VoiceChat.Networking;
    using VoiceChat.Playbacks;

    /// <summary>
    /// A wrapper class for <see cref="SpeakerToy"/>.
    /// </summary>
    public class Speaker : AdminToy, IWrapper<SpeakerToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Speaker"/> class.
        /// </summary>
        /// <param name="speakerToy">The <see cref="SpeakerToy"/> of the toy.</param>
        internal Speaker(SpeakerToy speakerToy)
            : base(speakerToy, AdminToyType.Speaker) => Base = speakerToy;

        /// <summary>
        /// Gets the prefab.
        /// </summary>
        public static SpeakerToy Prefab => PrefabHelper.GetPrefab<SpeakerToy>(PrefabType.SpeakerToy);

        /// <summary>
        /// Gets the base <see cref="SpeakerToy"/>.
        /// </summary>
        public SpeakerToy Base { get; }

        /// <summary>
        /// Gets or sets the volume of the audio source.
        /// </summary>
        /// <value>
        /// A <see cref="float"/> representing the volume level of the audio source,
        /// where 0.0 is silent and 1.0 is full volume if it's more it's will amplify it.
        /// </value>
        public float Volume
        {
            get => Base.NetworkVolume;
            set => Base.NetworkVolume = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the audio source is spatialized.
        /// </summary>
        /// <value>
        /// A <see cref="bool"/> where <c>true</c> means the audio source is spatial, allowing
        /// for 3D audio positioning relative to the listener; <c>false</c> means it is non-spatial.
        /// </value>
        public bool IsSpatial
        {
            get => Base.NetworkIsSpatial;
            set => Base.NetworkIsSpatial = value;
        }

        /// <summary>
        /// Gets or sets the maximum distance at which the audio source can be heard.
        /// </summary>
        /// <value>
        /// A <see cref="float"/> representing the maximum hearing distance for the audio source.
        /// Beyond this distance, the audio will not be audible.
        /// </value>
        public float MaxDistance
        {
            get => Base.NetworkMaxDistance;
            set => Base.NetworkMaxDistance = value;
        }

        /// <summary>
        /// Gets or sets the minimum distance at which the audio source reaches full volume.
        /// </summary>
        /// <value>
        /// A <see cref="float"/> representing the distance from the source at which the audio is heard at full volume.
        /// Within this range, volume will not decrease with proximity.
        /// </value>
        public float MinDistance
        {
            get => Base.NetworkMinDistance;
            set => Base.NetworkMinDistance = value;
        }

        /// <summary>
        /// Gets or sets the controller ID of speaker.
        /// </summary>
        public byte ControllerId
        {
            get => Base.NetworkControllerId;
            set => Base.NetworkControllerId = value;
        }

        /// <summary>
        /// Creates a new <see cref="Speaker"/>.
        /// </summary>
        /// <param name="position">The position of the <see cref="Speaker"/>.</param>
        /// <param name="rotation">The rotation of the <see cref="Speaker"/>.</param>
        /// <param name="scale">The scale of the <see cref="Speaker"/>.</param>
        /// <param name="spawn">Whether the <see cref="Speaker"/> should be initially spawned.</param>
        /// <returns>The new <see cref="Speaker"/>.</returns>
        public static Speaker Create(Vector3? position, Vector3? rotation, Vector3? scale, bool spawn)
        {
            Speaker speaker = new(UnityEngine.Object.Instantiate(Prefab))
            {
                Position = position ?? Vector3.zero,
                Rotation = Quaternion.Euler(rotation ?? Vector3.zero),
                Scale = scale ?? Vector3.one,
            };

            if (spawn)
                speaker.Spawn();

            return speaker;
        }

        /// <summary>
        /// Creates a new <see cref="Speaker"/>.
        /// </summary>
        /// <param name="transform">The transform to create this <see cref="Speaker"/> on.</param>
        /// <param name="spawn">Whether the <see cref="Speaker"/> should be initially spawned.</param>
        /// <param name="worldPositionStays">Whether the <see cref="Speaker"/> should keep the same world position.</param>
        /// <returns>The new <see cref="Speaker"/>.</returns>
        public static Speaker Create(Transform transform, bool spawn, bool worldPositionStays = true)
        {
            Speaker speaker = new(Object.Instantiate(Prefab, transform, worldPositionStays))
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.localScale.normalized,
            };

            if(spawn)
                speaker.Spawn();

            return speaker;
        }

        /// <summary>
        /// Plays audio through this speaker.
        /// </summary>
        /// <param name="message">An <see cref="AudioMessage"/> instance.</param>
        /// <param name="targets">Targets who will hear the audio. If <c>null</c>, audio will be sent to all players.</param>
        public static void Play(AudioMessage message, IEnumerable<Player> targets = null)
        {
            foreach (Player target in targets ?? Player.List)
                target.Connection.Send(message);
        }

        /// <summary>
        /// Plays audio through this speaker.
        /// </summary>
        /// <param name="samples">Audio samples.</param>
        /// <param name="length">The length of the samples array.</param>
        /// <param name="targets">Targets who will hear the audio. If <c>null</c>, audio will be sent to all players.</param>
        public void Play(byte[] samples, int? length = null, IEnumerable<Player> targets = null) => Play(new AudioMessage(ControllerId, samples, length ?? samples.Length), targets);
    }
}
