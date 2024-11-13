// -----------------------------------------------------------------------
// <copyright file="ExplodingFlashGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.Patches.Generic;
    using HarmonyLib;
    using InventorySystem.Items.ThrowableProjectiles;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="FlashbangGrenade.ServerFuseEnd()"/>.
    /// Adds the <see cref="Handlers.Map.ExplodingGrenade"/> event and <see cref="Config.CanFlashbangsAffectThrower"/>.
    /// </summary>
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.ServerFuseEnd))]
    internal static class ExplodingFlashGrenade
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            var ignoreLabel = generator.DefineLabel();

            var offset = 1;
            var index = newInstructions.FindLastIndex(instruction => instruction.StoresField(Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade._hitPlayerCount)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // ExplodingFlashGrenade.ProcessEvent(this, num)
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(ExplodingFlashGrenade), nameof(ProcessEvent))),

                    // ignore the foreach since exiled overwrite it
                    new(OpCodes.Br_S, ignoreLabel),
                });

            newInstructions[newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ble_S) - 3].WithLabels(ignoreLabel);

            for (var z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void ProcessEvent(FlashbangGrenade instance, float distance)
        {
            var targetToAffect = ListPool<Player>.Pool.Get();
            foreach (var referenceHub in ReferenceHub.AllHubs)
            {
                var player = Player.Get(referenceHub);
                if ((instance.transform.position - referenceHub.transform.position).sqrMagnitude >= distance)
                    continue;
                if (!ExiledEvents.Instance.Config.CanFlashbangsAffectThrower && instance.PreviousOwner.SameLife(new(referenceHub)))
                    continue;
                if (!IndividualFriendlyFire.CheckFriendlyFirePlayer(instance.PreviousOwner, player.ReferenceHub) && !instance.PreviousOwner.SameLife(new(referenceHub)))
                    continue;
                if (Physics.Linecast(instance.transform.position, player.CameraTransform.position, instance._blindingMask))
                    continue;

                targetToAffect.Add(player);
            }

            ExplodingGrenadeEventArgs explodingGrenadeEvent = new(Player.Get(instance.PreviousOwner.Hub), instance, targetToAffect);

            ListPool<Player>.Pool.Return(targetToAffect);

            Handlers.Map.OnExplodingGrenade(explodingGrenadeEvent);

            if (!explodingGrenadeEvent.IsAllowed)
                return;

            foreach (var player in explodingGrenadeEvent.TargetsToAffect)
            {
                instance.ProcessPlayer(player.ReferenceHub);
            }
        }
    }
}