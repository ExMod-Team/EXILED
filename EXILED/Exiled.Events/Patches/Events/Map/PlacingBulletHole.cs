// -----------------------------------------------------------------------
// <copyright file="PlacingBulletHole.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Attributes;
    using Decals;
    using Exiled.Events.EventArgs.Map;
    using Handlers;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ImpactEffectsModule.ServerSendImpactDecal(RaycastHit, Vector3, DecalPoolType)" />.
    /// Adds the <see cref="Map.PlacingBulletHole" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.PlacingBulletHole))]
    [HarmonyPatch(typeof(ImpactEffectsModule), nameof(ImpactEffectsModule.ServerSendImpactDecal))]
    internal static class PlacingBulletHole
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(PlacingBulletHoleEventArgs));
            LocalBuilder rotation = generator.DeclareLocal(typeof(Quaternion));

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // this.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ImpactEffectsModule), nameof(ImpactEffectsModule.Firearm))),
                    new(OpCodes.Call, typeof(Exiled.API.Features.Items.Item).GetMethods().Where(x => x.Name == "Get").First()),

                    // hit
                    new(OpCodes.Ldarg_1),

                    // PlacingBulletHole ev = new(Item, RaycastHit)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlacingBulletHoleEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Map.OnPlacingBulletHole(ev)
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnPlacingBulletHole))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBulletHoleEventArgs), nameof(PlacingBulletHoleEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // hit.info = ev.Position
                    new(OpCodes.Ldarga_S, 1),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBulletHoleEventArgs), nameof(PlacingBulletHoleEventArgs.Position))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(RaycastHit), nameof(RaycastHit.point))),

                    // hit.normal = ev.Rotation
                    new(OpCodes.Ldarga_S, 1),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBulletHoleEventArgs), nameof(PlacingBulletHoleEventArgs.Rotation))),
                    new(OpCodes.Stloc_S, rotation),
                    new(OpCodes.Ldloca_S, rotation),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Quaternion), nameof(Quaternion.eulerAngles))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(RaycastHit), nameof(RaycastHit.normal))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}