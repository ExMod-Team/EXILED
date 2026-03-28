// -----------------------------------------------------------------------
// <copyright file="CustomKeycard.cs" company="ExMod Team">
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
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Lockers;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.EventArgs.Player;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items.Keycards;
    using UnityEngine;

    /// <summary>
    /// The Custom keycard base class.
    /// </summary>
    public abstract class CustomKeycard : CustomItem
    {
        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException">Throws if specified <see cref="ItemType"/> is not Keycard.</exception>
        public override ItemType Type
        {
            get => base.Type;
            set
            {
                if (!value.IsKeycard())
                    throw new ArgumentOutOfRangeException(nameof(Type), value, "Invalid keycard type.");

                base.Type = value;
            }
        }

        /// <summary>
        /// Gets or sets name of keycard holder.
        /// </summary>
        public virtual string KeycardName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a label for keycard.
        /// </summary>
        public virtual string KeycardLabel { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a color of keycard label.
        /// </summary>
        public virtual Color32? KeycardLabelColor { get; set; }

        /// <summary>
        /// Gets or sets a tint color.
        /// </summary>
        public virtual Color32? TintColor { get; set; }

        /// <summary>
        /// Gets or sets the permissions for custom keycard.
        /// </summary>
        public virtual KeycardPermissions Permissions { get; set; } = KeycardPermissions.None;

        /// <summary>
        /// Gets or sets a color of keycard permissions.
        /// </summary>
        public virtual Color32? KeycardPermissionsColor { get; set; }

        /// <inheritdoc/>
        public override void Give(Player player, Item item, bool displayMessage = true)
        {
            if (item.Is(out Keycard card))
                SetupKeycard(card);
            base.Give(player, item, displayMessage);
        }

        /// <inheritdoc/>
        public override Pickup? Spawn(Vector3 position, Item item, Player? previousOwner = null)
        {
            if (item.Is(out Keycard card))
                SetupKeycard(card);

            return base.Spawn(position, item, previousOwner);
        }

        /// <summary>
        /// Setups keycard according to this class.
        /// </summary>
        /// <param name="keycard">Item instance.</param>
        protected virtual void SetupKeycard(Keycard keycard)
        {
            if (!keycard.Base.Customizable)
                return;

            DetailBase[] details = keycard.Base.Details;

            NametagDetail? nameDetail = details.OfType<NametagDetail>().FirstOrDefault();

            if (nameDetail != null && !string.IsNullOrEmpty(KeycardName))
                NametagDetail._customNametag = KeycardName;

            CustomItemNameDetail? raNameDetail = details.OfType<CustomItemNameDetail>().FirstOrDefault();

            if (raNameDetail != null)
                raNameDetail.Name = Name;

            CustomLabelDetail? labelDetail = details.OfType<CustomLabelDetail>().FirstOrDefault();

            if (labelDetail != null)
            {
                if (!string.IsNullOrEmpty(KeycardLabel))
                    CustomLabelDetail._customText = KeycardLabel;

                if (KeycardLabelColor.HasValue)
                    CustomLabelDetail._customColor = KeycardLabelColor.Value;
            }

            CustomPermsDetail? permsDetail = details.OfType<CustomPermsDetail>().FirstOrDefault();

            if (permsDetail != null)
            {
                CustomPermsDetail._customLevels = new((DoorPermissionFlags)Permissions);
                CustomPermsDetail._customColor = KeycardPermissionsColor;
            }

            CustomTintDetail? tintDetail = details.OfType<CustomTintDetail>().FirstOrDefault();

            if (tintDetail != null && TintColor.HasValue)
            {
                CustomTintDetail._customColor = TintColor.Value;
            }
        }

        /// <summary>
        /// Called when custom keycard interacts with a door.
        /// </summary>
        /// <param name="player">Owner of Custom keycard.</param>
        /// <param name="door">Door with which interacting.</param>
        protected virtual void OnInteractingDoor(Player player, Door door)
        {
        }

        /// <summary>
        /// Called when custom keycard interacts with a locker.
        /// </summary>
        /// <param name="player">Owner of Custom keycard.</param>
        /// <param name="chamber">Chamber with which interacting.</param>
        protected virtual void OnInteractingLocker(Player player, Chamber chamber)
        {
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.InteractingDoor += OnInternalInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker += OnInternalInteractingLocker;
            Exiled.Events.Handlers.Item.KeycardInteracting += OnInternalKeycardInteracting;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.InteractingDoor -= OnInternalInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker -= OnInternalInteractingLocker;
            Exiled.Events.Handlers.Item.KeycardInteracting -= OnInternalKeycardInteracting;
        }

        private void OnInternalKeycardInteracting(KeycardInteractingEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            OnInteractingDoor(ev.Player, ev.Door);
        }

        private void OnInternalInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnInteractingDoor(ev.Player, ev.Door);
        }

        private void OnInternalInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnInteractingLocker(ev.Player, ev.InteractingChamber);
        }
    }
}