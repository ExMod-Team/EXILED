// -----------------------------------------------------------------------
// <copyright file="DoorTypeExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features.Doors;

    /// <summary>
    /// A set of extensions for <see cref="DoorType"/>.
    /// </summary>
    public static class DoorTypeExtensions
    {
        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is a gate.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is a gate.</returns>
        public static bool IsGate(this DoorType door) => door is DoorType.GateA or DoorType.GateB or DoorType.Scp914Gate or DoorType.Scp173Gate or DoorType.GR18Gate
            or DoorType.CheckpointGateA or DoorType.CheckpointGateB or DoorType.UnknownGate or DoorType.Scp173NewGate or DoorType.SurfaceGate or DoorType.ElevatorGateA
            or DoorType.ElevatorGateB;

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is a checkpoint.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is a checkpoint.</returns>
        public static bool IsCheckpoint(this DoorType door) => door is DoorType.CheckpointLczA or DoorType.CheckpointLczB or DoorType.CheckpointGateA or DoorType.CheckpointGateB
            or DoorType.CheckpointEzHczA or DoorType.CheckpointEzHczB or DoorType.CheckpointArmoryA or DoorType.CheckpointArmoryB;

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an elevator.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an elevator.</returns>
        public static bool IsElevator(this DoorType door) => door is DoorType.ElevatorGateA or DoorType.ElevatorGateB or DoorType.ElevatorNuke or DoorType.ElevatorScp049
            or DoorType.ElevatorLczA or DoorType.ElevatorLczB or DoorType.ElevatorServerRoom or DoorType.UnknownElevator;

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an HID door.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an HID door.</returns>
        public static bool IsHID(this DoorType door) => door is DoorType.HIDChamber or DoorType.HIDLab;

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an SCP-related door.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an SCP-related door.</returns>
        public static bool IsScp(this DoorType door) => door is DoorType.Scp914Door or DoorType.Scp049Gate or DoorType.Scp049Armory or DoorType.Scp079First or DoorType.Scp079Second
            or DoorType.Scp096 or DoorType.Scp079Armory or DoorType.Scp106Primary or DoorType.Scp106Secondary or DoorType.Scp173Gate or DoorType.Scp173Connector or DoorType.Scp173Armory
            or DoorType.Scp173Bottom or DoorType.Scp914Gate or DoorType.Scp939Cryo or DoorType.Scp330 or DoorType.Scp330Chamber or DoorType.Scp173NewGate;

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an escape door.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an escape door.</returns>
        public static bool IsEscape(this DoorType door) => door is DoorType.EscapeFinal or DoorType.EscapeSecondary or DoorType.EscapePrimary;

        /// <summary>
        /// Checks if a <see cref="DoorType"/> is located in the Light Containment Zone (LCZ).
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns <c>true</c> if the <see cref="DoorType"/> is a door from LCZ; otherwise, <c>false</c>.</returns>
        public static bool IsLCZ(this DoorType door) => Door.Get(door).Zone == ZoneType.LightContainment;

        /// <summary>
        /// Checks if a <see cref="DoorType"/> is located in the Heavy Containment Zone (HCZ).
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns <c>true</c> if the <see cref="DoorType"/> is a door from HCZ; otherwise, <c>false</c>.</returns>
        public static bool IsHCZ(this DoorType door) => Door.Get(door).Zone == ZoneType.HeavyContainment;

        /// <summary>
        /// Checks if a <see cref="DoorType"/> is located in the Entrance Zone (EZ).
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns <c>true</c> if the <see cref="DoorType"/> is a door from EZ; otherwise, <c>false</c>.</returns>
        public static bool IsEZ(this DoorType door) => Door.Get(door).Zone == ZoneType.Entrance;

        /// <summary>
        /// Checks if a <see cref="DoorType"/> is located on the Surface.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns <c>true</c> if the <see cref="DoorType"/> is a door from Surface; otherwise, <c>false</c>.</returns>
        public static bool IsSurface(this DoorType door) => Door.Get(door).Zone == ZoneType.Surface;

        /// <summary>
        /// Checks if a <see cref="DoorType"/> is of an unknown type.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns <c>true</c> if the <see cref="DoorType"/> is unknown; otherwise, <c>false</c>.</returns>
        public static bool IsUnknown(this DoorType door) => door is DoorType.UnknownGate or DoorType.UnknownDoor or DoorType.UnknownElevator;
    }
}