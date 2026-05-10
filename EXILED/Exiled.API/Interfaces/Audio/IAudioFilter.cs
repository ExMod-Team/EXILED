// -----------------------------------------------------------------------
// <copyright file="IAudioFilter.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces.Audio
{
    /// <summary>
    /// Represents a custom filter for the speaker.
    /// </summary>
    public interface IAudioFilter
    {
        /// <summary>
        /// Processes the raw PCM audio frame directly before it is encoded and sending.
        /// </summary>
        /// <param name="frame">The array of PCM audio samples.</param>
        void Process(float[] frame);

        /// <summary>
        /// Resets the internal state and buffers of the filter.
        /// </summary>
        void Reset();
    }
}
