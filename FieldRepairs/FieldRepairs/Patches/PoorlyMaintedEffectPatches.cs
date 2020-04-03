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

            MechRepairState repairState = new MechRepairState(__instance, targetMech);
            foreach (MechComponent mc in repairState.DamagedComponents)
            {
                Mod.Log.Debug($"Damaging component: {mc.UIName}");
                mc.DamageComponent(hitInfo, ComponentDamageLevel.Destroyed, false);
            }

            for (int i = 0; i < repairState.ArmorHits; i++)
            {
                ArmorLocation location = LocationHelper.GetMechArmorLocation();
                targetMech.StatCollection.ModifyStat<float>(hitInfo.attackerId, hitInfo.stackItemUID, 
                    targetMech.GetStringForArmorLocation(location), StatCollection.StatOperation.Float_Subtract, damage, -1, true);
            }

            for (int i = 0; i < repairState.StructureHits; i++)
            {

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
