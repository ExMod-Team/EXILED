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
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="HitscanHitregModuleBase.ServerPerformHitscan" />.
    /// Adds the <see cref="Handlers.Player.Shot" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Shot))]
    [HarmonyPatch(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.ServerPerformHitscan))]
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

            var ev = new ShotEventArgs(hitregModule, hit, hitregModule.Firearm, null);
            Handlers.Player.OnShot(ev);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            /*
            IL_0020: ldarg.1      // targetRay
            IL_0021: ldloca.s     hitInfo
            IL_0023: ldloc.0      // maxDistance
            IL_0024: ldsfld       class CachedLayerMask InventorySystem.Items.Firearms.Modules.HitscanHitregModuleBase::HitregMask
            IL_0029: call         int32 CachedLayerMask::op_Implicit(class CachedLayerMask)
            IL_002e: call         bool [UnityEngine.PhysicsModule]UnityEngine.Physics::Raycast(valuetype [UnityEngine.CoreModule]UnityEngine.Ray, valuetype [UnityEngine.PhysicsModule]UnityEngine.RaycastHit&, float32, int32)
            IL_0033: brtrue.s     IL_0037
            [] <= Here
             */
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

            /*
            IL_0037: ldloca.s     hitInfo
            IL_0039: call         instance class [UnityEngine.PhysicsModule]UnityEngine.Collider [UnityEngine.PhysicsModule]UnityEngine.RaycastHit::get_collider()
            IL_003e: ldloca.s     component
            IL_0040: callvirt     instance bool [UnityEngine.CoreModule]UnityEngine.Component::TryGetComponent<class IDestructible>(!!0/*class IDestructible* /&)
            [] <= Here  // This position is reached whether IDestructible is found or not.
            IL_0045: brfalse.s    IL_005e
             */
            int destructibleGetIndex = newInstructions.FindIndex(i => i.Calls(Method(typeof(Component), nameof(Component.TryGetComponent))));

            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                destructibleGetIndex + 1,
                new[]
                {
                    // var ev = new ShotEventArgs(this, hit, firearm, component);
                    new(OpCodes.Ldarg_0), // this
                    new(OpCodes.Ldloc_1), // hit
                    new(OpCodes.Ldarg_0), // this.Firearm
                    new(OpCodes.Ldloc_2), // component
                    new(OpCodes.Callvirt, PropertyGetter(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.Firearm))),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShotEventArgs))[0]),

                    new(OpCodes.Dup), // Leave ShotEventArgs on the stack
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShot))),

                    // if (!ev.CanHurt) hitInfo.distance = float.MaxValue;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanHurt))),
                    new(OpCodes.Brtrue, continueLabel),

                    new(OpCodes.Ldloc_1), // hitInfo
                    new(OpCodes.Ldc_R4, float.MaxValue), // float.MaxValue
                    new(OpCodes.Stfld, Field(typeof(RaycastHit), nameof(RaycastHit.distance))), // hitInfo.distance = float.MaxValue

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
