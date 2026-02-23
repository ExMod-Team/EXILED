// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Utility
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using API.Interfaces;
    using Decals;
    using Exiled.Utility.Enums;
    using PlayerRoles;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        public bool Debug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-173 can be blocked by the tutorial.
        /// </summary>
        [Description("Indicates whether SCP-173 can be blocked by the tutorial")]
        public Dictionary<string, NewEnumForAllStuffThatWasAboutTutorial> CanTutorialBlockScp173 { get; set; } =
            Events.Events.Instance.Config.CanTutorialBlockScp173 ?
            new()
            {
                { RoleTypeId.Tutorial.ToString(), NewEnumForAllStuffThatWasAboutTutorial.CanTutorialBlockScp173 },
            }
            : new() { };

        /*
        /// <summary>
        /// Gets or sets a value indicating whether SCP-096 can be triggered by the tutorial.
        /// </summary>
        [Description("Indicates whether SCP-096 can be triggered by the tutorial")]
        public bool CanTutorialTriggerScp096 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether SCP-049 can activate the sense ability on tutorials.
        /// </summary>
        [Description("Indicates whether SCP-049 can sense tutorial players")]
        public bool CanScp049SenseTutorial { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether tutorial is affected by SCP-079 scan.
        /// </summary>
        [Description("Indicates whether tutorial is affected by SCP-079 scan.")]
        public bool TutorialNotAffectedByScp079Scan { get; set; } = false;
        */

        /// <summary>
        /// Gets or sets a value indicating whether flashbangs flash original thrower.
        /// </summary>
        [Description("Indicates whether flashbangs flash original thrower.")]
        public bool CanFlashbangsAffectThrower { get; set; } = Events.Events.Instance.Config.CanFlashbangsAffectThrower;

        /// <summary>
        /// Gets or sets a value indicating whether the inventory should be dropped before being set as spectator, through commands or plugins.
        /// </summary>
        [Description("Indicates whether the inventory should be dropped before being set as spectator, through commands or plugins")]
        public bool ShouldDropInventory { get; set; } = Events.Events.Instance.Config.ShouldDropInventory;

        /// <summary>
        /// Gets or sets a value indicating whether the blood can be spawned.
        /// </summary>
        [Description("Indicates whether the Decal (Blood, Bullet, Buckshot, GlassCrack) can be spawned")]
        public List<DecalPoolType> PreventDecalSpawn { get; set; } = Events.Events.Instance.Config.CanSpawnBlood ? new() { DecalPoolType.Blood } : new() { };

        /// <summary>
        /// Gets or sets a value indicating whether the SCP079 will recontained if there are no SCPs left.
        /// </summary>
        [Description("Indicates whether the SCP079 will recontained if there are no SCPs left.")]
        public bool RecontainScp079IfNoScpsLeft { get; set; } = Events.Events.Instance.Config.RecontainScp079IfNoScpsLeft;
    }
}
