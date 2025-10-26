// -----------------------------------------------------------------------
// <copyright file="PlayingFootstep.cs" company="ExMod Team">
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
    using PlayerRoles.FirstPersonControl.Thirdperson;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerRoles.PlayableScps.Scp939.Ripples;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FootstepRippleTrigger.OnFootstepPlayed(CharacterModel, float)" />
    /// to add the <see cref="Scp939.PlayingFootstep" /> event.
    /// </summary>
    [EventPatch(typeof(Scp939), nameof(Scp939.PlayingFootstep))]
    [HarmonyPatch(typeof(FootstepRippleTrigger), nameof(FootstepRippleTrigger.OnFootstepPlayed))]
    internal static class PlayingFootstep
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player target = Player.Get(characterModel.OwnerHub);
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(CharacterModel), nameof(CharacterModel.OwnerHub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // Player player = Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<Scp939Role>), nameof(StandardSubroutine<Scp939Role>.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // true;
                new(OpCodes.Ldc_I4_1),

                // PlayingFootstepEventArgs ev = (player, scp939, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlayingFootstepEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp939.OnPlayingFootstep(ev);
                new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnPlayingFootstep))),

                // if (!IsAllowed)
                //      return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlayingFootstepEventArgs), nameof(PlayingFootstepEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
