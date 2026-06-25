// -----------------------------------------------------------------------
// <copyright file="PlayerVoiceSource.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.PcmSources
{
    using System.Buffers;
    using System.Collections.Generic;

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
        private readonly Queue<float> pcmQueue;

        private float[] decodeBuffer;

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
            pcmQueue = new Queue<float>();
            decodeBuffer = ArrayPool<float>.Shared.Rent(VoiceChatSettings.PacketSizePerChannel);

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
        public bool BlockOriginalVoice { get; set; } = false;

        /// <summary>
        /// Gets the metadata of the streaming track.
        /// </summary>
        public TrackData TrackInfo { get; }

        /// <summary>
        /// Gets the total duration of the audio in seconds.
        /// </summary>
        public double TotalDuration => double.PositiveInfinity;

        /// <summary>
        /// Gets or sets the current playback position in seconds.
        /// </summary>
        public double CurrentTime
        {
            get => 0.0;
            set => Seek(value);
        }

        /// <summary>
        /// Gets a value indicating whether the end of the stream has been reached.
        /// </summary>
        public bool Ended => sourcePlayer?.GameObject == null;

        /// <summary>
        /// Reads PCM data from the stream into the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to fill with PCM data.</param>
        /// <param name="offset">The offset in the buffer at which to begin writing.</param>
        /// <param name="count">The maximum number of samples to read.</param>
        /// <returns>The number of samples read.</returns>
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
        public void Seek(double seconds)
        {
            Log.Info("[PlayerVoiceSource] Seeking is not supported for live player voice streams.");
        }

        /// <inheritdoc/>
        public void Reset()
        {
            Log.Info("[PlayerVoiceSource] Resetting is not supported for live player voice streams.");
        }

        /// <summary>
        /// Releases all resources used by the <see cref="PlayerVoiceSource"/>.
        /// </summary>
        public void Dispose()
        {
            LabApi.Events.Handlers.PlayerEvents.SendingVoiceMessage -= OnVoiceChatting;
            decoder?.Dispose();
            if (decodeBuffer != null)
            {
                ArrayPool<float>.Shared.Return(decodeBuffer);
                decodeBuffer = null;
            }
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