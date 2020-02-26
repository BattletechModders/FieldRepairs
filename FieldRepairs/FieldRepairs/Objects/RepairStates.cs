using BattleTech;
using System;
using System.Collections.Generic;

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

    public class MechNonEssentialComponents {
        // Only lists non-essential components
        public List<MechComponent> HeadComponents = new List<MechComponent>();
        public List<MechComponent> LeftArmComponents = new List<MechComponent>();
        public List<MechComponent> LeftLegComponents = new List<MechComponent>();
        public List<MechComponent> LeftTorsoComponents = new List<MechComponent>();
        public List<MechComponent> RightArmComponents = new List<MechComponent>();
        public List<MechComponent> RightLegComponents = new List<MechComponent>();
        public List<MechComponent> RightTorsoComponents = new List<MechComponent>();
        public List<MechComponent> CenterTorsoComponents = new List<MechComponent>();
    }

    public class MechRepairState : RepairState {
        public readonly Mech Target;
        public readonly int ArmorHits;
        public readonly int StructureHits;
        
        public readonly int ArmCompHits;
        
        public readonly int LegCompHits;
        
        public readonly int TorsoCompHits;
        public readonly int HeadCompHits;

        public readonly int AmmoBoxHits;
        public readonly int HeatSinkHits;
        public readonly int EngineHits;
        public readonly int GyroHits;
        public readonly int PilotSkillHits;
        public MechRepairState(PoorlyMaintainedEffect effect, Mech targetMech) : base(effect) {
            this.Target = targetMech;

            foreach (MechComponent mc in targetMech.allComponents) {

            }

            for (int i = 0; i < stateRolls; i++) {
                int randIdx = Mod.Random.Next(0, 9);
            }

            // Mechs have: structure, armor, components, weapons, ammo, limbs
        }
    }

    public class TurretRepairState : RepairState {
        public readonly Turret Target;
        public TurretRepairState(PoorlyMaintainedEffect effect, Turret targetTurret) : base(effect) {
            this.Target = targetTurret;
            // Turrets have armor, structure, components, weapons, ammo
        }

    }

    public class VehicleRepairState : RepairState {
        public readonly Vehicle Target;
        public VehicleRepairState(PoorlyMaintainedEffect effect, Vehicle targetVehicle) : base(effect) {
            this.Target = targetVehicle;
        }

    }
}
