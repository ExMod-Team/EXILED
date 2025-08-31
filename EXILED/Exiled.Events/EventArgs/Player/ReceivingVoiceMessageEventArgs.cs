// -----------------------------------------------------------------------
// <copyright file="ReceivingVoiceMessageEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------using System;

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles.Voice;
    using VoiceChat.Networking;

    using IVoiceRole = API.Features.Roles.IVoiceRole;

    /// <summary>
    /// Contains all information before player receiving a voice message.
    /// </summary>
    public class ReceivingVoiceMessageEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingVoiceMessageEventArgs" /> class.
        /// </summary>
        /// <param name="voiceMessage">
        /// <inheritdoc cref="VoiceMessage" />
        /// </param>
        /// <param name="receiver">
        /// The player receiving the voice message.
        /// </param>
        public ReceivingVoiceMessageEventArgs(VoiceMessage voiceMessage, ReferenceHub receiver)
        {
            Player = Player.Get(receiver);
            VoiceMessage = voiceMessage;
            Sender = Player.Get(voiceMessage.Speaker);

            if (Sender.Role is IVoiceRole iVR)
            {
                VoiceModule = iVR.VoiceModule;
            }
        }

        /// <summary>
        /// Gets the player receiving the voice message.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player sended the voice message.
        /// </summary>
        public Player Sender { get; }

        /// <summary>
        /// Gets or sets the <see cref="Sender"/>'s <see cref="VoiceMessage" />.
        /// </summary>
        public VoiceMessage VoiceMessage { get; set; }

        /// <summary>
        /// Gets the <see cref="Sender"/>'s <see cref="VoiceModuleBase" />.
        /// </summary>
        public VoiceModuleBase VoiceModule { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can receive the voice message.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
