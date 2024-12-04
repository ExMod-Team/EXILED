// -----------------------------------------------------------------------
// <copyright file="Lost.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp079;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079LostSignalHandler.ServerLoseSignal(float)" />.
    /// Adds the <see cref="Scp079.LosingSignal" /> and <see cref="Scp079.LostSignal"/> event.
    /// </summary>
    [EventPatch(typeof(Scp079), nameof(Scp079.LosingSignal))]
    [EventPatch(typeof(Scp079), nameof(Scp079.LostSignal))]
    [HarmonyPatch(typeof(Scp079LostSignalHandler), nameof(Scp079LostSignalHandler.ServerLoseSignal))]
    internal static class Lost
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // Role._lastOwner
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079LostSignalHandler), nameof(Scp079LostSignalHandler.Role))),
                    new(OpCodes.Ldfld, Field(typeof(PlayerRoleBase), nameof(PlayerRoleBase._lastOwner))),

                    // LosingSignalEventArgs ev = new(Role._lastOwner)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(LosingSignalEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Scp079.OnLosingSignal(ev)
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnLosingSignal))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(LosingSignalEventArgs), nameof(LosingSignalEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new[]
                {
                    // Role._lastOwner
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079LostSignalHandler), nameof(Scp079LostSignalHandler.Role))),
                    new(OpCodes.Ldfld, Field(typeof(PlayerRoleBase), nameof(PlayerRoleBase._lastOwner))),

                    // LostSignalEventArgs ev = new(Role._lastOwner)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(LostSignalEventArgs))[0]),

                    // Scp079.OnLosingSignal(ev)
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnLostSignal))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}