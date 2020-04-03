using BattleTech;
using CustomComponents;
using MechEngineer.Features.ComponentExplosions;
using MechEngineer.Features.CriticalEffects;
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
        public readonly int PilotSkillHits;

        public List<MechComponent> DamagedComponents = new List<MechComponent>();

        public MechRepairState(PoorlyMaintainedEffect effect, Mech targetMech) : base(effect)
        {
            this.Target = targetMech;

            // See https://github.com/BattletechModders/MechEngineer/issues/181

            // Parse the list of components on the target. Determine the max engine hits, engine components, etc
            ComponentSummary compSummary = AnalyzeComponents(targetMech);

            int engineHits = 0;
            int gyroHits = 0;

            int killSwitch = 0;
            for (int i = 0; i < stateRolls; i++)
            {
                bool isResolved = false;
                while (!isResolved)
                {
                    int randIdx = Mod.Random.Next(0, 9); // Number of indexes in the themeConfig
                    ThemeConfig themeConfig = ModState.CurrentThemeConfig();
                    DamageType damageType = themeConfig.MechTable[randIdx];
                    Mod.Log.Debug($"  {i} is damageType: {damageType}.");

                    switch (damageType)
                    {
                        case DamageType.Skill:
                            PilotSkillHits++;
                            isResolved = true;
                            break;
                        case DamageType.Gyro:
                            // Only accept 1 gyro hit, then fallback
                            if (gyroHits < compSummary.MaxGyroHits)
                            {
                                MechComponent gyroComp = compSummary.GyroParts.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(gyroComp);
                                compSummary.GyroParts.Remove(gyroComp);
                                gyroHits++;
                                isResolved = true;
                            }
                            break;
                        case DamageType.Engine:
                            // Only accept 2 engine hits, then fallback
                            if (engineHits < compSummary.MaxEngineHits)
                            {
                                MechComponent engineComp = compSummary.EngineParts.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(engineComp);
                                compSummary.EngineParts.Remove(engineComp);
                                engineHits++;
                                isResolved = true;
                            }
                            break;
                        case DamageType.HeatSink:
                            if (compSummary.HeatSinks.Count > 1)
                            {
                                MechComponent heatSink = compSummary.HeatSinks.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(heatSink);
                                compSummary.HeatSinks.Remove(heatSink);
                                isResolved = true;
                            }
                            break;
                        case DamageType.AmmoBox:
                            if (compSummary.AmmoBoxes.Count > 1)
                            {
                                AmmunitionBox ammoBox = compSummary.AmmoBoxes.GetRandomElement<AmmunitionBox>();
                                DamagedComponents.Add(ammoBox);
                                compSummary.AmmoBoxes.Remove(ammoBox);
                                isResolved = true;
                            }
                            break;
                        case DamageType.Component:
                            if (compSummary.Components.Count > 1)
                            {
                                MechComponent component = compSummary.Components.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(component);
                                compSummary.Components.Remove(component);
                                isResolved = true;
                            }
                            break;
                        case DamageType.Weapon:
                            if (compSummary.Weapons.Count > 1)
                            {
                                MechComponent weapon = compSummary.Weapons.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(weapon);
                                compSummary.Weapons.Remove(weapon);
                                isResolved = true;
                            }
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

                }
                killSwitch++;

                if (killSwitch > 100)
                {
                    Mod.Log.Info("Too many iterating, stopping and moving forward.");
                }


            }

        }

        private static ComponentSummary AnalyzeComponents(Mech targetMech)
        {
            ComponentSummary compSummary = new ComponentSummary();
            foreach (MechComponent mc in targetMech.allComponents)
            {
                Mod.Log.Debug($"  Checking component: {mc.Name} / {mc.UIName} / {mc.Description.UIName}");

                if (mc.mechComponentRef.Is<Category>(out Category ccCategory)) { }
                if (mc.mechComponentRef.Is<CriticalEffects>(out CriticalEffects meCritEffects)) { }

                if (mc.componentDef.CriticalComponent)
                {
                    Mod.Log.Debug($"  Skipping critical component: {mc.Description.UIName} in location: {(ChassisLocations)mc.Location}");
                }
                else if (mc.componentDef.IsCategory("Gyro"))
                {
                    // TODO: Make mod option
                    Mod.Log.Debug($"  - Found gyro: {mc.Description.UIName}");
                    compSummary.GyroParts.Add(mc);
                    if (meCritEffects != null && meCritEffects.MaxHits > compSummary.MaxGyroHits)
                    {
                        compSummary.MaxGyroHits = meCritEffects.MaxHits;
                        Mod.Log.Debug($"      gyro has maxhits: {compSummary.MaxGyroHits}");
                    }
                }
                else if (mc.componentDef.IsCategory("EnginePart")) 
                {
                    // TODO: Make mod option
                    Mod.Log.Debug($"  - Found engine: {mc.Description.UIName}");
                    compSummary.EngineParts.Add(mc);
                    if (meCritEffects != null && meCritEffects.MaxHits > compSummary.MaxEngineHits)
                    {
                        compSummary.MaxEngineHits = meCritEffects.MaxHits;
                        Mod.Log.Debug($"      engine has maxhits: {compSummary.MaxEngineHits}");
                    }
                }
                else if (mc.componentType == ComponentType.AmmunitionBox)
                {
                    Mod.Log.Debug($"  - Found ammoBox: {mc.Description.UIName}");
                    compSummary.AmmoBoxes.Add((AmmunitionBox)mc);
                }
                else if (mc.componentType == ComponentType.HeatSink)
                {
                    Mod.Log.Debug($"  - Found heatSink: {mc.Description.UIName}");
                    compSummary.HeatSinks.Add(mc);
                }
                else if (mc.componentType == ComponentType.Weapon)
                {
                    Mod.Log.Debug($"  - Found weapon: {mc.Description.UIName}");
                    compSummary.Weapons.Add(mc);
                    // Check weapons for volatile?
                    if (meCritEffects != null)
                    {
                        Mod.Log.Debug($"      weapon has maxhits: {meCritEffects.MaxHits}");
                    }
                    if (mc.componentDef.Is<ComponentExplosion>(out ComponentExplosion compExp))
                    {
                        Mod.Log.Debug($"      weapon has component explosion: {compExp.ExplosionDamage} / {compExp.HeatDamage} / {compExp.StabilityDamage}");                        
                    }
                }
                else
                {
                    Mod.Log.Debug($"  - Found component: {mc.Description.UIName} in location: {mc.Location}");
                    compSummary.Components.Add(mc);
                }
            }

            return compSummary;
        }

        private class ComponentSummary
        {
            // Only lists non-essential components
            public List<MechComponent> Components = new List<MechComponent>();

            public List<MechComponent> EngineParts = new List<MechComponent>();
            public int MaxEngineHits = 0;

            public List<MechComponent> GyroParts = new List<MechComponent>();
            public int MaxGyroHits = 0;

            public List<AmmunitionBox> AmmoBoxes = new List<AmmunitionBox>();

            public List<MechComponent> HeatSinks = new List<MechComponent>();

            public List<MechComponent> Weapons = new List<MechComponent>();
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
