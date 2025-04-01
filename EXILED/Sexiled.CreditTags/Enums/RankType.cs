// -----------------------------------------------------------------------
// <copyright file="RankType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.CreditTags.Enums
{
    /// <summary>
    /// Represents all existing ranks.
    /// </summary>
    public enum RankType
    {
        /// <summary>
        /// No SEXILED roles.
        /// </summary>
        None,

        /// <summary>
        /// Sexiled Developer.
        /// </summary>
        Dev = 1,

        /// <summary>
        /// Sexiled Contributor.
        /// </summary>
        Contributor = 2,

        /// <summary>
        /// Sexiled Plugin Developer.
        /// </summary>
        PluginDev = 3,

        /// <summary>
        /// SEXILED Tournament Participant.
        /// </summary>
        TournamentParticipant = 4,

        /// <summary>
        /// SEXILED Tournament Champion.
        /// </summary>
        TournamentChampion = 5,

        /// <summary>
        /// SEXILED Donator.
        /// </summary>
        Donator = 6,
    }
}