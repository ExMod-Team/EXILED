// -----------------------------------------------------------------------
// <copyright file="ElevatorArrivedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features;
    using Interactables.Interobjects;

    /// <summary>
    /// Contains all the information for when an elevator arrives at its destination.
    /// </summary>
    public class ElevatorArrivedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatorArrivedEventArgs"/> class.
        /// </summary>
        /// <param name="elevatorChamber"><inherit cref="ElevatorChamber"/></param>
        /// <param name="level"><inherit cref="ElevatorChamber.DestinationLevel"/></param>
        public ElevatorArrivedEventArgs(ElevatorChamber elevatorChamber, int level)
        {
            Elevator = elevatorChamber;
            Lift = Lift.Get(elevatorChamber);
            Level = level;
        }

        /// <summary>
        /// Gets the elevator chamber.
        /// </summary>
        public ElevatorChamber Elevator { get; }

        /// <summary>
        /// Gets the lift.
        /// </summary>
        public Lift Lift { get; }

        /// <summary>
        /// Gets the level the elevator arrived at.
        /// </summary>
        public int Level { get; }
    }
}