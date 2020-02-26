using BattleTech;
using System;
using System.Collections.Generic;
using static FieldRepairs.ModConfig;

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
        public List<AmmunitionBox> AmmoBoxes = new List<AmmunitionBox>();
        public List<MechComponent> HeatSinks = new List<MechComponent>();
    }

    public class MechRepairState : RepairState {
        public readonly Mech Target;
        public readonly int ArmorHits;
        public readonly int StructureHits;

        public List<MechComponent> DamageComponents = new List<MechComponent>();        

        public readonly int EngineHits;
        public readonly int GyroHits;
        public readonly int PilotSkillHits;
        public MechRepairState(PoorlyMaintainedEffect effect, Mech targetMech, MechNonEssentialComponents nonEssentials) : base(effect) {
            this.Target = targetMech;

            for (int i = 0; i < stateRolls; i++) {
                int randIdx = Mod.Random.Next(0, 9);
                ThemeConfig themeConfig = ModState.CurrentThemeConfig();
                DamageType damageType = themeConfig.MechTable[randIdx];

                bool isResolved = false;
                while (!isResolved) {
                    switch (damageType) {
                        case DamageType.Skill:
                            break;
                        case DamageType.Gyro:
                            // Only accept 1 gyro hit, then fallback
                            if (GyroHits == 1) {
                                
                            }
                            break;
                        case DamageType.Engine:
                            // Only accept 2 engine hits, then fallback
                            if (EngineHits == 2) {

                            }
                            break;
                        case DamageType.HeatSink:
                            break;
                        case DamageType.AmmoBox:
                            break;
                        case DamageType.TorsoComponent:
                            break;
                        case DamageType.LegComponent:
                            break;
                        case DamageType.ArmComponent:
                            break;
                        case DamageType.Structure:
                            StructureHits++;
                            isResolved = true;
                            break;
                        case DamageType.Armor:
                            ArmorHits++;
                            isResolved = true;
                            break;
                    }

                    isResolved = true;
                }

            }

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
