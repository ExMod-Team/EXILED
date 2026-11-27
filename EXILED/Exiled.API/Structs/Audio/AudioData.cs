// -----------------------------------------------------------------------
// <copyright file="AudioData.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs.Audio
{
    /// <summary>
    /// Represents raw audio data and its associated metadata.
    /// </summary>
    public struct AudioData
    {
        /// <summary>
        /// Gets the raw PCM audio samples.
        /// </summary>
        public float[] Pcm;

        /// <summary>
        /// Gets the metadata of the audio track, including its total duration.
        /// </summary>
        public TrackData TrackInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioData"/> struct.
        /// </summary>
        /// <param name="pcmData">The raw PCM float array containing the audio data.</param>
        /// <param name="trackInfo">The metadata associated with the audio track.</param>
        public AudioData(float[] pcmData, TrackData trackInfo)
        {
            Pcm = pcmData;
            TrackInfo = trackInfo;
        }
    }
}