// -----------------------------------------------------------------------
// <copyright file="ProcessDisarmMessage.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    #pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Disarming;

    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DisarmingHandlers.ServerProcessDisarmMessage" />.
    /// Adds the <see cref="Handlers.Player.Handcuffing" />, <see cref="Handlers.Player.RemovingHandcuffs" />, and <see cref="Handlers.Player.RemovedHandcuffs" /> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Handcuffing))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.RemovingHandcuffs))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.RemovedHandcuffs))]
    [HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
    internal static class ProcessDisarmMessage
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label returnLabel = generator.DefineLabel();

            int offset = -3;
            int index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerRemoveHandcuffsEvent))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Invoking RemovingHandcuffs event
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(msg.PlayerToDisarm)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // UncuffReason.Player
                    new(OpCodes.Ldc_I4_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // RemovingHandcuffsEventArgs ev = new(Player, Player, UncuffReason.Player, true)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovingHandcuffsEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnRemovingHandcuffs(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovingHandcuffs))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingHandcuffsEventArgs), nameof(RemovingHandcuffsEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // Invoking RemovedHandcuffs event
                    // Player.Get(referenceHub)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(msg.PlayerToDisarm)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // UncuffReason.Player
                    new(OpCodes.Ldc_I4_0),

                    // RemovedHandcuffsEventArgs ev = new(Player, Player, UncuffReason.Player)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovedHandcuffsEventArgs))[0]),

                    // Handlers.Player.OnRemovedHandcuffs(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovedHandcuffs))),
                });

            offset = -3;
            index = newInstructions.FindLastIndex(
                instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerHandcuffEvent))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Invoking Handcuffing event
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(msg.PlayerToDisarm)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // HandcuffingEventArgs evHandcuffing = new(Player, Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(HandcuffingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnHandcuffing(evHandcuffing)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHandcuffing))),

                    // if (!evHandcuffing.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(HandcuffingEventArgs), nameof(HandcuffingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="DisarmedPlayers.ValidateEntry(DisarmedPlayers.DisarmedEntry)" />.
    /// Invokes <see cref="Handlers.Player.RemovingHandcuffs" /> and <see cref="Handlers.Player.RemovedHandcuffs" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.RemovingHandcuffs))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.RemovedHandcuffs))]
    [HarmonyPatch(typeof(DisarmedPlayers), nameof(DisarmedPlayers.ValidateEntry))]
    internal static class Uncuff
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label returnLabel = generator.DefineLabel();

            int offset = 2;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(ReferenceHub), nameof(ReferenceHub.TryGetHubNetID)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Invoking RemovingHandcuffs event
                    // Player.Get(Cuffer)
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(Target)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // UncuffReason.CufferDied
                    new(OpCodes.Ldc_I4_2),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // RemovingHandcuffsEventArgs ev = new(Cuffer, Target, UncuffReason.CufferDied, true)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovingHandcuffsEventArgs))[0]),

                    // TODO: Uncomment this part in next major update to prevent breaking changes
                    // new(OpCodes.Dup),

                    // Handlers.Player.OnRemovingHandcuffs(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovingHandcuffs))),

                    // TODO: Uncomment this part in next major update to prevent breaking changes
                    // if (!ev.IsAllowed)
                    //    return true;
                    // new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingHandcuffsEventArgs), nameof(RemovingHandcuffsEventArgs.IsAllowed))),
                    // new(OpCodes.Brfalse_S, returnLabel),

                    // Invoking RemovedHandcuffs event
                    // Player.Get(Cuffer)
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(Target)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // UncuffReason.CufferDied
                    new(OpCodes.Ldc_I4_2),

                    // RemovedHandcuffsEventArgs ev = new(Cuffer, Target, UncuffReason.CufferDied)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovedHandcuffsEventArgs))[0]),

                    // Handlers.Player.OnRemovedHandcuffs(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovedHandcuffs))),
                });

            offset = 5;
            index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(PropertyGetter(typeof(PlayerRoles.PlayerRoleManager), nameof(PlayerRoles.PlayerRoleManager.CurrentRole)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Invoking RemovingHandcuffs event
                    // Player.Get(Cuffer)
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(Target)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // UncuffReason.CufferDied
                    new(OpCodes.Ldc_I4_2),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // RemovingHandcuffsEventArgs ev = new(Cuffer, Target, UncuffReason.CufferDied, true)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovingHandcuffsEventArgs))[0]),

                    // TODO: Uncomment this part in next major update to prevent breaking changes
                    // new(OpCodes.Dup),

                    // Handlers.Player.OnRemovingHandcuffs(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovingHandcuffs))),

                    // TODO: Uncomment this part in next major update to prevent breaking changes
                    // if (!ev.IsAllowed)
                    //    return true;
                    // new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingHandcuffsEventArgs), nameof(RemovingHandcuffsEventArgs.IsAllowed))),
                    // new(OpCodes.Brfalse_S, returnLabel),

                    // Invoking RemovedHandcuffs event
                    // Player.Get(Cuffer)
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(Target)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // UncuffReason.CufferDied
                    new(OpCodes.Ldc_I4_2),

                    // RemovedHandcuffsEventArgs ev = new(Cuffer, Target, UncuffReason.CufferDied)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovedHandcuffsEventArgs))[0]),

                    // Handlers.Player.OnRemovedHandcuffs(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovedHandcuffs))),
                });

            offset = 3;
            index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(PropertyGetter(typeof(UnityEngine.Vector3), nameof(UnityEngine.Vector3.sqrMagnitude)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Invoking RemovingHandcuffs event
                    // Player.Get(Cuffer)
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(Target)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // UncuffReason.OutOfRange
                    new(OpCodes.Ldc_I4_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // RemovingHandcuffsEventArgs ev = new(Cuffer, Target, UncuffReason.OutOfRange, true)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovingHandcuffsEventArgs))[0]),

                    // TODO: Uncomment this part in next major update to prevent breaking changes
                    // new(OpCodes.Dup),

                    // Handlers.Player.OnRemovingHandcuffs(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovingHandcuffs))),

                    // TODO: Uncomment this part in next major update to prevent breaking changes
                    // if (!ev.IsAllowed)
                    //    return true;
                    // new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingHandcuffsEventArgs), nameof(RemovingHandcuffsEventArgs.IsAllowed))),
                    // new(OpCodes.Brfalse_S, returnLabel),

                    // Invoking RemovedHandcuffs event
                    // Player.Get(Cuffer)
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(Target)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // UncuffReason.CufferDied
                    new(OpCodes.Ldc_I4_2),

                    // RemovedHandcuffsEventArgs ev = new(Cuffer, Target, UncuffReason.OutOfRange)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovedHandcuffsEventArgs))[0]),

                    // Handlers.Player.OnRemovedHandcuffs(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovedHandcuffs))),
                });

            newInstructions[newInstructions.Count - 2].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}