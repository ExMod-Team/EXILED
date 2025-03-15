// -----------------------------------------------------------------------
// <copyright file="Message.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.ComponentModel;

    using Exiled.API.Enums;

    /// <summary>
    /// A useful class for saving type-selective message configurations.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        public Message()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="content">The content of the message.</param>
        /// <param name="duration">The duration of the message, in seconds.</param>
        /// <param name="show">Whether the message should be shown.</param>
        /// <param name="type">The type of the message.</param>
        public Message(string content, ushort duration = 10, bool show = true, MessageType type = MessageType.Broadcast)
        {
            Content = content;
            Duration = duration;
            Show = show;
            Type = type;
        }

        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        [Description("The message content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the message duration.
        /// </summary>
        [Description("The message duration")]
        public ushort Duration { get; set; }

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        [Description("The message type")]
        public MessageType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message should be shown.
        /// </summary>
        [Description("Indicates whether the message should be shown")]
        public bool Show { get; set; }

        /// <summary>
        /// Returns the Message in a human-readable format.
        /// </summary>
        /// <returns>A string containing Message-related data.</returns>
        public override string ToString() => $"({Content}) {Duration} {Type}";
    }
}