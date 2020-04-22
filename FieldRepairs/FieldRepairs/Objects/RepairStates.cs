using BattleTech;
using static FieldRepairs.ModConfig;

namespace FieldRepairs {

    public abstract class RepairState {

        public int stateRolls = 2;
        public int effectRating = 0;

        public RepairState(PoorlyMaintainedEffect effect, UnitRollCfg rollCfg) {
            if (effect == null || effect.EffectData == null || 
                effect.EffectData.poorlyMaintainedEffectData != null ||
                rollCfg == null)
            {
                if (effect.EffectData.poorlyMaintainedEffectData.armorMod == 0.25f)
                {
                    effectRating = 25;
                    stateRolls = Mod.Random.Next(rollCfg.PM25_MinRolls, rollCfg.PM25_MaxRolls);
                    Mod.Log.Debug($"25% effect supplied, stateRolls = {stateRolls}");
                }
                else if (effect.EffectData.poorlyMaintainedEffectData.armorMod == 0.5f)
                {
                    effectRating = 50;
                    stateRolls = Mod.Random.Next(rollCfg.PM50_MinRolls, rollCfg.PM50_MaxRolls);
                    Mod.Log.Debug($"50% effect supplied, stateRolls = {stateRolls}");
                }
                else if (effect.EffectData.poorlyMaintainedEffectData.armorMod == 0.75f)
                {
                    effectRating = 75;
                    stateRolls = Mod.Random.Next(rollCfg.PM75_MinRolls, rollCfg.PM75_MaxRolls);
                    Mod.Log.Debug($"75% effect supplied, stateRolls = {stateRolls}");
                } 
                else
                {
                    Mod.Log.Debug($"Unknown effect, stateRolls = {stateRolls}");
                }
            }
            else
            {
                Mod.Log.Debug($"Unknown effect or config stateRolls = {stateRolls}");
            }
        }
    }

    public class BuildingRepairState : RepairState {
        public readonly Building Target;
        public BuildingRepairState(PoorlyMaintainedEffect effect, Building targetBuilding) : base(effect, null) {
            Target = targetBuilding;

            // Buildings only have structure
        }
    }
   
}
