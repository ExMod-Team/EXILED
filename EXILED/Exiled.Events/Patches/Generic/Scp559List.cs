// -----------------------------------------------------------------------
// <copyright file="Scp559List.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
#pragma warning disable CS0618

    using Exiled.API.Features;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Scp559Cake.Start"/>
    /// to control <see cref="Scp559.List"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp559Cake), nameof(Scp559Cake.Start))]
    internal class Scp559List
    {
        private static void Postfix(Scp559Cake __instance)
        {
            _ = new Scp559(__instance);
        }
    }
}