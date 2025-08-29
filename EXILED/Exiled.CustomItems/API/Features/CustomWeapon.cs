// -----------------------------------------------------------------------
// <copyright file="CustomWeapon.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using System;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;
    using InventorySystem.Items.Firearms.Modules;
    using UnityEngine;

    using Firearm = Exiled.API.Features.Items.Firearm;
    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// The Custom Weapon base class.
    /// </summary>
    public abstract class CustomWeapon : CustomItem
    {
        /// <summary>
        /// Gets or sets value indicating what <see cref="Attachment"/>s the weapon will have.
        /// </summary>
        public virtual AttachmentName[] Attachments { get; set; } = { };

        /// <inheritdoc/>
        public override ItemType Type
        {
            get => base.Type;
            set
            {
                if (!value.IsWeapon(false) && value != ItemType.None)
                    throw new ArgumentOutOfRangeException($"{nameof(Type)}", value, "Invalid weapon type.");

                base.Type = value;
            }
        }

        /// <summary>
        /// Gets or sets the weapon damage.
        /// </summary>
        public virtual float Damage { get; set; } = -1;

        /// <summary>
        /// Gets or sets a value indicating how big of a clip the weapon will have.
        /// </summary>
        /// <remarks>Warning for <see cref="ItemType.GunShotgun"/> and <see cref="ItemType.GunRevolver"/>.
        /// They are not fully compatible with this features.</remarks>
        public virtual byte ClipSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow friendly fire with this weapon on FF-enabled servers.
        /// </summary>
        public virtual bool FriendlyFire { get; set; }

        /// <inheritdoc />
        public override Pickup? Spawn(Vector3 position, Player? previousOwner = null)
        {
            if (Item.Create(Type) is not Firearm firearm)
            {
                Log.Debug($"{nameof(Spawn)}: Item is not Firearm.");
                return null;
            }

            if (!Attachments.IsEmpty())
                firearm.AddAttachment(Attachments);

            Pickup? pickup = firearm.CreatePickup(position);

            if (pickup is null)
            {
                Log.Debug($"{nameof(Spawn)}: Pickup is null.");
                return null;
            }

            if (ClipSize > 0)
                firearm.MagazineAmmo = ClipSize;

            pickup.Weight = Weight;
            pickup.Scale = Scale;
            if (previousOwner is not null)
                pickup.PreviousOwner = previousOwner;

            TrackedSerials.Add(pickup.Serial);
            return pickup;
        }

        /// <inheritdoc />
        public override Pickup? Spawn(Vector3 position, Item item, Player? previousOwner = null)
        {
            if (item is Firearm firearm)
            {
                if (!Attachments.IsEmpty())
                    firearm.AddAttachment(Attachments);

                if (ClipSize > 0)
                    firearm.MagazineAmmo = ClipSize;
                int ammo = firearm.MagazineAmmo;
                Log.Debug($"{nameof(Name)}.{nameof(Spawn)}: Spawning weapon with {ammo} ammo.");
                Pickup? pickup = firearm.CreatePickup(position);
                pickup.Scale = Scale;

                if (previousOwner is not null)
                    pickup.PreviousOwner = previousOwner;

                TrackedSerials.Add(pickup.Serial);
                return pickup;
            }

            return base.Spawn(position, item, previousOwner);
        }

        /// <inheritdoc/>
        public override void Give(Player player, bool displayMessage = true)
        {
            Item item = player.AddItem(Type);

            if (item is Firearm firearm)
            {
                if (!Attachments.IsEmpty())
                    firearm.AddAttachment(Attachments);

                if (ClipSize > 0)
                    firearm.MagazineAmmo = ClipSize;
            }

            Log.Debug($"{nameof(Give)}: Adding {item.Serial} to tracker.");
            TrackedSerials.Add(item.Serial);

            OnAcquired(player, item, displayMessage);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon += OnInternalReloading;
            Exiled.Events.Handlers.Player.ReloadedWeapon += OnInternalReloaded;
            Exiled.Events.Handlers.Player.Shooting += OnInternalShooting;
            Exiled.Events.Handlers.Player.Shot += OnInternalShot;
            Exiled.Events.Handlers.Player.Hurting += OnInternalHurting;
            Exiled.Events.Handlers.Item.ChangingAttachments += OnInternalChangingAttachment;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnInternalReloading;
            Exiled.Events.Handlers.Player.ReloadedWeapon -= OnInternalReloaded;
            Exiled.Events.Handlers.Player.Shooting -= OnInternalShooting;
            Exiled.Events.Handlers.Player.Shot -= OnInternalShot;
            Exiled.Events.Handlers.Player.Hurting -= OnInternalHurting;
            Exiled.Events.Handlers.Item.ChangingAttachments -= OnInternalChangingAttachment;

            base.UnsubscribeEvents();
        }

        /// <summary>
        /// Handles reloading for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ReloadingWeaponEventArgs"/>.</param>
        protected virtual void OnReloading(ReloadingWeaponEventArgs ev)
        {
        }

        /// <summary>
        /// Handles reloaded for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ReloadedWeaponEventArgs"/>.</param>
        protected virtual void OnReloaded(ReloadedWeaponEventArgs ev)
        {
        }

        /// <summary>
        /// Handles shooting for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ShootingEventArgs"/>.</param>
        protected virtual void OnShooting(ShootingEventArgs ev)
        {
        }

        /// <summary>
        /// Handles shot for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ShotEventArgs"/>.</param>
        protected virtual void OnShot(ShotEventArgs ev)
        {
        }

        /// <summary>
        /// Handles hurting for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="HurtingEventArgs"/>.</param>
        protected virtual void OnHurting(HurtingEventArgs ev)
        {
            if (ev.IsAllowed && Damage >= 0)
                ev.Amount = Damage;
        }

        /// <summary>
        /// Handles attachment changing for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ChangingAttachmentsEventArgs"/>.</param>
        protected virtual void OnChangingAttachment(ChangingAttachmentsEventArgs ev)
        {
        }

        private void OnInternalReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            if (ClipSize > 0 && ev.Firearm.TotalAmmo >= ClipSize)
            {
                ev.IsAllowed = false;
                return;
            }

            OnReloading(ev);
        }

        private void OnInternalReloaded(ReloadedWeaponEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            if (ClipSize > 0)
            {
                int ammoChambered = ((AutomaticActionModule?)ev.Firearm.Base.Modules.FirstOrDefault(x => x is AutomaticActionModule))?.SyncAmmoChambered ?? 0;
                int ammoToGive = ClipSize - ammoChambered;

                AmmoType ammoType = ev.Firearm.AmmoType;
                int firearmAmmo = ev.Firearm.MagazineAmmo;
                int ammoDrop = -(ClipSize - firearmAmmo - ammoChambered);

                int ammoInInventory = ev.Player.GetAmmo(ammoType) + firearmAmmo;
                if (ammoToGive < ammoInInventory)
                {
                    ev.Firearm.MagazineAmmo = ammoToGive;
                    int newAmmo = ev.Player.GetAmmo(ammoType) + ammoDrop;
                    ev.Player.SetAmmo(ammoType, (ushort)newAmmo);
                }
                else
                {
                    ev.Firearm.MagazineAmmo = ammoInInventory;
                    ev.Player.SetAmmo(ammoType, 0);
                }
            }

            OnReloaded(ev);
        }

        private void OnInternalShooting(ShootingEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            OnShooting(ev);
        }

        private void OnInternalShot(ShotEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            OnShot(ev);
        }

        private void OnInternalHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker is null)
            {
                return;
            }

            if (ev.Player is null)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: target null");
                return;
            }

            if (!Check(ev.Attacker.CurrentItem))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: !Check()");
                return;
            }

            if (ev.Attacker == ev.Player)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: attacker == target");
                return;
            }

            if (ev.DamageHandler is null)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Handler null");
                return;
            }

            if (!ev.DamageHandler.CustomBase.BaseIs(out FirearmDamageHandler firearmDamageHandler))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Handler not firearm");
                return;
            }

            if (!Check(firearmDamageHandler.Item))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: type != type");
                return;
            }

            if (!FriendlyFire && (ev.Attacker.Role.Team == ev.Player.Role.Team))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: FF is disabled for this weapon!");
                return;
            }

            OnHurting(ev);
        }

        private void OnInternalChangingAttachment(ChangingAttachmentsEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnChangingAttachment(ev);
        }
    }
}
