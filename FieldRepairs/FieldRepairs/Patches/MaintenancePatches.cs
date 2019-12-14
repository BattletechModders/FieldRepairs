using BattleTech;
using FieldRepairs.Helper;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldRepairs.Patches {

    // TODO: Probably need to patch CreatePoorlyMaintainedEffectData to update description?

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToBuilding")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToBuilding {
        static bool Postfix(PoorlyMaintainedEffect __instance, Building targetBuilding) {

            BuildingRepairState repairState = RepairsHelper.GetRepairState(__instance, targetBuilding);

            return false;
        }
    }

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToMech")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToMech {
        static bool Postfix(PoorlyMaintainedEffect __instance, Mech targetMech) {

            return false;
        }
    }

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToTurret")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToTurret {
        static bool Postfix(PoorlyMaintainedEffect __instance, Turret targetTurret) {

            return false;
        }
    }

    [HarmonyPatch(typeof(PoorlyMaintainedEffect), "ApplyEffectsToVehicle")]
    public static class PoorlyMaintainedEffect_ApplyEffectsToVehicle {
        static bool Postfix(PoorlyMaintainedEffect __instance, Vehicle targetVehicle) {

            return false;
        }
    }
}
