// -----------------------------------------------------------------------
// <copyright file="UsingVigor.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp106;
    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp106;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="UsingVigor"/>.
    /// <see cref="Handlers.Scp106.UsingVigor" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp106), nameof(Handlers.Scp106.UsingVigor))]
    [HarmonyPatch(typeof(Scp106VigorAbilityBase), nameof(Scp106VigorAbilityBase.VigorAmount), MethodType.Setter)]
    internal static class UsingVigor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Declare local variable for UsingVigorEventArgs
            LocalBuilder ev = generator.DeclareLocal(typeof(UsingVigorEventArgs));

            // Continue label for isAllowed check
            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                    // Player.Get(this.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106VigorAbilityBase), nameof(Scp106VigorAbilityBase.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // current vigor value
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106VigorAbilityBase), nameof(Scp106VigorAbilityBase.Vigor))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(VigorStat), nameof(VigorStat.CurValue))),

                    // new value
                    new(OpCodes.Ldarg_1),

                     // true (IsAllowed)
                    new(OpCodes.Ldc_I4_1),

                    // UsingVigorEventArgs ev = new(player, old vigor, new vigor, isallowed)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingVigorEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp106.OnUsingVigor(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp106), nameof(Handlers.Scp106.OnUsingVigor))),

                    // if (!ev.IsAllowed) return;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingVigorEventArgs), nameof(UsingVigorEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // Return;
                    new(OpCodes.Ret),

                    // continue label
                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),

                    // value = ev.NewVigor;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingVigorEventArgs), nameof(UsingVigorEventArgs.NewVigor))),
                    new(OpCodes.Starg_S, 1),
            });

            // Return the new instructions
            foreach (CodeInstruction newInstruction in newInstructions)
                yield return newInstruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
