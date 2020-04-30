using BattleTech;
using FieldRepairs.Helper;
using Harmony;
using Localize;
using System;
using System.Text;
using us.frostraptor.modUtils;


namespace FieldRepairs.Patches {

    // Vanilla implementation only comes in _25 = (0.25, 0), _50 = (0.50, 0), _75 = (0.75, 0) implementations - see AbstractActor::CreateSpawnEffectByTag

    //[HarmonyPatch(typeof(PoorlyMaintainedEffect), "CreatePoorlyMaintainedEffectData")]
    //public static class PoorlyMaintainedEffect_CreatePoorlyMaintainedEffectData {
    //    static void Postfix(CombatGameConstants constants, float armorReduction, float ammoReduction, ref EffectData __result) {
    //        Mod.Log.Trace("PME:CPMED - entered.");
    //    }
    //}

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToBuilding")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToBuilding {
        static bool Prefix(PoorlyMaintainedEffect __instance, Building targetBuilding) {
            Mod.Log.Trace("PME:AETB - entered.");
            //BuildingRepairState repairState = RepairsHelper.GetRepairState(__instance, targetBuilding);

            // Note that OnEffectBegin will invoke *every* ApplyEffects, and expects the ApplyEfect to check that the target isn't null. 
            if (targetBuilding == null) { return false; }

            return true;
        }
    }

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToMech")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToMech {
        static bool Prefix(PoorlyMaintainedEffect __instance, Mech targetMech) {
            Mod.Log.Trace("PME:AETM - entered.");

            // Note that OnEffectBegin will invoke *every* ApplyEffects, and expects the ApplyEfect to check that the target isn't null. 
            if (targetMech == null) { return false; }

            Mod.Log.Info($" Applying PoorlyMaintainedEffect to unit: {CombatantUtils.Label(targetMech)}");
            ModState.SuppressShowActorSequences = true;

            WeaponHitInfo hitInfo = new WeaponHitInfo(-1, -1, -1, -1, "", "", -1, 
                null, null, null, null, null, null, null, 
                new AttackDirection[] { AttackDirection.FromFront }, null, null, null);

            // Apply any structure damage first
            StringBuilder componentDamageSB = new StringBuilder();
            MechRepairState repairState = new MechRepairState(__instance, targetMech);
            foreach (MechComponent mc in repairState.DamagedComponents)
            {
                if (mc.componentType == ComponentType.AmmunitionBox)
                {
                    AmmunitionBox ab = (AmmunitionBox)mc;
                    float ammoReduction = Mod.Random.Next(
                        (int)(Mod.Config.PerHitPenalties.MaxAmmoLoss * 100f),
                        (int)(Mod.Config.PerHitPenalties.MinAmmoLoss * 100f)
                        ) / 100f;
                    int newAmmo = (int)Math.Floor(ab.CurrentAmmo * ammoReduction);
                    Mod.Log.Info($"Reducing ammoBox: {mc.UIName} from {ab.CurrentAmmo} x {ammoReduction} = {newAmmo}");
                    ab.StatCollection.Set<int>(ModStats.AmmoBoxCurrentAmmo, newAmmo);
                }
                else
                {
                    Mod.Log.Info($"Damaging component: {mc.UIName}");
                    Text localText = new Text(" - {0}\n", new object[] { mc.UIName });
                    componentDamageSB.Append(localText.ToString());

                    mc.DamageComponent(hitInfo, ComponentDamageLevel.Destroyed, false);
                }
            }

            int armorOrStructHeadHits = 0;
            // Then apply any armor hits
            for (int i = 0; i < repairState.ArmorHits; i++)
            {
                ArmorLocation location = LocationHelper.GetRandomMechArmorLocation();
                float maxArmor = targetMech.GetMaxArmor(location);
                float maxDamageRatio = Mod.Random.Next(
                    (int) (Mod.Config.PerHitPenalties.MinArmorLoss * 100), 
                    (int) (Mod.Config.PerHitPenalties.MaxArmorLoss * 100)
                    ) / 100f;
                float damage = (float)Math.Floor(maxArmor * maxDamageRatio);
                if (targetMech.GetCurrentArmor(location) - damage < 0) 
                {
                    damage = targetMech.GetCurrentArmor(location);
                }
                Mod.Log.Info($"Reducing armor in location {location} by {maxDamageRatio}% for {damage} points");

                if (damage != 0) 
                {
                    targetMech.StatCollection.ModifyStat<float>(hitInfo.attackerId, hitInfo.stackItemUID,
                        targetMech.GetStringForArmorLocation(location),
                        StatCollection.StatOperation.Float_Subtract, damage, -1, true);
                }

                if (location == ArmorLocation.Head) armorOrStructHeadHits++;
            }

            // We don't limit to armor damage locations here so we can represent that armor is easily scavenged
            for (int i = 0; i < repairState.StructureHits; i++)
            {
                ChassisLocations location = LocationHelper.GetRandomMechStructureLocation();
                float maxStructure = targetMech.GetMaxStructure(location);
                float maxDamageRatio = Mod.Random.Next(
                    (int)(Mod.Config.PerHitPenalties.MinStructureLoss * 100),
                    (int)(Mod.Config.PerHitPenalties.MaxStructureLoss * 100)
                    ) / 100f;
                float damage = (float)Math.Floor(maxStructure * maxDamageRatio);
                if (targetMech.GetCurrentStructure(location) - damage < 1)
                {
                    // Never allow a hit to completely remove a limb or location
                    damage = targetMech.GetCurrentStructure(location) - 1;
                }
                Mod.Log.Info($"Reducing structure in location {location} by {maxDamageRatio}% for {damage} points");

                if (damage != 0)
                {
                    targetMech.StatCollection.ModifyStat<float>(hitInfo.attackerId, hitInfo.stackItemUID,
                        targetMech.GetStringForStructureLocation(location),
                        StatCollection.StatOperation.Float_Subtract, damage, -1, true);
                }

                targetMech.UpdateLocationDamageLevel(location, hitInfo.attackerId, hitInfo.stackItemUID);

                if (location == ChassisLocations.Head) armorOrStructHeadHits++;
            }

            PilotHelper.ApplyPilotHealthDamage(targetMech, hitInfo, armorOrStructHeadHits, out string pilotHealthTooltipText);
            PilotHelper.ApplyPilotSkillDamage(targetMech, hitInfo, repairState.PilotSkillHits, out string pilotSkillDamageTooltipText);

            // Build the tooltip
            StringBuilder descSB = new StringBuilder();
            if (componentDamageSB.Length > 0)
            {
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_DAMAGE_COMP]);
                descSB.Append(localText.ToString());
                descSB.Append(componentDamageSB.ToString());
            }
            if (pilotHealthTooltipText != null)
            {
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_DAMAGE_PILOT]);
                descSB.Append(localText.ToString());
                descSB.Append(pilotHealthTooltipText);
            }
            if (pilotSkillDamageTooltipText != null)
            {
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_DAMAGE_SKILL]);
                descSB.Append(localText.ToString());
                descSB.Append(pilotSkillDamageTooltipText.ToString());
            }

            Text titleText = new Text(ModState.CurrentTheme.Label, new object[] { repairState.effectRating });
            __instance.EffectData.Description = new BaseDescriptionDef("PoorlyMaintained", 
                titleText.ToString(), descSB.ToString(), __instance.EffectData.Description.Icon);

            ModState.SuppressShowActorSequences = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToTurret")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToTurret {
        static bool Prefix(PoorlyMaintainedEffect __instance, Turret targetTurret) {
            Mod.Log.Trace("PME:AETT - entered.");

            // Note that OnEffectBegin will invoke *every* ApplyEffects, and expects the ApplyEfect to check that the target isn't null. 
            if (targetTurret == null) { return false; }

            Mod.Log.Info($" Applying PoorlyMaintainedEffect to unit: {CombatantUtils.Label(targetTurret)}");
            ModState.SuppressShowActorSequences = true;

            WeaponHitInfo hitInfo = new WeaponHitInfo(-1, -1, -1, -1, "", "", -1,
                null, null, null, null, null, null, null,
                new AttackDirection[] { AttackDirection.FromFront }, null, null, null);

            // Apply any structure damage first
            StringBuilder componentDamageSB = new StringBuilder();
            TurretRepairState repairState = new TurretRepairState(__instance, targetTurret);
            foreach (MechComponent mc in repairState.DamagedComponents)
            {
                if (mc.componentType == ComponentType.AmmunitionBox)
                {
                    AmmunitionBox ab = (AmmunitionBox)mc;
                    float ammoReduction = Mod.Random.Next(
                        (int)(Mod.Config.PerHitPenalties.MaxAmmoLoss * 100f),
                        (int)(Mod.Config.PerHitPenalties.MinAmmoLoss * 100f)
                        ) / 100f;
                    int newAmmo = (int)Math.Floor(ab.CurrentAmmo * ammoReduction);
                    Mod.Log.Info($"Reducing ammoBox: {mc.UIName} from {ab.CurrentAmmo} x {ammoReduction} = {newAmmo}");
                    ab.StatCollection.Set<int>(ModStats.AmmoBoxCurrentAmmo, newAmmo);
                }
                else
                {
                    Mod.Log.Info($"Damaging component: {mc.UIName}");
                    Text localText = new Text(" - {0}\n", new object[] { mc.UIName });
                    componentDamageSB.Append(localText.ToString());

                    mc.DamageComponent(hitInfo, ComponentDamageLevel.Destroyed, false);
                }
            }

            // Then apply any armor hits
            BuildingLocation structureLocation = BuildingLocation.Structure;
            for (int i = 0; i < repairState.ArmorHits; i++)
            {
                float maxArmor = targetTurret.GetMaxArmor(structureLocation);
                float maxDamageRatio = Mod.Random.Next(
                    (int)(Mod.Config.PerHitPenalties.MinArmorLoss * 100),
                    (int)(Mod.Config.PerHitPenalties.MaxArmorLoss * 100)
                    ) / 100f;
                float damage = (float)Math.Floor(maxArmor * maxDamageRatio);
                if (targetTurret.GetCurrentArmor(structureLocation) - damage < 0)
                {
                    damage = targetTurret.GetCurrentArmor(structureLocation);
                }
                Mod.Log.Info($"Reducing armor in location {structureLocation} by {maxDamageRatio}% for {damage} points");

                if (damage != 0)
                {
                    targetTurret.StatCollection.ModifyStat<float>(hitInfo.attackerId, hitInfo.stackItemUID,
                        targetTurret.GetStringForArmorLocation(structureLocation),
                        StatCollection.StatOperation.Float_Subtract, damage, -1, true);
                }

            }

            // We don't limit to armor damage locations here so we can represent that armor is easily scavenged
            for (int i = 0; i < repairState.StructureHits; i++)
            {
                float maxStructure = targetTurret.GetMaxStructure(structureLocation);
                float maxDamageRatio = Mod.Random.Next(
                    (int)(Mod.Config.PerHitPenalties.MinStructureLoss * 100),
                    (int)(Mod.Config.PerHitPenalties.MaxStructureLoss * 100)
                    ) / 100f;
                float damage = (float)Math.Floor(maxStructure * maxDamageRatio);
                if (targetTurret.GetCurrentStructure(structureLocation) - damage < 1)
                {
                    // Never allow a hit to completely remove a limb or location
                    damage = targetTurret.GetCurrentStructure(structureLocation) - 1;
                }
                Mod.Log.Info($"Reducing structure in location {structureLocation} by {maxDamageRatio}% for {damage} points");

                if (damage != 0)
                {
                    targetTurret.StatCollection.ModifyStat<float>(hitInfo.attackerId, hitInfo.stackItemUID,
                        targetTurret.GetStringForStructureLocation(structureLocation),
                        StatCollection.StatOperation.Float_Subtract, damage, -1, true);
                }

                targetTurret.UpdateLocationDamageLevel(structureLocation, hitInfo.attackerId, hitInfo.stackItemUID);

            }

            // Turrets have no head armor, can't take health hits
            PilotHelper.ApplyPilotSkillDamage(targetTurret, hitInfo, repairState.PilotSkillHits, out string pilotSkillDamageTooltipText);

            // Build the tooltip
            StringBuilder descSB = new StringBuilder();
            if (componentDamageSB.Length > 0)
            {
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_DAMAGE_COMP]);
                descSB.Append(localText.ToString());
                descSB.Append(componentDamageSB.ToString());
            }

            if (pilotSkillDamageTooltipText != null)
            {
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_DAMAGE_SKILL]);
                descSB.Append(localText.ToString());
                descSB.Append(pilotSkillDamageTooltipText.ToString());
            }

            Text titleText = new Text(ModState.CurrentTheme.Label, new object[] { repairState.effectRating });
            __instance.EffectData.Description = new BaseDescriptionDef("PoorlyMaintained",
                titleText.ToString(), descSB.ToString(), __instance.EffectData.Description.Icon);

            ModState.SuppressShowActorSequences = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToVehicle")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToVehicle {
        static bool Prefix(PoorlyMaintainedEffect __instance, Vehicle targetVehicle) {
            Mod.Log.Trace("PME:AETV - entered.");

            // Note that OnEffectBegin will invoke *every* ApplyEffects, and expects the ApplyEfect to check that the target isn't null. 
            if (targetVehicle == null) { return false; }

            Mod.Log.Info($" Applying PoorlyMaintainedEffect to unit: {CombatantUtils.Label(targetVehicle)}");
            ModState.SuppressShowActorSequences = true;

            WeaponHitInfo hitInfo = new WeaponHitInfo(-1, -1, -1, -1, "", "", -1,
                null, null, null, null, null, null, null,
                new AttackDirection[] { AttackDirection.FromFront }, null, null, null);

            // Apply any structure damage first
            StringBuilder componentDamageSB = new StringBuilder();
            VehicleRepairState repairState = new VehicleRepairState(__instance, targetVehicle);
            foreach (MechComponent mc in repairState.DamagedComponents)
            {
                if (mc.componentType == ComponentType.AmmunitionBox)
                {
                    AmmunitionBox ab = (AmmunitionBox)mc;
                    float ammoReduction = Mod.Random.Next(
                        (int)(Mod.Config.PerHitPenalties.MaxAmmoLoss * 100f),
                        (int)(Mod.Config.PerHitPenalties.MinAmmoLoss * 100f)
                        ) / 100f;
                    int newAmmo = (int)Math.Floor(ab.CurrentAmmo * ammoReduction);
                    Mod.Log.Info($"Reducing ammoBox: {mc.UIName} from {ab.CurrentAmmo} x {ammoReduction} = {newAmmo}");
                    ab.StatCollection.Set<int>(ModStats.AmmoBoxCurrentAmmo, newAmmo);
                }
                else
                {
                    Mod.Log.Info($"Damaging component: {mc.UIName}");
                    Text localText = new Text(" - {0}\n", new object[] { mc.UIName });
                    componentDamageSB.Append(localText.ToString());

                    mc.DamageComponent(hitInfo, ComponentDamageLevel.Destroyed, false);
                }
            }

            // Then apply any armor hits
            for (int i = 0; i < repairState.ArmorHits; i++)
            {
                VehicleChassisLocations location = LocationHelper.GetRandomVehicleLocation();
                float maxArmor = targetVehicle.GetMaxArmor(location);
                float maxDamageRatio = Mod.Random.Next(
                    (int)(Mod.Config.PerHitPenalties.MinArmorLoss * 100),
                    (int)(Mod.Config.PerHitPenalties.MaxArmorLoss * 100)
                    ) / 100f;
                float damage = (float)Math.Floor(maxArmor * maxDamageRatio);
                if (targetVehicle.GetCurrentArmor(location) - damage < 0)
                {
                    damage = targetVehicle.GetCurrentArmor(location);
                }
                Mod.Log.Info($"Reducing armor in location {location} by {maxDamageRatio}% for {damage} points");

                if (damage != 0)
                {
                    targetVehicle.StatCollection.ModifyStat<float>(hitInfo.attackerId, hitInfo.stackItemUID,
                        targetVehicle.GetStringForArmorLocation(location),
                        StatCollection.StatOperation.Float_Subtract, damage, -1, true);
                }

            }

            // We don't limit to armor damage locations here so we can represent that armor is easily scavenged
            for (int i = 0; i < repairState.StructureHits; i++)
            {
                VehicleChassisLocations location = LocationHelper.GetRandomVehicleLocation();
                float maxStructure = targetVehicle.GetMaxStructure(location);
                float maxDamageRatio = Mod.Random.Next(
                    (int)(Mod.Config.PerHitPenalties.MinStructureLoss * 100),
                    (int)(Mod.Config.PerHitPenalties.MaxStructureLoss * 100)
                    ) / 100f;
                float damage = (float)Math.Floor(maxStructure * maxDamageRatio);
                if (targetVehicle.GetCurrentStructure(location) - damage < 1)
                {
                    // Never allow a hit to completely remove a limb or location
                    damage = targetVehicle.GetCurrentStructure(location) - 1;
                }
                Mod.Log.Info($"Reducing structure in location {location} by {maxDamageRatio}% for {damage} points");

                if (damage != 0)
                {
                    targetVehicle.StatCollection.ModifyStat<float>(hitInfo.attackerId, hitInfo.stackItemUID,
                        targetVehicle.GetStringForStructureLocation(location),
                        StatCollection.StatOperation.Float_Subtract, damage, -1, true);
                }

                targetVehicle.UpdateLocationDamageLevel(location, hitInfo.attackerId, hitInfo.stackItemUID);

            }

            // Vehicles have no head armor, can't take health hits
            PilotHelper.ApplyPilotSkillDamage(targetVehicle, hitInfo, repairState.PilotSkillHits, out string pilotSkillDamageTooltipText);

            // Build the tooltip
            StringBuilder descSB = new StringBuilder();
            if (componentDamageSB.Length > 0)
            {
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_DAMAGE_COMP]);
                descSB.Append(localText.ToString());
                descSB.Append(componentDamageSB.ToString());
            }

            if (pilotSkillDamageTooltipText != null)
            {
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_DAMAGE_SKILL]);
                descSB.Append(localText.ToString());
                descSB.Append(pilotSkillDamageTooltipText.ToString());
            }

            Text titleText = new Text(ModState.CurrentTheme.Label, new object[] { repairState.effectRating });
            __instance.EffectData.Description = new BaseDescriptionDef("PoorlyMaintained", 
                titleText.ToString(), descSB.ToString(), __instance.EffectData.Description.Icon);

            ModState.SuppressShowActorSequences = false;
            return false;
        }
    }

}
