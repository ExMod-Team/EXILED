// -----------------------------------------------------------------------
// <copyright file="TransmittingEventArgs.cs" company="ExMod Team">
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

    /// <summary>
    /// Contains all information regarding the player using the radio.
    /// </summary>
    public class TransmittingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransmittingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="voiceMessage">
        /// <inheritdoc cref="VoiceMessage" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public TransmittingEventArgs(Player player, VoiceMessage voiceMessage, bool isAllowed)
        {
            Player = player;
            VoiceMessage = voiceMessage;
            if (player.Role is IVoiceRole voiceRole)
                VoiceModule = voiceRole.VoiceModule;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's transmitting.
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
        /// Gets or sets a value indicating whether the player can transmit.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}