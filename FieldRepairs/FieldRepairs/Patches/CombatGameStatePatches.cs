using BattleTech;
using Harmony;
using System;

namespace FieldRepairs.Patches {

    // Determine the theme for the match and mark the effects are needing to be applied
    [HarmonyPatch(typeof(CombatGameState), "FirstTimeInit")]
    public static class CombatGameState_FirstTimeInit {

        public static void Postfix(CombatGameState __instance) {
            Array themeValues = Enum.GetValues(typeof(StateTheme));
            int themeIdx = Mod.Random.Next(0, themeValues.Length - 1);
            ModState.CurrentTheme = (StateTheme)themeValues.GetValue(themeIdx);
            Mod.Log.Info($"Set StateTheme to: {ModState.CurrentTheme}");
        }
    }

}
