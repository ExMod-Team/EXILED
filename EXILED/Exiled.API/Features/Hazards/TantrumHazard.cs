// -----------------------------------------------------------------------
// <copyright file="TantrumHazard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Hazards
{
    using Exiled.API.Enums;
    using global::Hazards;
    using Mirror;
    using RelativePositioning;
    using UnityEngine;

    /// <summary>
    /// A wrapper for <see cref="TantrumEnvironmentalHazard"/>.
    /// </summary>
    public class TantrumHazard : TemporaryHazard
    {
        private static TantrumEnvironmentalHazard tantrumPrefab;

        /// <summary>
        /// Initializes a new instance of the <see cref="TantrumHazard"/> class.
        /// </summary>
        /// <param name="hazard">The <see cref="TantrumEnvironmentalHazard"/> instance.</param>
        public TantrumHazard(TantrumEnvironmentalHazard hazard)
            : base(hazard)
        {
            Base = hazard;
        }

        /// <summary>
        /// Gets the tantrum prefab.
        /// </summary>
        public static TantrumEnvironmentalHazard TantrumPrefab
        {
            get
            {
                if (tantrumPrefab == null)
                    tantrumPrefab = PrefabHelper.GetPrefab<TantrumEnvironmentalHazard>(PrefabType.TantrumObj);

                return tantrumPrefab;
            }
        }

        /// <summary>
        /// Gets the <see cref="TantrumEnvironmentalHazard"/>.
        /// </summary>
        public new TantrumEnvironmentalHazard Base { get; }

        /// <inheritdoc />
        public override HazardType Type => HazardType.Tantrum;

        /// <summary>
        /// Gets or sets a value indicating whether sizzle should be played.
        /// </summary>
        public bool PlaySizzle
        {
            get => Base.PlaySizzle;
            set => Base.PlaySizzle = value;
        }

        /// <summary>
        /// Gets or sets the synced position.
        /// </summary>
        public RelativePosition SynchronisedPosition
        {
            get => Base.SynchronizedPosition;
            set => Base.SynchronizedPosition = value;
        }

        /// <summary>
        /// Gets or sets the correct position of tantrum hazard.
        /// </summary>
        public Transform CorrectPosition
        {
            get => Base._correctPosition;
            set => Base._correctPosition = value;
        }

        /// <summary>
        /// Places a Tantrum (SCP-173's ability) in the indicated position.
        /// </summary>
        /// <param name="position">The position where you want to spawn the Tantrum.</param>
        /// <param name="isActive">Whether the tantrum will apply the <see cref="EffectType.Stained"/> effect.</param>
        /// <remarks>If <paramref name="isActive"/> is <see langword="true"/>, the tantrum is moved slightly up from its original position. Otherwise, the collision will not be detected and the slowness will not work.</remarks>
        /// <returns>The <see cref="TantrumHazard"/> instance.</returns>
        public static TantrumHazard PlaceTantrum(Vector3 position, bool isActive = true)
        {
            TantrumEnvironmentalHazard tantrum = Object.Instantiate(TantrumPrefab);

            if (!isActive)
                tantrum.SynchronizedPosition = new(position);
            else
                tantrum.SynchronizedPosition = new(position + (Vector3.up * 0.25f));

            tantrum._destroyed = !isActive;

            NetworkServer.Spawn(tantrum.gameObject);

            return Get<TantrumHazard>(tantrum);
        }
    }
}