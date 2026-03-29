// -----------------------------------------------------------------------
// <copyright file="PlacedAmnesticCloud.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp939;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp939AmnesticCloudAbility.OnStateEnabled" />
    /// to add the <see cref="Scp939.PlacedAmnesticCloud" /> event.
    /// </summary>
    [EventPatch(typeof(Scp939), nameof(Scp939.PlacedAmnesticCloud))]
    [HarmonyPatch(typeof(Scp939AmnesticCloudAbility), nameof(Scp939AmnesticCloudAbility.OnStateEnabled))]
    internal static class PlacedAmnesticCloud
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder cloud = generator.DeclareLocal(typeof(Scp939AmnesticCloudInstance));

            const int offset = -2;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Callvirt && x.OperandIs(Method(typeof(Scp939AmnesticCloudInstance), nameof(Scp939AmnesticCloudInstance.ServerSetup)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Scp939AmnesticCloudInstance  cloud = Object.Instantiate<Scp939AmnesticCloudInstance>(this._instancePrefab)
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Stloc_S, cloud),
                });

            index = newInstructions.Count - 1;

            // Scp939.OnPlacedAmnesticCloud(new PlacedAmnesticCloudEventArgs(this.Owner, cloud));
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this.Owner
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp939AmnesticCloudAbility), nameof(Scp939AmnesticCloudAbility.Owner))),

                    // cloud
                    new CodeInstruction(OpCodes.Ldloc_S, cloud),

                    // Scp939.OnPlacedAmnesticCloud(new PlacedAmnesticCloudEventArgs(this.Owner, cloud));
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlacedAmnesticCloudEventArgs))[0]),
                    new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnPlacedAmnesticCloud))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}