// -----------------------------------------------------------------------
// <copyright file="SendingVoiceMessageEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles.Voice;
    using VoiceChat.Networking;

    using IVoiceRole = API.Features.Roles.IVoiceRole;

    /// <summary>
    /// Contains all information before a player sending a voice message.
    /// </summary>
    public class SendingVoiceMessageEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendingVoiceMessageEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player who's sending the voice message.</param>
        /// <param name="voiceMessage">The voice message being sent.</param>
        /// <param name="isAllowed">Indicates whether the player is allowed to send the voice message.</param>
        public SendingVoiceMessageEventArgs(Player player, VoiceMessage voiceMessage, bool isAllowed)
        {
            Player = player;
            VoiceMessage = voiceMessage;

            if (Player.Role is IVoiceRole iVR)
            {
                VoiceModule = iVR.VoiceModule;
            }

            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's sending voice message.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the <see cref="Player"/>'s <see cref="VoiceMessage" />.
        /// </summary>
        public VoiceMessage VoiceMessage { get; set; }

        /// <summary>
        /// Gets the <see cref="Player"/>'s <see cref="VoiceModuleBase" />.
        /// </summary>
        public VoiceModuleBase VoiceModule { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can send this voice message.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
