// -----------------------------------------------------------------------
// <copyright file="IAsyncPcmSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces.Audio
{
    /// <summary>
    /// Represents an audio source that loads its data asynchronously and can potentially fail.
    /// </summary>
    public interface IAsyncPcmSource
    {
        /// <summary>
        /// Gets a value indicating whether the asynchronous source has finished loading/buffering and is ready to be played.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Gets a value indicating whether the source failed to load.
        /// </summary>
        bool IsFailed { get; }
    }
}