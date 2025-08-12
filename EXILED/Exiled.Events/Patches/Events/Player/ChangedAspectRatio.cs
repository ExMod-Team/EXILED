// -----------------------------------------------------------------------
// <copyright file="ChangedAspectRatio.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using CentralAuth;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AspectRatioSync.UserCode_CmdSetAspectRatio__Single" />.
    /// Adds the <see cref="Handlers.Player.ChangedRatio" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangedRatio))]
    [HarmonyPatch(typeof(AspectRatioSync), nameof(AspectRatioSync.UserCode_CmdSetAspectRatio__Single))]
    internal class ChangedAspectRatio
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder oldRatio = generator.DeclareLocal(typeof(float));
            LocalBuilder hub = generator.DeclareLocal(typeof(ReferenceHub));

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // float oldRatio = this.AspectRatio;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AspectRatioSync), nameof(AspectRatioSync.AspectRatio))),
                new(OpCodes.Stloc_S, oldRatio.LocalIndex),
            });

            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ret);
            newInstructions[index].WithLabels(retLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // ReferenceHub hub = this.GetComponent<ReferenceHub>();
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Component), nameof(Component.GetComponent)).MakeGenericMethod(typeof(ReferenceHub))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, hub.LocalIndex),

                // if (hub.authManager._targetInstanceMode != ClientInstanceMode.ReadyClient) return;
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.authManager))),
                new(OpCodes.Ldfld, Field(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager._targetInstanceMode))),
                new(OpCodes.Ldc_I4, 1),
                new(OpCodes.Bne_Un_S, retLabel),

                // hub
                new(OpCodes.Ldloc_S, hub.LocalIndex),

                // oldRatio
                new(OpCodes.Ldloc_S, oldRatio.LocalIndex),

                // aspectRatio
                new(OpCodes.Ldarg_1),

                // ChangedRatioEventArgs ev = new ChangedRatioEventArgs(ReferenceHub, float, float)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangedRatioEventArgs))[0]),

                // Handlers.Player.OnChangedRatio(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangedRatio))),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
