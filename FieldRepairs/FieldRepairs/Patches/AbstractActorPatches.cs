using System.Collections.Generic;

namespace FieldRepairs.Patches
{
    //[HarmonyPatch(typeof(AbstractActor), "CreateSpawnEffectByTag")]
    //public static class AbstractActor_CreateSpawnEffectByTag {
    //    public static void Prefix(AbstractActor __instance) {
    //        Mod.Log.Trace?.Write("AA:CSEBT - entered.");

    //        // TODO: Intercept existing CreateEffect call? 
    //        // TODO: Set base description defs here?
    //    }
    //}

    // This patch is necessary to eliminate some NREs when actors spawn and effects that target visibility or 
    //   pathfinder effects fire. This can occur before the UI is available, which is what causes these errors.
    //   When FR is processing damage, suppress the visibility and pathfinder updates.
    [HarmonyPatch(typeof(AbstractActor), "OnEffectEnd")]
    static class AbstractActor_OnEffectEnd
    {
        static bool Prefix(AbstractActor __instance, Effect effect, List<Effect> ___markEffects, int ___StealthPipsPrevious)
        {
            if (ModState.SuppressShowActorSequences)
            {
                Mod.Log.Debug?.Write("Suppressing pathfinder updates and visiblity updates during damage call.");

                ___markEffects.Remove(effect);

                if (effect.EffectData.targetingData.specialRules == AbilityDef.SpecialRules.Aura)
                {
                    __instance.Combat.MessageCenter.PublishMessage(new AuraRemovedMessage(effect.creatorID, __instance.GUID, effect.id, effect.EffectData));
                }
                if (__instance.StealthPipsCurrent != ___StealthPipsPrevious)
                {
                    __instance.Combat.MessageCenter.PublishMessage(new StealthChangedMessage(__instance.GUID, __instance.StealthPipsCurrent));
                }

                return false;
            }

            return true;
        }
    }
}
