// -----------------------------------------------------------------------
// <copyright file="VoiceLinesEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;

    using Interfaces;

    using static PlayerRoles.PlayableScps.Scp3114.Scp3114VoiceLines;

    /// <summary>
    /// Contains all information prior to sending voiceline SCP-3114.
    /// </summary>
    public class VoiceLinesEventArgs : IScp3114Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceLinesEventArgs" /> class.
        /// </summary>
        /// <param name="scp3114VoiceLines"><inheritdoc cref="Scp3114VoiceLines" /> </param>
        /// <param name="voiceLineDefinition"><inheritdoc cref="VoiceLine" /> </param>
        /// <param name="clipId"><inheritdoc cref="ClipId" /> </param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed" /></param>
        public VoiceLinesEventArgs(PlayerRoles.PlayableScps.Scp3114.Scp3114VoiceLines scp3114VoiceLines, VoiceLinesDefinition voiceLineDefinition, byte clipId, bool isAllowed = true)
        {
            Scp3114VoiceLines = scp3114VoiceLines;
            Player = Player.Get(scp3114VoiceLines.Owner);
            Scp3114 = Player.Role.As<Scp3114Role>();
            VoiceLine = voiceLineDefinition;
            ClipId = clipId;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }

        /// <summary>
        /// Gets the <see cref="PlayerRoles.PlayableScps.Scp3114.Scp3114VoiceLines" />.
        /// </summary>
        public PlayerRoles.PlayableScps.Scp3114.Scp3114VoiceLines Scp3114VoiceLines { get; }

        /// <summary>
        /// Gets or sets the <see cref="VoiceLinesDefinition" />.
        /// </summary>
        public VoiceLinesDefinition VoiceLine { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PlayerRoles.PlayableScps.Scp3114.Scp3114VoiceLines.VoiceLinesName" />.
        /// </summary>
        public VoiceLinesName VoiceLinesName
        {
            get => VoiceLine.Label;
            set
            {
                if (value != VoiceLine.Label)
                    VoiceLine = GetVoiceLinesDefinition(value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="VoiceLinesDefinition" />.
        /// </summary>
        public byte ClipId
        {
            get => (byte)VoiceLine._lastIndex;
            set
            {
                value = (byte)(value % VoiceLine._order.Count);
                VoiceLine._lastIndex = value;
            }
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        private VoiceLinesDefinition GetVoiceLinesDefinition(VoiceLinesName lineToPlay)
        {
            foreach (VoiceLinesDefinition voiceLinesDefinition in Scp3114VoiceLines._voiceLines)
            {
                if (voiceLinesDefinition.Label == lineToPlay)
                    return voiceLinesDefinition;
            }

            Log.Error($"[{typeof(VoiceLinesEventArgs)}.{nameof(GetVoiceLinesDefinition)}] return Null");
            return null;
        }
    }
}