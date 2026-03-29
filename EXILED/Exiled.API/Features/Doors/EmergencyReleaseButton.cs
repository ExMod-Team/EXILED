// -----------------------------------------------------------------------
// <copyright file="EmergencyReleaseButton.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Interfaces;
    using Interactables.Interobjects;

    /// <summary>
    /// A wrapper for <see cref="EmergencyReleaseButton"/>.
    /// </summary>
    public class EmergencyReleaseButton : IWrapper<EmergencyDoorRelease>
    {
        /// <summary>
        /// Dictionary containing base object with linked wrapper.
        /// </summary>
        internal static readonly Dictionary<EmergencyDoorRelease, EmergencyReleaseButton> ObjectToWrapper = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmergencyReleaseButton"/> class.
        /// </summary>
        /// <param name="base"><inheritdoc cref="Base"/></param>
        internal EmergencyReleaseButton(EmergencyDoorRelease @base)
        {
            Base = @base;
            Door = Door.Get(Base._controlledDoor);

            ObjectToWrapper[@base] = this;
        }

        /// <summary>
        /// Gets all instaces of <see cref="EmergencyReleaseButton"/>.
        /// </summary>
        public static IReadOnlyCollection<EmergencyReleaseButton> List => ObjectToWrapper.Values;

        /// <inheritdoc />
        public EmergencyDoorRelease Base { get; }

        /// <summary>
        /// Gets the door which will be opened if this button is interacted.
        /// </summary>
        public Door Door { get; }

        /// <summary>
        /// Gets or sets a value indicating whether button can be used.
        /// </summary>
        public bool IsReady
        {
            get => Base._isReady;
            set => Base._isReady = value;
        }

        /// <summary>
        /// Gets or sets default time after which button can be used again.
        /// </summary>
        public float InitialTimer
        {
            get => Base._initialTimer;
            set => Base._initialTimer = value;
        }

        /// <summary>
        /// Gets or sets time after which button can be used again.
        /// </summary>
        public float ReleaseTimer
        {
            get => Base._releaseTimer;
            set => Base._releaseTimer = value;
        }

        /// <summary>
        /// Gets an <see cref="EmergencyReleaseButton"/> by it's base-game instance.
        /// </summary>
        /// <param name="emergencyDoorRelease">The <see cref="EmergencyDoorRelease"/> instance.</param>
        /// <returns>An <see cref="EmergencyReleaseButton"/> instance if found. Otherwise, <c>null</c>.</returns>
        public static EmergencyReleaseButton Get(EmergencyDoorRelease emergencyDoorRelease)
        {
            if (ObjectToWrapper.TryGetValue(emergencyDoorRelease, out EmergencyReleaseButton wrapper))
                return wrapper;

            return new(emergencyDoorRelease);
        }

        /// <summary>
        /// Gets an <see cref="EmergencyReleaseButton"/> by door.
        /// </summary>
        /// <param name="door">Door which is linked with <see cref="EmergencyReleaseButton"/>.</param>
        /// <returns>An <see cref="EmergencyReleaseButton"/> instance if found. Otherwise, <c>null</c>.</returns>
        public static EmergencyReleaseButton Get(Door door) => Get(x => x.Door == door).FirstOrDefault();

        /// <summary>
        /// Gets all <see cref="EmergencyReleaseButton"/> according to the condition.
        /// </summary>
        /// <param name="predicate">Condition to satisfy.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of found instances.</returns>
        public static IEnumerable<EmergencyReleaseButton> Get(Func<EmergencyReleaseButton, bool> predicate) => List.Where(predicate);
    }
}