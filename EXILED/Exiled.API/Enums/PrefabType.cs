// -----------------------------------------------------------------------
// <copyright file="PrefabType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Exiled.API.Features.Attributes;

    /// <summary>
    /// Type of prefab.
    /// </summary>
    public enum PrefabType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1602 // Enumeration items should be documented
        [Prefab(3816198336, "Player")]
        Player,

        [Prefab(1883254029, "EZ BreakableDoor")]
        EZBreakableDoor,

        [Prefab(2295511789, "HCZ BreakableDoor")]
        HCZBreakableDoor,

        [Prefab(3038351124, "LCZ BreakableDoor")]
        LCZBreakableDoor,

        [Prefab(400539138, "HCZ OneSided")]
        HCZOneSided,

        [Prefab(2060920286, "HCZ TwoSided")]
        HCZTwoSided,

        [Prefab(3343949480, "OpenHallway")]
        HCZOpenHallway,

        [Prefab(3999209566, "OpenHallway Construct A")]
        HCZOpenHallway_Construct_A,

        [Prefab(38976586, "OpenHallway Clutter A")]
        HCZOpenHallway_Clutter_A,

        [Prefab(1687661105, "OpenHallway Clutter B")]
        HCZOpenHallway_Clutter_B,

        [Prefab(147203050, "OpenHallway Clutter C")]
        HCZOpenHallway_Clutter_C,

        [Prefab(1102032353, "OpenHallway Clutter D")]
        HCZOpenHallway_Clutter_D,

        [Prefab(2490430134, "OpenHallway Clutter E")]
        HCZOpenHallway_Clutter_E,

        [Prefab(2673083832, "OpenHallway Clutter F")]
        HCZOpenHallway_Clutter_F,

        [Prefab(2536312960, "OpenHallway Clutter G")]
        HCZOpenHallway_Clutter_G,

        [Prefab(2176035362, "HCZ BulkDoor")]
        HCZBulkDoor,

        [Prefab(1704345398, "sportTargetPrefab")]
        SportTarget,

        [Prefab(858699872, "dboyTargetPrefab")]
        DBoyTarget,

        [Prefab(3613149668, "binaryTargetPrefab")]
        BinaryTarget,

        [Prefab(1306864341, "TantrumObj")]
        TantrumObj,

        [Prefab(1321952889, "PrimitiveObjectToy")]
        PrimitiveObjectToy,

        [Prefab(3956448839, "LightSourceToy")]
        LightSourceToy,

        [Prefab(712426663, "SpeakerToy")]
        SpeakerToy,

        [Prefab(2672653014, "RegularKeycardPickup")]
        RegularKeycardPickup,

        [Prefab(335436768, "ChaosKeycardPickup")]
        ChaosKeycardPickup,

        [Prefab(248357067, "RadioPickup")]
        RadioPickup,

        [Prefab(1925130715, "FirearmPickup")]
        FirearmPickup,

        [Prefab(1925130715, "Com15Pickup")]
        Com15Pickup,

        [Prefab(2808038258, "MedkitPickup")]
        MedkitPickup,

        [Prefab(2606539874, "FlashlightPickup")]
        FlashlightPickup,

        [Prefab(2974277164, "MicroHidPickup")]
        MicroHidPickup,

        [Prefab(1367360155, "SCP500Pickup")]
        SCP500Pickup,

        [Prefab(689511071, "SCP207Pickup")]
        SCP207Pickup,

        [Prefab(4056235189, "Ammo12gaPickup")]
        Ammo12gaPickup,

        [Prefab(212068596, "E11SRPickup")]
        E11SRPickup,

        [Prefab(1982658896, "CrossvecPickup")]
        CrossvecPickup,

        [Prefab(2474630775, "Ammo556mmPickup")]
        Ammo556mmPickup,

        [Prefab(3462306180, "Fsp9Pickup")]
        Fsp9Pickup,

        [Prefab(2405374689, "LogicerPickup")]
        LogicerPickup,

        [Prefab(1273232029, "HegPickup")]
        HegPickup,

        [Prefab(3871663704, "FlashbangPickup")]
        FlashbangPickup,

        [Prefab(1499866827, "Ammo44calPickup")]
        Ammo44calPickup,

        [Prefab(3685499023, "Ammo762mmPickup")]
        Ammo762mmPickup,

        [Prefab(2344368365, "Ammo9mmPickup")]
        Ammo9mmPickup,

        [Prefab(1749039070, "Com18Pickup")]
        Com18Pickup,

        [Prefab(3525743409, "Scp018Projectile")]
        Scp018Projectile,

        [Prefab(3711531185, "SCP268Pickup")]
        SCP268Pickup,

        [Prefab(1573779433, "AdrenalinePrefab")]
        AdrenalinePrefab,

        [Prefab(3124923193, "PainkillersPickup")]
        PainkillersPickup,

        [Prefab(3134959991, "CoinPickup")]
        CoinPickup,

        [Prefab(941440279, "Light Armor Pickup")]
        LightArmorPickup,

        [Prefab(3118088094, "Combat Armor Pickup")]
        CombatArmorPickup,

        [Prefab(3164421243, "Heavy Armor Pickup")]
        HeavyArmorPickup,

        [Prefab(1861159387, "RevolverPickup")]
        RevolverPickup,

        [Prefab(3814984482, "AkPickup")]
        AkPickup,

        [Prefab(3180035653, "ShotgunPickup")]
        ShotgunPickup,

        [Prefab(464602874, "Scp330Pickup")]
        Scp330Pickup,

        [Prefab(1983050408, "Scp2176Projectile")]
        Scp2176Projectile,

        [Prefab(2088018000, "SCP244APickup Variant")]
        SCP244APickup,

        [Prefab(3030062014, "SCP244BPickup Variant")]
        SCP244BPickup,

        [Prefab(2702950243, "SCP1853Pickup")]
        SCP1853Pickup,

        [Prefab(3881162440, "DisruptorPickup")]
        DisruptorPickup,

        [Prefab(504857316, "Com45Pickup")]
        Com45Pickup,

        [Prefab(303271247, "SCP1576Pickup")]
        SCP1576Pickup,

        [Prefab(2915316078, "JailbirdPickup")]
        JailbirdPickup,

        [Prefab(1209253563, "AntiSCP207Pickup")]
        AntiSCP207Pickup,

        [Prefab(2216560136, "FRMG0Pickup")]
        FRMG0Pickup,

        [Prefab(74988289, "A7Pickup")]
        A7Pickup,

        [Prefab(3532394942, "LanternPickup")]
        LanternPickup,

        [Prefab(4143962266, "SCP1344Pickup")]
        Scp1344Pickup,

        [Prefab(825024811, "Amnestic Cloud Hazard")]
        AmnesticCloudHazard,

        [Prefab(2286635216, "Scp018PedestalStructure Variant")]
        Scp018PedestalStructure,

        [Prefab(664776131, "Scp207PedestalStructure Variant")]
        Scp207PedestalStructure,

        [Prefab(3724306703, "Scp244PedestalStructure Variant")]
        Scp244PedestalStructure,

        [Prefab(3849573771, "Scp268PedestalStructure Variant")]
        Scp268PedestalStructure,

        [Prefab(373821065, "Scp500PedestalStructure Variant")]
        Scp500PedestalStructure,

        [Prefab(3962534659, "Scp1853PedestalStructure Variant")]
        Scp1853PedestalStructure,

        [Prefab(3578915554, "Scp2176PedestalStructure Variant")]
        Scp2176PedestalStructure,

        [Prefab(3372339835, "Scp1576PedestalStructure Variant")]
        Scp1576PedestalStructure,

        [Prefab(2399831573, "AntiScp207PedestalStructure Variant")]
        AntiScp207PedestalStructure,

        [Prefab(1763950070, "Scp1344PedestalStructure Variant")]
        Scp1344PedestalStructure,

        [Prefab(2830750618, "LargeGunLockerStructure")]
        LargeGunLockerStructure,

        [Prefab(2372810204, "Experimental Weapon Locker")]
        ExperimentalLockerStructure,

        [Prefab(3352879624, "RifleRackStructure")]
        RifleRackStructure,

        [Prefab(1964083310, "MiscLocker")]
        MiscLocker,

        [Prefab(2724603877, "GeneratorStructure")]
        GeneratorStructure,

        [Prefab(1783091262, "Spawnable Work Station Structure")]
        WorkstationStructure,

        [Prefab(4040822781, "RegularMedkitStructure")]
        RegularMedkitStructure,

        [Prefab(2525847434, "AdrenalineMedkitStructure")]
        AdrenalineMedkitStructure,

        [Prefab(427210814, "HegProjectile")]
        HegProjectile,

        [Prefab(2409733045, "FlashbangProjectile")]
        FlashbangProjectile,

        [Prefab(1062458989, "SCP-173_Ragdoll")]
        Scp173Ragdoll,

        [Prefab(1951328980, "Ragdoll_1")]
        Ragdoll1,

        [Prefab(992490681, "SCP-106_Ragdoll")]
        Scp106Ragdoll,

        [Prefab(3219675689, "Ragdoll_4")]
        Ragdoll4,

        [Prefab(417388851, "Ragdoll_7")]
        Ragdoll7,

        [Prefab(3185790062, "Ragdoll_6")]
        Ragdoll6,

        [Prefab(2567420661, "Ragdoll_8")]
        Ragdoll8,

        [Prefab(149379640, "SCP-096_Ragdoll")]
        Scp096Ragdoll,

        [Prefab(1862774274, "Ragdoll_10")]
        Ragdoll10,

        [Prefab(2710373253, "Ragdoll_Tut")]
        RagdollTutorial,

        [Prefab(1389252654, "Ragdoll_12")]
        Ragdoll12,

        [Prefab(3175759689, "SCP-939_Ragdoll")]
        Scp939Ragdoll,

        [Prefab(3721192489, "Scp3114_Ragdoll")]
        Scp3114Ragdoll,

        [Prefab(2588580243, "ElevatorChamber")]
        ElevatorChamber,

        [Prefab(1757973841, "ElevatorChamber_Gates")]
        ElevatorChamber_Gates,

        [Prefab(912031041, "ElevatorChamberNuke")]
        ElevatorChamberNuke,

        [Prefab(3087007600, "CapybaraToy")]
        CapybaraToy,

        [Prefab(3539746802, "Sinkhole")]
        Sinkhole,

        [Prefab(1548138668, "AutoRagdoll")]
        AutoRagdoll,

        [Prefab(1323017091, "ElevatorChamberCargo")]
        ElevatorChamberCargo,

        [Prefab(359728307, "InvisibleInteractableToy")]
        InvisibleInteractableToy,

        [Prefab(1824808402, "EzArmCameraToy")]
        EzArmCameraToy,

        [Prefab(3375932423, "EzCameraToy")]
        EzCameraToy,

        [Prefab(144958943, "HczCameraToy")]
        HczCameraToy,

        [Prefab(2026969629, "LczCameraToy")]
        LczCameraToy,

        [Prefab(1548138668, "SzCameraToy")]
        SzCameraToy,

        [Prefab(2842703865, "KeycardPickup_Chaos")]
        KeycardPickupChaos,

        [Prefab(162530276, "TextToy")]
        TextToy,

        [Prefab(4046276968, "Spawnable Unsecured Pryable GateDoor")]
        SpawnableUnsecuredPryableGateDoor,

        [Prefab(2332883846, "CullableParentToy")]
        CullableParentToy,

        [Prefab(3938583646, "WaypointToy")]
        WaypointToy,

        [Prefab(1891631329, "PrismaticCloud")]
        PrismaticCloud,

        [Prefab(3938583646, "TantrumObj (Brown Candy)")]
        TantrumObjBrownCandy,

        [Prefab(1145481038, "Scp1509Pickup")]
        Scp1509Pickup,

        [Prefab(1359696107, "Hubert Moon")]
        HubertMoon,

        [Prefab(4075838184, "Scp018Projectile Halloween")]
        Scp018ProjectileHalloween,

        [Prefab(3262457219, "JailbirdPickup Halloween")]
        JailbirdPickupHalloween,

        [Prefab(1712001893, "Scp1509PedestalStructure Variant")]
        Scp1509PedestalStructureVariant,

        [Prefab(517392265, "SCP-173 Ragdoll - MrNutty Sombrero Variant")]
        Scp173RagdollMrNuttySombreroVariant,

        [Prefab(2174094462, "SCP-049 Ragdoll Halloween")]
        Scp049RagdollHalloween,

        [Prefab(4255002108, "SCP-096 Ragdoll Halloween")]
        Scp096RagdollHalloween,

        [Prefab(2556293836, "Zombie Ragdoll Halloween")]
        ZombieRagdollHalloween,

        [Prefab(213466224, "SCP-939 Ragdoll Halloween")]
        Scp939RagdollHalloween,
    }
}