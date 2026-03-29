// -----------------------------------------------------------------------
// <copyright file="DamageTypeExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Enums;
    using Features;
    using InventorySystem.Items.MicroHID.Modules;
    using InventorySystem.Items.Scp1509;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp1507;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerStatsSystem;

    /// <summary>
    /// A set of extensions for <see cref="DamageType"/>.
    /// </summary>
    public static class DamageTypeExtensions
    {
        private static readonly Dictionary<DeathTranslation, DamageType> TranslationConversionInternal = new()
        {
            { DeathTranslations.Recontained, DamageType.Recontainment },
            { DeathTranslations.Warhead, DamageType.Warhead },
            { DeathTranslations.Scp049, DamageType.Scp049 },
            { DeathTranslations.Unknown, DamageType.Unknown },
            { DeathTranslations.Asphyxiated, DamageType.Asphyxiation },
            { DeathTranslations.Bleeding, DamageType.Bleeding },
            { DeathTranslations.Falldown, DamageType.Falldown },
            { DeathTranslations.PocketDecay, DamageType.PocketDimension },
            { DeathTranslations.Decontamination, DamageType.Decontamination },
            { DeathTranslations.Poisoned, DamageType.Poison },
            { DeathTranslations.Scp207, DamageType.Scp207 },
            { DeathTranslations.SeveredHands, DamageType.SeveredHands },
            { DeathTranslations.MicroHID, DamageType.MicroHidPrimaryFire },
            { DeathTranslations.Tesla, DamageType.Tesla },
            { DeathTranslations.Explosion, DamageType.ExplosionCustom },
            { DeathTranslations.Scp096, DamageType.Scp096Other },
            { DeathTranslations.Scp173, DamageType.Scp173 },
            { DeathTranslations.Scp939Lunge, DamageType.Scp939LungeTarget },
            { DeathTranslations.Zombie, DamageType.Scp0492 },
            { DeathTranslations.BulletWounds, DamageType.Firearm },
            { DeathTranslations.Crushed, DamageType.Crushed },
            { DeathTranslations.UsedAs106Bait, DamageType.FemurBreaker },
            { DeathTranslations.FriendlyFireDetector, DamageType.FriendlyFireDetector },
            { DeathTranslations.Hypothermia, DamageType.Hypothermia },
            { DeathTranslations.CardiacArrest, DamageType.CardiacArrest },
            { DeathTranslations.Scp939Other, DamageType.Scp939Claw },
            { DeathTranslations.Scp3114Slap, DamageType.Scp3114 },
            { DeathTranslations.MarshmallowMan, DamageType.Marshmallow },
            { DeathTranslations.Scp1344, DamageType.SeveredEyes },
            { DeathTranslations.Scp1507Peck, DamageType.Scp1507 },
            { DeathTranslations.Scp127Bullets, DamageType.Scp127 },
            { DeathTranslations.Scp1509, DamageType.Scp1509 },
        };

        private static readonly Dictionary<byte, DamageType> TranslationIdConversionInternal = TranslationConversionInternal.ToDictionary(x => x.Key.Id, x => x.Value);

        private static readonly Dictionary<ItemType, DamageType> ItemConversionInternal = new()
        {
            { ItemType.GunCrossvec, DamageType.Crossvec },
            { ItemType.GunLogicer, DamageType.Logicer },
            { ItemType.GunRevolver, DamageType.Revolver },
            { ItemType.GunShotgun, DamageType.Shotgun },
            { ItemType.GunAK, DamageType.AK },
            { ItemType.GunCOM15, DamageType.Com15 },
            { ItemType.GunCom45, DamageType.Com45 },
            { ItemType.GunCOM18, DamageType.Com18 },
            { ItemType.GunFSP9, DamageType.Fsp9 },
            { ItemType.GunE11SR, DamageType.E11Sr },
            { ItemType.MicroHID, DamageType.MicroHidPrimaryFire },
            { ItemType.ParticleDisruptor, DamageType.ParticleDisruptor },
            { ItemType.Jailbird, DamageType.Jailbird },
            { ItemType.GunFRMG0, DamageType.Frmg0 },
            { ItemType.GunA7, DamageType.A7 },
            { ItemType.GunSCP127, DamageType.Scp127 },
        };

        /// <summary>
        /// Gets conversion information between <see cref="DeathTranslation.Id"/>s and <see cref="DamageType"/>s.
        /// </summary>
        public static IReadOnlyDictionary<byte, DamageType> TranslationIdConversion => TranslationIdConversionInternal;

        /// <summary>
        /// Gets conversion information between <see cref="DeathTranslation"/>s and <see cref="DamageType"/>s.
        /// </summary>
        public static IReadOnlyDictionary<DeathTranslation, DamageType> TranslationConversion => TranslationConversionInternal;

        /// <summary>
        /// Gets conversion information between <see cref="ItemType"/>s and <see cref="DamageType"/>s.
        /// </summary>
        public static IReadOnlyDictionary<ItemType, DamageType> ItemConversion => ItemConversionInternal;

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by a weapon.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <param name="checkNonFirearm">Indicates whether the MicroHid and Jailbird damage type should be taken into account.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by weapon.</returns>
        public static bool IsWeapon(this DamageType type, bool checkNonFirearm = true) => type switch
        {
            DamageType.Crossvec or DamageType.Logicer or DamageType.Revolver or DamageType.Shotgun or DamageType.AK or DamageType.Com15 or DamageType.Com18 or DamageType.E11Sr or DamageType.Fsp9 or DamageType.ParticleDisruptor or DamageType.Com45 or DamageType.Frmg0 or DamageType.A7 => true,
            DamageType.MicroHidPrimaryFire or DamageType.MicroHidChargeFire or DamageType.MicroHidBrokenFire or DamageType.Jailbird when checkNonFirearm => true,
            _ => false,
        };

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by a SCP.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <param name="checkItems">Indicates whether the SCP-items damage types should be taken into account.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by SCP.</returns>
        public static bool IsScp(this DamageType type, bool checkItems = true) => type switch
        {
            DamageType.Scp or DamageType.Scp049 or DamageType.Scp096SlapLeft or DamageType.Scp096SlapRight or DamageType.Scp096Charge or DamageType.Scp096GateKill or DamageType.Scp096Other or DamageType.Scp106 or DamageType.Scp173 or DamageType.Scp939 or DamageType.Scp939Claw or DamageType.Scp939LungeTarget or DamageType.Scp939LungeSecondary or DamageType.Scp0492 or DamageType.Scp3114 or DamageType.Scp3114SkinSteal => true,
            DamageType.Scp018 or DamageType.Scp207 when checkItems => true,
            _ => false,
        };

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by <see cref="RoleTypeId.Scp096"/>>.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by SCP.</returns>
        public static bool IsScp096(this DamageType type) => type is DamageType.Scp096SlapLeft or DamageType.Scp096SlapRight or DamageType.Scp096Charge or DamageType.Scp096GateKill or DamageType.Scp096Other;

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by <see cref="RoleTypeId.Scp939"/>>.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by SCP.</returns>
        public static bool IsScp939(this DamageType type) => type is DamageType.Scp939 or DamageType.Scp939Claw or DamageType.Scp939LungeTarget or DamageType.Scp939LungeSecondary;

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by <see cref="RoleTypeId.Scp049"/>>.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by SCP.</returns>
        public static bool IsScp049(this DamageType type) => type is DamageType.Scp049 or DamageType.CardiacArrest;

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by <see cref="RoleTypeId.Scp3114"/>>.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by SCP.</returns>
        public static bool IsScp3114(this DamageType type) => type is DamageType.Scp3114 or DamageType.Scp3114SkinSteal or DamageType.Strangled;

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by <see cref="RoleTypeId.Scp3114"/>>.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by SCP.</returns>
        public static bool IsMicroHid(this DamageType type) => type is DamageType.MicroHidPrimaryFire or DamageType.MicroHidChargeFire or DamageType.MicroHidBrokenFire;

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by a status effect.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by status effect.</returns>
        public static bool IsStatusEffect(this DamageType type) => type switch
        {
            DamageType.Asphyxiation or DamageType.Poison or DamageType.Bleeding or DamageType.Scp207 or DamageType.Hypothermia or DamageType.Strangled or DamageType.SeveredHands or DamageType.SeveredEyes or DamageType.PocketDimension => true,
            _ => false,
        };

        /// <summary>
        /// Gets the <see cref="DamageType"/> of an <see cref="DamageHandlerBase"/>s.
        /// </summary>
        /// <param name="damageHandlerBase">The DamageHandler to convert.</param>
        /// <returns>The <see cref="DamageType"/> of the <see cref="DamageHandlerBase"/>.</returns>
        public static DamageType GetDamageType(DamageHandlerBase damageHandlerBase)
        {
            switch (damageHandlerBase)
            {
                case Features.DamageHandlers.GenericDamageHandler genericDamageHandler:
                    return GetDamageType(genericDamageHandler.Base);
                case CustomReasonDamageHandler:
                    return DamageType.Custom;
                case WarheadDamageHandler:
                    return DamageType.Warhead;
                case ExplosionDamageHandler explosionDamageHandler:
                    return explosionDamageHandler.ExplosionType switch
                    {
                        ExplosionType.Grenade => DamageType.ExplosionGrenade,
                        ExplosionType.SCP018 => DamageType.ExplosionScp018,
                        ExplosionType.PinkCandy => DamageType.ExplosionPinkCandy,
                        ExplosionType.Cola => DamageType.ExplosionCola,
                        ExplosionType.Disruptor => DamageType.ExplosionDisruptor,
                        ExplosionType.Jailbird => DamageType.ExplosionJailbird,
                        _ => DamageType.ExplosionCustom,
                    };
                case Scp018DamageHandler:
                    return DamageType.Scp018;
                case RecontainmentDamageHandler:
                    return DamageType.Recontainment;
                case Scp096DamageHandler scp096DamageHandler:
                    return scp096DamageHandler._attackType switch
                    {
                        Scp096DamageHandler.AttackType.GateKill => DamageType.Scp096GateKill,
                        Scp096DamageHandler.AttackType.SlapLeft => DamageType.Scp096SlapLeft,
                        Scp096DamageHandler.AttackType.SlapRight => DamageType.Scp096SlapRight,
                        Scp096DamageHandler.AttackType.Charge => DamageType.Scp096Charge,
                        _ => DamageType.Scp096Other,
                    };
                case MicroHidDamageHandler microHidDamageHandler:
                    return microHidDamageHandler.FiringMode switch
                    {
                        MicroHidFiringMode.PrimaryFire => DamageType.MicroHidPrimaryFire,
                        MicroHidFiringMode.ChargeFire => DamageType.MicroHidChargeFire,
                        MicroHidFiringMode.BrokenFire => DamageType.MicroHidBrokenFire,
                        _ => DamageType.Unknown,
                    };
                case DisruptorDamageHandler:
                    return DamageType.ParticleDisruptor;
                case Scp1507DamageHandler:
                    return DamageType.Scp1507;
                case Scp956DamageHandler:
                    return DamageType.Scp956;
                case SnowballDamageHandler:
                    return DamageType.SnowBall;
                case GrayCandyDamageHandler:
                    return DamageType.GrayCandy;
                case Scp1509DamageHandler:
                    return DamageType.Scp1509;
                case MarshmallowDamageHandler:
                    return DamageType.Marshmallow;
                case Scp049DamageHandler scp049DamageHandler:
                    return scp049DamageHandler.DamageSubType switch
                    {
                        Scp049DamageHandler.AttackType.CardiacArrest => DamageType.CardiacArrest,
                        Scp049DamageHandler.AttackType.Instakill => DamageType.Scp049,
                        Scp049DamageHandler.AttackType.Scp0492 => DamageType.Scp0492,
                        _ => DamageType.Unknown,
                    };
                case Scp939DamageHandler scp939DamageHandler:
                    return scp939DamageHandler.Scp939DamageType switch
                    {
                        Scp939DamageType.Claw => DamageType.Scp939Claw,
                        Scp939DamageType.LungeTarget => DamageType.Scp939LungeTarget,
                        Scp939DamageType.LungeSecondary => DamageType.Scp939LungeSecondary,
                        _ => DamageType.Scp939,
                    };
                case Scp3114DamageHandler scp3114DamageHandler:
                    return scp3114DamageHandler.Subtype switch
                    {
                        Scp3114DamageHandler.HandlerType.Strangulation => DamageType.Strangled,
                        Scp3114DamageHandler.HandlerType.SkinSteal => DamageType.Scp3114SkinSteal,
                        Scp3114DamageHandler.HandlerType.Slap => DamageType.Scp3114,
                        _ => DamageType.Unknown,
                    };
                case FirearmDamageHandler firearmDamageHandler:
                    {
                        if (ItemConversion.ContainsKey(firearmDamageHandler.WeaponType))
                            return ItemConversion[firearmDamageHandler.WeaponType];

                        if (damageHandlerBase.GetType().Assembly.FullName.StartsWith("Assembly-CSharp"))
                            Log.Warn($"{nameof(DamageTypeExtensions)}.{nameof(damageHandlerBase)}: No matching {nameof(DamageType)} for {nameof(ScpDamageHandler)} with ItemType {firearmDamageHandler.WeaponType}, type will be reported as {DamageType.Firearm}. Report this to EXILED Devs.");

                        return DamageType.Firearm;
                    }

                case ScpDamageHandler scpDamageHandler:
                    {
                        if (scpDamageHandler._translationId == DeathTranslations.PocketDecay.Id)
                            return DamageType.Scp106;

                        if (TranslationIdConversion.ContainsKey(scpDamageHandler._translationId))
                            return TranslationIdConversion[scpDamageHandler._translationId];

                        if (damageHandlerBase.GetType().Assembly.FullName.StartsWith("Assembly-CSharp"))
                            Log.Warn($"{nameof(DamageTypeExtensions)}.{nameof(damageHandlerBase)}: No matching {nameof(DamageType)} for {nameof(ScpDamageHandler)} with ID {scpDamageHandler._translationId}, type will be reported as {DamageType.Scp}. Report this to EXILED Devs.");

                        return DamageType.Scp;
                    }

                case UniversalDamageHandler universal:
                    {
                        DeathTranslation translation = DeathTranslations.TranslationsById[universal.TranslationId];

                        if (TranslationIdConversion.ContainsKey(translation.Id))
                            return TranslationIdConversion[translation.Id];

                        if (damageHandlerBase.GetType().Assembly.FullName.StartsWith("Assembly-CSharp"))
                            Log.Warn($"{nameof(DamageTypeExtensions)}.{nameof(damageHandlerBase)}: No matching {nameof(DamageType)} for {nameof(UniversalDamageHandler)} with ID {translation.Id}, type will be reported as {DamageType.Unknown}. Report this to EXILED Devs.");

                        return DamageType.Unknown;
                    }
            }

                default:
                    {
                        if (damageHandlerBase.GetType().Assembly.FullName.StartsWith("Assembly-CSharp"))
                            Log.Warn($"{nameof(DamageTypeExtensions)}.{nameof(damageHandlerBase)}: No matching {nameof(DamageType)} with Type {damageHandlerBase.GetType()}, type will be reported as {DamageType.Unknown}. Report this to EXILED Devs.");
                        return DamageType.Unknown;
                    }
            }
        }
    }
}
