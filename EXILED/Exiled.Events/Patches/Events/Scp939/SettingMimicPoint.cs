// -----------------------------------------------------------------------
// <copyright file="SettingMimicPoint.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Exiled.Events.Patches.Events.Scp939
{
    using Attributes;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp939.Mimicry;
    using RelativePositioning;

    /// <summary>
    /// Patches <see cref="MimicPointController.ServerProcessCmd"/>.
    /// Adds the <see cref="Handlers.Scp939.SettingMimicPoint" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp939), nameof(Handlers.Scp939.SettingMimicPoint))]
    [HarmonyPatch(typeof(MimicPointController), nameof(MimicPointController.ServerProcessCmd))]
    internal static class SettingMimicPoint
    {
        private static bool Prefix(ref MimicPointController __instance, ref NetworkReader reader)
        {
            __instance.ServerProcessCmd(reader);
            if (__instance.Active)
            {
                __instance._syncMessage = MimicPointController.RpcStateMsg.RemovedByUser;
                __instance.Active = false;
            }
            else
            {
                __instance._syncMessage = MimicPointController.RpcStateMsg.PlacedByUser;
                __instance._syncPos = new RelativePosition(__instance.CastRole.FpcModule.Position);
                __instance.Active = true;
            }

            __instance.ServerSendRpc(true);
            return true;
        }
    }
}