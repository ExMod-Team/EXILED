// -----------------------------------------------------------------------
// <copyright file="Scp127Magazine.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.FirearmModules.Primary
{
    using InventorySystem.Items.Firearms.Modules.Scp127;

    /// <summary>
    /// Represents a normal magazine for SCP-127.
    /// </summary>
    public class Scp127Magazine : NormalMagazine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp127Magazine"/> class.
        /// </summary>
        /// <param name="magazine"><inheritdoc cref="MagazineModule"/></param>
        public Scp127Magazine(Scp127MagazineModule magazine)
            : base(magazine)
        {
            MagazineModule = magazine;
        }

        /// <inheritdoc cref="NormalMagazine.MagazineModule"/>
        public new Scp127MagazineModule MagazineModule { get; }

        /// <summary>
        /// Gets or sets the kill bonus.
        /// </summary>
        public int KillBonus
        {
            get => MagazineModule.KillBonus;
            set => MagazineModule.KillBonus = value;
        }

        /// <summary>
        /// Gets or sets the rank up bonus.
        /// </summary>
        public int RankUpBonus
        {
            get => MagazineModule.RankUpBonus;
            set => MagazineModule.RankUpBonus = value;
        }

        /// <summary>
        /// Gets or sets all settings.
        /// </summary>
        public Scp127MagazineModule.RegenerationSettings[] RegenerationPerTier
        {
            get => MagazineModule._regenerationPerTier;
            set => MagazineModule._regenerationPerTier = value;
        }

        /// <summary>
        /// Gets the current setting.
        /// </summary>
        public Scp127MagazineModule.RegenerationSettings ActiveSetting => MagazineModule.ActiveSettings;

        /// <summary>
        /// Gets or sets a pause in bullets regeneration process.
        /// </summary>
        public float RemainingRegenPause
        {
            get => MagazineModule._remainingRegenPause;
            set => MagazineModule._remainingRegenPause = value;
        }

        /// <summary>
        /// Gets or sets the amount of bullets that should be regenerated.
        /// </summary>
        public float RegenProgress
        {
            get => MagazineModule._regenProgress;
            set => MagazineModule._regenProgress = value;
        }
    }
}