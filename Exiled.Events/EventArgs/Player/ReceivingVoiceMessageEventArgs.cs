// -----------------------------------------------------------------------
// <copyright file="ReceivingVoiceMessageEventArgs.cs" company="ExMod Team">
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
    /// Contains all information before a player receives a voice message.
    /// </summary>
    public class ReceivingVoiceMessageEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingVoiceMessageEventArgs" /> class.
        /// </summary>
        /// <param name="receiver">The player receiving the voice message.</param>
        /// <param name="sender">The player sending the voice message.</param>
        /// <param name="voiceModule">The senders voice module.</param>
        /// <param name="voiceMessage">The voice message being sent.</param>
        public ReceivingVoiceMessageEventArgs(Player receiver, Player sender, VoiceModuleBase voiceModule, VoiceMessage voiceMessage)
        {
            Sender = sender;
            Player = receiver;
            VoiceMessage = voiceMessage;
            VoiceModule = voiceModule;
        }

        /// <summary>
        /// Gets the player receiving the voice message.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player sending the voice message.
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
