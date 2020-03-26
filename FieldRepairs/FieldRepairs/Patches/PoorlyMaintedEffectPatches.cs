using BattleTech;
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

            Mod.Log.Debug($"-- Building critical components list for actor: {CombatantUtils.Label(targetMech)}");
            MechNonEssentialComponents nonEssentials = new MechNonEssentialComponents();
            foreach (MechComponent mc in targetMech.allComponents) {
                if (mc.componentDef.CriticalComponent) {
                    Mod.Log.Trace($"  Skipping critical component: {mc.Description.UIName} in location: {(ChassisLocations)mc.Location}");
                } else if (mc.componentType == ComponentType.AmmunitionBox) {
                    Mod.Log.Debug($"  - Found ammoBox {mc.Description.UIName}");
                    nonEssentials.AmmoBoxes.Add(mc);
                } else if (mc.componentType == ComponentType.HeatSink) {
                    Mod.Log.Debug($"  - Found heatSink {mc.Description.UIName}");
                    nonEssentials.HeatSinks.Add(mc);
                } else {
                    switch ((ChassisLocations)mc.Location) {
                        case ChassisLocations.Head:
                            nonEssentials.HeadComponents.Add(mc);
                            break;
                        case ChassisLocations.LeftArm:
                            nonEssentials.LeftArmComponents.Add(mc);
                            break;
                        case ChassisLocations.LeftLeg:
                            nonEssentials.LeftLegComponents.Add(mc);
                            break;
                        case ChassisLocations.LeftTorso:
                            nonEssentials.LeftTorsoComponents.Add(mc);
                            break;
                        case ChassisLocations.RightArm:
                            nonEssentials.RightArmComponents.Add(mc);
                            break;
                        case ChassisLocations.RightLeg:
                            nonEssentials.RightLegComponents.Add(mc);
                            break;
                        case ChassisLocations.RightTorso:
                            nonEssentials.RightTorsoComponents.Add(mc);
                            break;
                        case ChassisLocations.CenterTorso:
                            nonEssentials.CenterTorsoComponents.Add(mc);
                            break;
                        default:
                            Mod.Log.Debug($" WARN: Unexpected location: {mc.Location} for mech component. Skipping {mc.Description.UIName}");
                            break;
                    }
                }
            }

            MechRepairState mechRepairState = new MechRepairState(__instance, targetMech, nonEssentials);

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
