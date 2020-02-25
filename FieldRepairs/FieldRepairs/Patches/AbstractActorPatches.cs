using BattleTech;
using Harmony;
using us.frostraptor.modUtils;

namespace FieldRepairs.Patches {
    [HarmonyPatch(typeof(AbstractActor), "CreateSpawnEffectByTag")]
    public static class AbstractActor_CreateSpawnEffectByTag {
        public static void Prefix(AbstractActor __instance) {
            Mod.Log.Trace("AA:CSEBT - entered.");

            // TODO: Intercept existing CreateEffect call? 
            // TODO: Set base description defs here?
        }
    }
}
