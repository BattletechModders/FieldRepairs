namespace FieldRepairs.Patches
{
    [HarmonyPatch(typeof(UnitSpawnPointGameLogic), "Spawn")]
    public static class UnitSpawnPointGameLogic_Spawn
    {
        public static void Prefix(ref bool __runOriginal, UnitSpawnPointGameLogic __instance)
        {
            if (!__runOriginal) return;

            Mod.Log.Trace?.Write("USPGL:S - entered.");
            if (Mod.Config.Skirmish.Tag != null && !Mod.Config.Skirmish.Tag.Equals(""))
            {
                if (__instance.Combat.ActiveContract.ContractTypeValue.IsSkirmish)
                {
                    Mod.Log.Debug?.Write($"Contract is skirmish. Existing tags are: {__instance.spawnEffectTags}");
                    if (!__instance.Combat.HostilityMatrix.IsLocalPlayerFriendly(__instance.teamDefinitionGuid))
                    {
                        Mod.Log.Debug?.Write($"Unit belongs to enemy or neutral team, adding flag: {Mod.Config.Skirmish.Tag}");
                        __instance.spawnEffectTags.Add(Mod.Config.Skirmish.Tag);
                    }
                }

            }
        }
    }
}
