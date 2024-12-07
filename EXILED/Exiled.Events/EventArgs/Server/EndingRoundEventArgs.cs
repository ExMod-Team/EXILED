// -----------------------------------------------------------------------
// <copyright file="EndingRoundEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using API.Enums;
    using Interfaces;

    /// <summary>
    /// Contains all information before ending a round.
    /// </summary>
    public class EndingRoundEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndingRoundEventArgs" /> class.
        /// </summary>
        /// <param name="classList">
        /// <inheritdoc cref="RoundSummary.SumInfo_ClassList" />
        /// </param>
        /// <param name="isForceEnded">
        /// <inheritdoc cref="IsForceEnded" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public EndingRoundEventArgs(RoundSummary.SumInfo_ClassList classList, bool isForceEnded, bool isAllowed)
        {
            ClassList = classList;
            LeadingTeam = GetLeadingTeam(classList);
            IsForceEnded = isForceEnded;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the round summary class list.
        /// </summary>
        public RoundSummary.SumInfo_ClassList ClassList { get; set; }

        /// <summary>
        /// Gets or sets the leading team.
        /// </summary>
        public LeadingTeam LeadingTeam { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the round is ended by API call.
        /// </summary>
        public bool IsForceEnded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the round is going to finish or not.
        /// </summary>
        public bool IsAllowed { get; set; }

        private LeadingTeam GetLeadingTeam(RoundSummary.SumInfo_ClassList classList)
        {
            // NW logic
            int facilityForces = classList.mtf_and_guards + classList.scientists;
            int chaosInsurgency = classList.chaos_insurgents + classList.class_ds;
            int anomalies = classList.scps_except_zombies + classList.zombies;
            int num4 = facilityForces > 0 ? 1 : 0;
            bool flag1 = chaosInsurgency > 0;
            bool flag2 = anomalies > 0;
            RoundSummary.LeadingTeam leadingTeam = RoundSummary.LeadingTeam.Draw;
            if (num4 != 0)
                leadingTeam = RoundSummary.EscapedScientists >= RoundSummary.EscapedClassD ? RoundSummary.LeadingTeam.FacilityForces : RoundSummary.LeadingTeam.Draw;
            else if (flag2 || flag2 & flag1)
                leadingTeam = RoundSummary.EscapedClassD > RoundSummary.SurvivingSCPs ? RoundSummary.LeadingTeam.ChaosInsurgency : (RoundSummary.SurvivingSCPs > RoundSummary.EscapedScientists ? RoundSummary.LeadingTeam.Anomalies : RoundSummary.LeadingTeam.Draw);
            else if (flag1)
                leadingTeam = RoundSummary.EscapedClassD >= RoundSummary.EscapedScientists ? RoundSummary.LeadingTeam.ChaosInsurgency : RoundSummary.LeadingTeam.Draw;

            return (LeadingTeam)leadingTeam;
        }
    }
}