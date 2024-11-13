// -----------------------------------------------------------------------
// <copyright file="ExplodingFragGrenade.cs" company="Exiled Team">
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
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;

    using Footprinting;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ExplosionGrenade.Explode(Footprint, Vector3, ExplosionGrenade)"/>.
    /// Adds the <see cref="Handlers.Map.ExplodingGrenade"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.ExplodingGrenade))]
    [HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.Explode))]
    internal static class ExplodingFragGrenade
    {
        /// <summary>
        /// Trims colliders from the given array.
        /// </summary>
        /// <param name="ev"><inheritdoc cref="ExplodingGrenadeEventArgs"/></param>
        /// <param name="colliderArray">The list of colliders to trim from.</param>
        /// <returns>An array of colliders.</returns>
        public static Collider[] TrimColliders(ExplodingGrenadeEventArgs ev, Collider[] colliderArray)
        {
            var colliders = ListPool<Collider>.Pool.Get();

            foreach (var collider in colliderArray)
            {
                if (!collider.TryGetComponent(out IDestructible dest) || Player.Get(dest.NetworkId) is not Player player || ev.TargetsToAffect.Contains(player))
                {
                    colliders.Add(collider);
                }
            }

            var array = colliders.ToArray();
            ListPool<Collider>.Pool.Return(colliders);

            return array;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            var offset = 1;
            var index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_3) + offset;

            var returnLabel = generator.DefineLabel();

            var ev = generator.DeclareLocal(typeof(ExplodingGrenadeEventArgs));

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // attacker;
                    new(OpCodes.Ldarg_0),

                    // position
                    new(OpCodes.Ldarg_1),

                    // grenade
                    new(OpCodes.Ldarg_2),

                    // Collider[]
                    new(OpCodes.Ldloc_3),

                    // ExplodingGrenadeEventArgs ev = new(player, position, grenade, colliders);
                    new(OpCodes.Newobj, DeclaredConstructor(typeof(ExplodingGrenadeEventArgs), new[] { typeof(Footprint), typeof(Vector3), typeof(ExplosionGrenade), typeof(Collider[]) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Map.OnExplodingGrenade(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnExplodingGrenade))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // colliders = TrimColliders(ev, colliders)
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Ldloc_3),
                    new(OpCodes.Call, Method(typeof(ExplodingFragGrenade), nameof(TrimColliders))),
                    new(OpCodes.Stloc_3),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (var z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}