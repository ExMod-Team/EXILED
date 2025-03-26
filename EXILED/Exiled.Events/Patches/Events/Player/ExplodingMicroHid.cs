// -----------------------------------------------------------------------
// <copyright file="ExplodingMicroHid.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Exiled.Events.Patches.Events.Player
{
    using Attributes;

    using Exiled.Events.EventArgs.Player;

    using Footprinting;
    using HarmonyLib;

    using Interactables;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items.MicroHID.Modules;
    using MapGeneration;

    using PlayerRoles.FirstPersonControl;
    using PlayerStatsSystem;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="InventorySystem.Items.MicroHID.Modules.ChargeFireModeModule.ServerExplode"/>.
    /// Adds the <see cref="ExplodingMicroHid" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ExplodingMicroHID))]
    [HarmonyPatch(typeof(ChargeFireModeModule), nameof(ChargeFireModeModule.ServerExplode))]
    internal static class ExplodingMicroHid
    {
        private static bool Prefix(ref ChargeFireModeModule __instance)
        {
            if (__instance._alreadyExploded)
            {
                return false;
            }

            ExplodingMicroHIDEventArgs ev = new(__instance.MicroHid);
            Exiled.Events.Handlers.Player.OnExplodingMicroHID(ev);

            if (!ev.IsAllowed)
            {
                return false;
            }

            __instance._alreadyExploded = true;
            __instance.MicroHid.BrokenSync.ServerSetBroken();
            ReferenceHub owner = __instance.Item.Owner;
            IFpcRole fpcRole = owner.roleManager.CurrentRole as IFpcRole;
            if (fpcRole == null)
            {
                return false;
            }

            __instance.Energy -= 0.25f;
            Vector3 position = fpcRole.FpcModule.Position;
            int num;
            HitregUtils.OverlapSphere(position, 10f, out num, null);
            for (int i = 0; i < num; i++)
            {
                InteractableCollider interactableCollider;
                if (HitregUtils.DetectionsNonAlloc[i].TryGetComponent<InteractableCollider>(out interactableCollider))
                {
                    IDamageableDoor damageableDoor = interactableCollider.Target as IDamageableDoor;
                    if (damageableDoor != null && __instance.CheckIntercolLineOfSight(position, interactableCollider))
                    {
                        damageableDoor.ServerDamage(350f, DoorDamageType.Grenade, new Footprint(owner));
                    }
                }
            }

            RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPositionRaycasts(position, true);
            foreach (RoomLightController roomLightController in RoomLightController.Instances)
            {
                if (!(roomLightController.Room != roomIdentifier) && roomLightController.LightsEnabled)
                {
                    roomLightController.ServerFlickerLights(1f);
                }
            }

            owner.playerStats.DealDamage(new MicroHidDamageHandler(125f, __instance.MicroHid));
            return true;
        }
    }
}