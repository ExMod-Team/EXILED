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
    using Exiled.API.Interfaces;
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.EventArgs.Player;
    using Interactables.Interobjects.DoorUtils;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Autosync;
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
        public virtual Color32? KeycardLabelColor { get; set; } = new Color32(100, 100, 100, 222);

        /// <summary>
        /// Gets or sets a tint color.
        /// </summary>
        public virtual Color32? TintColor { get; set; } = new Color32(100, 200, 100, 222);

        /// <summary>
        /// Gets or sets the permissions for custom keycard.
        /// </summary>
        public virtual KeycardPermissions Permissions { get; set; } = KeycardPermissions.None;

        /// <summary>
        /// Gets or sets a color of keycard permissions.
        /// </summary>
        public virtual Color32? KeycardPermissionsColor { get; set; } = new Color32(100, 100, 200, 222);

        /// <inheritdoc/>
        public override void Give(Player player, Item item, bool displayMessage = true)
        {
            
            Log.Info("Wuhhh 11111");
            if (item.Is(out Keycard card))
                SetupKeycard(card);
            base.Give(player, item, displayMessage);

        }

        /// <inheritdoc/>
        public override Pickup? Spawn(Vector3 position, Item item, Player? previousOwner = null)
        {
            Log.Info("Wuhhh 122222");
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
            Log.Info("waaa");
            if (!keycard.Base.Customizable)
            {
                Log.Info("Not Customizable");
                return;
            }
            // InventoryItemLoader.TryGetItem<KeycardItem>(keycard.Base.ItemTypeId, out item);
            // if (!keycard.Base.TryGetTemplate(keycard.Base.ItemTypeId, out BaseKeycardItem template))
            // {
            //     
            // }
            //     
            DetailBase[] details = keycard.Base.Details;
            
            
            CustomItemNameDetail? raNameDetail = details.OfType<CustomItemNameDetail>().FirstOrDefault();

            if (raNameDetail != null)
            {
                if (string.IsNullOrEmpty(Name))
                {
                    Name = string.Empty;
                }
                raNameDetail.Name = Name;
            }
            



            CustomLabelDetail? labelDetail = details.OfType<CustomLabelDetail>().FirstOrDefault();

            if (labelDetail != null)
            {
                if (string.IsNullOrEmpty(KeycardLabel))
                {
                    KeycardLabel = string.Empty;
                }
                CustomLabelDetail._customText = KeycardLabel;


                KeycardLabelColor ??= new Color32(100, 200, 200, 222);
                CustomLabelDetail._customColor = KeycardLabelColor.Value;
            }

            CustomPermsDetail? permsDetail = details.OfType<CustomPermsDetail>().FirstOrDefault();

            if (permsDetail != null)
            {
                CustomPermsDetail._customLevels = new((DoorPermissionFlags)Permissions);
                KeycardPermissionsColor ??= new Color32(200, 100, 200, 222);
                CustomPermsDetail._customColor = KeycardPermissionsColor;
            }
            
            CustomRankDetail? rankDetail = details.OfType<CustomRankDetail>().FirstOrDefault();

            if (rankDetail != null)
            {

                rankDetail.SetArguments(new ArraySegment<object>(new object[]
                {
                    1,
                }, 0, 1));
            }
            
            // SerialNumberDetail? serialDetail = details.OfType<SerialNumberDetail>().FirstOrDefault();
            //
            // if (serialDetail != null)
            // {
            //
            //     serialDetail.SetArguments(new ArraySegment<object>(new object[]
            //     {
            //         0
            //     }, 0, 1));
            // }


            CustomTintDetail? tintDetail = details.OfType<CustomTintDetail>().FirstOrDefault();
            if (tintDetail != null)
            {
                TintColor ??= new Color32(100, 200, 100, 200);
                CustomTintDetail._customColor = TintColor.Value;
            }

            CustomWearDetail? wearDetail = details.OfType<CustomWearDetail>().FirstOrDefault();
            if (wearDetail != null)
            {
                wearDetail.SetArguments(new ArraySegment<object>(
                    new object[]
                {
                    1,
                }, 0, 1));
            }

            NametagDetail? nameDetail = details.OfType<NametagDetail>().FirstOrDefault();

            if (nameDetail != null)
            {
                if (string.IsNullOrEmpty(KeycardName))
                {
                    KeycardName = string.Empty;
                }
                NametagDetail._customNametag = KeycardName;
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