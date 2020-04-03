using BattleTech;
using Harmony;
using us.frostraptor.modUtils;

namespace FieldRepairs.Patches {
    [HarmonyPatch(typeof(UnitSpawnPointGameLogic), "Spawn")]
    public static class UnitSpawnPointGameLogic_Spawn {
        public static void Prefix(UnitSpawnPointGameLogic __instance, string ___teamDefinitionGuid) {
            Mod.Log.Trace("USPGL:S - entered.");
            if (__instance.Combat.ActiveContract.ContractTypeValue.IsSkirmish) {
                Mod.Log.Debug($"Contract is skirmish. Existing tags are: {__instance.spawnEffectTags}");
                if (!__instance.Combat.HostilityMatrix.IsLocalPlayerFriendly(___teamDefinitionGuid)) {
                    Mod.Log.Debug($"Unit  belongs to enemy or neutral team, adding flag: {Mod.Config.Skirmish.Tag}");
                    __instance.spawnEffectTags.Add(Mod.Config.Skirmish.Tag);
                }
            }
        }
    }
}
