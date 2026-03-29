// -----------------------------------------------------------------------
// <copyright file="SendingCassieMessageEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Cassie
{
    using System;
    using System.Text;

    using Exiled.API.Features.Pools;
    using global::Cassie;
    using Interfaces;
    using Subtitles;

    /// <summary>
    /// Contains all the information after sending a C.A.S.S.I.E. message.
    /// </summary>
    public class SendingCassieMessageEventArgs : IDeniableEvent
    {
        private readonly CassieAnnouncement announcement;
        private readonly CassieTtsPayload payload;

        private string customSubtitles;
        private float glitchScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendingCassieMessageEventArgs" /> class.
        /// </summary>
        /// <param name="annc">The announcement to populate all properties from.</param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed"/>
        /// </param>
        public SendingCassieMessageEventArgs(CassieAnnouncement annc, bool isAllowed = true)
        {
            announcement = annc;
            payload = annc.Payload;

            Words = payload.Content;
            switch (payload.SubtitleSource)
            {
                case CassieTtsPayload.SubtitleMode.None:
                case CassieTtsPayload.SubtitleMode.Automatic:
                    CustomSubtitles = string.Empty;
                    break;
                case CassieTtsPayload.SubtitleMode.Custom:
                    CustomSubtitles = payload._customSubtitle;
                    break;
                case CassieTtsPayload.SubtitleMode.FromTranslation:
                    StringBuilder builder = StringBuilderPool.Pool.Get();
                    SubtitleController controller = SubtitleController.Singleton;

                    foreach (SubtitlePart part in payload._subtitleMessage.SubtitleParts)
                    {
                        Subtitle subtitle = controller.Subtitles[part.Subtitle];
                        builder.Append(controller.GetTranslation(subtitle));
                    }

                    CustomSubtitles = StringBuilderPool.Pool.ToStringReturn(builder);

                    break;
                default:
                    CustomSubtitles = string.Empty;
                    break;
            }

            MakeHold = payload.PlayBackground;
            GlitchScale = annc.GlitchScale;
            MakeNoise = annc.GlitchScale is not 0;
            SubtitleSource = payload.SubtitleSource;

            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Words { get; set; }

        /// <summary>
        /// Gets or sets the message subtitles.
        /// </summary>
        public string CustomSubtitles
        {
            get => customSubtitles;
            set
            {
                if (customSubtitles != value)
                    SubtitleSource = CassieTtsPayload.SubtitleMode.Custom;

                customSubtitles = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the message should be held.
        /// </summary>
        public bool MakeHold { get; set; }

        /// <summary>
        /// Gets or sets a value controlling how glitchy this CASSIE message is.
        /// </summary>
        public float GlitchScale
        {
            get => glitchScale;
            set
            {
                if (!MakeNoise && value is not 0)
                    MakeNoise = true;

                glitchScale = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the message should make noise.
        /// </summary>
        public bool MakeNoise { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message can be sent.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed.
        /// </summary>
        [Obsolete("Useless and will be removed in Exiled 10.")]
        public bool IsCustomAnnouncement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating where the subtitles for this message came from.
        /// </summary>
        public CassieTtsPayload.SubtitleMode SubtitleSource { get; set; }

        /// <summary>
        /// Gets a <see cref="CassieAnnouncement"/> consisting of all properties in this event.
        /// </summary>
        public CassieAnnouncement Announcement
        {
            get
            {
                CassieTtsPayload newPayload;

                // I love readonly fields :)
                if (SubtitleSource is CassieTtsPayload.SubtitleMode.FromTranslation)
                {
                    newPayload = new CassieTtsPayload(Words, MakeHold, payload._subtitleMessage.SubtitleParts);
                }
                else
                {
                    if (SubtitleSource is CassieTtsPayload.SubtitleMode.Automatic)
                        newPayload = new CassieTtsPayload(Words, true, MakeHold);
                    else
                        newPayload = new CassieTtsPayload(Words, CustomSubtitles, MakeHold);
                }

                return announcement switch
                {
                    CassieScpTerminationAnnouncement =>

                        // this is disabled via patch b/c termination messages are not modifiable at the stage the SendCassieMessage patch is in.
                        throw new InvalidOperationException("SendCassieMessage was called for a SCP termination message!"),

                    CassieWaveAnnouncement waveAnnc => new CassieWaveAnnouncement(waveAnnc.Wave, newPayload),
                    Cassie079RecontainAnnouncement recontainAnnc => new Cassie079RecontainAnnouncement(recontainAnnc._callback, false, newPayload),
                    _ => new CassieAnnouncement(newPayload, 0, GlitchScale / (API.Features.Warhead.IsDetonated ? 2F : 1F) * (MakeNoise ? 1F : 0F)),
                };
            }
        }
    }
}