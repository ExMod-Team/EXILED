// -----------------------------------------------------------------------
// <copyright file="LayerMask.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable CS1591

    /// <summary>
    /// All available LayerMasks.
    /// </summary>
    [Flags]
    public enum LayerMask
    {
        All = -1,
        None = 0,

        /// <summary>
        /// Collides with everything.
        /// </summary>
        Default = 1 << 0,
        TransparentFX = 1 << 1,

        /// <summary>
        /// All colliders that ignore raycasts.
        /// </summary>
        IgnoreRaycast = 1 << 2,
        Water = 1 << 4,
        UI = 1 << 5,
        Player = 1 << 8,

        /// <summary>
        /// All interactables.
        /// </summary>
        InteractableNoPlayerCollision = 1 << 9,
        Viewmodel = 1 << 10,
        RenderAfterFog = 1 << 12,

        /// <summary>
        /// Any Hitbox layer, including player.
        /// </summary>
        Hitbox = 1 << 13,

        /// <summary>
        /// Can only be seen through.
        /// </summary>
        Glass = 1 << 14,
        InvisibleCollider = 1 << 16,
        Ragdoll = 1 << 17,

        /// <summary>
        /// All Scp079 cameras.
        /// </summary>
        CCTV = 1 << 18,
        Grenade = 1 << 20,
        Phantom = 1 << 21,
        OnlyWorldCollision = 1 << 25,

        /// <summary>
        /// All doors.
        /// </summary>
        Door = 1 << 27,
        Skybox = 1 << 28,

        /// <summary>
        /// Can be seen and shoot through, but not walked through.
        /// </summary>
        Fence = 1 << 29,

        // Custom layers used by SCP:SL
        Scp173Teleport = Default | Water | UI | Door | Fence,

        Scp049Resurect = Default,

        AttackMask = Default | Door | Glass,

        InteractionMask = Default | Player | InteractableNoPlayerCollision | Hitbox | Glass | Door | Fence,

        InteractionAnticheatMask = Default | Glass | Door | InteractableNoPlayerCollision,
    }
}
