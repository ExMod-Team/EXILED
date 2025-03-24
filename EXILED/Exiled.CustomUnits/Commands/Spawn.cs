// -----------------------------------------------------------------------
// <copyright file="Spawn.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomUnits.Commands
{
    using System;
    using System.Collections.Generic;

    using CommandSystem;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.CustomUnits.API.Features;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// A command that spawns a custom unit.
    /// </summary>
    [CommandHandler(typeof(Parent))]
    public class Spawn : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "spawn";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "sp", "force" };

        /// <inheritdoc/>
        public string Description { get; } = "Spawns a unit.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("customroles.give"))
                {
                    response = "Permission Denied, required: customroles.give";
                    return false;
                }

                if (arguments.Count == 0)
                {
                    response = "give <Custom role name/Custom role ID> [Nickname/PlayerID/UserID/all/*]";
                    return false;
                }

                if (!CustomUnit.TryGet(arguments.At(0), out CustomUnit? unit) || unit is null)
                {
                    response = $"Custom role {arguments.At(0)} not found!";
                    return false;
                }

                if (arguments.Count == 1)
                {
                    unit.Spawn();

                    response = $"Unit {unit.Name} ({unit.Id}) spawn forced.";
                    return false;
                }

                List<Player> players = ListPool<Player>.Pool.Get(arguments.At(1) switch
                {
                    "*" or "all" => Player.List,
                    _ => arguments.At(0).ParsePlayers(),
                });

                unit.Spawn(players);

                response = $"Forced spawn of {unit.Name} with {players.Count} players.";
                ListPool<Player>.Pool.Return(players);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                response = "Error";
                return false;
            }
        }
    }
}