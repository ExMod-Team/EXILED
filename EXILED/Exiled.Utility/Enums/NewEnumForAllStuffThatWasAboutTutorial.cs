// -----------------------------------------------------------------------
// <copyright file="NewEnumForAllStuffThatWasAboutTutorial.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Utility.Enums
{
    using System;

    /// <summary>
    /// All available screen aspect ratio types.
    /// </summary>
    [Flags]
    public enum NewEnumForAllStuffThatWasAboutTutorial
    {
        /// <summary>
        /// All.
        /// </summary>
        All = -1,

        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Prevent from blocking Scp173.
        /// </summary>
        CanBlockScp173 = 1,

        /// <summary>
        /// Prevent from being target of Scp096.
        /// </summary>
        CanTriggerScp096 = 2,

        /// <summary>
        /// Prevent from being target of Scp049 and stagger Scp0492.
        /// </summary>
        CanScp049Sense = 4,

        /// <summary>
        /// Prevent from being target of Scp049 and stagger Scp0492.
        /// </summary>
        CanScp0492Sense = 4,

        /// <summary>
        /// Prevent Player to be Alarmed by Scp079.
        /// </summary>
        NotAffectedByScp079Scan = 16,
    }
}
