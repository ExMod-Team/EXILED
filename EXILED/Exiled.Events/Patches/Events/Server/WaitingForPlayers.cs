// -----------------------------------------------------------------------
// <copyright file="WaitingForPlayers.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using API.Features;

    using HarmonyLib;

    /// <summary>
    ///     Patches <see cref="CharacterClassManager.Start" />.
    ///     Adds the <see cref="Handlers.Server.WaitingForPlayers" /> event.
    /// </summary>
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.Start))]
    internal static class WaitingForPlayers
    {
        private static void Postfix(ReferenceHub __instance)
        {
            if (!__instance.isLocalPlayer)
                return;

            Server.Host = new Player(__instance);
            Handlers.Server.OnWaitingForPlayers();
        }
    }
}
