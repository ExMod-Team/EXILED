// -----------------------------------------------------------------------
// <copyright file="DamageHandlerBase.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Enums;
    using Exiled.API.Features.Items;
    using Extensions;
    using InventorySystem.Items.Scp1509;
    using PlayerRoles.PlayableScps.Scp1507;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerStatsSystem;

    using BaseHandler = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// A wrapper to easily manipulate the behavior of <see cref="BaseHandler"/>.
    /// </summary>
    public abstract class DamageHandlerBase
    {
        private DamageType damageType;
        private CassieAnnouncement cassieAnnouncement;

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandlerBase"/> class.
        /// </summary>
        protected DamageHandlerBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandlerBase"/> class.
        /// </summary>
        /// <param name="baseHandler">The base <see cref="BaseHandler"/>.</param>
        protected DamageHandlerBase(BaseHandler baseHandler)
        {
            Base = baseHandler;
        }

        /// <summary>
        /// All available <see cref="DamageHandler"/> actions.
        /// </summary>
        public enum Action : byte
        {
            /// <summary>
            /// None.
            /// </summary>
            None,

            /// <summary>
            /// The result is determined by a damage action.
            /// </summary>
            Damage,

            /// <summary>
            /// The result is determined by a death action.
            /// </summary>
            Death,
        }

        /// <summary>
        /// Gets or sets the base <see cref="BaseHandler"/>.
        /// </summary>
        public BaseHandler Base { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="CassieAnnouncement"/> belonging to this <see cref="DamageHandler"/> instance.
        /// </summary>
        public virtual CassieAnnouncement CassieDeathAnnouncement
        {
            get => cassieAnnouncement ?? Base.CassieDeathAnnouncement;
            protected set => cassieAnnouncement = value;
        }

        /// <summary>
        /// Gets the text to show in the server logs.
        /// </summary>
        public virtual string ServerLogsText => Base.ServerLogsText;

        /// <summary>
        /// Gets or sets the <see cref="DamageType"/> for the damage handler.
        /// </summary>
        public virtual DamageType Type
        {
            get
            {
                if (damageType != DamageType.Unknown)
                    return damageType;

                damageType = GetDamageType();
                return damageType;
            }

            protected set
            {
                if (!Enum.IsDefined(typeof(DamageType), value))
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DamageType));

                damageType = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="PlayerStatsSystem.DeathTranslation"/>.
        /// </summary>
        public virtual DeathTranslation DeathTranslation => DamageTypeExtensions.TranslationConversion.FirstOrDefault(translation => translation.Value == Type).Key;

        /// <summary>
        /// Implicitly converts the given <see cref="DamageHandlerBase"/> instance to a <see cref="BaseHandler"/> object.
        /// </summary>
        /// <param name="damageHandlerBase">The <see cref="DamageHandlerBase"/> instance.</param>
        public static implicit operator BaseHandler(DamageHandlerBase damageHandlerBase) => damageHandlerBase.Base;

        /// <summary>
        /// Applies the damage to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to damage.</param>
        /// <returns>The <see cref="Action"/> of the call to this method.</returns>
        public abstract Action ApplyDamage(Player player);

        /// <summary>
        /// Computes and processes the damage.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to damage.</param>
        public virtual void ProcessDamage(Player player)
        {
        }

        /// <summary>
        /// Unsafely casts the damage handler to the specified <see cref="BaseHandler"/> type.
        /// </summary>
        /// <typeparam name="T">The specified <see cref="BaseHandler"/> type.</typeparam>
        /// <returns>A <see cref="BaseHandler"/> object.</returns>
        public T As<T>()
            where T : BaseHandler => Base as T;

        /// <summary>
        /// Unsafely casts the damage handler to the specified <see cref="DamageHandlerBase"/> type.
        /// </summary>
        /// <typeparam name="T">The specified <see cref="DamageHandlerBase"/> type.</typeparam>
        /// <returns>A <see cref="DamageHandlerBase"/> object.</returns>
        public T BaseAs<T>()
            where T : DamageHandlerBase => this as T;

        /// <summary>
        /// Safely casts the damage handler to the specified <see cref="BaseHandler"/> type.
        /// </summary>
        /// <typeparam name="T">The specified <see cref="BaseHandler"/> type.</typeparam>
        /// <param name="param">The casted <see cref="BaseHandler"/>.</param>
        /// <returns>A <see cref="BaseHandler"/> object.</returns>
        public bool Is<T>(out T param)
            where T : BaseHandler
        {
            param = default;

            if (Base is not T cast)
                return false;

            param = cast;
            return true;
        }

        /// <summary>
        /// Safely casts the damage handler to the specified <see cref="DamageHandlerBase"/> type.
        /// </summary>
        /// <typeparam name="T">The specified <see cref="DamageHandlerBase"/> type.</typeparam>
        /// <param name="param">The casted <see cref="DamageHandlerBase"/>.</param>
        /// <returns>A <see cref="DamageHandlerBase"/> object.</returns>
        public bool BaseIs<T>(out T param)
            where T : DamageHandlerBase
        {
            param = default;

            if (this is not T cast)
                return false;

            param = cast;
            return true;
        }

        /// <summary>
        /// Gets the <see cref="DamageType"/> assosiated with <paramref name="damageHandler"/>.
        /// </summary>
        /// <param name="damageHandler"><see cref="BaseHandler"/> from which <see cref="DamageType"/> should be get. If <c>null</c>, <see cref="Base"/> will be used.</param>
        /// <returns>Assosiated <see cref="DamageType"/>.</returns>
        protected DamageType GetDamageType(BaseHandler damageHandler = null)
        {
            damageHandler ??= Base;

            switch (damageHandler)
            {
                case GenericDamageHandler genericDamageHandler:
                    return GetDamageType(genericDamageHandler.Base);
                case CustomReasonDamageHandler:
                    return DamageType.Custom;
                case WarheadDamageHandler:
                    return DamageType.Warhead;
                case ExplosionDamageHandler:
                    return DamageType.Explosion;
                case Scp018DamageHandler:
                    return DamageType.Scp018;
                case RecontainmentDamageHandler:
                    return DamageType.Recontainment;
                case MicroHidDamageHandler:
                    return DamageType.MicroHid;
                case DisruptorDamageHandler:
                    return DamageType.ParticleDisruptor;
                case Scp939DamageHandler:
                    return DamageType.Scp939;
                case JailbirdDamageHandler:
                    return DamageType.Jailbird;
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
                case Scp3114DamageHandler scp3114DamageHandler:
                    return scp3114DamageHandler.Subtype switch
                    {
                        Scp3114DamageHandler.HandlerType.Strangulation => DamageType.Strangled,
                        Scp3114DamageHandler.HandlerType.SkinSteal => DamageType.Scp3114,
                        Scp3114DamageHandler.HandlerType.Slap => DamageType.Scp3114,
                        _ => DamageType.Unknown,
                    };
                case Scp049DamageHandler scp049DamageHandler:
                    return scp049DamageHandler.DamageSubType switch
                    {
                        Scp049DamageHandler.AttackType.CardiacArrest => DamageType.CardiacArrest,
                        Scp049DamageHandler.AttackType.Instakill => DamageType.Scp049,
                        Scp049DamageHandler.AttackType.Scp0492 => DamageType.Scp0492,
                        _ => DamageType.Unknown,
                    };
                case UniversalDamageHandler universal:
                    DeathTranslation translation = DeathTranslations.TranslationsById[universal.TranslationId];

                    if (DamageTypeExtensions.TranslationIdConversion.ContainsKey(translation.Id))
                        return DamageTypeExtensions.TranslationIdConversion[translation.Id];

                    Log.Warn($"{nameof(DamageHandler)}.{nameof(Type)}: No matching {nameof(DamageType)} for {nameof(UniversalDamageHandler)} with ID {translation.Id}, type will be reported as {DamageType.Unknown}. Report this to EXILED Devs.");
                    break;
                case PlayerStatsSystem.FirearmDamageHandler firearmDamageHandler:
                    return Item.Get<Firearm>(firearmDamageHandler.Firearm).FirearmType switch
                    {
                        FirearmType.A7 => DamageType.A7,
                        FirearmType.Com15 => DamageType.Com15,
                        FirearmType.Com18 => DamageType.Com18,
                        FirearmType.Crossvec => DamageType.Crossvec,
                        FirearmType.Logicer => DamageType.Logicer,
                        FirearmType.Revolver => DamageType.Revolver,
                        FirearmType.Scp127 => DamageType.Scp127,
                        FirearmType.Shotgun => DamageType.Shotgun,
                        FirearmType.AK => DamageType.AK,
                        FirearmType.ParticleDisruptor => DamageType.ParticleDisruptor,
                        FirearmType.E11SR => DamageType.E11Sr,
                        FirearmType.FSP9 => DamageType.Fsp9,
                        FirearmType.FRMG0 => DamageType.Frmg0,
                        _ => DamageType.Firearm
                    };
            }

            return DamageType.Unknown;
        }

        /// <summary>
        /// A wrapper to easily manipulate the behavior of <see cref="BaseHandler.CassieAnnouncement"/>.
        /// </summary>
        public class CassieAnnouncement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CassieAnnouncement"/> class.
            /// </summary>
            /// <param name="announcement">The announcement to be set.</param>
            public CassieAnnouncement(string announcement)
            {
                Announcement = announcement;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CassieAnnouncement"/> class.
            /// </summary>
            /// <param name="announcement">The announcement to be set.</param>
            /// <param name="subtitleParts">The subtitles to be set.</param>
            public CassieAnnouncement(string announcement, IEnumerable<Subtitles.SubtitlePart> subtitleParts)
                : this(announcement)
            {
                SubtitleParts = subtitleParts;
            }

            /// <summary>
            /// Gets the default announcement.
            /// </summary>
            public static CassieAnnouncement Default => BaseHandler.CassieAnnouncement.Default;

            /// <summary>
            /// Gets or sets the announcement.
            /// </summary>
            public string Announcement { get; set; }

            /// <summary>
            /// Gets or sets a <see cref="IEnumerable{T}"/> of <see cref="Subtitles.SubtitlePart"/> which determines the result of the subtitle belonging to this <see cref="CassieAnnouncement"/> instance.
            /// </summary>
            public IEnumerable<Subtitles.SubtitlePart> SubtitleParts { get; set; }

            /// <summary>
            /// Implicitly converts the given <see cref="CassieAnnouncement"/> instance to a <see cref="BaseHandler.CassieAnnouncement"/> object.
            /// </summary>
            /// <param name="cassieAnnouncement">The <see cref="CassieAnnouncement"/> instance.</param>
            public static implicit operator BaseHandler.CassieAnnouncement(CassieAnnouncement cassieAnnouncement) =>
                new()
                {
                    Announcement = cassieAnnouncement.Announcement,
                    SubtitleParts = cassieAnnouncement.SubtitleParts?.ToArray() ?? Array.Empty<Subtitles.SubtitlePart>(),
                };

            /// <summary>
            /// Implicitly converts the given <see cref="BaseHandler.CassieAnnouncement"/> instance to a <see cref="CassieAnnouncement"/> object.
            /// </summary>
            /// <param name="cassieAnnouncement">The <see cref="CassieAnnouncement"/> instance.</param>
            public static implicit operator CassieAnnouncement(BaseHandler.CassieAnnouncement cassieAnnouncement) =>
                new(cassieAnnouncement.Announcement, cassieAnnouncement.SubtitleParts);
        }
    }
}