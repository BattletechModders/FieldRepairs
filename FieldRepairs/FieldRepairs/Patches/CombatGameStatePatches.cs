namespace FieldRepairs.Patches
{

    // Determine the theme for the match and mark the effects are needing to be applied
    [HarmonyPatch(typeof(CombatGameState), "FirstTimeInit")]
    public static class CombatGameState_FirstTimeInit
    {

        public static void Postfix(CombatGameState __instance)
        {
            int themeIdx = Mod.Random.Next(0, Mod.Config.Themes.Count - 1);
            ModState.CurrentTheme = Mod.Config.Themes[themeIdx];
            Mod.Log.Info?.Write($"Set StateTheme to: {ModState.CurrentTheme}");
        }
    }

}
