// -----------------------------------------------------------------------
// <copyright file="GlassType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Features;

    /// <summary>
    /// Unique identifier for the different types of Window.
    /// </summary>
    /// <seealso cref="Window.Type"/>
    public enum GlassType
    {
        /// <summary>
        /// Represents an unknown window.
        /// </summary>
        Unknown,

        /// <summary>
        /// Represents all the windows in <see cref="RoomType.LczGlassBox"/>.
        /// </summary>
        GR18,

        /// <summary>
        /// Represents the window in <see cref="RoomType.Hcz049"/>.
        /// </summary>
        Scp049,

        /// <summary>
        /// Represents the windows in <see cref="RoomType.HczHid"/>.
        /// </summary>
        MicroHid,

        /// <summary>
        /// Represents the window in <see cref="RoomType.Hcz079"/>.
        /// </summary>
        Scp079,

        /// <summary>
        /// Represents the <see cref="Recontainer.ActivatorWindow"/>.
        /// </summary>
        Scp079Trigger,

        /// <summary>
        /// Represents the window in <see cref="RoomType.Lcz330"/>.
        /// </summary>
        Scp330,

        /// <summary>
        /// Represents all the windows in <see cref="RoomType.LczPlants"/>.
        /// </summary>
        Plants,

        /// <summary>
        /// Represents all the windows in <see cref="RoomType.HczEzCheckpointA"/>.
        /// </summary>
        HczEzCheckpointA,

        /// <summary>
        /// Represents all the windows in <see cref="RoomType.HczEzCheckpointB"/>.
        /// </summary>
        HczEzCheckpointB,

        /// <summary>
        /// Represents the window in <see cref="RoomType.HczTestRoom"/>.
        /// </summary>
        TestRoom,

        /// <summary>
        /// Represents the window in <see cref="RoomType.Hcz127"/>.
        /// </summary>
        Scp127Small,

        /// <summary>
        /// Represents the window in <see cref="RoomType.Hcz127"/>.
        /// </summary>
        Scp127Large,

        /// <summary>
        /// Represents the window in <see cref="RoomType.EzGateA"/>.
        /// </summary>
        GateAPit,

        /// <summary>
        /// Represents the window in <see cref="RoomType.EzGateA"/>.
        /// </summary>
        GateAArmory,

        /// <summary>
        /// Represents the window in <see cref="RoomType.HczLoadingBay"/>.
        /// </summary>
        HczLoadingBay,

        /// <summary>
        /// Represents the window in <see cref="RoomType.SurfaceCassie"/> next to the Hallway.
        /// </summary>
        SurfaceCassieUpperHall,

        /// <summary>
        /// Represents the window in <see cref="RoomType.SurfaceCassie"/> on lower part at Door Section.
        /// </summary>
        SurfaceCassieLowerHall,

        /// <summary>
        /// Represents the window in <see cref="RoomType.SurfaceCassie"/> inside Cassie Chamber.
        /// </summary>
        SurfaceCassieChamber,

        /// <summary>
        /// Represents the window in <see cref="RoomType.SurfaceGateA"/> on lower part of the Stair.
        /// </summary>
        SurfaceStairLowerGateA,

        /// <summary>
        /// Represents the window in <see cref="RoomType.SurfaceGateA"/> next to the Security Check.
        /// </summary>
        SurfaceBigWindowGateA,

        /// <summary>
        /// Represents the window in <see cref="RoomType.SurfaceGateA"/> on the upper part of the Stair.
        /// </summary>
        SurfaceStairUpperGateA,

        /// <summary>
        /// Represents the window in <see cref="RoomType.SurfaceGateA"/> on the .
        /// </summary>
        SurfaceSmallWindowGateA,

        /// <summary>
        /// Represents the window in <see cref="RoomType.SurfaceGateA"/> that on the Roof part.
        /// </summary>
        SurfaceRoofGateA,

        /// <summary>
        /// Represents the window in <see cref="RoomType.SurfaceGateA"/> that in the Upstair Corner on Road side.
        /// </summary>
        SurfaceUpstairCornerRoadGateA,

        /// <summary>
        /// Represents the window in <see cref="RoomType.SurfaceGateA"/> that inside the AdminOffice.
        /// </summary>
        SurfaceAdminOfficeGateA,
    }
}