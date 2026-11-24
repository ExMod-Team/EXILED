// -----------------------------------------------------------------------
// <copyright file="Window.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DamageHandlers;

    using Enums;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Doors;
    using Exiled.API.Interfaces;

    using MapGeneration.Spawnables;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="BreakableGlass"/>.
    /// </summary>
    public class Window : IWrapper<BreakableGlass>, IWorldSpace
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="BreakableGlass"/>s and their corresponding <see cref="Window"/>.
        /// </summary>
        internal static readonly Dictionary<BreakableGlass, Window> BreakableWindowToWindow = new(new ComponentsEqualityComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="window">The base <see cref="BreakableGlass"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/> for this window.</param>
        internal Window(BreakableGlass window, Room room)
        {
            BreakableWindowToWindow.Add(window, this);
            Base = window;
            Room = room;
            Type = GetGlassType();

            if (Type is GlassType.Unknown)
                Log.Warn($"[GLASSTYPE UNKNOWN] Room = ({Room}) BASE = [{Base}] HP = {{{Base?.Health}}}");
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Window"/> which contains all the <see cref="Window"/> instances.
        /// </summary>
        public static IReadOnlyCollection<Window> List => BreakableWindowToWindow.Values;

        /// <summary>
        /// Gets the base-game <see cref="BreakableGlass"/> for this window.
        /// </summary>
        public BreakableGlass Base { get; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the window.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the window's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets the <see cref="Features.Room"/> the window is in.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets the window's <see cref="GlassType"/>.
        /// </summary>
        public GlassType Type { get; }

        /// <summary>
        /// Gets the window's <see cref="ZoneType"/>.
        /// </summary>
        public ZoneType Zone => Room.Zone;

        /// <summary>
        /// Gets or sets the window's position.
        /// </summary>
        public Vector3 Position
        {
            get => GameObject.transform.position;
            set => GameObject.transform.position = value;
        }

        /// <summary>
        /// Gets a value indicating whether this window is breakable.
        /// </summary>
        public bool IsBreakable => !Base.IsBroken;

        /// <summary>
        /// Gets or sets a value indicating whether this window is broken.
        /// </summary>
        public bool IsBroken
        {
            get => Base.IsBroken;
            set => Base.IsBroken = value;
        }

        /// <summary>
        /// Gets or sets if the window's remaining health. No effect if the window cannot be broken.
        /// </summary>
        public float Health
        {
            get => Base.Health;
            set => Base.Health = value;
        }

        /// <summary>
        /// Gets or sets the window's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => GameObject.transform.rotation;
            set => GameObject.transform.rotation = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this window can be broken by SCP.
        /// </summary>
        public bool DisableScpDamage
        {
            get => Base._preventScpDamage;
            set => Base._preventScpDamage = value;
        }

        /// <summary>
        /// Gets or sets a value indicating who is the LastAttacker.
        /// </summary>
        public Player LastAttacker
        {
            get => Player.Get(Base.LastAttacker.Hub);
            set => Base.LastAttacker = value.Footprint;
        }

        /// <summary>
        /// Gets the window object associated with a specific <see cref="Window"/>, or creates a new one if there isn't one.
        /// </summary>
        /// <param name="breakableWindow">The base-game <see cref="Window"/>.</param>
        /// <returns>A <see cref="Door"/> wrapper object.</returns>
        public static Window Get(BreakableGlass breakableWindow) => BreakableWindowToWindow.TryGetValue(breakableWindow, out Window window)
            ? window
            : new(breakableWindow, breakableWindow.GetComponentInParent<Room>());

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Window"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Window"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<Window> Get(Func<Window, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Try-get a <see cref="Window"/> belonging to the <see cref="BreakableGlass"/>, if any.
        /// </summary>
        /// <param name="breakableWindow">The <see cref="BreakableGlass"/> instance.</param>
        /// <param name="window">A <see cref="Window"/> or <see langword="null"/> if not found.</param>
        /// <returns>Whether a window was found.</returns>
        public static bool TryGet(BreakableGlass breakableWindow, out Window window)
        {
            window = Get(breakableWindow);
            return window is not null;
        }

        /// <summary>
        /// Try-get a <see cref="IEnumerable{T}"/> of <see cref="Window"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <param name="windows">A <see cref="IEnumerable{T}"/> of <see cref="Window"/> which contains elements that satify the condition.</param>
        /// <returns>Whether at least one window was found.</returns>
        public static bool TryGet(Func<Window, bool> predicate, out IEnumerable<Window> windows)
        {
            windows = Get(predicate);
            return windows.Any();
        }

        /// <summary>
        /// Break the window.
        /// </summary>
        public void BreakWindow() => Base.ServerDamageWindow(Health);

        /// <summary>
        /// Damages the window.
        /// </summary>
        /// <param name="amount">The amount of damage to deal.</param>
        public void DamageWindow(float amount) => Base.ServerDamageWindow(amount);

        /// <summary>
        /// Damages the window.
        /// </summary>
        /// <param name="amount">The amount of damage to deal.</param>
        /// <param name="handler">The handler of damage.</param>
        public void DamageWindow(float amount, DamageHandlerBase handler)
        {
            Base.Damage(amount, handler, Vector3.zero);
        }

        /// <summary>
        /// Returns the Window in a human-readable format.
        /// </summary>
        /// <returns>A string containing Window-related data.</returns>
        public override string ToString() => $"{Type} ({Health}) [{IsBroken}] *{DisableScpDamage}*";

        private GlassType GetGlassType() => Base.name.GetBefore('_').RemoveBracketsOnEndOfName() switch
        {
            "BreakableGlass" => Room?.Type switch
            {
                RoomType.SurfaceCassie => Base.name[16] switch
                {
                    '1' => GlassType.SurfaceCassieUpperHall,
                    '2' or '3' => GlassType.SurfaceCassieLowerHall,
                    '4' or '5' or '6' => GlassType.SurfaceCassieChamber,
                    _ => GlassType.Unknown,
                },
                RoomType.SurfaceGateA => Base.name[17] switch
                {
                    '1' => GlassType.SurfaceUpstairCornerRoadGateA,
                    '2' or '3' => GlassType.SurfaceBigWindowGateA,
                    '4' => GlassType.SurfaceStairUpperGateA,
                    '5' or '6' => GlassType.SurfaceSmallWindowGateA,
                    '7' => GlassType.SurfaceRoofGateA,
                    '8' => GlassType.SurfaceStairLowerGateA,
                    '9' or '0' => GlassType.SurfaceAdminOfficeGateA,
                    _ => GlassType.Unknown,
                },
                _ => GlassType.Unknown,
            },
            "B272sa" => Room?.Type switch
            {
                RoomType.LczGlassBox => GlassType.GR18,
                RoomType.Lcz330 => GlassType.Scp330,
                _ => GlassType.Unknown,
            },
            "GLASS" => Room?.Type switch
            {
                RoomType.Hcz079 => GlassType.Scp079,
                RoomType.HczHid => GlassType.MicroHid,
                RoomType.HczEzCheckpointA => GlassType.HczEzCheckpointA,
                RoomType.HczEzCheckpointB => GlassType.HczEzCheckpointB,
                RoomType.EzGateA when Base.name[7] == '5' => GlassType.GateAArmory,
                RoomType.EzGateA => GlassType.GateAPit,
                RoomType.HczLoadingBay => GlassType.HczLoadingBay,
                _ => GlassType.Unknown,
            },
            "Window" => Room?.Type switch
            {
                RoomType.Hcz049 => GlassType.Scp049,
                RoomType.Hcz127 when Base.name[8] == '3' => GlassType.Scp127Large,
                RoomType.Hcz127 => GlassType.Scp127Small,
                RoomType.HczHid => GlassType.MicroHid,
                RoomType.HczTestRoom => GlassType.TestRoom,
                _ => GlassType.Unknown,
            },
            "Glass" => Room?.Type switch
            {
                RoomType.Hcz079 => GlassType.Scp079Trigger,
                RoomType.HczHid => GlassType.MicroHid,
                _ => GlassType.Unknown,
            },
            "glass" => GlassType.Scp079,
            "LCZ Breakable Glass" => GlassType.Plants,
            _ => GlassType.Unknown,
        };
    }
}