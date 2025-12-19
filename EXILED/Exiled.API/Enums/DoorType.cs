// -----------------------------------------------------------------------
// <copyright file="DoorType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    using Exiled.API.Features.Doors;
    using Interactables.Interobjects;

    /// <summary>
    /// Unique identifier for the different types of doors.
    /// </summary>
    /// <seealso cref="Door.Type"/>
    /// <seealso cref="Door.Get(DoorType)"/>
    public enum DoorType
    {
        /// <summary>
        /// Represents an unknown door.
        /// </summary>
        UnknownDoor = 0,

        /// <summary>
        /// Represents the 914 door.
        /// </summary>
        Scp914Door,

        /// <summary>
        /// Represents the GR18_INNER door.
        /// </summary>
        GR18Inner,

        /// <summary>
        /// Represents the Unsecured door.
        /// </summary>
        Scp049Gate,

        /// <summary>
        /// Represents the 049_ARMORY door.
        /// </summary>
        Scp049Armory,

        /// <summary>
        /// Represents the 079_FIRST door.
        /// </summary>
        Scp079First,

        /// <summary>
        /// Represents the 079_SECOND door.
        /// </summary>
        Scp079Second,

        /// <summary>
        /// Represents the 096 door.
        /// </summary>
        Scp096,

        /// <summary>
        /// Represents the 079_ARMORY door.
        /// </summary>
        Scp079Armory,

        /// <summary>
        /// Represents the 106_PRIMARY door.
        /// </summary>
        Scp106Primary,

        /// <summary>
        /// Represents the 106_SECONDARY door.
        /// </summary>
        Scp106Secondary,

        /// <summary>
        /// Represents the 173_GATE door.
        /// </summary>
        Scp173Gate,

        /// <summary>
        /// Represents the door between the 173 gate and the 173 armory.
        /// </summary>
        Scp173Connector,

        /// <summary>
        /// Represents the 173_ARMORY door.
        /// </summary>
        Scp173Armory,

        /// <summary>
        /// Represents the 173_BOTTOM door.
        /// </summary>
        Scp173Bottom,

        /// <summary>
        /// Represents the GR18 gate.
        /// </summary>
        GR18Gate,

        /// <summary>
        /// Represents the 914 gate.
        /// </summary>
        Scp914Gate,

        /// <summary>
        /// Represents the 939_CRYO door.
        /// </summary>
        Scp939Cryo,

        /// <summary>
        /// Represents the CHECKPOINT_LCZ_A door.
        /// </summary>
        CheckpointLczA,

        /// <summary>
        /// Represents the CHECKPOINT_LCZ_B door.
        /// </summary>
        CheckpointLczB,

        /// <summary>
        /// Represents any entrance zone styled door.
        /// </summary>
        EntranceDoor,

        /// <summary>
        /// Represents the ESCAPE_PRIMARY door.
        /// </summary>
        EscapePrimary,

        /// <summary>
        /// Represents the ESCAPE_SECONDARY door.
        /// </summary>
        EscapeSecondary,

        /// <summary>
        /// Represents the GATE_A door.
        /// </summary>
        GateA,

        /// <summary>
        /// Represents the GATE_B door.
        /// </summary>
        GateB,

        /// <summary>
        /// Represents the HCZ_ARMORY door.
        /// </summary>
        HczArmory,

        /// <summary>
        /// Represents any heavy containment styled door.
        /// </summary>
        HeavyContainmentDoor,

        /// <summary>
        /// Represents any heavy containment styled door.
        /// </summary>
        HeavyBulkDoor,

        /// <summary>
        /// Represents the HID_CHAMBER door.
        /// </summary>
        HIDChamber,

        /// <summary>
        /// Represents the HID_UPPER door.
        /// </summary>
        [Obsolete("This Door has been renamed too HID_LAB.")]
        HIDUpper,

        /// <summary>
        /// Represents the HID_LAB door.
        /// </summary>
#pragma warning disable CS0618
        HIDLab = HIDUpper,
#pragma warning restore CS0618

        /// <summary>
        /// Represents the HID_LOWER door.
        /// </summary>
        [Obsolete("This Door has been removed from the game.")]
        HIDLower,

        /// <summary>
        /// Represents the INTERCOM door.
        /// </summary>
        Intercom,

        /// <summary>
        /// Represents the LCZ_ARMORY door.
        /// </summary>
        LczArmory,

        /// <summary>
        /// Represents the LCZ_CAFE door.
        /// </summary>
        LczCafe,

        /// <summary>
        /// Represents the LCZ_WC door.
        /// </summary>
        LczWc,

        /// <summary>
        /// Represents any light containment styled door.
        /// </summary>
        LightContainmentDoor,

        /// <summary>
        /// Represents the NUKE_ARMORY door.
        /// </summary>
        [Obsolete("This Door has been removed from the game.")]
        NukeArmory,

        /// <summary>
        /// Represents the NUKE_SURFACE door.
        /// </summary>
        NukeSurface,

        /// <summary>
        /// Represents any of the Class-D cell doors.
        /// </summary>
        PrisonDoor,

        /// <summary>
        /// Represents the SURFACE_GATE door.
        /// </summary>
        SurfaceGate,

        /// <summary>
        /// Represents the 330 door.
        /// </summary>
        Scp330,

        /// <summary>
        /// Represents the 330_CHAMBER door.
        /// </summary>
        Scp330Chamber,

        /// <summary>
        /// Represents the Gate in the Checkpoint between EZ and HCZ.
        /// </summary>
        CheckpointGateA,

        /// <summary>
        /// Represents the Gate in the Checkpoint between EZ and HCZ.
        /// </summary>
        CheckpointGateB,

        /// <summary>
        /// Represents a door than Yamato never implemented.
        /// </summary>
        [Obsolete("This Door has never been in the game.")]
        SurfaceDoor,

        /// <summary>
        /// Represents the CHECKPOINT_EZ_HCZ_A door.
        /// </summary>
        CheckpointEzHczA,

        /// <summary>
        /// Represents the CHECKPOINT_EZ_HCZ_B door.
        /// </summary>
        CheckpointEzHczB,

        /// <summary>
        /// Represents an unknown Gate.
        /// </summary>
        UnknownGate,

        /// <summary>
        /// Represents an unknown Elevator.
        /// </summary>
        UnknownElevator,

        /// <summary>
        /// Represents the Elevator door for <see cref="ElevatorGroup.GateA01"/> or <see cref="ElevatorGroup.GateA02"/>.
        /// </summary>
        ElevatorGateA,

        /// <summary>
        /// Represents the Elevator door for <see cref="ElevatorGroup.GateB"/>.
        /// </summary>
        ElevatorGateB,

        /// <summary>
        /// Represents the Elevator door for <see cref="ElevatorGroup.Nuke01"/>.
        /// </summary>
        ElevatorNuke,

        /// <summary>
        /// Represents the Elevator door for <see cref="ElevatorGroup.Scp049"/>.
        /// </summary>
        ElevatorScp049,

        /// <summary>
        /// Represents the Elevator door for <see cref="ElevatorGroup.LczA01"/> and <see cref="ElevatorGroup.LczA02"/>.
        /// </summary>
        ElevatorLczA,

        /// <summary>
        /// Represents the Elevator door for <see cref="ElevatorGroup.LczB01"/> and <see cref="ElevatorGroup.LczB02"/>.
        /// </summary>
        ElevatorLczB,

        /// <summary>
        /// Represents the Armory door in <see cref="RoomType.HczEzCheckpointA"/>.
        /// </summary>
        CheckpointArmoryA,

        /// <summary>
        /// Represents the Armory door in <see cref="RoomType.HczEzCheckpointB"/>.
        /// </summary>
        CheckpointArmoryB,

        /// <summary>
        /// Represents the door inside <see cref="RoomType.LczAirlock"/> with <see cref="Interactables.Interobjects.AirlockController"/> component.
        /// </summary>
        Airlock,

        /// <summary>
        /// Represents the New Gate where Scp173 spawn in the <see cref="RoomType.Hcz049"/>.
        /// </summary>
        Scp173NewGate,

        /// <summary>
        /// Represents the ESCAPE_FINAL door.
        /// </summary>
        EscapeFinal,

        /// <summary>
        /// Represents the Elevator door for <see cref="ElevatorGroup.ServerRoom"/>.
        /// </summary>
        ElevatorServerRoom,

        /// <summary>
        /// Represents the HCZ_127_LAB door.
        /// </summary>
        Hcz127Lab,

        /// <summary>
        /// Represents the door in the <see cref="RoomType.HczServerRoom"/> Closet.
        /// </summary>
        ServerRoomCloset,

        /// <summary>
        /// Represents the checkpoint that handle door for <see cref="DoorType.Scp106Primary"/> and <see cref="DoorType.Scp106Secondary"/>.
        /// </summary>
        Scp106Checkpoint,

        /// <summary>
        /// Represents the door in the <see cref="RoomType.HczTestRoom"/>.
        /// </summary>
        TestRoom,

        /// <summary>
        /// Represents the door in the <see cref="RoomType.LczPlants"/> Closet.
        /// </summary>
        PlantsCloset,

        /// <summary>
        /// Represents the door used for Checkpoint.
        /// </summary>
        Checkpoint,

        /// <summary>
        /// Represents the door in the <see cref="RoomType.EzGateA"/> Armory.
        /// </summary>
        GateAArmory,

        /// <summary>
        /// Represents the door in <see cref="RoomType.HczLoadingBay"/>.
        /// </summary>
        HczLoadingBay,
    }
}