// -----------------------------------------------------------------------
// <copyright file="MirrorExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    using AdminToys;
    using AudioPooling;
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Items.Keycards;
    using Exiled.API.Features.Pickups.Keycards;
    using Features;
    using Features.Pools;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Autosync;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Keycards;
    using MEC;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.Blood;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.PlayableScps.Scp1507;
    using PlayerRoles.Spectating;
    using PlayerRoles.Voice;
    using RelativePositioning;
    using Respawning;
    using UnityEngine;
    using Utils.Networking;

    /// <summary>
    /// A set of extensions for <see cref="Mirror"/> Networking.
    /// </summary>
    public static class MirrorExtensions
    {
        private static readonly Dictionary<Type, MethodInfo> WriterExtensionsValue = new();
        private static readonly Dictionary<string, ulong> SyncVarDirtyBitsValue = new();
        private static readonly Dictionary<string, string> RpcFullNamesValue = new();
        private static readonly ReadOnlyDictionary<Type, MethodInfo> ReadOnlyWriterExtensionsValue = new(WriterExtensionsValue);
        private static readonly ReadOnlyDictionary<string, ulong> ReadOnlySyncVarDirtyBitsValue = new(SyncVarDirtyBitsValue);
        private static readonly ReadOnlyDictionary<string, string> ReadOnlyRpcFullNamesValue = new(RpcFullNamesValue);
        private static MethodInfo setDirtyBitsMethodInfoValue;
        private static MethodInfo sendSpawnMessageMethodInfoValue;
        private static string[] adminToyBaseSyncVarsValue;

        /// <summary>
        /// Gets <see cref="MethodInfo"/> corresponding to <see cref="Type"/>.
        /// </summary>
        public static ReadOnlyDictionary<Type, MethodInfo> WriterExtensions
        {
            get
            {
                if (WriterExtensionsValue.Count == 0)
                {
                    foreach (MethodInfo method in typeof(NetworkWriterExtensions).GetMethods().Where(x => !x.IsGenericMethod && x.GetCustomAttribute(typeof(ObsoleteAttribute)) == null && (x.GetParameters()?.Length == 2)))
                        WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);

                    Type fuckNorthwood = Assembly.GetAssembly(typeof(RoleTypeId)).GetType("Mirror.GeneratedNetworkCode");
                    foreach (MethodInfo method in fuckNorthwood.GetMethods().Where(x => !x.IsGenericMethod && (x.GetParameters()?.Length == 2) && (x.ReturnType == typeof(void))))
                        WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);

                    foreach (Type serializer in typeof(ServerConsole).Assembly.GetTypes().Where(x => x.Name.EndsWith("Serializer")))
                    {
                        foreach (MethodInfo method in serializer.GetMethods().Where(x => (x.ReturnType == typeof(void)) && x.Name.StartsWith("Write")))
                            WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);
                    }
                }

                return ReadOnlyWriterExtensionsValue;
            }
        }

        /// <summary>
        /// Gets a all DirtyBit <see cref="ulong"/> from <see cref="StringExtensions"/>(format:classname.methodname).
        /// </summary>
        public static ReadOnlyDictionary<string, ulong> SyncVarDirtyBits
        {
            get
            {
                if (SyncVarDirtyBitsValue.Count == 0)
                {
                    foreach (PropertyInfo property in typeof(ServerConsole).Assembly.GetTypes()
                        .SelectMany(x => x.GetProperties())
                        .Where(m => m.Name.StartsWith("Network")))
                    {
                        MethodInfo setMethod = property.GetSetMethod();

                        if (setMethod is null)
                            continue;

                        MethodBody methodBody = setMethod.GetMethodBody();

                        if (methodBody is null)
                            continue;

                        byte[] bytecodes = methodBody.GetILAsByteArray();

                        if (!SyncVarDirtyBitsValue.ContainsKey($"{property.ReflectedType.Name}.{property.Name}"))
                            SyncVarDirtyBitsValue.Add($"{property.ReflectedType.Name}.{property.Name}", bytecodes[bytecodes.LastIndexOf((byte)OpCodes.Ldc_I8.Value) + 1]);
                    }
                }

                return ReadOnlySyncVarDirtyBitsValue;
            }
        }

        /// <summary>
        /// Gets Rpc's FullName <see cref="string"/> corresponding to <see cref="StringExtensions"/>(format:classname.methodname).
        /// </summary>
        public static ReadOnlyDictionary<string, string> RpcFullNames
        {
            get
            {
                if (RpcFullNamesValue.Count == 0)
                {
                    foreach (MethodInfo method in typeof(ServerConsole).Assembly.GetTypes()
                        .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                        .Where(m => m.GetCustomAttributes(typeof(ClientRpcAttribute), false).Length > 0 || m.GetCustomAttributes(typeof(TargetRpcAttribute), false).Length > 0))
                    {
                        MethodBody methodBody = method.GetMethodBody();

                        if (methodBody is null)
                            continue;

                        byte[] bytecodes = methodBody.GetILAsByteArray();

                        if (!RpcFullNamesValue.ContainsKey($"{method.ReflectedType.Name}.{method.Name}"))
                            RpcFullNamesValue.Add($"{method.ReflectedType.Name}.{method.Name}", method.Module.ResolveString(BitConverter.ToInt32(bytecodes, bytecodes.IndexOf((byte)OpCodes.Ldstr.Value) + 1)));
                    }
                }

                return ReadOnlyRpcFullNamesValue;
            }
        }

        /// <summary>
        /// Gets a <see cref="NetworkBehaviour.SetSyncVarDirtyBit(ulong)"/>'s <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo SetDirtyBitsMethodInfo => setDirtyBitsMethodInfoValue ??= typeof(NetworkBehaviour).GetMethod(nameof(NetworkBehaviour.SetSyncVarDirtyBit));

        /// <summary>
        /// Gets a NetworkServer.SendSpawnMessage's <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo SendSpawnMessageMethodInfo => sendSpawnMessageMethodInfoValue ??= typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Gets all <see cref="AdminToyBase"/> sync var names.
        /// </summary>
        public static string[] AdminToyBaseSyncVars => adminToyBaseSyncVarsValue ??= typeof(AdminToyBase).GetProperties().Where(property => property.Name.Contains("Network")).Select(property => property.Name).ToArray();

        /// <summary>
        /// Plays a beep sound that only the target <paramref name="player"/> can hear.
        /// </summary>
        /// <param name="player">Target to play sound to.</param>
        public static void PlayBeepSound(this Player player) => SendFakeTargetRpc(player, ReferenceHub._hostHub.networkIdentity, typeof(AmbientSoundPlayer), nameof(AmbientSoundPlayer.RpcPlaySound), 7);

        /// <summary>
        /// Set <see cref="Player.CustomInfo"/> on the <paramref name="target"/> player that only the <paramref name="player"/> can see.
        /// </summary>
        /// <param name="player">Only this player can see info.</param>
        /// <param name="target">Target to set info.</param>
        /// <param name="info">Setting info.</param>
        public static void SetPlayerInfoForTargetOnly(this Player player, Player target, string info) => player.SendFakeSyncVar(target.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_customPlayerInfoString), info);

        /// <summary>
        /// Plays a gun sound that only the <paramref name="player"/> can hear.
        /// </summary>
        /// <param name="player">Target to play.</param>
        /// <param name="position">Position to play on.</param>
        /// <param name="itemType">Weapon' sound to play.</param>
        /// <param name="volume">Sound's volume to set.</param>
        /// <param name="audioClipId">GunAudioMessage's audioClipId to set (default = 0).</param>
        [Obsolete("This method is not working. Use PlayGunSound(Player, Vector3, FirearmType, float, int, bool) overload instead.")]
        public static void PlayGunSound(this Player player, Vector3 position, ItemType itemType, byte volume, byte audioClipId = 0)
            => PlayGunSound(player, position, itemType.GetFirearmType(), volume, audioClipId);

        /// <summary>
        /// Plays a gun sound that only the <paramref name="player"/> can hear.
        /// </summary>
        /// <param name="player">Target to play.</param>
        /// <param name="position">Position to play on.</param>
        /// <param name="firearmType">Weapon's sound to play.</param>
        /// <param name="pitch">Speed of sound.</param>
        /// <param name="clipIndex">Index of clip.</param>
        public static void PlayGunSound(this Player player, Vector3 position, FirearmType firearmType, float pitch = 1, int clipIndex = 0)
        {
            if (firearmType is FirearmType.ParticleDisruptor or FirearmType.None)
                return;

            Features.Items.Firearm firearm = Features.Items.Firearm.ItemTypeToFirearmInstance[firearmType];

            if (firearm == null)
                return;

            using (NetworkWriterPooled writer = NetworkWriterPool.Get())
            {
                writer.WriteUShort(NetworkMessageId<RoleSyncInfo>.Id);
                new RoleSyncInfo(Server.Host.ReferenceHub, RoleTypeId.ClassD, player.ReferenceHub, null).Write(writer);
                writer.WriteRelativePosition(new RelativePosition(0, 0, 0, 0, false));
                writer.WriteUShort(0);
                player.Connection.Send(writer);
            }

            firearm.BarrelAmmo = 1;
            firearm.BarrelMagazine.IsCocked = true;
            player.SendFakeSyncVar(Server.Host.Inventory.netIdentity, typeof(Inventory), nameof(Inventory.NetworkCurItem), firearm.Identifier);

            if (!firearm.Base.TryGetModule(out AudioModule audioModule))
                return;

            Timing.CallDelayed(0.1f, () => // due to selecting item we need to delay shot a bit
            {
                audioModule.SendRpc(player.ReferenceHub, writer =>
                    audioModule.ServerSend(writer, clipIndex, pitch, MixerChannel.Weapons, 12f, position, false));

                player.SendFakeSyncVar(Server.Host.Inventory.netIdentity, typeof(Inventory), nameof(Inventory.NetworkCurItem), ItemIdentifier.None);

                player.Connection.Send(new RoleSyncInfo(Server.Host.ReferenceHub, Server.Host.Role, player.ReferenceHub, null));
            });
        }

        /// <summary>
        /// Place blood that only the <paramref name="player"/> can see.
        /// </summary>
        /// <param name="player">Target to play.</param>
        /// <param name="position">The position of the blood decal.</param>
        /// <param name="origin">The direction of the blood decal.</param>
        /// <param name="roleTypeId">The RoleTypeId from who blood come from.</param>
        /// <param name="gettingShotSoundIndex">The sound than player get when getting shot.</param>
        public static void PlaceBlood(this Player player, Vector3 position, Vector3 origin, RoleTypeId roleTypeId, int gettingShotSoundIndex)
        {
            if (!roleTypeId.TryGetRoleBase(out PlayerRoleBase playerRoleBase) || playerRoleBase is not IBleedableRole)
                return;

            Features.Items.Firearm firearm = Features.Items.Firearm.ItemTypeToFirearmInstance[FirearmType.Com15];

            if (firearm == null)
                return;

            using (NetworkWriterPooled writer = NetworkWriterPool.Get())
            {
                writer.WriteUShort(NetworkMessageId<RoleSyncInfo>.Id);
                new RoleSyncInfo(Server.Host.ReferenceHub, RoleTypeId.ClassD, player.ReferenceHub, null).Write(writer);
                writer.WriteRelativePosition(new RelativePosition(0, 0, 0, 0, false));
                writer.WriteUShort(0);
                player.Connection.Send(writer);
            }

            player.SendFakeSyncVar(Server.Host.Inventory.netIdentity, typeof(Inventory), nameof(Inventory.NetworkCurItem), firearm.Identifier);

            if (!firearm.Base.TryGetModule(out ImpactEffectsModule impactEffectsModule))
                return;

            Timing.CallDelayed(0.1f, () => // due to selecting item we need to delay shot a bit
            {
                using (NetworkWriterPooled writer = NetworkWriterPool.Get())
                {
#pragma warning disable SA1116 // Split parameters should start on line after declaration
                    impactEffectsModule.SendRpc(writer =>
                    {
                        writer.WriteSubheader(ImpactEffectsModule.RpcType.PlayerHit);
                        writer.WriteReferenceHub(Server.Host.ReferenceHub);
                        writer.WriteRelativePosition(new RelativePosition(position));
                        writer.WriteRelativePosition(new RelativePosition(origin));
                        writer.WriteByte(255);
                        writer.WriteRoleType(RoleTypeId.ClassD);
                    }, true);
#pragma warning restore SA1116 // Split parameters should start on line after declaration
                }

                player.SendFakeSyncVar(Server.Host.Inventory.netIdentity, typeof(Inventory), nameof(Inventory.NetworkCurItem), ItemIdentifier.None);

                player.Connection.Send(new RoleSyncInfo(Server.Host.ReferenceHub, Server.Host.Role, player.ReferenceHub, null));
            });
        }

        /// <summary>
        /// Sets <see cref="Features.Intercom.DisplayText"/> that only the <paramref name="target"/> player can see.
        /// </summary>
        /// <param name="target">Only this player can see Display Text.</param>
        /// <param name="text">Text displayed to the player.</param>
        public static void SetIntercomDisplayTextForTargetOnly(this Player target, string text) => target.SendFakeSyncVar(IntercomDisplay._singleton.netIdentity, typeof(IntercomDisplay), nameof(IntercomDisplay.Network_overrideText), text);

        /// <summary>
        /// Resync <see cref="Features.Intercom.DisplayText"/>.
        /// </summary>
        public static void ResetIntercomDisplayText() => ResyncSyncVar(IntercomDisplay._singleton.netIdentity, typeof(IntercomDisplay), nameof(IntercomDisplay.Network_overrideText));

        /// <summary>
        /// Sets <see cref="Room.Color"/> of a <paramref name="room"/> that only the <paramref name="target"/> player can see.
        /// </summary>
        /// <param name="room">Room to modify.</param>
        /// <param name="target">Only this player can see room color.</param>
        /// <param name="color">Color to set.</param>
        public static void SetRoomColorForTargetOnly(this Room room, Player target, Color color) => target.SendFakeSyncVar(room.RoomLightControllerNetIdentity, typeof(RoomLightController), nameof(RoomLightController.NetworkOverrideColor), color);

        /// <summary>
        /// Sets the lights of a <paramref name="room"/> to be either on or off, visible only to the <paramref name="target"/> player.
        /// </summary>
        /// <param name="room">The room to modify the lights of.</param>
        /// <param name="target">The player who will see the lights state change.</param>
        /// <param name="value">The state to set the lights to. True for on, false for off.</param>
        public static void SetRoomLightsForTargetOnly(this Room room, Player target, bool value) => target.SendFakeSyncVar(room.RoomLightControllerNetIdentity, typeof(RoomLightController), nameof(RoomLightController.NetworkLightsEnabled), value);

        /// <summary>
        /// Sets <see cref="Player.DisplayNickname"/> of a <paramref name="player"/> that only the <paramref name="target"/> player can see.
        /// </summary>
        /// <param name="target">Only this player can see the name changed.</param>
        /// <param name="player">Player that will desync the CustomName.</param>
        /// <param name="name">Nickname to set.</param>
        public static void SetName(this Player target, Player player, string name)
        {
            target.SendFakeSyncVar(player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), name);
        }

        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleTypeId"/> changes.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="type">Model type.</param>
        /// <param name="skipJump">Whether to skip the little jump that works around an invisibility issue.</param>
        /// <param name="unitId">The UnitNameId to use for the player's new role, if the player's new role uses unit names. (is NTF).</param>
        public static void ChangeAppearance(this Player player, RoleTypeId type, bool skipJump = false, byte unitId = 0) => ChangeAppearance(player, type, Player.List.Where(x => x != player), skipJump, unitId);

        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleTypeId"/> changes.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="type">Model type.</param>
        /// <param name="playersToAffect">The players who should see the changed appearance.</param>
        /// <param name="skipJump">Whether to skip the little jump that works around an invisibility issue.</param>
        /// <param name="unitId">The UnitNameId to use for the player's new role, if the player's new role uses unit names. (is NTF).</param>
        public static void ChangeAppearance(this Player player, RoleTypeId type, IEnumerable<Player> playersToAffect, bool skipJump = false, byte unitId = 0)
        {
            if (!player.IsConnected || !RoleExtensions.TryGetRoleBase(type, out PlayerRoleBase roleBase))
                return;

            bool isRisky = type.GetTeam() is Team.Dead || player.IsDead;

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteUShort(38952);
            writer.WriteUInt(player.NetId);
            writer.WriteRoleType(type);

            if (roleBase is HumanRole humanRole && humanRole.UsesUnitNames)
            {
                if (player.Role.Base is not HumanRole)
                    isRisky = true;
                writer.WriteByte(unitId);
            }

            if (roleBase is ZombieRole)
            {
                if (player.Role.Base is not ZombieRole)
                    isRisky = true;

                writer.WriteUShort((ushort)Mathf.Clamp(Mathf.CeilToInt(player.MaxHealth), ushort.MinValue, ushort.MaxValue));
                writer.WriteBool(true);
            }

            if (roleBase is Scp1507Role)
            {
                if (player.Role.Base is not Scp1507Role)
                    isRisky = true;

                writer.WriteByte((byte)player.Role.SpawnReason);
            }

            if (roleBase is FpcStandardRoleBase fpc)
            {
                if (player.Role.Base is not FpcStandardRoleBase playerfpc)
                    isRisky = true;
                else
                    fpc = playerfpc;

                ushort value = 0;
                fpc?.FpcModule.MouseLook.GetSyncValues(0, out value, out ushort _);
                writer.WriteRelativePosition(player.RelativePosition);
                writer.WriteUShort(value);
            }

            foreach (Player target in playersToAffect)
            {
                if (target != player || !isRisky)
                    target.Connection.Send(writer.ToArraySegment());
                else
                    Log.Error($"Prevent Seld-Desync of {player.Nickname} with {type}");
            }

            NetworkWriterPool.Return(writer);

            // To counter a bug that makes the player invisible until they move after changing their appearance, we will teleport them upwards slightly to force a new position update for all clients.
            if (!skipJump)
                player.Position += Vector3.up * 0.25f;
        }

        /// <summary>
        /// Resynchronizes a specific effect from the effect owner to the target player.
        /// </summary>
        /// <param name="effectOwner">The player who owns the effect to be resynchronized.</param>
        /// <param name="target">The target player to whom the effect will be resynchronized.</param>
        /// <param name="effect">The type of effect to be resynchronized.</param>
        public static void ResyncEffectTo(this Player effectOwner, Player target, EffectType effect) => effectOwner.SendFakeEffectTo(target, effect, effectOwner.GetEffect(effect).Intensity);

        /// <summary>
        /// Resynchronizes a specific effect from the effect owner to the target players.
        /// </summary>
        /// <param name="effectOwner">The player who owns the effect to be resynchronized.</param>
        /// <param name="targets">The list of target players to whom the effect will be resynchronized.</param>
        /// <param name="effect">The type of effect to be resynchronized.</param>
        public static void ResyncEffectTo(this Player effectOwner, IEnumerable<Player> targets, EffectType effect) => effectOwner.SendFakeEffectTo(targets, effect, effectOwner.GetEffect(effect).Intensity);

        /// <summary>
        /// Sends a fake effect to a list of target players, simulating the effect as if it originated from the effect owner.
        /// </summary>
        /// <param name="effectOwner">The player who owns the effect.</param>
        /// <param name="targets">The list of target players to whom the effect will be sent.</param>
        /// <param name="effect">The type of effect to be sent.</param>
        /// <param name="intensity">The intensity of the effect.</param>
        public static void SendFakeEffectTo(this Player effectOwner, IEnumerable<Player> targets, EffectType effect, byte intensity)
        {
            foreach (Player target in targets)
            {
                effectOwner.SendFakeEffectTo(target, effect, intensity);
            }
        }

        /// <summary>
        /// Sends a fake effect to a target player, simulating the effect as if it originated from the effect owner.
        /// </summary>
        /// <param name="effectOwner">The player who owns the effect.</param>
        /// <param name="target">The target player to whom the effect will be sent.</param>
        /// <param name="effect">The type of effect to be sent.</param>
        /// <param name="intensity">The intensity of the effect.</param>
        public static void SendFakeEffectTo(this Player effectOwner, Player target, EffectType effect, byte intensity)
        {
            SendFakeSyncObject(target, effectOwner.NetworkIdentity, typeof(PlayerEffectsController), (writer) =>
            {
                StatusEffectBase foundEffect = effectOwner.GetEffect(effect);
                int foundIndex = effectOwner.ReferenceHub.playerEffectsController.AllEffects.IndexOf(foundEffect);
                if (foundIndex == -1)
                {
                    Log.Error($"Effect {effect} not found in {effectOwner.Nickname}'s effects list.");
                    return;
                }

                writer.WriteULong(0b0001);
                writer.WriteUInt(1);
                writer.WriteByte((byte)SyncList<byte>.Operation.OP_SET);
                writer.WriteUInt((uint)foundIndex);
                writer.WriteByte(intensity);
            });
        }

        /// <summary>
        /// Makes a player not spectatable to another player.
        /// </summary>
        /// <param name="target">The player who will become not spectatable.</param>
        /// <param name="viewer">The viewer who will see this change.</param>
        /// <param name="value">The faked value.</param>
        public static void SetFakeSpectatable(Player target, Player viewer, bool value) => viewer.Connection.Send(new SpectatableVisibilityMessages.SpectatableVisibilityMessage(target.ReferenceHub, value));

        /// <summary>
        /// Makes the server resend a message to all clients updating a keycards details to current values.
        /// </summary>
        /// <param name="customKeycardItem">The keycard to resync.</param>
        public static void ResyncKeycardItem(CustomKeycardItem customKeycardItem)
        {
            if (KeycardDetailSynchronizer.Database.Remove(customKeycardItem.Serial))
            {
                KeycardDetailSynchronizer.ServerProcessItem(customKeycardItem.Base);
            }
        }

        /// <summary>
        /// Makes the server resend a message to all clients updating a keycards details to current values.
        /// </summary>
        /// <param name="customKeycard">The keycard to resync.</param>
        public static void ResyncKeycardPickup(CustomKeycardPickup customKeycard)
        {
            if (KeycardDetailSynchronizer.Database.Remove(customKeycard.Serial))
            {
                KeycardDetailSynchronizer.ServerProcessPickup(customKeycard.Base);
            }
        }

        /// <summary>
        /// Send CASSIE announcement that only <see cref="Player"/> can hear.
        /// </summary>
        /// <param name="player">Target to send.</param>
        /// <param name="words">Announcement words.</param>
        /// <param name="makeHold">Same on <see cref="Cassie.Message(string, bool, bool, bool)"/>'s isHeld.</param>
        /// <param name="makeNoise">Same on <see cref="Cassie.Message(string, bool, bool, bool)"/>'s isNoisy.</param>
        /// <param name="isSubtitles">Same on <see cref="Cassie.Message(string, bool, bool, bool)"/>'s isSubtitles.</param>
        public static void PlayCassieAnnouncement(this Player player, string words, bool makeHold = false, bool makeNoise = true, bool isSubtitles = false)
        {
            foreach (RespawnEffectsController controller in RespawnEffectsController.AllControllers)
            {
                if (controller != null)
                {
                    SendFakeTargetRpc(player, controller.netIdentity, typeof(RespawnEffectsController), nameof(RespawnEffectsController.RpcCassieAnnouncement), words, makeHold, makeNoise, isSubtitles);
                }
            }
        }

        /// <summary>
        /// Send CASSIE announcement with custom subtitles for translation that only <see cref="Player"/> can hear and see it.
        /// </summary>
        /// <param name="player">Target to send.</param>
        /// <param name="words">The message to be reproduced.</param>
        /// <param name="translation">The translation should be show in the subtitles.</param>
        /// <param name="customSubtitles">The custom subtitles to show.</param>
        /// <param name="makeHold">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isHeld.</param>
        /// <param name="makeNoise">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isNoisy.</param>
        /// <param name="isSubtitles">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isSubtitles.</param>
        public static void MessageTranslated(this Player player, string words, string translation, string customSubtitles, bool makeHold = false, bool makeNoise = true, bool isSubtitles = true)
        {
            StringBuilder announcement = StringBuilderPool.Pool.Get();

            string[] cassies = words.Split('\n');
            string[] translations = translation.Split('\n');

            for (int i = 0; i < cassies.Length; i++)
                announcement.Append($"{translations[i].Replace(' ', ' ')}<size=0> {cassies[i]} </size><split>");

            string message = StringBuilderPool.Pool.ToStringReturn(announcement);

            foreach (RespawnEffectsController controller in RespawnEffectsController.AllControllers)
            {
                if (controller != null)
                {
                    SendFakeTargetRpc(player, controller.netIdentity, typeof(RespawnEffectsController), nameof(RespawnEffectsController.RpcCassieAnnouncement), message, makeHold, makeNoise, isSubtitles, customSubtitles);
                }
            }
        }

        /// <summary>
        /// Moves object for the player.
        /// </summary>
        /// <param name="player">Target to send.</param>
        /// <param name="identity">The <see cref="Mirror.NetworkIdentity"/> to move.</param>
        /// <param name="pos">The position to change.</param>
        public static void MoveNetworkIdentityObject(this Player player, NetworkIdentity identity, Vector3 pos)
        {
            if (identity == null)
                return;

            identity.gameObject.transform.position = pos;
            ObjectDestroyMessage objectDestroyMessage = new()
            {
                netId = identity.netId,
            };

            player.Connection.Send(objectDestroyMessage, 0);
            SendSpawnMessageMethodInfo?.Invoke(null, new object[] { identity, player.Connection });
        }

        /// <summary>
        /// Sends to the player a Fake Change Scene.
        /// </summary>
        /// <param name="player">The player to send the Scene.</param>
        /// <param name="newSceneName">The new Scene the client will load.</param>
        public static void SendFakeSceneLoading(this Player player, ScenesType newSceneName)
        {
            SceneMessage message = new()
            {
                sceneName = newSceneName.ToString(),
            };

            player.Connection.Send(message);
        }

        /// <summary>
        /// Emulation of the method SCP:SL uses to change scene.
        /// </summary>
        /// <param name="scene">The new Scene the client will load.</param>
        public static void ChangeSceneToAllClients(ScenesType scene)
        {
            SceneMessage message = new()
            {
                sceneName = scene.ToString(),
            };

            NetworkServer.SendToAll(message);
        }

        /// <summary>
        /// Scales an object for the specified player.
        /// </summary>
        /// <param name="player">Target to send.</param>
        /// <param name="identity">The <see cref="Mirror.NetworkIdentity"/> to scale.</param>
        /// <param name="scale">The scale the object needs to be set to.</param>
        public static void ScaleNetworkIdentityObject(this Player player, NetworkIdentity identity, Vector3 scale)
        {
            if (identity == null)
                return;

            identity.gameObject.transform.localScale = scale;
            ObjectDestroyMessage objectDestroyMessage = new()
            {
                netId = identity.netId,
            };

            player.Connection.Send(objectDestroyMessage, 0);
            SendSpawnMessageMethodInfo?.Invoke(null, new object[] { identity, player.Connection });
        }

        /// <summary>
        /// Moves object for all the players.
        /// </summary>
        /// <param name="identity">The <see cref="NetworkIdentity"/> to move.</param>
        /// <param name="pos">The position to change.</param>
        public static void MoveNetworkIdentityObject(this NetworkIdentity identity, Vector3 pos)
        {
            if (identity == null)
                return;

            identity.gameObject.transform.position = pos;
            ObjectDestroyMessage objectDestroyMessage = new()
            {
                netId = identity.netId,
            };

            foreach (Player ply in Player.List)
            {
                ply.Connection.Send(objectDestroyMessage, 0);
                SendSpawnMessageMethodInfo?.Invoke(null, new object[] { identity, ply.Connection });
            }
        }

        /// <summary>
        /// Scales an object for all players.
        /// </summary>
        /// <param name="identity">The <see cref="Mirror.NetworkIdentity"/> to scale.</param>
        /// <param name="scale">The scale the object needs to be set to.</param>
        public static void ScaleNetworkIdentityObject(this NetworkIdentity identity, Vector3 scale)
        {
            if (identity == null)
                return;

            identity.gameObject.transform.localScale = scale;
            ObjectDestroyMessage objectDestroyMessage = new()
            {
                netId = identity.netId,
            };

            foreach (Player ply in Player.List)
            {
                ply.Connection.Send(objectDestroyMessage, 0);
                SendSpawnMessageMethodInfo?.Invoke(null, new object[] { identity, ply.Connection });
            }
        }

        /// <summary>
        /// Send fake values to client's <see cref="SyncVarAttribute"/>.
        /// </summary>
        /// <typeparam name="T">Target SyncVar property type.</typeparam>
        /// <param name="target">Target to send.</param>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="propertyName">Property name starting with Network.</param>
        /// <param name="value">Value of send to target.</param>
        public static void SendFakeSyncVar<T>(this Player target, NetworkIdentity behaviorOwner, Type targetType, string propertyName, T value)
        {
            if (!target.IsConnected || behaviorOwner == null)
                return;

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            NetworkWriterPooled writer2 = NetworkWriterPool.Get();
            MakeCustomSyncWriter(behaviorOwner, targetType, null, CustomSyncVarGenerator, writer, writer2);
            target.Connection.Send(new EntityStateMessage
            {
                netId = behaviorOwner.netId,
                payload = writer.ToArraySegment(),
            });

            NetworkWriterPool.Return(writer);
            NetworkWriterPool.Return(writer2);
            void CustomSyncVarGenerator(NetworkWriter targetWriter)
            {
                bool isAdminToy = targetType.BaseType == typeof(AdminToyBase);

                // all admin toys have toy-specific sync vars, so we need to write correct dirty bits for both the toys DeserializeSyncVars, but also AdminToyBase.DeserializeSyncVars in correct order
                bool isBase = AdminToyBaseSyncVars.Contains(propertyName);

                if (isAdminToy && !isBase)
                {
                    // no base sync var changes
                    targetWriter.WriteULong(0);
                }

                targetWriter.WriteULong(SyncVarDirtyBits[$"{targetType.Name}.{propertyName}"]);
                WriterExtensions[typeof(T)]?.Invoke(null, new object[2] { targetWriter, value });

                if (isAdminToy && isBase)
                {
                    // no sub-toy sync var changes
                    targetWriter.WriteULong(0);
                }
            }
        }

        /// <summary>
        /// Force resync to client's <see cref="SyncVarAttribute"/>.
        /// </summary>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="propertyName">Property name starting with Network.</param>
        public static void ResyncSyncVar(NetworkIdentity behaviorOwner, Type targetType, string propertyName)
        {
            if (behaviorOwner == null)
                return;
            SetDirtyBitsMethodInfo.Invoke(behaviorOwner.gameObject.GetComponent(targetType), new object[] { SyncVarDirtyBits[$"{targetType.Name}.{propertyName}"] });
        }

        /// <summary>
        /// Send fake values to client's <see cref="ClientRpcAttribute"/>.
        /// </summary>
        /// <param name="target">Target to send.</param>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="rpcName">Property name starting with Rpc.</param>
        /// <param name="values">Values of send to target.</param>
        public static void SendFakeTargetRpc(Player target, NetworkIdentity behaviorOwner, Type targetType, string rpcName, params object[] values)
        {
            if (!target.IsConnected || behaviorOwner == null)
                return;

            NetworkWriterPooled writer = NetworkWriterPool.Get();

            foreach (object value in values)
                WriterExtensions[value.GetType()].Invoke(null, new[] { writer, value });

            RpcMessage msg = new()
            {
                netId = behaviorOwner.netId,
                componentIndex = (byte)GetComponentIndex(behaviorOwner, targetType),
                functionHash = (ushort)RpcFullNames[$"{targetType.Name}.{rpcName}"].GetStableHashCode(),
                payload = writer.ToArraySegment(),
            };

            target.Connection.Send(msg);

            NetworkWriterPool.Return(writer);
        }

        /// <summary>
        /// Send fake values to client's <see cref="SyncObject"/>.
        /// </summary>
        /// <param name="target">Target to send.</param>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="customAction">Custom writing action.</param>
        /// <example>
        /// EffectOnlySCP207.
        /// <code>
        ///  MirrorExtensions.SendFakeSyncObject(player, player.NetworkIdentity, typeof(PlayerEffectsController), (writer) => {
        ///   writer.WriteULong(1ul);                                            // DirtyObjectsBit
        ///   writer.WriteUInt(1);                                               // DirtyIndexCount
        ///   writer.WriteByte((byte)SyncList&lt;byte&gt;.Operation.OP_SET);     // Operations
        ///   writer.WriteUInt(17);                                              // EditIndex
        ///  });
        /// </code>
        /// </example>
        public static void SendFakeSyncObject(Player target, NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customAction)
        {
            if (!target.IsConnected)
                return;

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            NetworkWriterPooled writer2 = NetworkWriterPool.Get();
            MakeCustomSyncWriter(behaviorOwner, targetType, customAction, null, writer, writer2);
            target.ReferenceHub.networkIdentity.connectionToClient.Send(new EntityStateMessage() { netId = behaviorOwner.netId, payload = writer.ToArraySegment() });
            NetworkWriterPool.Return(writer);
            NetworkWriterPool.Return(writer2);
        }

        /// <summary>
        /// Edit <see cref="NetworkIdentity"/>'s parameter and sync.
        /// </summary>
        /// <param name="identity">Target object.</param>
        /// <param name="customAction">Edit function.</param>
        public static void EditNetworkObject(NetworkIdentity identity, Action<NetworkIdentity> customAction)
        {
            customAction.Invoke(identity);

            ObjectDestroyMessage objectDestroyMessage = new()
            {
                netId = identity.netId,
            };

            foreach (Player player in Player.List)
            {
                player.Connection.Send(objectDestroyMessage, 0);
                SendSpawnMessageMethodInfo.Invoke(null, new object[] { identity, player.Connection });
            }
        }

        // Get components index in identity.(private)
        private static int GetComponentIndex(NetworkIdentity identity, Type type)
        {
            return Array.FindIndex(identity.NetworkBehaviours, (x) => x.GetType() == type);
        }

        // Make custom writer(private)
        private static void MakeCustomSyncWriter(NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customSyncObject, Action<NetworkWriter> customSyncVar, NetworkWriter owner, NetworkWriter observer)
        {
            ulong value = 0;
            NetworkBehaviour behaviour = null;

            // Get NetworkBehaviors index (behaviorDirty use index)
            for (int i = 0; i < behaviorOwner.NetworkBehaviours.Length; i++)
            {
                if (behaviorOwner.NetworkBehaviours[i].GetType() == targetType)
                {
                    behaviour = behaviorOwner.NetworkBehaviours[i];
                    value = 1UL << (i & 31);
                    break;
                }
            }

            if (!behaviour)
                throw new InvalidOperationException($"Failed to find a valid NetworkBehaviour for NetworkIdentity: [{behaviorOwner.name}], type: [{targetType.FullName}]. Please verify you are using the correct Type in SendFakeSync Var/Object methods.");

            // Write target NetworkBehavior's dirty
            Compression.CompressVarUInt(owner, value);

            // Write init position
            int position = owner.Position;
            owner.WriteByte(0);
            int position2 = owner.Position;

            // Write custom sync data
            if (customSyncObject is not null)
                customSyncObject(owner);
            else
                behaviour.SerializeObjectsDelta(owner);

            // Write custom syncvar
            customSyncVar?.Invoke(owner);

            // Write syncdata position data
            int position3 = owner.Position;
            owner.Position = position;
            owner.WriteByte((byte)(position3 - position2 & 255));
            owner.Position = position3;

            // Copy owner to observer
            if (behaviour.syncMode != SyncMode.Observers)
                observer.WriteBytes(owner.ToArraySegment().Array, position, owner.Position - position);
        }
    }
}
