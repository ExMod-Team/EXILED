// -----------------------------------------------------------------------
// <copyright file="CreatedAmnesticCloud.cs" company="ExMod Team">
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
    /// Patches <see cref="Scp939AmnesticCloudInstance.State" /> setter.
    /// to add the <see cref="Scp939.UpdatedCloudState" /> event.
    /// </summary>
    [EventPatch(typeof(Scp939), nameof(Scp939.UpdatedCloudState))]
    [HarmonyPatch(typeof(Scp939AmnesticCloudInstance), nameof(Scp939AmnesticCloudInstance.State), MethodType.Setter)]
    internal static class CreatedAmnesticCloud
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder hub = generator.DeclareLocal(typeof(ReferenceHub));

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new[]
                {
                    // if (!ReferenceHub.TryGetHubNetID(_syncOwner, out ReferenceHub owner))
                    //     return;
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp939AmnesticCloudInstance), nameof(Scp939AmnesticCloudInstance._syncOwner))),
                    new(OpCodes.Ldloca_S, hub.LocalIndex),
                    new(OpCodes.Call, Method(typeof(ReferenceHub), nameof(ReferenceHub.TryGetHubNetID))),
                    new(OpCodes.Brfalse_S, ret),

                    // owner
                    new(OpCodes.Ldloc_S, hub.LocalIndex),

                    // CloudState
                    new(OpCodes.Ldarg_1),

                    // this
                    new(OpCodes.Ldarg_0),

                    // Scp939.OnCreatedAmnesticCloud(new UpdatedCloudStateEventArgs(owner, CloudState, this));
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpdatedCloudStateEventArgs))[0]),
                    new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnUpdatedCloudState))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}