// -----------------------------------------------------------------------
// <copyright file="EndingSense.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Exiled.Events.Patches.Events.Scp049
{
    using Exiled.API.Features.Items;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp049;
    using HarmonyLib;
    using InventorySystem.Items.Usables.Scp1344;
    using PlayerRoles.PlayableScps.Scp049;

    /// <summary>
    /// Patches <see cref="Scp049SenseAbility.ServerLoseTarget"/>.
    /// Adds the <see cref="Handlers.Scp049.EndingSense" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.EndingSense))]
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerLoseTarget))]
    internal static class EndingSense
    {
        private static bool Prefix(ref Scp049SenseAbility __instance)
        {
            EndingSenseEventArgs ev = new EndingSenseEventArgs(API.Features.Player.Get(__instance.Owner), API.Features.Player.Get(__instance.Target));
            Exiled.Events.Handlers.Scp049.OnEndingSense(ev);

            if (!ev.IsAllowed)
            {
                return false;
            }

            __instance.HasTarget = false;
            __instance.Cooldown.Trigger(ev.Cooldown);
            __instance.ServerSendRpc(true);
            return true;
        }
    }
}