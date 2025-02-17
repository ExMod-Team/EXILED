// -----------------------------------------------------------------------
// <copyright file="FixRemoteAdminCommandNotAddNpcToList.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using System.Linq;

    using Exiled.API.Features;

    using GameCore;
    using HarmonyLib;

    /// <summary>
    /// Fix for RA command does not adding <see cref="Npc"/> to <see cref="Npc.List"/>.
    /// </summary>
    [HarmonyPatch(typeof(DummyUtils), nameof(DummyUtils.SpawnDummy))]
    internal static class FixRemoteAdminCommandNotAddNpcToList
    {
        private static void Postfix(ReferenceHub __result)
        {
            Npc npc = new Npc(__result);

            Npc.Dictionary.Add(npc.GameObject, npc);
        }
    }
}
