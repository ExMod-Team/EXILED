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
        [Description("Indicates whether RoleTypeId or CustomRole(Name or Id) prevented behaviours")]
        public Dictionary<string, NewEnumForAllStuffThatWasAboutTutorial> NewStuffThatWasAboutTutorial { get; set; } = new()
            {
                { RoleTypeId.Tutorial.ToString(), NewEnumForAllStuffThatWasAboutTutorial.None },
            };

        /// <summary>
        /// Gets or sets a value indicating whether flashbangs flash original thrower.
        /// </summary>
        [Description("Indicates whether flashbangs flash original thrower.")]
        public bool CanFlashbangsAffectThrower { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the inventory should be dropped before being set as spectator, through commands or plugins.
        /// </summary>
        [Description("Indicates whether the inventory should be dropped before being set as spectator, through commands or plugins")]
        public bool ShouldDropInventory { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether Decal spawned.
        /// </summary>
        [Description("Indicates whether the Decal (Blood, Bullet, Buckshot, GlassCrack) can be spawned")]
        public List<DecalPoolType> PreventDecalSpawn { get; set; } = new() { };

        /// <summary>
        /// Gets or sets a value indicating whether the SCP079 will recontained if there are no SCPs left.
        /// </summary>
        [Description("Indicates whether the SCP079 will recontained if there are no SCPs left.")]
        public bool RecontainScp079IfNoScpsLeft { get; set; } = true;
    }
}
