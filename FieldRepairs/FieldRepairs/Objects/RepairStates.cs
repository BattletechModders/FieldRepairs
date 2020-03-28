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


    public class MechRepairState : RepairState {
        public readonly Mech Target;
        public readonly int ArmorHits;
        public readonly int StructureHits;

        public List<MechComponent> DamagedComponents = new List<MechComponent>();
        public MechComponent EngineComponent;
        public MechComponent Gyro;

        public readonly int EngineHits;
        public readonly int GyroHits;
        public readonly int PilotSkillHits;
        public MechRepairState(PoorlyMaintainedEffect effect, Mech targetMech) : base(effect)
        {
            this.Target = targetMech;

            // See https://github.com/BattletechModders/MechEngineer/issues/181

            // Parse the list of components on the target. Determine the max engine hits, engine components, etc
            AnalyzeComponents(targetMech);

            for (int i = 0; i < stateRolls; i++)
            {
                int randIdx = Mod.Random.Next(0, 9); // Number of indexes in the themeConfig
                ThemeConfig themeConfig = ModState.CurrentThemeConfig();
                DamageType damageType = themeConfig.MechTable[randIdx];
                Mod.Log.Debug($"  {i} is damageType: {damageType}.");

                bool isResolved = false;
                while (!isResolved)
                {
                    switch (damageType)
                    {
                        case DamageType.Skill:
                            break;
                        case DamageType.Gyro:
                            // Only accept 1 gyro hit, then fallback
                            if (GyroHits == 1)
                            {

                            }
                            break;
                        case DamageType.Engine:
                            // Only accept 2 engine hits, then fallback
                            if (EngineHits == 2)
                            {

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

        private static ComponentSummary AnalyzeComponents(Mech targetMech)
        {
            ComponentSummary compSummary = new ComponentSummary();
            foreach (MechComponent mc in targetMech.allComponents)
            {
                if (mc.componentDef.CriticalComponent)
                {
                    Mod.Log.Trace($"  Skipping critical component: {mc.Description.UIName} in location: {(ChassisLocations)mc.Location}");
                }
                else if (mc.componentType == ComponentType.AmmunitionBox)
                {
                    Mod.Log.Debug($"  - Found ammoBox {mc.Description.UIName}");
                    compSummary.AmmoBoxes.Add((AmmunitionBox)mc);
                }
                else if (mc.componentType == ComponentType.HeatSink)
                {
                    Mod.Log.Debug($"  - Found heatSink {mc.Description.UIName}");
                    compSummary.HeatSinks.Add(mc);
                }
                else
                {
                    switch ((ChassisLocations)mc.Location)
                    {
                        case ChassisLocations.Head:
                            compSummary.InHead.Add(mc);
                            break;
                        case ChassisLocations.LeftArm:
                            compSummary.InLeftArm.Add(mc);
                            break;
                        case ChassisLocations.LeftLeg:
                            compSummary.InLeftLeg.Add(mc);
                            break;
                        case ChassisLocations.LeftTorso:
                            compSummary.InLeftTorso.Add(mc);
                            break;
                        case ChassisLocations.RightArm:
                            compSummary.InRightArm.Add(mc);
                            break;
                        case ChassisLocations.RightLeg:
                            compSummary.InRightLeg.Add(mc);
                            break;
                        case ChassisLocations.RightTorso:
                            compSummary.InRightTorso.Add(mc);
                            break;
                        case ChassisLocations.CenterTorso:
                            compSummary.InCenterTorso.Add(mc);
                            break;
                        default:
                            Mod.Log.Debug($" WARN: Unexpected location: {mc.Location} for mech component. Skipping {mc.Description.UIName}");
                            break;
                    }
                }
            }

            return compSummary;
        }

        private class ComponentSummary
        {
            // Only lists non-essential components
            public List<MechComponent> InHead = new List<MechComponent>();
            public List<MechComponent> InLeftArm = new List<MechComponent>();
            public List<MechComponent> InLeftLeg = new List<MechComponent>();
            public List<MechComponent> InLeftTorso = new List<MechComponent>();
            public List<MechComponent> InRightArm = new List<MechComponent>();
            public List<MechComponent> InRightLeg = new List<MechComponent>();
            public List<MechComponent> InRightTorso = new List<MechComponent>();
            public List<MechComponent> InCenterTorso = new List<MechComponent>();

            public List<MechComponent> Engines = new List<MechComponent>();
            public List<MechComponent> Gyros = new List<MechComponent>();

            public List<AmmunitionBox> AmmoBoxes = new List<AmmunitionBox>();
            public List<MechComponent> HeatSinks = new List<MechComponent>();
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
