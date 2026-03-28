// -----------------------------------------------------------------------
// <copyright file="TalkingEventArgs.cs" company="ExMod Team">
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
    /// Contains all information before SCP-127 voice line is played.
    /// </summary>
    public class TalkingEventArgs : IScp127Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TalkingEventArgs"/> class.
        /// </summary>
        /// <param name="scp127"><inheritdoc cref="Scp127"/></param>
        /// <param name="voiceLine"><inheritdoc cref="VoiceLine"/></param>
        /// <param name="voiceLinePriority"><inheritdoc cref="Priority"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public TalkingEventArgs(Scp127 scp127, Scp127VoiceLinesTranslation voiceLine, Scp127VoiceTriggerBase.VoiceLinePriority voiceLinePriority, bool isAllowed = true)
        {
            Scp127 = scp127;
            VoiceLine = voiceLine;
            Priority = voiceLinePriority;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player => Scp127.Owner;

        /// <inheritdoc/>
        public Item Item => Scp127;

        /// <inheritdoc/>
        public Scp127 Scp127 { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets a voice line which is played.
        /// </summary>
        public Scp127VoiceLinesTranslation VoiceLine { get; set; }

        /// <summary>
        /// Gets or sets a priority for this play.
        /// </summary>
        public Scp127VoiceTriggerBase.VoiceLinePriority Priority { get; set; }
    }
}