// -----------------------------------------------------------------------
// <copyright file="SpawningRagdollEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Interfaces;

    using PlayerRoles;
    using PlayerRoles.Ragdolls;
    using PlayerStatsSystem;
    using RelativePositioning;
    using UnityEngine;

    /// <summary>
    /// Contains all information before spawning a player ragdoll.
    /// </summary>
    public class SpawningRagdollEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningRagdollEventArgs" /> class.
        /// </summary>
        /// <param name="info">
        /// <inheritdoc cref="Info" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public SpawningRagdollEventArgs(RagdollData info, bool isAllowed = true)
        {
            Info = info;
            Player = Player.Get(info.OwnerHub);
            Scale = Player.Scale;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the spawning position of the ragdoll.
        /// </summary>
        public Vector3 Position
        {
            get => Info.StartRelativePosition.Position;
            set => Info = new RagdollData(Player.ReferenceHub, DamageHandlerBase, Role, new(value), Info.StartRelativeRotation, Scale, Nickname, CreationTime);
        }

        /// <summary>
        /// Gets or sets the ragdoll's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => WaypointBase.GetWorldRotation(Info.StartRelativePosition.WaypointId, Info.StartRelativeRotation);
            set => Info = new RagdollData(Player.ReferenceHub, DamageHandlerBase, Role, Info.StartRelativePosition, WaypointBase.GetWorldRotation(Info.StartRelativePosition.WaypointId, value), Scale, Nickname, CreationTime);
        }

        /// <summary>
        /// Gets or sets the ragdoll's scale with RagdollData.
        /// </summary>
        public Vector3 Scale
        {
            get => Info.Scale;
            set => Info = new RagdollData(Player.ReferenceHub, DamageHandlerBase, Role, Info.StartRelativePosition, Info.StartRelativeRotation, Vector3.Scale(value, RagdollManager.GetDefaultScale(Role)), Nickname, CreationTime);
        }

        /// <summary>
        /// Gets or sets the ragdoll's scale with GameObject.
        /// </summary>
        public Vector3 RagdollScale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the ragdoll's <see cref="RoleTypeId" />.
        /// </summary>
        public RoleTypeId Role
        {
            get => Info.RoleType;
            set => Info = new RagdollData(Player.ReferenceHub, DamageHandlerBase, value, Info.StartRelativePosition, Info.StartRelativeRotation, Scale, Nickname, CreationTime);
        }

        /// <summary>
        /// Gets the ragdoll's creation time.
        /// </summary>
        public double CreationTime => Info.CreationTime;

        /// <summary>
        /// Gets or sets the ragdoll's nickname.
        /// </summary>
        public string Nickname
        {
            get => Info.Nickname;
            set => Info = new RagdollData(Player.ReferenceHub, DamageHandlerBase, Role, Info.StartRelativePosition, Info.StartRelativeRotation, Scale, value, CreationTime);
        }

        /// <summary>
        /// Gets or sets the ragdoll's <see cref="RagdollData" />.
        /// </summary>
        public RagdollData Info { get; set; }

        /// <summary>
        /// Gets or sets the ragdoll's <see cref="PlayerStatsSystem.DamageHandlerBase" />.
        /// </summary>
        public DamageHandlerBase DamageHandlerBase
        {
            get => Info.Handler;
            set => Info = new RagdollData(Player.ReferenceHub, value, Role, Info.StartRelativePosition, Info.StartRelativeRotation, Scale, Nickname, CreationTime);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ragdoll can be spawned.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the <see cref="Player">Owner</see> of the ragdoll.
        /// </summary>
        public Player Player { get; }
    }
}