// -----------------------------------------------------------------------
// <copyright file="InteractingDoor.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using Attributes;
    using EventArgs.Player;
    using Handlers;
    using HarmonyLib;
    using Interactables.Interobjects.DoorUtils;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;

    /// <summary>
    /// Patches <see cref="DoorVariant.ServerInteract(ReferenceHub, byte)" />.
    /// Adds the <see cref="Handlers.Player.InteractingDoor" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.InteractingDoor))]
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal static class InteractingDoor
    {
#pragma warning disable SA1313
        private static bool Prefix(DoorVariant __instance, ReferenceHub ply, byte colliderId)
        {
            PermissionUsed callback;
            bool canOpen = __instance.RequiredPermissions.CheckPermissions(ply, __instance, out callback);
            bool pluginRequestSent = false;

            InteractingDoorEventArgs exiledEvent = new(API.Features.Player.Get(ply), __instance, colliderId, canOpen);
            Exiled.Events.Handlers.Player.OnInteractingDoor(exiledEvent);

            if (!exiledEvent.CanInteract)
                return false;
            canOpen = exiledEvent.IsAllowed;

            if (__instance.ActiveLocks > 0 && !__instance.TryResolveLock(ply, out pluginRequestSent))
            {
                if (__instance._remainingDeniedCooldown <= 0.0)
                {
                    __instance._remainingDeniedCooldown = __instance.DeniedCooldown;
                    __instance.LockBypassDenied(ply, colliderId);
                    if (callback != null)
                        callback(__instance, false);
                }

                PlayerEvents.OnInteractedDoor(new PlayerInteractedDoorEventArgs(ply, __instance, canOpen));
            }
            else
            {
                if (!__instance.AllowInteracting(ply, colliderId))
                    return false;
                if (!pluginRequestSent)
                {
                    PlayerInteractingDoorEventArgs ev = new(ply, __instance, canOpen);
                    PlayerEvents.OnInteractingDoor(ev);
                    if (!ev.IsAllowed)
                        return false;
                    canOpen = ev.CanOpen;
                }

                if (canOpen)
                {
                    __instance.NetworkTargetState = !__instance.TargetState;
                    __instance._triggerPlayer = ply;
                    if (__instance.DoorName != null)
                        ServerLogs.AddLog(ServerLogs.Modules.Door, ply.LoggedNameFromRefHub() + " " + (__instance.TargetState ? "opened" : "closed") + " " + __instance.DoorName + ".", ServerLogs.ServerLogType.GameEvent);
                    if (callback != null)
                        callback(__instance, true);
                }
                else if (__instance._remainingDeniedCooldown <= 0.0)
                {
                    __instance._remainingDeniedCooldown = __instance.DeniedCooldown;
                    __instance.PermissionsDenied(ply, colliderId);
                    DoorEvents.TriggerAction(__instance, DoorAction.AccessDenied, ply);
                    if (callback != null)
                        callback(__instance, false);
                }

                PlayerEvents.OnInteractedDoor(new PlayerInteractedDoorEventArgs(ply, __instance, canOpen));
            }

            return false;
        }

        /*
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingDoorEventArgs));

            CodeInstruction[] interactingEvent = new CodeInstruction[]
            {
                // Player.Get(ply)
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // __instance
                new(OpCodes.Ldarg_0),

                // colliderId
                new(OpCodes.Ldarg_2),

                // IsAllowed
                new(OpCodes.Ldloc_1),

                // CanInteract
                new(OpCodes.Ldc_I4_1),

                // InteractingDoorEventArgs ev = new(Player.Get(ply), __instance, colliderId, bool, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingDoorEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                // Handlers.Player.OnInteractingDoor(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingDoor))),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // if (!ev.CanInteract) return false;
                new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.CanInteract))),
                new(OpCodes.Brfalse_S, retLabel),

                // CanInteract = ev.IsAllowed (Reminder __instance is done on purpose because we prefer than IDeniableEvent when cancel use CanInteract and not Interacting)
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))),
                new(OpCodes.Stloc_1),
            };

            int offset = 4;
            int index = newInstructions.FindLastIndex(x => x.Calls(Method(typeof(DoorLockUtils), nameof(DoorLockUtils.HasFlagFast), new System.Type[] { typeof(DoorLockMode), typeof(DoorLockMode) }))) + offset;
            newInstructions.InsertRange(index, interactingEvent);

            offset = 2;
            index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloc_0) + offset;
            newInstructions.InsertRange(index, interactingEvent);

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }*/
    }
}