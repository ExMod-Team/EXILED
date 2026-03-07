// -----------------------------------------------------------------------
// <copyright file="SpawnPrefabType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Utility.Commands
{
    using System;

    using CommandSystem;
    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Command for spawning <see cref="PrefabType"/>.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnPrefabType : ICommand, IUsageProvider
    {
        /// <inheritdoc />
        public string Command { get; } = "PrefabType";

        /// <inheritdoc />
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc />
        public string Description { get; } = "Spawning PrefabType";

        /// <inheritdoc />
        public string[] Usage { get; } = new string[] { $"{string.Join(", ", EnumUtils<PrefabType>.Names)}", };

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
            {
                return false;
            }

            Player.TryGet(sender, out Player player);
            if (player is null)
            {
                response = "You must be a player to use this command.";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = $"Please, use: {Command} {Usage}";
                return false;
            }

            if (Enum.TryParse(arguments.At(0), out PrefabType prefabType))
            {
                response = $"\"{arguments.At(0)}\" is not a valid prefab type.";
                return false;
            }

            PrefabHelper.Spawn(prefabType, player.Position, player.Rotation);

            response = "Spawning prefab type...";
            return true;
        }
    }
}