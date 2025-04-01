// -----------------------------------------------------------------------
// <copyright file="EventPatchAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.Attributes
{
    using System;

    using Sexiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// An attribute to contain data about an event patch.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class EventPatchAttribute : Attribute
    {
        private readonly Type handlerType;
        private readonly string eventName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPatchAttribute"/> class.
        /// </summary>
        /// <param name="eventName">The <see cref="Type"/> of the handler class that contains the event.</param>
        /// <param name="handlerType">The name of the event.</param>
        internal EventPatchAttribute(Type handlerType, string eventName)
        {
            this.handlerType = handlerType;
            this.eventName = eventName;
        }

        /// <summary>
        /// Gets the <see cref="ISexiledEvent"/> that will be raised by this patch.
        /// </summary>
        internal ISexiledEvent Event => (ISexiledEvent)handlerType.GetProperty(eventName)?.GetValue(null);
    }
}