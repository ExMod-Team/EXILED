// -----------------------------------------------------------------------
// <copyright file="PermissionsProvider.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.Keycards
{
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items;
    using InventorySystem.Items.Autosync;
    using InventorySystem.Items.Keycards;
    using Mirror;

    /// <summary>
    /// A simple class for providing permissions with <see cref="DoorPermissionFlags"/>.
    /// </summary>
    public class PermissionsProvider : IDoorPermissionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionsProvider"/> class.
        /// </summary>
        /// <param name="flags">The flags of this provider.</param>
        /// <param name="type">The <see cref="ItemType"/> to imitate.</param>
        /// <param name="serial">The serial to imitate.</param>
        public PermissionsProvider(DoorPermissionFlags flags, ItemType type, ushort serial)
        {
            Flags = flags;
            Type = type;
            Serial = serial;

            PermissionsUsedCallback = (_, success) =>
            {
                using (new AutosyncRpc(new ItemIdentifier(Type, Serial), out NetworkWriter writer))
                {
                    writer.WriteSubheader(KeycardItem.MsgType.OnKeycardUsed);
                    writer.WriteBool(success);
                }
            };
        }

        /// <summary>
        /// Gets or sets the permissions of this provider.
        /// </summary>
        public DoorPermissionFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ItemType"/> this <see cref="PermissionsProvider"/> imitates.
        /// </summary>
        public ItemType Type { get; set; }

        /// <summary>
        /// Gets or sets the serial this <see cref="PermissionsProvider"/> imitates.
        /// </summary>
        public ushort Serial { get; set; }

        /// <summary>
        /// Gets the action called when this <see cref="PermissionsProvider"/> is used.
        /// </summary>
        public PermissionUsed PermissionsUsedCallback { get; }

        /// <summary>
        /// Gets the permissions of this <see cref="PermissionsProvider"/>.
        /// </summary>
        /// <param name="requester">Not used.</param>
        /// <returns>The <see cref="DoorPermissionFlags"/> of this <see cref="PermissionsProvider"/>.</returns>
        public DoorPermissionFlags GetPermissions(IDoorPermissionRequester requester)
        {
            return Flags;
        }
    }
}