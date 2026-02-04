// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Items.Keycards;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Armor;
    using InventorySystem.Items.Autosync;
    using InventorySystem.Items.Firearms.Ammo;
    using InventorySystem.Items.Jailbird;
    using InventorySystem.Items.Keycards;
    using InventorySystem.Items.MarshmallowMan;
    using InventorySystem.Items.MicroHID;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Radio;
    using InventorySystem.Items.Scp1509;
    using InventorySystem.Items.ThrowableProjectiles;
    using InventorySystem.Items.ToggleableLights;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp1344;
    using InventorySystem.Items.Usables.Scp1576;
    using InventorySystem.Items.Usables.Scp244;
    using InventorySystem.Items.Usables.Scp330;
    using UnityEngine;

    using BaseConsumable = InventorySystem.Items.Usables.Consumable;

    /// <summary>
    /// A wrapper class for <see cref="ItemBase"/>.
    /// </summary>
    public class Item : TypeCastObject<Item>, IWrapper<ItemBase>
    {
        /// <summary>
        /// A dictionary of all <see cref="ItemBase"/>'s that have been converted into <see cref="Item"/>.
        /// </summary>
        internal static readonly Dictionary<ItemBase, Item> BaseToItem = new(new ComponentsEqualityComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        public Item(ItemBase itemBase)
        {
            Base = itemBase;

            if (Base is ModularAutosyncItem modularItem && modularItem.InstantiationStatus is AutosyncInstantiationStatus.Template or AutosyncInstantiationStatus.SimulatedInstance)
                return;

            BaseToItem.Add(itemBase, this);

            if (Base.ItemSerial is 0 && itemBase.Owner != null)
            {
                ushort serial = ItemSerialGenerator.GenerateNext();
                Serial = serial;
                itemBase.OnAdded(null);
                if (Base is ModularAutosyncItem syncItem)
                    syncItem.InstantiationStatus = AutosyncInstantiationStatus.SimulatedInstance;
#if DEBUG
                Log.Debug($"{nameof(Item)}.ctor: Generating new serial number. Serial should now be: {serial}. // {Serial}");
#endif
            }
#if DEBUG
            Log.Debug($"{nameof(Item)}.ctor: New item created with Serial: {Serial}");
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        internal Item(ItemType type)
            : this(Server.Host.Inventory.CreateItemInstance(new(type, 0), false))
        {
        }

        /// <summary>
        /// Gets a list of all <see cref="Item"/>'s on the server.
        /// </summary>
        public static IEnumerable<Item> List => BaseToItem.Values;

        /// <summary>
        /// Gets or sets the unique serial number for the item.
        /// </summary>
        public ushort Serial
        {
            get => Base.ItemSerial;
            set => Base.ItemSerial = value;
        }

        /// <summary>
        /// Gets a value indicating whether if the item are in an inventory.
        /// </summary>
        public bool IsInInventory => Owner != Server.Host && Owner.HasItem(this);

        /// <summary>
        /// Gets or sets the scale for the item.
        /// </summary>
        public virtual Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets the <see cref="ItemBase"/> of the item.
        /// </summary>
        public ItemBase Base { get; }

        /// <summary>
        /// Gets the <see cref="ItemType"/> of the item.
        /// </summary>
        public ItemType Type => Base.ItemTypeId;

        /// <summary>
        /// Gets the <see cref="ItemCategory"/> of the item.
        /// </summary>
        public ItemCategory Category => Base.Category;

        /// <summary>
        /// Gets the <see cref="ItemTierFlags"/> of the item.
        /// </summary>
        public ItemTierFlags TierFlags => Base.TierFlags;

        /// <summary>
        /// Gets the Weight of the item.
        /// </summary>
        public float Weight => Base.Weight;

        /// <summary>
        /// Gets a value indicating whether this item is ammunition.
        /// </summary>
        public bool IsAmmo => this is Ammo;

        /// <summary>
        /// Gets a value indicating whether this item is armor.
        /// </summary>
        public bool IsArmor => this is Armor;

        /// <summary>
        /// Gets a value indicating whether this item is a keycard.
        /// </summary>
        public bool IsKeycard => this is Keycard;

        /// <summary>
        /// Gets a value indicating whether this item will be destroy when being used.
        /// </summary>
        public bool IsConsumable => this is Consumable;

        /// <summary>
        /// Gets a value indicating whether this item is a throwable item.
        /// </summary>
        public bool IsThrowable => this is Throwable;

        /// <summary>
        /// Gets a value indicating whether this item can be used by a player.
        /// </summary>
        public bool IsUsable => this is Usable;

        /// <summary>
        /// Gets a value indicating whether this item is a weapon.
        /// </summary>
        public bool IsWeapon => this is Firearm || Type is ItemType.Jailbird or ItemType.MicroHID or ItemType.SCP1509;

        /// <summary>
        /// Gets a value indicating whether or not this item is a firearm.
        /// </summary>
        public bool IsFirearm => this is Firearm;

        /// <summary>
        /// Gets a value indicating whether this item emits light.
        /// </summary>
        public bool IsLightEmitter => Base is ILightEmittingItem;

        /// <summary>
        /// Gets a value indicating whether this item can be used to disarm players.
        /// </summary>
        public bool IsDisarmer => Base is IDisarmingItem;

        /// <summary>
        /// Gets the <see cref="Player"/> who owns the item.
        /// </summary>
        public Player Owner => Player.Get(Base.Owner) ?? Server.Host;

        /// <summary>
        /// Gets or sets a reason for adding this item to the inventory.
        /// </summary>
        public ItemAddReason AddReason
        {
            get => Base.ServerAddReason;
            set => Base.ServerAddReason = value;
        }

        /// <summary>
        /// Gets the <see cref="ItemIdentifier"/> for this item.
        /// </summary>
        public ItemIdentifier Identifier => Base.ItemId;

        /// <summary>
        /// Gets an existing <see cref="Item"/> or creates a new instance of one.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to convert into an item.</param>
        /// <returns>The item wrapper for the given <see cref="ItemBase"/>.</returns>
        public static Item Get(ItemBase itemBase)
        {
            if (itemBase == null)
                return null;

            if (BaseToItem.TryGetValue(itemBase, out Item item))
                return item;

            return itemBase switch
            {
                InventorySystem.Items.Firearms.Firearm firearm => firearm.ItemTypeId switch
                {
                    ItemType.GunSCP127 => new Scp127(firearm),
                    _ => new Firearm(firearm),
                },
                KeycardItem keycard => keycard switch
                {
                    ChaosKeycardItem chaosKeycardItem => new ChaosKeycard(chaosKeycardItem),
                    SingleUseKeycardItem singleUseKeycardItem => new SingleUseKeycard(singleUseKeycardItem),
                    _ => keycard.ItemTypeId switch
                    {
                        ItemType.KeycardCustomTaskForce => new TaskForceKeycard(keycard),
                        ItemType.KeycardCustomSite02 => new Site02Keycard(keycard),
                        ItemType.KeycardCustomManagement => new ManagementKeycard(keycard),
                        ItemType.KeycardCustomMetalCase => new MetalKeycard(keycard),
                        _ => new Keycard(keycard),
                    }
                },
                UsableItem usable => usable switch
                {
                    Scp330Bag scp330Bag => new Scp330(scp330Bag),
                    Scp244Item scp244Item => new Scp244(scp244Item),
                    Scp1576Item scp1576 => new Scp1576(scp1576),
                    Scp1344Item scp1344 => new Scp1344(scp1344),
                    BaseConsumable consumable => new Consumable(consumable),
                    _ => new Usable(usable),
                },
                RadioItem radio => new Radio(radio),
                MicroHIDItem micro => new MicroHid(micro),
                BodyArmor armor => new Armor(armor),
                AmmoItem ammo => new Ammo(ammo),
                ToggleableLightItemBase flashlight => new Flashlight(flashlight),
                JailbirdItem jailbird => new Jailbird(jailbird),
                ThrowableItem throwable => throwable.Projectile switch
                {
                    FlashbangGrenade => new FlashGrenade(throwable),
                    ExplosionGrenade => new ExplosiveGrenade(throwable),
                    Scp2176Projectile => new Scp2176(throwable),
                    Scp018Projectile => new Scp018(throwable),
                    _ => new Throwable(throwable),
                },
                Scp1509Item scp1509 => new Scp1509(scp1509),
                MarshmallowItem marshmallow => new Marshmallow(marshmallow),
                _ => new(itemBase),
            };
        }

        /// <summary>
        /// Gets an existing <see cref="Item"/> or creates a new instance of one.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to convert into an item.</param>
        /// <typeparam name="T">The specified <see cref="Item"/> type.</typeparam>
        /// <returns>The item wrapper for the given <see cref="ItemBase"/>.</returns>
        public static T Get<T>(ItemBase itemBase)
            where T : Item => Get(itemBase) as T;

        /// <summary>
        /// Gets the Item belonging to the specified serial.
        /// </summary>
        /// <param name="serial">The Item serial.</param>
        /// <returns>Returns the Item found or <see langword="null"/> if not found.</returns>
        public static Item Get(ushort serial) => List.FirstOrDefault(x => x.Serial == serial);

        /// <summary>
        /// Gets the Item belonging to the specified serial.
        /// </summary>
        /// <param name="serial">The Item serial.</param>
        /// <typeparam name="T">The specified <see cref="Item"/> type.</typeparam>
        /// <returns>Returns the Item found or <see langword="null"/> if not found.</returns>
        public static T Get<T>(ushort serial)
            where T : Item => Get(serial) as T;

        /// <summary>
        /// Creates a new <see cref="Item"/> with the proper inherited subclass.
        /// <para>
        /// Based on the <paramref name="type"/>, the returned <see cref="Item"/> can be casted into a subclass to gain more control over the object.
        /// <br />- Usable items (Adrenaline, Medkit, Painkillers, SCP-207, SCP-268, and SCP-500) should be casted to the <see cref="Usable"/> class.
        /// <br />- All valid ammo should be casted to the <see cref="Ammo"/> class.
        /// <br />- All valid firearms (not including the Micro HID) should be casted to the <see cref="Firearm"/> class.
        /// <br />- All valid keycards should be casted to the <see cref="Keycard"/> class.
        /// <br />- All valid armor should be casted to the <see cref="Armor"/> class.
        /// <br />- Explosive grenades and SCP-018 should be casted to the <see cref="ExplosiveGrenade"/> class.
        /// <br />- Flash grenades should be casted to the <see cref="FlashGrenade"/> class.
        /// </para>
        /// <para>
        /// <br />The following have their own respective classes:
        /// <br />- Flashlights can be casted to <see cref="Flashlight"/>.
        /// <br />- Radios can be casted to <see cref="Radio"/>.
        /// <br />- The Micro HID can be casted to <see cref="MicroHid"/>.
        /// <br />- SCP-244 A and B variants can be casted to <see cref="Scp244"/>.
        /// <br />- SCP-330 can be casted to <see cref="Scp330"/>.
        /// <br />- SCP-2176 can be casted to the <see cref="Scp2176"/> class.
        /// <br />- SCP-1576 can be casted to the <see cref="Scp1576"/> class.
        /// <br />- SCP-1344 can be casted to the <see cref="Scp1344"/> class.
        /// <br />- Jailbird can be casted to the <see cref="Jailbird"/> class.
        /// </para>
        /// <para>
        /// Items that are not listed above do not have a subclass, and can only use the base <see cref="Item"/> class.
        /// </para>
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        /// <param name="owner">The <see cref="Player"/> who owns the item by default.</param>
        /// <returns>The <see cref="Item"/> created. This can be cast as a subclass.</returns>
        public static Item Create(ItemType type, Player owner = null) => type.GetTemplate() switch
        {
            InventorySystem.Items.Firearms.Firearm => type switch
            {
                ItemType.GunSCP127 => new Scp127(),
                _ => new Firearm(type),
            },
            KeycardItem keycard => keycard switch
            {
                ChaosKeycardItem => new ChaosKeycard(type),
                SingleUseKeycardItem => new SingleUseKeycard(type),
                _ => type switch
                {
                    ItemType.KeycardCustomTaskForce => new TaskForceKeycard(type),
                    ItemType.KeycardCustomSite02 => new Site02Keycard(type),
                    ItemType.KeycardCustomManagement => new ManagementKeycard(type),
                    ItemType.KeycardCustomMetalCase => new MetalKeycard(type),
                    _ => new Keycard(type, owner),
                }
            },
            UsableItem usable => usable switch
            {
                Scp330Bag => new Scp330(),
                Scp244Item => new Scp244(type),
                Scp1576Item => new Scp1576(),
                Scp1344Item => new Scp1344(),
                BaseConsumable => new Consumable(type),
                _ => new Usable(type),
            },
            RadioItem => new Radio(),
            MicroHIDItem => new MicroHid(),
            BodyArmor => new Armor(type),
            AmmoItem => new Ammo(type),
            ToggleableLightItemBase => new Flashlight(type),
            JailbirdItem => new Jailbird(),
            ThrowableItem throwable => throwable.Projectile switch
            {
                FlashbangGrenade => new FlashGrenade(owner),
                ExplosionGrenade => new ExplosiveGrenade(type, owner),
                Scp2176Projectile => new Scp2176(owner),
                Scp018Projectile => new Scp018(type, owner),
                _ => new Throwable(type, owner),
            },
            Scp1509Item => new Scp1509(),
            MarshmallowItem => new Marshmallow(type, owner),
            _ => new(type),
        };

        /// <summary>
        /// Creates a new <see cref="Item"/> with the proper inherited subclass.
        /// <para>
        /// Based on the <paramref name="type"/>, the returned <see cref="Item"/> can be casted into a subclass to gain more control over the object.
        /// <br />- Usable items (Adrenaline, Medkit, Painkillers, SCP-207, SCP-268, and SCP-500) should be casted to the <see cref="Usable"/> class.
        /// <br />- All valid ammo should be casted to the <see cref="Ammo"/> class.
        /// <br />- All valid firearms (not including the Micro HID) should be casted to the <see cref="Firearm"/> class.
        /// <br />- All valid keycards should be casted to the <see cref="Keycard"/> class.
        /// <br />- All valid armor should be casted to the <see cref="Armor"/> class.
        /// <br />- Explosive grenades and SCP-018 should be casted to the <see cref="ExplosiveGrenade"/> class.
        /// <br />- Flash grenades should be casted to the <see cref="FlashGrenade"/> class.
        /// </para>
        /// <para>
        /// <br />The following have their own respective classes:
        /// <br />- Flashlights can be casted to <see cref="Flashlight"/>.
        /// <br />- Radios can be casted to <see cref="Radio"/>.
        /// <br />- The Micro HID can be casted to <see cref="MicroHid"/>.
        /// <br />- SCP-244 A and B variants can be casted to <see cref="Scp244"/>.
        /// <br />- SCP-330 can be casted to <see cref="Scp330"/>.
        /// <br />- SCP-2176 can be casted to the <see cref="Scp2176"/> class.
        /// <br />- SCP-1576 can be casted to the <see cref="Scp1576"/> class.
        /// <br />- SCP-1344 can be casted to the <see cref="Scp1344"/> class.
        /// <br />- Jailbird can be casted to the <see cref="Jailbird"/> class.
        /// </para>
        /// <para>
        /// Items that are not listed above do not have a subclass, and can only use the base <see cref="Item"/> class.
        /// </para>
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        /// <param name="owner">The <see cref="Player"/> who owns the item by default.</param>
        /// <typeparam name="T">The specified <see cref="Item"/> type.</typeparam>
        /// <returns>The <see cref="Item"/> created. This can be cast as a subclass.</returns>
        public static T Create<T>(ItemType type, Player owner = null)
            where T : Item => Create(type, owner) as T;

        /// <summary>
        /// Gives this item to a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        public void Give(Player player) => player.AddItem(Base, this);

        /// <summary>
        /// Destroy this item.
        /// </summary>
        public void Destroy() => Owner.RemoveItem(this);

        /// <summary>
        /// Creates the <see cref="Pickup"/> that based on this <see cref="Item"/>.
        /// </summary>
        /// <param name="position">The location to spawn the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="spawn">Whether the <see cref="Pickup"/> should be initially spawned.</param>
        /// <returns>The created <see cref="Pickup"/>.</returns>
        public virtual Pickup CreatePickup(Vector3 position, Quaternion? rotation = null, bool spawn = true)
        {
            PickupSyncInfo info = new(Type, Weight, Serial);

            ItemPickupBase ipb = InventoryExtensions.ServerCreatePickup(Base, info, position, rotation ?? Quaternion.identity);

            Base.OnRemoved(ipb);

            Pickup pickup = Pickup.Get(ipb);

            if (spawn)
                pickup.Spawn();

            return pickup;
        }

        /// <summary>
        /// Clones the current item with a different serial.
        /// </summary>
        /// <returns> Cloned item object. </returns>
        public virtual Item Clone() => Create(Type);

        /// <summary>
        /// Returns the Item in a human readable format.
        /// </summary>
        /// <returns>A string containing Item-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* ={Owner}=";

        /// <summary>
        /// Changes the owner of the <see cref="Item"/>.
        /// </summary>
        /// <param name="oldOwner">Old <see cref="Item"/> owner.</param>
        /// <param name="newOwner">New <see cref="Item"/> owner.</param>
        public void ChangeItemOwner(Player oldOwner, Player newOwner)
        {
            if (oldOwner != null && newOwner != null)
            {
                ChangeOwner(oldOwner, newOwner);
            }
        }

        /// <summary>
        /// Change the owner of the <see cref="Item"/>.
        /// </summary>
        /// <param name="oldOwner">old <see cref="Item"/> owner.</param>
        /// <param name="newOwner">new <see cref="Item"/> owner.</param>
        internal virtual void ChangeOwner(Player oldOwner, Player newOwner)
        {
            Base.OnRemoved(null);

            Base.Owner = newOwner.ReferenceHub;

            Base.OnAdded(null);
        }

        // TODO: remove use of GetWorldScale after NW fix WaypointToy.

        /// <summary>
        /// Helper method for saving data between items and pickups.
        /// </summary>
        /// <param name="pickup"><see cref="Pickup"/>-related data to give to the <see cref="Item"/>.</param>
        /// <remarks>
        /// Analog to <see cref="ReadPickupInfoAfter(Pickup)"/>, but it is called before item initialization.
        /// <see cref="ItemBase.OnAdded(ItemPickupBase)"/>.
        /// </remarks>
        /// <see cref="ReadPickupInfoAfter"/>
        internal virtual void ReadPickupInfoBefore(Pickup pickup)
        {
            if (pickup is not null)
            {
                Scale = pickup.GameObject.GetWorldScale();
            }
        }

        /// <summary>
        /// Helper method for saving data between items and pickups.
        /// </summary>
        /// <param name="pickup"><see cref="Pickup"/>-related data to give to the <see cref="Item"/>.</param>
        /// <remarks>
        /// Analog to <see cref="ReadPickupInfoAfter(Pickup)"/>, but it is called after item initialization.
        /// <see cref="ItemBase.OnAdded(ItemPickupBase)"/>.
        /// </remarks>
        /// <see cref="ReadPickupInfoBefore"/>
        internal virtual void ReadPickupInfoAfter(Pickup pickup)
        {
        }
    }
}
