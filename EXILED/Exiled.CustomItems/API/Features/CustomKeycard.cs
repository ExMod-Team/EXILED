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
    using Exiled.API.Features.Items.Keycards;
    using Exiled.API.Features.Lockers;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces.Keycards;
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.EventArgs.Player;
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

        /// <summary>
        /// Gets or sets the wear of a keycard.
        /// </summary>
        /// <remarks>
        /// Only works on CustomSite02 keycards with values 0-4 and CustomMetalCase keycards with values 0-5.
        /// </remarks>
        public virtual byte Wear { get; set; } = byte.MaxValue;

        /// <summary>
        /// Gets or sets the serial number of a keycard.
        /// </summary>
        /// <remarks>
        /// Only works on CustomMetalCase and CustomTaskForce keycards. Max length is 12. Non-numerical characters will be replaced with '-'.
        /// </remarks>
        public virtual string SerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the rank of a keycard.
        /// </summary>
        /// <remarks>
        /// Only works on CustomTaskForce keycards with values 0-3. Value runs in reverse (3 is rank 1, 2 -> 2, 1 -> 3, 0 -> 0).
        /// </remarks>
        public virtual byte Rank { get; set; } = byte.MaxValue;

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
            if (keycard is CustomKeycardItem customKeycard)
            {
                customKeycard.Permissions = Permissions;

                if (KeycardPermissionsColor.HasValue)
                    customKeycard.PermissionsColor = KeycardPermissionsColor.Value;

                if (TintColor.HasValue)
                    customKeycard.Color = TintColor.Value;

                if (!string.IsNullOrEmpty(Name))
                    customKeycard.ItemName = Name;

                if (!string.IsNullOrEmpty(KeycardName) && customKeycard is INameTagKeycard nametag)
                    nametag.NameTag = KeycardName;

                if (customKeycard is ILabelKeycard label)
                {
                    if (!string.IsNullOrEmpty(KeycardLabel))
                        label.Label = KeycardLabel;
                    if (KeycardLabelColor.HasValue)
                        label.LabelColor = KeycardLabelColor.Value;
                }

                if (customKeycard is IWearKeycard wear)
                    wear.Wear = Wear;

                if (customKeycard is ISerialNumberKeycard serialNumber)
                    serialNumber.SerialNumber = SerialNumber;

                if (customKeycard is IRankKeycard rank)
                    rank.Rank = Rank;
            }
            else if (keycard.Base.Customizable)
            {
                // Some keycards have customizable name tags but nothing else. This should handle those.
                DetailBase[] details = keycard.Base.Details;

                NametagDetail? nametag = details.OfType<NametagDetail>().FirstOrDefault();

                if (nametag != null)
                {
                    NametagDetail._customNametag = KeycardName;

                    if (KeycardDetailSynchronizer.Database.Remove(keycard.Serial))
                    {
                        KeycardDetailSynchronizer.ServerProcessItem(keycard.Base);
                    }
                }
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