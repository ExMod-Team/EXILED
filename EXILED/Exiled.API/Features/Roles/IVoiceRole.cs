// -----------------------------------------------------------------------
// <copyright file="IVoiceRole.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles.Voice;

    /// <summary>
    /// Interface for all roles with <see cref="VoiceModuleBase"/>.
    /// </summary>
    public interface IVoiceRole
    {
        /// <summary>
        /// Gets the <see cref="VoiceModuleBase"/> of the role.
        /// </summary>
        public VoiceModuleBase VoiceModule { get; }
    }
}