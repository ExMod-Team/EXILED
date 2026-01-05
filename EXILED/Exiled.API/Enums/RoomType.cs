// -----------------------------------------------------------------------
// <copyright file="RoomType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Unique identifier for the different types of rooms.
    /// </summary>
    /// <seealso cref="Features.Room.Type"/>
    /// <seealso cref="Features.Room.Get(RoomType)"/>
    public enum RoomType
    {
        /// <summary>
        /// Unknown Room Type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Light Containment Zone's Armory.
        /// </summary>
        LczArmory,

        /// <summary>
        /// Light Containment Zone's Curved Hall.
        /// </summary>
        LczCurve,

        /// <summary>
        /// Light Containment Zone's Straight Hall.
        /// </summary>
        LczStraight,

        /// <summary>
        /// Light Containment Zone's SCP-914 room.
        /// </summary>
        Lcz914,

        /// <summary>
        /// Light Containment Zone's 4-Way intersection.
        /// </summary>
        LczCrossing,

        /// <summary>
        /// Light Containment Zone's 3-Way intersection.
        /// </summary>
        LczTCross,

        /// <summary>
        /// Light Containment Zone's PC-15's room.
        /// </summary>
        LczCafe,

        /// <summary>
        /// Light Containment Zone's VT-00's room.
        /// </summary>
        LczPlants,

        /// <summary>
        /// Light Containment Zone's Water Closet.
        /// </summary>
        LczToilets,

        /// <summary>
        /// Light Containment Zone's Airlock room.
        /// </summary>
        LczAirlock,

        /// <summary>
        /// Light Containment Zone's PT-00 room.
        /// </summary>
        Lcz173,

        /// <summary>
        /// Light Containment Zone's Class-D spawn room.
        /// </summary>
        LczClassDSpawn,

        /// <summary>
        /// Light Containment Zone's Checkpoint B room.
        /// </summary>
        LczCheckpointB,

        /// <summary>
        /// Light Containment Zone's GR-18's room.
        /// </summary>
        LczGlassBox,

        /// <summary>
        /// Light Containment Zone's Checkpoint A room.
        /// </summary>
        LczCheckpointA,

        /// <summary>
        /// Heavy Containment Zone's SCP-079 room.
        /// </summary>
        Hcz079,

        /// <summary>
        /// Heavy Containment Zone's Entrance Checkpoint A room.
        /// </summary>
        HczEzCheckpointA,

        /// <summary>
        /// Heavy Containment Zone's Entrance Checkpoint B room.
        /// </summary>
        HczEzCheckpointB,

        /// <summary>
        /// Heavy Containment Zone's 3-Way Intersection + Armory room.
        /// </summary>
        HczArmory,

        /// <summary>
        /// Heavy Containment Zone's SCP-939 room.
        /// </summary>
        Hcz939,

        /// <summary>
        /// Heavy Containment Zone's MicroHID straight hall.
        /// </summary>
        HczHid,

        /// <summary>
        /// Heavy Containment Zone's SCP-049 + SCP-173's room.
        /// </summary>
        Hcz049,

        /// <summary>
        /// Heavy Containment Zone's 4-way intersection.
        /// </summary>
        HczCrossing,

        /// <summary>
        /// Heavy Containment Zone's SCP-106 room.
        /// </summary>
        Hcz106,

        /// <summary>
        /// Heavy Containment Zone's nuke room.
        /// </summary>
        HczNuke,

        /// <summary>
        /// Heavy Containment Zone's Tesla straight hall.
        /// </summary>
        HczTesla,

        /// <summary>
        /// Heavy Containment Zone's cruved hall.
        /// </summary>
        HczCurve,

        /// <summary>
        /// Heavy Containment Zone's SCP-096 room.
        /// </summary>
        Hcz096,

        /// <summary>
        /// Entrance Zone's Red Vent room.
        /// </summary>
        EzVent,

        /// <summary>
        /// Entrance Zone's Intercom room.
        /// </summary>
        EzIntercom,

        /// <summary>
        /// Entrance Zone's Gate A room.
        /// </summary>
        EzGateA,

        /// <summary>
        /// Entrance Zone's straight hall with PC's on a lower level.
        /// </summary>
        EzDownstairsPcs,

        /// <summary>
        /// Entrance Zone's curved hall.
        /// </summary>
        EzCurve,

        /// <summary>
        /// Entrance Zone's straight hall with PC's on the main level.
        /// </summary>
        EzPcs,

        /// <summary>
        /// Entrance Zone's 4-way intersection.
        /// </summary>
        EzCrossing,

        /// <summary>
        /// Entrance Zone's Red Collapsed Tunnel Room.
        /// </summary>
        EzCollapsedTunnel,

        /// <summary>
        /// Entrance Zone's straight hall with Dr.L's or Dr. Gear's locked room.
        /// </summary>
        EzConference,

        /// <summary>
        /// Entrance Zone's straight hall with Chef's locked room.
        /// </summary>
        EzChef,

        /// <summary>
        /// Entrance Zone's straight hall.
        /// </summary>
        EzStraight,

        /// <summary>
        /// Entrance Zone's straight hall with a different placement of seasonal objects.
        /// </summary>
        EzStraightColumn,

        /// <summary>
        /// Entrance Zone's Cafeteria Room.
        /// </summary>
        EzCafeteria,

        /// <summary>
        /// Entrance Zone's straight hall with PC's and upper level.
        /// </summary>
        EzUpstairsPcs,

        /// <summary>
        /// Entrance Zone's Gate B room.
        /// </summary>
        EzGateB,

        /// <summary>
        /// Entrance Zone's Shelter room.
        /// </summary>
        EzShelter,

        /// <summary>
        /// The Pocket Dimension.
        /// </summary>
        Pocket,

        /// <summary>
        /// The Surface.
        /// </summary>
        Surface,

        /// <summary>
        /// Heavy Containment Zone's straight hall with ceiling fan.
        /// </summary>
        HczStraight,

        /// <summary>
        /// Entrance Zone's 3-way intersection.
        /// </summary>
        EzTCross,

        /// <summary>
        /// Light Containment Zone's SCP-330 room.
        /// </summary>
        Lcz330,

        /// <summary>
        /// Entrance Zone's straight hall before the entrance/heavy checkpoint.
        /// </summary>
        EzCheckpointHallwayA,

        /// <summary>
        /// Entrance Zone's straight hall before the entrance/heavy checkpoint.
        /// </summary>
        EzCheckpointHallwayB,

        /// <summary>
        /// Heavy Containment Zone's test room's straight hall.
        /// </summary>
        HczTestRoom,

        /// <summary>
        /// Heavy Containment Zone's Elevator System A room.
        /// </summary>
        HczElevatorA,

        /// <summary>
        /// Heavy Containment Elevator Zone's System B room.
        /// </summary>
        HczElevatorB,

        /// <summary>
        /// Heavy Containment Zone's cross room with waterfall.
        /// </summary>
        HczDss08,

        /// <summary>
        /// Heavy Containment Zone's cross room with waterfall.
        /// </summary>
        HczCrossRoomWater = HczDss08,

        /// <summary>
        /// Heavy Containment Zone's corner.
        /// </summary>
        HczCornerDeep,

        /// <summary>
        /// Heavy Containment Zone's 3-way intersection with storage crates obstructing the passage.
        /// </summary>
        HczIntersectionJunk,

        /// <summary>
        /// Heavy Containment Zone's 3-way intersection.
        /// </summary>
        HczIntersection,

        /// <summary>
        /// Heavy Containment Zone's straight hall with pipelines and sanitary door.
        /// </summary>
        HczStraightC,

        /// <summary>
        /// Heavy Containment Zone's straight hall with pipelines obstructing the passage.
        /// </summary>
        HczStraightPipeRoom,

        /// <summary>
        /// Heavy Containment Zone's straight hall.
        /// </summary>
        HczStraightVariant,

        /// <summary>
        /// Entrance Zone's straight hall with Dr.L's and conference room 9b locked room.
        /// </summary>
        EzSmallrooms,

        /// <summary>
        /// Heavy Containment Zone's SCP-127 room.
        /// </summary>
        Hcz127,

        /// <summary>
        /// Heavy Containment Zone's storage / server room.
        /// </summary>
        HczServerRoom,

        /// <summary>
        /// Heavy Containment Zone's straight hall room with lava.
        /// </summary>
        HczIncineratorWayside,

        /// <summary>
        /// Heavy Containment Zone's straight hall room with lava.
        /// </summary>
        HczDss12 = HczIncineratorWayside,

        /// <summary>
        /// Heavy Containment Zone's T-intersection with a ramp in it.
        /// </summary>
        HczLoadingBay,
    }
}