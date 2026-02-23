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
        /// Unknown aspect ratio.
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
        /// Prevent from being target of Scp049.
        /// </summary>
        CanScp049Sense = 4,

        /// <summary>
        /// Prevent Player to be Alarmed by Scp079.
        /// </summary>
        NotAffectedByScp079Scan = 8,
    }
}
