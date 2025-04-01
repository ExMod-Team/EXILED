// -----------------------------------------------------------------------
// <copyright file="AutoUpdateFiles.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Loader
{
    using System;

    /// <summary>
    /// Automatically updates with Reference used to generate Sexiled.
    /// </summary>
    public static class AutoUpdateFiles
    {
        /// <summary>
        /// Gets which SCP: SL version generated Sexiled.
        /// </summary>
        public static readonly Version RequiredSCPSLVersion = new(14, 0, 0, 2);
    }
}