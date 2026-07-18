// -----------------------------------------------------------------------
// <copyright file="PlayerVoiceSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.PcmSources
{
    using System.Collections.Concurrent;

    using Exiled.API.Features;
    using Exiled.API.Interfaces.Audio;
    using Exiled.API.Structs.Audio;

    using LabApi.Events.Arguments.PlayerEvents;

    using VoiceChat;
    using VoiceChat.Codec;

    /// <summary>
    /// Provides a <see cref="IPcmSource"/> that captures and decodes live microphone input from a specific player.
    /// </summary>
    public sealed class PlayerVoiceSource : IPcmSource, ILiveSource
    {
        private readonly Player sourcePlayer;
        private readonly OpusDecoder decoder;
        private readonly float[] decodeBuffer;
        private readonly ConcurrentQueue<float> pcmQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerVoiceSource"/> class.
        /// </summary>
        /// <param name="player">The player whose voice will be captured.</param>
        /// <param name="blockOriginalVoice">If <c>true</c>, prevents the player's original voice message's from being heard while broadcasting.</param>
        public PlayerVoiceSource(Player player, bool blockOriginalVoice = false)
        {
            sourcePlayer = player;
            BlockOriginalVoice = blockOriginalVoice;

            decoder = new OpusDecoder();
            pcmQueue = new ConcurrentQueue<float>();
            decodeBuffer = new float[VoiceChatSettings.PacketSizePerChannel];

            TrackInfo = new TrackData
            {
                Path = $"{player.Nickname}-Mic",
                Duration = double.PositiveInfinity,
            };

            LabApi.Events.Handlers.PlayerEvents.SendingVoiceMessage += OnVoiceChatting;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player's original voice chat should be blocked while being broadcasted by this source.
        /// </summary>
        public bool BlockOriginalVoice { get; set; }

        /// <inheritdoc/>
        public TrackData TrackInfo { get; }

        /// <inheritdoc/>
        public double TotalDuration => double.PositiveInfinity;

        /// <inheritdoc/>
        public double CurrentTime
        {
            get => 0.0;
            set => Seek(value);
        }

        /// <inheritdoc/>
        public bool Ended => sourcePlayer?.GameObject == null;

        /// <inheritdoc/>
        public int Read(float[] buffer, int offset, int count)
        {
            if (Ended)
                return 0;

            int read = 0;
            while (read < count && pcmQueue.TryDequeue(out float sample))
            {
                buffer[offset + read] = sample;
                read++;
            }

            return read;
        }

        /// <inheritdoc/>
        public void Seek(double seconds) => Log.Info("[PlayerVoiceSource] Seeking is not supported for live player voice streams.");

        /// <inheritdoc/>
        public void Reset() => Log.Info("[PlayerVoiceSource] Resetting is not supported for live player voice streams.");

        /// <inheritdoc/>
        public void Dispose()
        {
            LabApi.Events.Handlers.PlayerEvents.SendingVoiceMessage -= OnVoiceChatting;
            decoder?.Dispose();
        }

        private void OnVoiceChatting(PlayerSendingVoiceMessageEventArgs ev)
        {
            if (ev.Player != sourcePlayer)
                return;

            if (ev.Message.DataLength <= 2)
                return;

            if (BlockOriginalVoice)
                ev.IsAllowed = false;

            int decodedSamples = decoder.Decode(ev.Message.Data, ev.Message.DataLength, decodeBuffer);

            for (int i = 0; i < decodedSamples; i++)
            {
                pcmQueue.Enqueue(decodeBuffer[i]);
            }
        }
    }
}