// -----------------------------------------------------------------------
// <copyright file="QueuedTrack.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs.Audio
{
    using System;

    using Exiled.API.Interfaces.Audio;

    /// <summary>
    /// Represents a track waiting in the queue, along with its specific playback options.
    /// </summary>
    public readonly struct QueuedTrack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedTrack"/> struct.
        /// </summary>
        /// <param name="name">The name, path, or identifier of the track (used for displaying or removing from queue).</param>
        /// <param name="sourceFactory">A function that returns the instantiated <see cref="IPcmSource"/>.</param>
        public QueuedTrack(string name, Func<IPcmSource> sourceFactory)
        {
            Name = name;
            SourceProvider = sourceFactory;
        }

        /// <summary>
        /// Gets the name, path, or identifier of the track.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the provider function used to create the custom audio source on demand.
        /// </summary>
        public Func<IPcmSource> SourceProvider { get; }
    }
}