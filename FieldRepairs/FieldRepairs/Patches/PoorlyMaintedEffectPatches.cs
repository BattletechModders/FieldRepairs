using BattleTech;
using FieldRepairs.Helper;
using Harmony;
using us.frostraptor.modUtils;


namespace FieldRepairs.Patches {

    // Vanilla implementation only comes in _25 = (0.25, 0), _50 = (0.50, 0), _75 = (0.75, 0) implementations - see AbstractActor::CreateSpawnEffectByTag

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "CreatePoorlyMaintainedEffectData")]
    public static class PoorlyMaintainedEffect_CreatePoorlyMaintainedEffectData {
        static void Postfix(CombatGameConstants constants, float armorReduction, float ammoReduction, ref EffectData __result) {
            Mod.Log.Trace("PME:CPMED - entered.");
            
        }
    }

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToBuilding")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToBuilding {
        static void Postfix(PoorlyMaintainedEffect __instance, Building targetBuilding) {
            Mod.Log.Trace("PME:AETB - entered.");
            //BuildingRepairState repairState = RepairsHelper.GetRepairState(__instance, targetBuilding);
        }
    }

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToMech")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToMech {
        static bool Prefix(PoorlyMaintainedEffect __instance, Mech targetMech) {
            Mod.Log.Trace("PME:AETM - entered.");

            Mod.Log.Debug($" Applying PoorlyMaintainedEffect to unit: {CombatantUtils.Label(targetMech)}");

            WeaponHitInfo hitInfo = new WeaponHitInfo(-1, -1, -1, -1, "", "", -1, 
                null, null, null, null, null, null, null, 
                new AttackDirection[] { AttackDirection.FromFront }, null, null, null);

            // Apply any structure damage first
            MechRepairState repairState = new MechRepairState(__instance, targetMech);
            foreach (MechComponent mc in repairState.DamagedComponents)
            {
                Mod.Log.Debug($"Damaging component: {mc.UIName}");
                mc.DamageComponent(hitInfo, ComponentDamageLevel.Destroyed, false);
            }

            int armorOrStructHeadHits = 0;
            // Then apply any armor hits
            for (int i = 0; i < repairState.ArmorHits; i++)
            {
                ArmorLocation location = LocationHelper.GetMechArmorLocation();
                float maxArmor = targetMech.GetMaxArmor(location);
                float maxDamageRatio = Mod.Random.Next(
                    (int) (Mod.Config.MinArmorLossPerHit * 100), 
                    (int) (Mod.Config.MaxArmorLossPerHit * 100)
                    ) / 100f;
                float damage = maxArmor * maxDamageRatio;
                if (targetMech.GetCurrentArmor(location) - damage < 0) 
                {
                    damage = targetMech.GetCurrentArmor(location);
                }
                Mod.Log.Debug($"Reducing armor in location {location} by {maxDamageRatio}% for {damage} points");

                if (damage != 0) 
                {
                    targetMech.StatCollection.ModifyStat<float>(hitInfo.attackerId, hitInfo.stackItemUID,
                        targetMech.GetStringForArmorLocation(location),
                        StatCollection.StatOperation.Float_Subtract, damage, -1, true);
                }

                if (location == ArmorLocation.Head) armorOrStructHeadHits++;
            }

            // TODO: Should we limit to locations that have taken armor damage? Probably not as we
            //   want to represent that armor damage is easily repaired
            for (int i = 0; i < repairState.StructureHits; i++)
            {
                ChassisLocations location = LocationHelper.GetChassisLocations();
                float maxStructure = targetMech.GetMaxStructure(location);
                float maxDamageRatio = Mod.Random.Next(
                    (int)(Mod.Config.MinStructureLossPerHit * 100),
                    (int)(Mod.Config.MaxStructureLossPerHit * 100)
                    ) / 100f;
                float damage = maxStructure * maxDamageRatio;
                if (targetMech.GetCurrentStructure(location) - damage < 1)
                {
                    // Never allow a hit to completely remove a limb or location
                    damage = targetMech.GetCurrentStructure(location) - 1;
                }
                Mod.Log.Debug($"Reducing structure in location {location} by {maxDamageRatio}% for {damage} points");
                targetMech.UpdateLocationDamageLevel(location, hitInfo.attackerId, hitInfo.stackItemUID);

                if (location == ChassisLocations.Head) armorOrStructHeadHits++;
            }

            // Apply any pilot hits
            if (armorOrStructHeadHits > 0)
            {
                int healthDamage = armorOrStructHeadHits;
                if (targetMech.pilot.BonusHealth > 0)
                {
                    int absorbedDamage = 0;
                    if (targetMech.pilot.BonusHealth >= healthDamage) 
                    {
                        absorbedDamage = healthDamage;
                        healthDamage = 0;
                    }
                    else 
                    {
                        absorbedDamage = targetMech.pilot.BonusHealth;
                        healthDamage = healthDamage - targetMech.pilot.BonusHealth;
                    }

                    Mod.Log.Debug($"Bonus health aborbs: {absorbedDamage} leaving: {healthDamage} healthDamage.");
                    targetMech.pilot.StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID, 
                        "BonusHealth", StatCollection.StatOperation.Int_Subtract, absorbedDamage, -1, true);
                }

                if (healthDamage > (targetMech.pilot.Health - 1))
                {
                    Mod.Log.Debug($"Health damage: {healthDamage} would kill pilot, reducing to maxHealth: {targetMech.pilot.Health} - 1");
                    healthDamage = targetMech.pilot.Health - 1;
                }

                if (healthDamage > 0)
                {
                    Mod.Log.Debug($"Adding {healthDamage} to {CombatantUtils.Label(targetMech)}");
                    targetMech.pilot.StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID, 
                        "Injuries", StatCollection.StatOperation.Int_Add, healthDamage, -1, true);
                }

            }
            
            // Apply any pilot skill reductions
            if (repairState.PilotSkillHits > 0)
            {
                Mod.Log.Debug($"Applying {repairState.PilotSkillHits} hits to pilot skills.");
                int totalMod = 0;
                for (int i = 0; i < repairState.PilotSkillHits; i++)
                {
                    totalMod += Mod.Random.Next(Mod.Config.MinSkillPenaltyPerHit, Mod.Config.MaxSkillPenaltyPerHit);
                }
                Mod.Log.Debug($"  A total penalty of -{totalMod} will be applied to all pilot skills");


            }
            

            return false;
        }
    }

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToTurret")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToTurret {
        static void Postfix(PoorlyMaintainedEffect __instance, Turret targetTurret) {
            Mod.Log.Trace("PME:AETT - entered.");
        }
    }

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToVehicle")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToVehicle {
        static void Postfix(PoorlyMaintainedEffect __instance, Vehicle targetVehicle) {
            Mod.Log.Trace("PME:AETV - entered.");
        }
    }
}
