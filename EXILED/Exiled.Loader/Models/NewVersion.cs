// -----------------------------------------------------------------------
// <copyright file="NewVersion.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Models
{
    using Exiled.Loader.GHApi.Models;

    /// <summary>
    /// An asset containing all data about a new version.
    /// </summary>
    public readonly struct NewVersion
    {
        /// <summary>
        /// The release.
        /// </summary>
        public readonly Release Release;

        /// <summary>
        /// The asset of the release.
        /// </summary>
        public readonly ReleaseAsset Asset;

        /// <summary>
        /// Indicates if the release is a prerelease.
        /// </summary>
        public readonly bool IsPrerelease;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewVersion"/> struct.
        /// </summary>
        /// <param name="release"><inheritdoc cref="Release"/></param>
        /// <param name="asset"><inheritdoc cref="Asset"/></param>
        /// <param name="isPrerelease"><inheritdoc cref="IsPrerelease"/></param>
        public NewVersion(Release release, ReleaseAsset asset, bool isPrerelease)
        {
            Release = release;
            Asset = asset;
            IsPrerelease = isPrerelease;
        }
    }
}