// -----------------------------------------------------------------------
// <copyright file="TalkedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp127
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Firearms.Modules.Scp127;

    /// <summary>
    /// Contains all information after SCP-127 voice line is played.
    /// </summary>
    public class TalkedEventArgs : IScp127Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TalkedEventArgs"/> class.
        /// </summary>
        /// <param name="scp127"><inheritdoc cref="Scp127"/></param>
        /// <param name="voiceLine"><inheritdoc cref="VoiceLine"/></param>
        /// <param name="voiceLinePriority"><inheritdoc cref="Priority"/></param>
        public TalkedEventArgs(Scp127 scp127, Scp127VoiceLinesTranslation voiceLine, Scp127VoiceTriggerBase.VoiceLinePriority voiceLinePriority)
        {
            Scp127 = scp127;
            VoiceLine = voiceLine;
            Priority = voiceLinePriority;
        }

        /// <inheritdoc/>
        public Player Player => Scp127.Owner;

        /// <inheritdoc/>
        public Item Item => Scp127;

        /// <inheritdoc/>
        public Scp127 Scp127 { get; }

        /// <summary>
        /// Gets a voice line which is played.
        /// </summary>
        public Scp127VoiceLinesTranslation VoiceLine { get; }

        /// <summary>
        /// Gets a priority for this play.
        /// </summary>
        public Scp127VoiceTriggerBase.VoiceLinePriority Priority { get; }
    }
}