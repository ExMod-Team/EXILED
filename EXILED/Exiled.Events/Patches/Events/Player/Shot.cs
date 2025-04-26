// -----------------------------------------------------------------------
// <copyright file="Shot.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Firearms.Modules.Misc;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="HitscanHitregModuleBase.ServerPerformHitscan" />.
    /// Adds the <see cref="Handlers.Player.Shot" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Shot))]
    [HarmonyPatch(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.ServerAppendPrescan))]
    internal static class Shot
    {
        private static void ProcessRaycastMiss(HitscanHitregModuleBase hitregModule, Ray ray, float maxDistance)
        {
            RaycastHit hit = new()
            {
                distance = maxDistance,
                point = ray.GetPoint(maxDistance),
                normal = -ray.direction,
            };

            ShotEventArgs ev = new(hitregModule, hit, hitregModule.Firearm, null);
            Handlers.Player.OnShot(ev);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            MethodInfo raycastMethod = Method(typeof(Physics), nameof(Physics.Raycast), new[] { typeof(Ray), typeof(RaycastHit).MakeByRefType(), typeof(float), typeof(int) });
            int raycastFailIndex = newInstructions.FindIndex(i => i.Calls(raycastMethod)) + 2;

            newInstructions.InsertRange(
                raycastFailIndex,
                new CodeInstruction[]
                {
                    // ProcessRaycastMiss(this, targetRay, maxDistance);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Shot), nameof(ProcessRaycastMiss))),
                });

            int destructibleGetIndex = newInstructions.FindIndex(i => i.operand is MethodInfo { Name: nameof(Component.TryGetComponent) }) + 1;

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ShotEventArgs));

            newInstructions.InsertRange(
                destructibleGetIndex,
                new[]
                {
                    // var ev = new ShotEventArgs(this, hitInfo, firearm, component);
                    new(OpCodes.Ldarg_0), // this
                    new(OpCodes.Ldloc_1), // hitInfo
                    new(OpCodes.Ldarg_0), // this.Firearm
                    new(OpCodes.Callvirt, PropertyGetter(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.Firearm))),
                    new(OpCodes.Ldloc_2), // component
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShotEventArgs))[0]),
                    new(OpCodes.Dup), // Leave ShotEventArgs on the stack
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    new(OpCodes.Dup), // Leave ShotEventArgs on the stack
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShot))),

                    // if (!ev.CanHurt) hitInfo.distance = maxDistance;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanHurt))),
                    new(OpCodes.Brtrue, continueLabel),

                    new(OpCodes.Ldloca_S, 1), // hitInfo address
                    new(OpCodes.Ldloc_0), // maxDistance
                    new(OpCodes.Call, PropertySetter(typeof(RaycastHit), nameof(RaycastHit.distance))), // hitInfo.distance = maxDistance

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            int impactEffectsIndex = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(HitscanResult), nameof(HitscanResult.Obstacles)))) - 1;
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                impactEffectsIndex,
                new[]
                {
                    // if (!ev.CanSpawnImpactEffects) return;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).MoveLabelsFrom(newInstructions[impactEffectsIndex]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanSpawnImpactEffects))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
