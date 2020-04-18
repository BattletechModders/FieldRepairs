using BattleTech;

namespace FieldRepairs {

    public abstract class RepairState {

        public readonly int stateRolls;

        public RepairState(PoorlyMaintainedEffect effect) {
            if (effect.EffectData.poorlyMaintainedEffectData != null) {
                if (effect.EffectData.poorlyMaintainedEffectData.armorMod == 0.25f) {
                    stateRolls = Mod.Random.Next(Mod.Config.State.NumRolls25Effect - Mod.Config.State.Skew,
                        Mod.Config.State.NumRolls25Effect + Mod.Config.State.Skew);
                    Mod.Log.Debug($"25% effect supplied, stateRolls = {stateRolls}");
                } else if (effect.EffectData.poorlyMaintainedEffectData.armorMod == 0.5f) {
                    stateRolls = Mod.Random.Next(Mod.Config.State.NumRolls50Effect - Mod.Config.State.Skew,
                        Mod.Config.State.NumRolls50Effect + Mod.Config.State.Skew);
                    Mod.Log.Debug($"50% effect supplied, stateRolls = {stateRolls}");
                } else if (effect.EffectData.poorlyMaintainedEffectData.armorMod == 0.75f) {
                    stateRolls = Mod.Random.Next(Mod.Config.State.NumRolls75Effect - Mod.Config.State.Skew,
                        Mod.Config.State.NumRolls75Effect + Mod.Config.State.Skew);
                    Mod.Log.Debug($"75% effect supplied, stateRolls = {stateRolls}");
                } else {
                    stateRolls = Mod.Random.Next(Mod.Config.State.NumRollsDefault - Mod.Config.State.Skew,
                        Mod.Config.State.NumRollsDefault + Mod.Config.State.Skew);
                    Mod.Log.Debug($"Unidentified effect supplied, stateRolls = {stateRolls}");
                }
                
            }
        }
    }

    public class BuildingRepairState : RepairState {
        public readonly Building Target;
        public BuildingRepairState(PoorlyMaintainedEffect effect, Building targetBuilding) : base(effect) {
            Target = targetBuilding;

            // Buildings only have structure
        }
    }
   
}
