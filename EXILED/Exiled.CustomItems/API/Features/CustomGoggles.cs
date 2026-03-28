// -----------------------------------------------------------------------
// <copyright file="CustomGoggles.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using EventArgs;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp1344;

    using InventorySystem.Items.Usables.Scp1344;

    using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.Wearables;

    /// <summary>
    /// The Custom Goggles base class.
    /// </summary>
    public abstract class CustomGoggles : CustomItem
    {
        /// <summary>
        /// Gets or sets the <see cref="ItemType"/> to use for these goggles.
        /// This is locked to <see cref="ItemType.SCP1344"/>.
        /// </summary>
        public override ItemType Type
        {
            get => ItemType.SCP1344;
            set => base.Type = ItemType.SCP1344;
        }

        /// <summary>
        /// Gets or sets the duration, in seconds, that the item has been worn.
        /// </summary>
        public virtual float WearingTime { get; set; } = Scp1344Item.ActivationTime;

        /// <summary>
        /// Gets or sets the time, in seconds, required to remove the item.
        /// </summary>
        public virtual float RemovingTime { get; set; } = Scp1344Item.DeactivationTime;

        /// <summary>
        /// Gets or sets a value indicating whether the default SCP-1344 effect should be removed when wearing the goggles.
        /// </summary>
        public virtual bool Remove1344Effect { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the glasses can be safely removed without bad effects.
        /// </summary>
        public virtual bool CanBeRemoveSafely { get; set; } = true;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem += OnInternalUsingItem;
            Exiled.Events.Handlers.Player.ItemRemoved += OnInternalItemRemoved;
            Exiled.Events.Handlers.Scp1344.Deactivating += OnInternalDeactivating;
            Exiled.Events.Handlers.Scp1344.ChangedStatus += OnInternalChangedStatus;
            Exiled.Events.Handlers.Scp1344.ChangingStatus += OnInternalChangingStatus;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnInternalUsingItem;
            Exiled.Events.Handlers.Player.ItemRemoved -= OnInternalItemRemoved;
            Exiled.Events.Handlers.Scp1344.Deactivating -= OnInternalDeactivating;
            Exiled.Events.Handlers.Scp1344.ChangedStatus -= OnInternalChangedStatus;
            Exiled.Events.Handlers.Scp1344.ChangingStatus -= OnInternalChangingStatus;
            base.UnsubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void OnOwnerChangingRole(OwnerChangingRoleEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            if (Item.Get(ev.Item) is not Scp1344 { IsWorn: true } scp1344)
                return;

            InternalRemove(ev.Player, scp1344);
        }

        /// <summary>
        /// Called when the player equips the goggles.
        /// </summary>
        /// <param name="player">The player who equipped the goggles.</param>
        /// <param name="goggles">The <see cref="Scp1344"/> item being worn.</param>
        protected virtual void OnWornGoggles(Player player, Scp1344 goggles)
        {
        }

        /// <summary>
        /// Called when the player removes the goggles.
        /// </summary>
        /// <param name="player">The player who removed the goggles.</param>
        /// <param name="goggles">The <see cref="Scp1344"/> item being removed.</param>
        protected virtual void OnRemovedGoggles(Player player, Scp1344 goggles)
        {
        }

        private void OnInternalUsingItem(UsingItemEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            if (ev.Item.Type != ItemType.SCP1344)
                return;

            foreach (Item item in ev.Player.Items)
            {
                if (item.Type != ItemType.SCP1344)
                    continue;

                if (item is not Scp1344 { IsWorn: true })
                    continue;

                ev.IsAllowed = false;
                break;
            }
        }

        private void OnInternalDeactivating(DeactivatingEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            if (!Check(ev.Item))
                return;

            if (!CanBeRemoveSafely)
                return;

            ev.NewStatus = Scp1344Status.Idle;
            ev.IsAllowed = false;
        }

        private void OnInternalChangedStatus(ChangedStatusEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            switch (ev.Scp1344Status)
            {
                case Scp1344Status.Deactivating:
                    ev.Scp1344.Base._useTime = Scp1344Item.DeactivationTime - RemovingTime;
                    break;

                case Scp1344Status.Activating:
                    ev.Scp1344.Base._useTime = Scp1344Item.ActivationTime - WearingTime;
                    break;

                case Scp1344Status.Active:
                    InternalEquip(ev.Player, ev.Scp1344);
                    break;
            }
        }

        private void InternalEquip(Player player, Scp1344 goggles)
        {
            if (Remove1344Effect)
            {
                player.DisableEffect(EffectType.Scp1344);
                player.ReferenceHub.EnableWearables(WearableElements.Scp1344Goggles);
            }

            OnWornGoggles(player, goggles);
        }

        private void InternalRemove(Player player, Scp1344 goggles)
        {
            if (!Remove1344Effect)
                player.DisableEffect(EffectType.Scp1344);

            if (CanBeRemoveSafely)
            {
                player.DisableEffect(EffectType.Blinded);
                player.ReferenceHub?.DisableWearables(WearableElements.Scp1344Goggles);
            }

            OnRemovedGoggles(player, goggles);
        }

        private void OnInternalItemRemoved(ItemRemovedEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            if (ev.Item is not Scp1344 { IsWorn: true } scp1344)
                return;

            InternalRemove(ev.Player, scp1344);
        }

        private void OnInternalChangingStatus(ChangingStatusEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            if (!Check(ev.Item))
                return;

            if (ev.Scp1344StatusOld != Scp1344Status.Deactivating || ev.Scp1344StatusNew != Scp1344Status.Idle)
                return;

            InternalRemove(ev.Player, ev.Scp1344);
        }
    }
}
