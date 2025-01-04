// -----------------------------------------------------------------------
// <copyright file="SpawnLocationType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.API.Enums
{
    using System;

    /// <summary>
    /// All of the valid spawn location types.
    /// </summary>
    public enum SpawnLocationType
    {
        /// <summary>
        /// The inside of SCP-330's room.
        /// </summary>
        Inside330,

        /// <summary>
        /// The inside of SCP-330's room test chamber.
        /// </summary>
        Inside330Chamber,

        /// <summary>
        /// The inside of SCP-049's Armory room.
        /// </summary>
        Inside049Armory = 3,

        /// <summary>
        /// The inside of the inner SCP-079 door.
        /// </summary>
        Inside079Secondary,

        /// <summary>
        /// The inside of SCP-096's locked room behind its spawn.
        /// </summary>
        Inside096,

        /// <summary>
        /// The inside of the armory next to SCP-173's spawn.
        /// </summary>
        Inside173Armory,

        /// <summary>
        /// The inside of the door at the bottom of SCP-173's stairs.
        /// </summary>
        Inside173Bottom,

        /// <summary>
        /// On the side closest to SCP-173's spawn, on the top of the stairs.
        /// </summary>
        Inside173Connector,

        /// <summary>
        /// Inside the first Escape door.
        /// </summary>
        InsideEscapePrimary,

        /// <summary>
        /// Inside the second Escape door.
        /// </summary>
        InsideEscapeSecondary,

        /// <summary>
        /// Just inside the Intercom door.
        /// </summary>
        InsideIntercom,

        /// <summary>
        /// Inside the LCZ Armory.
        /// </summary>
        InsideLczArmory,

        /// <summary>
        /// Inside the LCZ PC-15 room.
        /// </summary>
        InsideLczCafe,

        /// <summary>
        /// Inside the Nuke armory.
        /// </summary>
        InsideNukeArmory,

        /// <summary>
        /// Inside the surface nuke room.
        /// </summary>
        InsideSurfaceNuke,

        /// <summary>
        /// Inside the first SCP-079 gate.
        /// </summary>
        Inside079First,

        /// <summary>
        /// Inside SCP-173's gate.
        /// </summary>
        Inside173Gate,

        /// <summary>
        /// Just inside of SCP-914.
        /// </summary>
        Inside914,

        /// <summary>
        /// Inside the Gate-A room.
        /// </summary>
        InsideGateA,

        /// <summary>
        /// Inside the Gate-B room.
        /// </summary>
        InsideGateB,

        /// <summary>
        /// Inside the GR-18 door.
        /// </summary>
        InsideGr18,

        /// <summary>
        /// Inside the 3-way intersection HCZ Armory.
        /// </summary>
        InsideHczArmory,

        /// <summary>
        /// Inside the chamber that contains the Micro-HID in Micro-HID room.
        /// </summary>
        InsideHidChamber,

        /// <summary>
        /// Inside the lower door that leads to the stairs in Micro-HID room.
        /// </summary>
        InsideHidLower,

        /// <summary>
        /// Inside the upper door that leads into the Micro-HID room just after the stairs.
        /// </summary>
        InsideHidUpper,

        /// <summary>
        /// Just inside the LCZ WC door.
        /// </summary>
        InsideLczWc,
    }
}