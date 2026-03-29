// -----------------------------------------------------------------------
// <copyright file="PlacingMimicPoint.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp939.Mimicry;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="MimicPointController.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Scp939.PlacingMimicPoint"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp939), nameof(Handlers.Scp939.PlacingMimicPoint))]
    [HarmonyPatch(typeof(MimicPointController), nameof(MimicPointController.ServerProcessCmd))]
    internal class PlacingMimicPoint
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Stfld) + offset;

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(PlacingMimicPointEventArgs));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(this.Owner)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(MimicPointController), nameof(MimicPointController.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this._syncPos
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(MimicPointController), nameof(MimicPointController._syncPos))),

                // true
                new(OpCodes.Ldc_I4_1),

                // PlacingMimicPointEventArgs = new(Player.Get(this.Owner), this._syncPos, true)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlacingMimicPointEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp939.OnPlacingMimicPoint(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Scp939), nameof(Handlers.Scp939.OnPlacingMimicPoint))),

                // if (!ev.IsAllowed)
                //     return
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingMimicPointEventArgs), nameof(PlacingMimicPointEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // this._syncPos = ev.Position
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingMimicPointEventArgs), nameof(PlacingMimicPointEventArgs.Position))),
                new(OpCodes.Stfld, Field(typeof(MimicPointController), nameof(MimicPointController._syncPos))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}