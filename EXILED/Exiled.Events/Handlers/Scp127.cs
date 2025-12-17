// -----------------------------------------------------------------------
// <copyright file="Scp127.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp127;
    using Exiled.Events.Features;
    using LabApi.Events.Arguments.Scp127Events;

#pragma warning disable SA1623
    /// <summary>
    /// SCP-127 event handlers.
    /// </summary>
    public static class Scp127
    {
        /// <summary>
        /// Invoked before SCP-127 voice line is played.
        /// </summary>
        public static Event<TalkingEventArgs> Talking { get; set; } = new();

        /// <summary>
        /// Invoked after SCP-127 voice line is played.
        /// </summary>
        public static Event<TalkedEventArgs> Talked { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-127 gains experience.
        /// </summary>
        public static Event<GainingExperienceEventArgs> GainingExperience { get; set; } = new();

        /// <summary>
        /// Invoked after SCP-127 gains experience.
        /// </summary>
        public static Event<GainedExperienceEventArgs> GainedExperience { get; set; } = new();

        /// <summary>
        /// Called before SCP-127 voice line is played.
        /// </summary>
        /// <param name="ev">The <see cref="Scp127TalkingEventArgs"/> instance.</param>
        public static void OnTalking(Scp127TalkingEventArgs ev)
        {
            TalkingEventArgs eventArgs = new(API.Features.Items.Item.Get<API.Features.Items.Scp127>(ev.Scp127Item.Base), ev.VoiceLine, ev.Priority, ev.IsAllowed);
            Talking.InvokeSafely(eventArgs);

            ev.Priority = eventArgs.Priority;
            ev.VoiceLine = eventArgs.VoiceLine;
            ev.IsAllowed = eventArgs.IsAllowed;
        }

        /// <summary>
        /// Called after SCP-127 voice line is played.
        /// </summary>
        /// <param name="ev">The <see cref="Scp127TalkedEventArgs"/> instance.</param>
        public static void OnTalked(Scp127TalkedEventArgs ev)
            => Talked.InvokeSafely(new(API.Features.Items.Item.Get<API.Features.Items.Scp127>(ev.Scp127Item.Base), ev.VoiceLine, ev.Priority));

        /// <summary>
        /// Called before SCP-127 gains experience.
        /// </summary>
        /// <param name="ev">The <see cref="Scp127GainingExperienceEventArgs"/> instance.</param>
        public static void OnGainingExperience(Scp127GainingExperienceEventArgs ev)
        {
            GainingExperienceEventArgs eventArgs = new(API.Features.Items.Item.Get<API.Features.Items.Scp127>(ev.Scp127Item.Base), ev.ExperienceGain, ev.IsAllowed);
            GainingExperience.InvokeSafely(eventArgs);

            ev.ExperienceGain = eventArgs.Experience;
            ev.IsAllowed = eventArgs.IsAllowed;
        }

        /// <summary>
        /// Called after SCP-127 gains experience.
        /// </summary>
        /// <param name="ev">The <see cref="Scp127GainExperienceEventArgs"/> instance.</param>
        public static void OnGainedExperience(Scp127GainExperienceEventArgs ev) =>
            GainedExperience.InvokeSafely(new(API.Features.Items.Item.Get<API.Features.Items.Scp127>(ev.Scp127Item.Base), ev.ExperienceGain));
    }
}