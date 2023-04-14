using Localize;
using System;

namespace FieldRepairs.Patches
{

    // This patch is necessary to prevent some spam in the cleaned_output_log. The game applies
    //   a ShowActorInfoSequence when a floatie is generated, which happens from both ME and vanilla's
    //   implementation of DamageComponent. When we damage components ahead of the camera being available,
    //   this causes many NREs to spam the logs. Instead, when we're applying the PoorlyMaintainedEffect
    //   we suppress the effect. 
    [HarmonyPatch(typeof(ShowActorInfoSequence), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(ICombatant), typeof(Text), typeof(FloatieMessage.MessageNature), typeof(bool) })]
    static class ShowActorInfoSequencePatches_ctor
    {
        static void Prefix(ref bool __runOriginal, ICombatant combatant, ref bool useCamera)
        {
            if (!__runOriginal) return;

            Mod.Log.Trace?.Write("SAIS:ctor - entered.");

            if (ModState.SuppressShowActorSequences)
            {
                Mod.Log.Trace?.Write("Suppressing floaties by forcing camera to false.");
                useCamera = false;
            }
        }

        static void Postfix(ShowActorInfoSequence __instance)
        {
            if (ModState.SuppressShowActorSequences)
            {
                Mod.Log.Trace?.Write("Suppressing floaties by forcing state to finished.");
                __instance.state = ShowActorInfoSequence.SequenceState.Finished;
            }
        }
    }
}
