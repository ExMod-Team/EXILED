// -----------------------------------------------------------------------
// <copyright file="SpawningTeamVehicle.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;

    using HarmonyLib;

    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="WaveUpdateMessage.ServerSendUpdate"/>.
    /// Adds the <see cref="Handlers.Map.SpawningTeamVehicle"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.SpawningTeamVehicle))]
    [HarmonyPatch(typeof(WaveUpdateMessage), nameof(WaveUpdateMessage.ServerSendUpdate))]
    internal static class SpawningTeamVehicle
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            LocalBuilder msg = generator.DeclareLocal(typeof(WaveUpdateMessage));

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Newobj) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new(OpCodes.Stloc_S, msg.LocalIndex),

                // if (type != RespawnEffectsController.EffectType.Selection)
                //    goto continueLabel;
                new(OpCodes.Ldloca_S, msg.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(WaveUpdateMessage), nameof(WaveUpdateMessage.IsTrigger))),
                new(OpCodes.Brfalse_S, continueLabel),

                // wave
                new(OpCodes.Ldarg_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // SpawningTeamVehicleEventArgs ev = new(SpawnableWaveBase, bool);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningTeamVehicleEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Map.OnSpawningTeamVehicle(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnSpawningTeamVehicle))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningTeamVehicleEventArgs), nameof(SpawningTeamVehicleEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),

                new CodeInstruction(OpCodes.Ldloc_S, msg.LocalIndex).WithLabels(continueLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}