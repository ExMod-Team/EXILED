// -----------------------------------------------------------------------
// <copyright file="ChangingAttachmentsEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;
    using API.Features.Items;
    using API.Structs;

    using Exiled.API.Extensions;

    using Interfaces;

    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// Contains all information before changing item attachments.
    /// </summary>
    public class ChangingAttachmentsEventArgs : IPlayerEvent, IDeniableEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAttachmentsEventArgs" /> class.
        /// </summary>
        /// <param name="request">
        /// The request received from client.
        /// </param>
        public ChangingAttachmentsEventArgs(AttachmentsChangeRequest request)
        {
            Firearm = Item.Get<Firearm>(request.WeaponSerial);
            Player = Firearm.Owner;
            NewAttachmentIdentifiers = Firearm.FirearmType.GetAttachmentIdentifiers(request.AttachmentsCode).ToList();
        }

        /// <summary>
        /// Gets the old <see cref="AttachmentIdentifier" />.
        /// </summary>
        public IEnumerable<AttachmentIdentifier> CurrentAttachmentIdentifiers => Firearm.AttachmentIdentifiers;

        /// <summary>
        /// Gets or sets the new <see cref="AttachmentIdentifier" />.
        /// </summary>
        public List<AttachmentIdentifier> NewAttachmentIdentifiers { get; set; }

        /// <summary>
        /// Gets the <see cref="CurrentAttachmentIdentifiers" /> code.
        /// </summary>
        public uint CurrentCode => Firearm.Base.GetCurrentAttachmentsCode();

        /// <summary>
        /// Gets the <see cref="NewAttachmentIdentifiers" /> code.
        /// </summary>
        public uint NewCode => NewAttachmentIdentifiers.GetAttachmentsCode();

        /// <summary>
        /// Gets or sets a value indicating whether the attachments can be changed.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm" /> which is being modified.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the <see cref="API.Features.Player" /> who's changing attachments.
        /// </summary>
        public Player Player { get; }
    }
}