// -----------------------------------------------------------------------
// <copyright file="GainedExperienceEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp127
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Firearms.Modules.Scp127;

    /// <summary>
    /// Contains all information before SCP-127 gains experience.
    /// </summary>
    public class GainedExperienceEventArgs : IScp127Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GainedExperienceEventArgs"/> class.
        /// </summary>
        /// <param name="scp127"><inheritdoc cref="Scp127"/></param>
        /// <param name="experience"><inheritdoc cref="Experience"/></param>
        public GainedExperienceEventArgs(Scp127 scp127, float experience)
        {
            Scp127 = scp127;
            Experience = experience;
            Tier = Scp127.TierManagerModule.GetTierForExp(Experience + Scp127.Experience);
        }

        /// <inheritdoc />
        public Player Player => Scp127.Owner;

        /// <inheritdoc />
        public Item Item => Scp127;

        /// <inheritdoc />
        public Scp127 Scp127 { get; }

        /// <summary>
        /// Gets the experience that SCP-127 has gained.
        /// </summary>
        public float Experience { get; }

        /// <summary>
        /// Gets the tier.
        /// </summary>
        public Scp127Tier Tier { get; }
    }
}