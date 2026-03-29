// -----------------------------------------------------------------------
// <copyright file="MicroHid.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System;
    using System.Reflection;

    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;

    using InventorySystem.Items.Autosync;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.MicroHID;
    using InventorySystem.Items.MicroHID.Modules;

    using Random = UnityEngine.Random;

    /// <summary>
    /// A wrapper class for <see cref="MicroHIDItem"/>.
    /// </summary>
    public class MicroHid : Item, IWrapper<MicroHIDItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHid"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="MicroHIDItem"/> class.</param>
        public MicroHid(MicroHIDItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHid"/> class, as well as a new Micro HID item.
        /// </summary>
        internal MicroHid()
            : this((MicroHIDItem)Server.Host.Inventory.CreateItemInstance(new(ItemType.MicroHID, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="EnergyManagerModule"/> of the MicroHID.
        /// </summary>
        public EnergyManagerModule EnergyManager => Base.EnergyManager;

        /// <summary>
        /// Gets the <see cref="BrokenSyncModule"/> of the MicroHID.
        /// </summary>
        public BrokenSyncModule BrokenModule => Base.BrokenSync;

        /// <summary>
        /// Gets the <see cref="InputSyncModule"/> of the MicroHID.
        /// </summary>
        public InputSyncModule InputModule => Base.InputSync;

        /// <summary>
        /// Gets the <see cref="CycleController"/> of the MicroHID.
        /// </summary>
        public CycleController CycleController => Base.CycleController;

        /// <summary>
        /// Gets or sets the remaining energy in the MicroHID.
        /// </summary>
        /// <value>Maximum energy is <c>1</c>. Minimum energy is <c>0</c>.</value>
        public float Energy
        {
            get => EnergyManager.Energy;
            set => EnergyManager.ServerSetEnergy(Serial, value);
        }

        /// <summary>
        /// Gets the <see cref="MicroHIDItem"/> base of the item.
        /// </summary>
        public new MicroHIDItem Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the MicroHID is broken.
        /// </summary>
        public bool IsBroken
        {
            get => BrokenModule.Broken;
            set => BrokenModule.ServerSetBroken(Serial, value);
        }

        /// <summary>
        /// Gets a time when this <see cref="MicroHid"/> was broken.
        /// </summary>
        /// <value>A time when this <see cref="MicroHid"/> was broken, or <c>0</c> if it is not broken.</value>
        public float BrokeTime => BrokenSyncModule.TryGetBrokenElapsed(Serial, out float time) ? time : 0;

        /// <summary>
        /// Gets or sets the <see cref="MicroHidPhase"/>.
        /// </summary>
        public MicroHidPhase State
        {
            get => CycleController.Phase;
            set => CycleController.Phase = value;
        }

        /// <summary>
        /// Gets or sets progress of winging up.
        /// </summary>
        /// <value>A value between <c>0</c> and <c>1</c>.</value>
        public float WindUpProgress
        {
            get => CycleController.ServerWindUpProgress;
            set => CycleController.ServerWindUpProgress = value;
        }

        /// <summary>
        /// Gets or sets the last received <see cref="MicroHidFiringMode"/>.
        /// </summary>
        public MicroHidFiringMode LastFiringMode
        {
            get => CycleController.LastFiringMode;
            set => CycleController.LastFiringMode = value;
        }

        /// <summary>
        /// Gets or sets the last received <see cref="InputSyncModule.SyncData"/>.
        /// </summary>
        public InputSyncModule.SyncData LastReceived
        {
            get => InputModule._lastReceived;
            set => InputModule._lastReceived = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="LastReceived"/> is <see cref="InputSyncModule.SyncData.Primary"/>.
        /// </summary>
        public bool IsPrimary => InputModule.Primary;

        /// <summary>
        /// Starts firing the MicroHID.
        /// </summary>
        /// <param name="firingMode">Fire mode.</param>
        public void Fire(MicroHidFiringMode firingMode = MicroHidFiringMode.PrimaryFire)
        {
            switch (firingMode)
            {
                case MicroHidFiringMode.PrimaryFire:
                    if (TryGetFireController(MicroHidFiringMode.PrimaryFire, out PrimaryFireModeModule primaryFireModeModule))
                        primaryFireModeModule.ServerFire();
                    break;
                case MicroHidFiringMode.ChargeFire:
                    if (TryGetFireController(MicroHidFiringMode.ChargeFire, out ChargeFireModeModule chargeFireModeModule))
                        chargeFireModeModule.ServerFire();
                    break;
                default:
                    if (TryGetFireController(MicroHidFiringMode.BrokenFire, out BrokenFireModeModule brokenFireModeModule))
                        brokenFireModeModule.ServerFire();
                    break;
            }
        }

        /// <summary>
        /// Recharges the MicroHID.
        /// </summary>
        public void Recharge()
        {
            if (IsBroken)
                Energy = Random.value;
            else
                Energy = 1;
        }

        /// <summary>
        /// Explodes the MicroHID.
        /// </summary>
        public void Explode()
        {
            if (TryGetFireController(MicroHidFiringMode.ChargeFire, out ChargeFireModeModule module))
                module.ServerExplode();
        }

        /// <summary>
        /// Tries to get a <see cref="FiringModeControllerModule"/> assosiated with the specified <see cref="MicroHidFiringMode"/>.
        /// </summary>
        /// <param name="firingMode">Target firing mode.</param>
        /// <param name="module">Found module or <c>null</c>.</param>
        /// <typeparam name="T">Type of module.</typeparam>
        /// <returns><c>true</c> if module was found, <c>false</c> otherwise.</returns>
        public bool TryGetFireController<T>(MicroHidFiringMode firingMode, out T module)
            where T : FiringModeControllerModule
        {
            if (CycleController._firingModeControllers.Count == 0)
                CycleController.RecacheFiringModes(Base);

            module = (T)CycleController._firingModeControllers.Find(x => x.AssignedMode == firingMode);
            return module != null;
        }

        /// <summary>
        /// Tries to get a <see cref="FiringModeControllerModule"/> assosiated with the last <see cref="MicroHidFiringMode"/>.
        /// </summary>
        /// <param name="module">Found module or <c>null</c>.</param>
        /// <returns><c>true</c> if module was found, <c>false</c> otherwise.</returns>
        public bool TryGetLastFireController(out FiringModeControllerModule module) => TryGetFireController(LastFiringMode, out module);

        /// <summary>
        /// Clones current <see cref="MicroHid"/> object.
        /// </summary>
        /// <returns> New <see cref="MicroHid"/> object. </returns>
        public override Item Clone() => new MicroHid()
        {
            State = State,
            Energy = Energy,
        };

        /// <summary>
        /// Returns the MicroHid in a human readable format.
        /// </summary>
        /// <returns>A string containing MicroHid-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Energy}| -{State}-";

        /// <inheritdoc/>
        internal override void ChangeOwner(Player oldOwner, Player newOwner)
        {
            Base.Owner = newOwner.ReferenceHub;

            for (int i = 0; i < Base.AllSubcomponents.Length; i++)
            {
                Base.AllSubcomponents[i].OnAdded();
            }

            Base.InstantiationStatus = newOwner == Server.Host ? AutosyncInstantiationStatus.SimulatedInstance : AutosyncInstantiationStatus.InventoryInstance;
        }
    }
}