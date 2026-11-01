// -----------------------------------------------------------------------
// <copyright file="FirearmType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    /// <summary>
    /// Represents a firearm.
    /// </summary>
    /// <seealso cref="Extensions.ItemExtensions.GetAttachmentIdentifiers(FirearmType, uint)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetBaseCode(FirearmType)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetFirearmType(ItemType)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetItemType(FirearmType)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetMaxAmmo(FirearmType)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetWeaponAmmoType(FirearmType)"/>
    /// <seealso cref="Extensions.ItemExtensions.TryGetAttachments(FirearmType, uint, out System.Collections.Generic.IEnumerable{Structs.AttachmentIdentifier})"/>
    /// <seealso cref="Features.Items.Firearm.FirearmType"/>
    [Flags]
    public enum FirearmType
    {
        /// <summary>
        /// Not a firearm.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents the <see cref="ItemType.GunCOM15"/>.
        /// </summary>
        Com15 = 1,

        /// <summary>
        /// Represents the <see cref="ItemType.GunCOM18"/>.
        /// </summary>
        Com18 = 2,

        /// <summary>
        /// Represents the <see cref="ItemType.GunE11SR"/>.
        /// </summary>
        E11SR = 4,

        /// <summary>
        /// Represents the <see cref="ItemType.GunCrossvec"/>.
        /// </summary>
        Crossvec = 8,

        /// <summary>
        /// Represents the <see cref="ItemType.GunFSP9"/>.
        /// </summary>
        FSP9 = 16,

        /// <summary>
        /// Represents the <see cref="ItemType.GunLogicer"/>.
        /// </summary>
        Logicer = 32,

        /// <summary>
        /// Represents the <see cref="ItemType.GunRevolver"/>.
        /// </summary>
        Revolver = 64,

        /// <summary>
        /// Represents the <see cref="ItemType.GunAK"/>.
        /// </summary>
        AK = 128,

        /// <summary>
        /// Represents the <see cref="ItemType.GunShotgun"/>.
        /// </summary>
        Shotgun = 256,

        /// <summary>
        /// Represents the <see cref="ItemType.GunCom45"/>.
        /// </summary>
        Com45 = 512,

        /// <summary>
        /// Represents the <see cref="ItemType.ParticleDisruptor"/>.
        /// </summary>
        ParticleDisruptor = 1024,

        /// <summary>
        /// Represents the <see cref="ItemType.GunFRMG0"/>.
        /// </summary>
        FRMG0 = 2048,

        /// <summary>
        /// Represents the <see cref="ItemType.GunA7"/>.
        /// </summary>
        A7 = 4096,

        /// <summary>
        /// Represents the <see cref="ItemType.GunSCP127"/>.
        /// </summary>
        Scp127 = 8192,
    }
}
