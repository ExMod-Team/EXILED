// -----------------------------------------------------------------------
// <copyright file="TrackData.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs.Audio
{
    using System;

    /// <summary>
    /// Contains metadata about a audio track.
    /// </summary>
    public struct TrackData
    {
        /// <summary>
        /// Gets the title of the track, if available in the metadata.
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// Gets the artist of the track, if available in the metadata.
        /// </summary>
        public string Artist { get; internal set; }

        /// <summary>
        /// Gets the total duration of the track in seconds.
        /// </summary>
        public double Duration { get; internal set; }

        /// <summary>
        /// Gets the file path of the track.
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the track data is completely empty.
        /// </summary>
        public readonly bool IsEmpty => string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(Artist) && Duration <= 0;

        /// <summary>
        /// Gets a formatted display name for the track.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(Title))
                    return $"{Artist} - {Title}";

                if (!string.IsNullOrEmpty(Title))
                    return Title;

                if (!string.IsNullOrEmpty(Path))
                    return System.IO.Path.GetFileNameWithoutExtension(Path);

                return "Unknown Track";
            }
        }

        /// <summary>
        /// Gets the duration formatted as a digital clock string.
        /// </summary>
        public readonly string FormattedDuration
        {
            get
            {
                TimeSpan t = TimeSpan.FromSeconds(Duration);
                return t.Hours > 0 ? t.ToString(@"hh\:mm\:ss") : t.ToString(@"mm\:ss");
            }
        }
    }
}
