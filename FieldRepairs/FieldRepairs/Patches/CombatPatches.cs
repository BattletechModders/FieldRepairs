using BattleTech;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldRepairs.Patches {

    // Determine the theme for the match and mark the effects are needing to be applied
    [HarmonyPatch(typeof(CombatGameState), "FirstTimeInit")]
    public static class CombatGameState_FirstTimeInit {

        public static void Postfix(CombatGameState __instance) {

        }
    }

    // Do nothing, because presumably we've already applied the effects
    [HarmonyPatch(typeof(CombatGameState), "InitFromSave")]
    public static class CombatGameState_InitFromSave {

        public static void Postfix(CombatGameState __instance) {

        }
    }

    // Clear the state so the theme will be recaluated
    [HarmonyPatch(typeof(CombatGameState), "OnCombatGameDestroyed")]
    public static class CombatGameState__Init {

        public static void Postfix(CombatGameState __instance) {

        }
    }

}
