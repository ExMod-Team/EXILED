// -----------------------------------------------------------------------
// <copyright file="FixLockerBeingCallLate.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using HarmonyLib;
    using MapGeneration.Distributors;

    /// <summary>
    /// Patches the <see cref="Locker.Start"/> delegate.
    /// Fix NW using Update instead of Start.
    /// </summary>
    [HarmonyPatch(typeof(Locker), nameof(Locker.Start))]
    internal class FixLockerBeingCallLate
    {
        private static void Postfix(Locker __instance)
        {
            if (!__instance._serverChambersFilled)
            {
                __instance.ServerFillChambers();
                __instance._serverChambersFilled = true;
            }
        }
    }
}
