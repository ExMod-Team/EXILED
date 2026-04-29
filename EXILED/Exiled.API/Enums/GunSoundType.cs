// -----------------------------------------------------------------------
// <copyright file="GunSoundType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.API.Enums
{
    /// <summary>
    /// Represents the type of sound a firearm can produce.
    /// </summary>
    /// <seealso cref="Extensions.ItemExtensions.GetSoundType(ItemType, int)"/>
    public enum GunSoundType
    {
        /// <summary>
        /// Unknown or unmapped sound.
        /// </summary>
        Unknown,

        /// <summary>
        /// Sound of a gunshot.
        /// </summary>
        Fire,

        /// <summary>
        /// Sound of a suppressed gunshot.
        /// </summary>
        SuppressedFire,

        /// <summary>
        /// Sound of a dry fire (trigger pulled with an empty chamber).
        /// </summary>
        DryFire,

        /// <summary>
        /// Sound of a firearm being equipped or drawn.
        /// </summary>
        Equip,

        /// <summary>
        /// Sound of a rare or special equip animation.
        /// </summary>
        EquipRare,

        /// <summary>
        /// Sound of a firearm being cocked or charged.
        /// </summary>
        Cock,

        /// <summary>
        /// Sound of an initial or special cocking animation.
        /// </summary>
        CockInitial,

        /// <summary>
        /// Sound of a magazine being inserted.
        /// </summary>
        ReloadInsert,

        /// <summary>
        /// Sound of a magazine being ejected.
        /// </summary>
        ReloadEject,

        /// <summary>
        /// Sound of a general reload action (often used for revolvers or shotguns).
        /// </summary>
        Reload,

        /// <summary>
        /// Sound of a bolt or slide being pulled back.
        /// </summary>
        BoltOpen,

        /// <summary>
        /// Sound of a bolt or slide releasing forward.
        /// </summary>
        BoltClose,

        /// <summary>
        /// Sound of a round being ejected from the chamber.
        /// </summary>
        ChamberEject,

        /// <summary>
        /// Sound of a revolver's cylinder rotating.
        /// </summary>
        CylinderRotate,

        /// <summary>
        /// Sound of a revolver's cylinder being spun playfully.
        /// </summary>
        CylinderSpin,

        /// <summary>
        /// Sound of a gunshot using special buckshot ammunition.
        /// </summary>
        FireBuckshot,

        /// <summary>
        /// Sound of loading buckshot ammunition into an empty firearm.
        /// </summary>
        ReloadBuckshotEmpty,

        /// <summary>
        /// Sound of ejecting or emptying buckshot ammunition.
        /// </summary>
        EjectBuckshot,

        /// <summary>
        /// Sound of the weapon being inspected.
        /// </summary>
        Inspect,

        /// <summary>
        /// Sound of a single shot in Disintegrator mode.
        /// </summary>
        FireDisintegrator,

        /// <summary>
        /// Sound of the last shot in Disintegrator mode.
        /// </summary>
        FireDisintegratorLast,

        /// <summary>
        /// Sound of a shot in 3x Burst mode.
        /// </summary>
        FireBurst3x,

        /// <summary>
        /// Sound of the last shot in a 3x Burst sequence.
        /// </summary>
        FireBurst3xLast,

        /// <summary>
        /// Sound of general weapon handling, rustling, or adjusting grip.
        /// </summary>
        WeaponHandling,

        /// <summary>
        /// Sound of a weapon's stock being extended or adjusted.
        /// </summary>
        StockExtend,

        /// <summary>
        /// Sound of the second sequential shot, often used in double-shot shotgun modes.
        /// </summary>
        FireDouble,

        /// <summary>
        /// Sound of a single shotgun shell being ejected during unload.
        /// </summary>
        ShellEject,

        /// <summary>
        /// Sound indicating the completion of a shotgun unload sequence.
        /// </summary>
        UnloadComplete,

        /// <summary>
        /// Sound of a magazine being firmly tapped or locked into the firearm.
        /// </summary>
        ReloadLock,

        /// <summary>
        /// Sound of a drum or extended magazine being ejected.
        /// </summary>
        ReloadEjectDrum,

        /// <summary>
        /// Sound of a drum or extended magazine being inserted.
        /// </summary>
        ReloadInsertDrum,

        /// <summary>
        /// Sound of a drum or extended magazine being firmly tapped or locked into place.
        /// </summary>
        ReloadLockDrum,
    }
}