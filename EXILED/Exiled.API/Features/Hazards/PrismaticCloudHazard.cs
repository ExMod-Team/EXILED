// -----------------------------------------------------------------------
// <copyright file="PrismaticCloudHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Hazards
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using global::Hazards;
    using Mirror;
    using RelativePositioning;
    using UnityEngine;

    /// <summary>
    /// A wrapper for <see cref="PrismaticCloud"/>.
    /// </summary>
    public class PrismaticCloudHazard : TemporaryHazard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrismaticCloudHazard"/> class.
        /// </summary>
        /// <param name="hazard">The <see cref="TantrumEnvironmentalHazard"/> instance.</param>
        public PrismaticCloudHazard(PrismaticCloud hazard)
            : base(hazard)
        {
            Base = hazard;
        }

        /// <summary>
        /// Gets the <see cref="PrismaticCloud"/>.
        /// </summary>
        public new PrismaticCloud Base { get; }

        /// <inheritdoc />
        public override HazardType Type => HazardType.Tantrum;

        /// <summary>
        /// Gets the decay speed.
        /// </summary>
        public float DecaySpeed => Base.DecaySpeed;

        /// <summary>
        /// Gets distance of explode.
        /// </summary>
        public float ExplodeDistance => Base._explodeDistance;

        /// <summary>
        /// Gets or sets the ignored targets.
        /// </summary>
        public IEnumerable<Player> IgnoredTargets
        {
            get => Base.IgnoredTargets.Select(Player.Get);
            set => Base.IgnoredTargets = value.Select(x => x.ReferenceHub).ToList();
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
        /// Places a Prismatic (Halloween's ability) in the indicated position.
        /// </summary>
        /// <param name="position">The position where you want to spawn the Tantrum.</param>
        /// <param name="isActive">Whether or not the tantrum will apply the <see cref="EffectType.Prismatic"/> effect.</param>
        /// <remarks>If <paramref name="isActive"/> is <see langword="true"/>, the tantrum is moved slightly up from its original position. Otherwise, the collision will not be detected and the slowness will not work.</remarks>
        /// <returns>The <see cref="TantrumHazard"/> instance.</returns>
        public static PrismaticCloudHazard PlaceTantrum(Vector3 position, bool isActive = true)
        {
            PrismaticCloud prismatic = Object.Instantiate(PrefabHelper.GetPrefab<PrismaticCloud>(PrefabType.PrismaticCloud));

            if (!isActive)
                prismatic.SynchronizedPosition = new(position);
            else
                prismatic.SynchronizedPosition = new(position + (Vector3.up * 0.25f));

            prismatic._destroyed = !isActive;

            NetworkServer.Spawn(prismatic.gameObject);

            return Get<PrismaticCloudHazard>(prismatic);
        }

        /// <summary>
        /// Enables effects for target.
        /// </summary>
        /// <param name="player">Target to affect.</param>
        public void EnableEffects(Player player) => Base.ServerEnableEffect(player.ReferenceHub);
    }
}