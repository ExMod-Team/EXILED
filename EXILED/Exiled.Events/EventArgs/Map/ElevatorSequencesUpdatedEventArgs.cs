// -----------------------------------------------------------------------
// <copyright file="ElevatorSequencesUpdatedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using Interactables.Interobjects;

    /// <summary>
    /// Contains all information after an elevator sequence is updated.
    /// </summary>
    public class ElevatorSequencesUpdatedEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatorSequencesUpdatedEventArgs"/> class.
        /// </summary>
        /// <param name="elevatorChamber"><inheritdoc cref="ElevatorChamber"/></param>
        /// <param name="sequence"><inheritdoc cref="ElevatorChamber.ElevatorSequence"/></param>
        public ElevatorSequencesUpdatedEventArgs(ElevatorChamber elevatorChamber, ElevatorChamber.ElevatorSequence sequence)
        {
            Elevator = elevatorChamber;
            Lift = Lift.Get(elevatorChamber);
            Sequence = sequence;
        }

        /// <summary>
        /// Gets the elevator chamber that triggered this event.
        /// </summary>
        public ElevatorChamber Elevator { get; }

        /// <summary>
        /// Gets the lift that triggered this event.
        /// </summary>
        public Lift Lift { get; }

        /// <summary>
        /// Gets the new sequence of the elevator.
        /// </summary>
        public ElevatorChamber.ElevatorSequence Sequence { get; }
    }
}