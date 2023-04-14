using System.Collections.Generic;

namespace FieldRepairs.Patches
{

    // This patch is necessary to eliminate some NREs when actors spawn and effects that target visibility or 
    //   pathfinder effects fire. This can occur before the UI is available, which is what causes these errors.
    //   When FR is processing damage, suppress the visibility and pathfinder updates.
    [HarmonyPatch(typeof(AbstractActor), "OnEffectEnd")]
    static class AbstractActor_OnEffectEnd
    {
        static void Prefix(ref bool __runOriginal, AbstractActor __instance, Effect effect)
        {
            if (!__runOriginal) return;

            if (ModState.SuppressShowActorSequences)
            {
                Mod.Log.Debug?.Write("Suppressing pathfinder updates and visiblity updates during damage call.");
                __instance.markEffects.Remove(effect);

                if (effect.EffectData.targetingData.specialRules == AbilityDef.SpecialRules.Aura)
                {
                    __instance.Combat.MessageCenter.PublishMessage(new AuraRemovedMessage(effect.creatorID, __instance.GUID, effect.id, effect.EffectData));
                }
                if (__instance.StealthPipsCurrent != __instance.StealthPipsPrevious)
                {
                    __instance.Combat.MessageCenter.PublishMessage(new StealthChangedMessage(__instance.GUID, __instance.StealthPipsCurrent));
                }

                __runOriginal = false;
            }
        }
    }
}
