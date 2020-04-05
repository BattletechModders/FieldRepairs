using BattleTech;
using FieldRepairs.Helper;
using Harmony;
using Localize;
using System;
using System.Text;
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
            StringBuilder componentDamageSB = new StringBuilder();
            MechRepairState repairState = new MechRepairState(__instance, targetMech);
            foreach (MechComponent mc in repairState.DamagedComponents)
            {
                Mod.Log.Debug($"Damaging component: {mc.UIName}");
                Text localText = new Text(" - {0}\n", new object[] { mc.UIName });
                componentDamageSB.Append(localText.ToString());

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
                float damage = (float)Math.Floor(maxArmor * maxDamageRatio);
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

            // We don't limit to armor damage locations here so we can represent that armor is easily scavenged
            for (int i = 0; i < repairState.StructureHits; i++)
            {
                ChassisLocations location = LocationHelper.GetChassisLocations();
                float maxStructure = targetMech.GetMaxStructure(location);
                float maxDamageRatio = Mod.Random.Next(
                    (int)(Mod.Config.MinStructureLossPerHit * 100),
                    (int)(Mod.Config.MaxStructureLossPerHit * 100)
                    ) / 100f;
                float damage = (float)Math.Floor(maxStructure * maxDamageRatio);
                if (targetMech.GetCurrentStructure(location) - damage < 1)
                {
                    // Never allow a hit to completely remove a limb or location
                    damage = targetMech.GetCurrentStructure(location) - 1;
                }
                Mod.Log.Debug($"Reducing structure in location {location} by {maxDamageRatio}% for {damage} points");

                if (damage != 0)
                {
                    targetMech.StatCollection.ModifyStat<float>(hitInfo.attackerId, hitInfo.stackItemUID,
                        targetMech.GetStringForStructureLocation(location),
                        StatCollection.StatOperation.Float_Subtract, damage, -1, true);
                }

                targetMech.UpdateLocationDamageLevel(location, hitInfo.attackerId, hitInfo.stackItemUID);

                if (location == ChassisLocations.Head) armorOrStructHeadHits++;
            }

            // Apply any pilot hits
            StringBuilder pilotDamageSB = new StringBuilder();
            if (armorOrStructHeadHits > 0)
            {
                int healthDamage = armorOrStructHeadHits;
                if (targetMech.pilot.BonusHealth > 0)
                {
                    int absorbedDamage;
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
                    Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_PILOT_BONUS_HEALTH], new object[] { absorbedDamage });
                    pilotDamageSB.Append(localText.ToString());
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
                    Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_PILOT_HEALTH], new object[] { healthDamage });
                    pilotDamageSB.Append(localText.ToString());
                }

            }

            // Apply any pilot skill reductions
            StringBuilder pilotSkillSB = new StringBuilder();
            if (repairState.PilotSkillHits > 0)
            {
                Mod.Log.Debug($"Applying {repairState.PilotSkillHits} hits to pilot skills.");
                int totalMod = 0;
                for (int i = 0; i < repairState.PilotSkillHits; i++)
                {
                    totalMod += Mod.Random.Next(Mod.Config.MinSkillPenaltyPerHit, Mod.Config.MaxSkillPenaltyPerHit);
                }
                Mod.Log.Debug($"  A total penalty of -{totalMod} will be applied to all pilot skills");

                int pilotingMod = targetMech.pilot.Piloting - totalMod >= 1 ? totalMod : targetMech.pilot.Piloting - 1;
                int gunneryMod= targetMech.pilot.Gunnery - totalMod >= 1 ? totalMod : targetMech.pilot.Gunnery - 1;
                int tacticsMod = targetMech.pilot.Tactics - totalMod >= 1 ? totalMod : targetMech.pilot.Tactics - 1;
                int gutsMod = targetMech.pilot.Guts - totalMod >= 1 ? totalMod : targetMech.pilot.Guts - 1;

                Mod.Log.Debug($"  reducing piloting: -{pilotingMod}  gunnery: -{gunneryMod}  tactics: -{tacticsMod}  guts: -{gutsMod}"); ;

                targetMech.pilot.StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID,
                    "Piloting", StatCollection.StatOperation.Int_Subtract, pilotingMod, -1, true);
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_SKILL_PILOTING], new object[] { pilotingMod });
                pilotSkillSB.Append(localText.ToString());

                targetMech.pilot.StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID,
                    "Gunnery", StatCollection.StatOperation.Int_Subtract, gunneryMod, -1, true);
                localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_SKILL_GUNNERY], new object[] { gunneryMod });
                pilotSkillSB.Append(localText.ToString());

                targetMech.pilot.StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID,
                    "Tactics", StatCollection.StatOperation.Int_Subtract, tacticsMod, -1, true);
                localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_SKILL_TACTICS], new object[] { tacticsMod });
                pilotSkillSB.Append(localText.ToString());

                targetMech.pilot.StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID,
                    "Guts", StatCollection.StatOperation.Int_Subtract, gutsMod, -1, true);
                localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_SKILL_GUTS], new object[] { gutsMod });
                pilotSkillSB.Append(localText.ToString());
            }

            StringBuilder descSB = new StringBuilder();
            if (componentDamageSB.Length > 0)
            {
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_DAMAGE_COMP]);
                descSB.Append(localText.ToString());
                descSB.Append(componentDamageSB.ToString());
            }
            if (pilotDamageSB.Length > 0)
            {
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_DAMAGE_PILOT]);
                descSB.Append(localText.ToString());
                descSB.Append(pilotDamageSB.ToString());
            }
            if (pilotSkillSB.Length > 0)
            {
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_DAMAGE_SKILL]);
                descSB.Append(localText.ToString());
                descSB.Append(pilotSkillSB.ToString());
            }


            __instance.EffectData.Description = new BaseDescriptionDef("PoorlyMaintained", ModState.CurrentTheme.Label,
                descSB.ToString(), __instance.EffectData.Description.Icon);

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
