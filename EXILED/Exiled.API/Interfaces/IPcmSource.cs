// -----------------------------------------------------------------------
// <copyright file="IPcmSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using System;

    /// <summary>
    /// Represents a source of PCM audio data.
    /// </summary>
    public interface IPcmSource : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the end of the PCM source has been reached.
        /// </summary>
        bool Ended { get; }

        /// <summary>
        /// Gets the total duration of the audio in seconds.
        /// </summary>
        double TotalDuration { get; }

        /// <summary>
        /// Gets or sets the current playback position in seconds.
        /// </summary>
        double CurrentTime { get; set; }

        /// <summary>
        /// Reads a sequence of PCM samples into the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read the samples into.</param>
        /// <param name="offset">The zero-based index in the buffer at which to begin storing the data read from the source.</param>
        /// <param name="count">The maximum number of samples to read.</param>
        /// <returns>The total number of samples read into the buffer.</returns>
        int Read(float[] buffer, int offset, int count);

        /// <summary>
        /// Seeks to the specified position in the PCM source.
        /// </summary>
        /// <param name="seconds">The position in seconds to seek to.</param>
        void Seek(double seconds);

        /// <summary>
        /// Resets the PCM source to its initial state, allowing reading from the beginning.
        /// </summary>
        void Reset();
    }
}
