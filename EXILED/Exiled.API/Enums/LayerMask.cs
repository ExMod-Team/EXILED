// -----------------------------------------------------------------------
// <copyright file="LayerMask.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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

        Default = 1 << 0,
        TransparentFX = 1 << 1,
        IgnoreRaycast = 1 << 2,
        Water = 1 << 4,
        UI = 1 << 5,
        Player = 1 << 8,
        InteractableNoPlayerCollision = 1 << 9,
        Viewmodel = 1 << 10,
        RenderAfterFog = 1 << 12,
        Hitbox = 1 << 13,
        Glass = 1 << 14,
        InvisibleCollider = 1 << 16,
        Ragdoll = 1 << 17,
        CCTV = 1 << 18,
        Grenade = 1 << 20,
        Phantom = 1 << 21,
        OnlyWorldCollision = 1 << 25,
        Door = 1 << 27,
        Skybox = 1 << 28,
        Fence = 1 << 29,

        // Custom layers use by SCP:SL
        Scp173Teleport = Default | Water | UI | Door | Fence,

        Scp049Resurect = Default,

        AttackMask = Default | Door | Glass,

        InteractionMask = Default | Player | InteractableNoPlayerCollision | Hitbox | Glass | Door | Fence,

        InteractionAnticheatMask = Default | Glass | Door | InteractableNoPlayerCollision,
    }
}
