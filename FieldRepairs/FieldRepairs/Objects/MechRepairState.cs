using BattleTech;
using CustomComponents;
using MechEngineer.Features.ComponentExplosions;
using MechEngineer.Features.CriticalEffects;
using System.Collections.Generic;
using static FieldRepairs.ModConfig;

namespace FieldRepairs {

    public class MechRepairState : RepairState {
        public readonly Mech Target;

        public readonly int ArmorHits;
        public readonly int StructureHits;
        public readonly int PilotSkillHits;

        public List<MechComponent> DamagedComponents = new List<MechComponent>();

        public MechRepairState(PoorlyMaintainedEffect effect, Mech targetMech) : base(effect, Mod.Config.DamageRollsConfig.MechRolls)
        {
            this.Target = targetMech;

            // See https://github.com/BattletechModders/MechEngineer/issues/181

            // Parse the list of components on the target. Determine the max engine hits, engine components, etc
            ComponentSummary compSummary = AnalyzeComponents(targetMech);

            int engineHits = 0;
            int gyroHits = 0;


            for (int i = 0; i < stateRolls; i++)
            {
                int killSwitch = 0;
                bool isResolved = false;
                while (!isResolved)
                {
                    int randIdx = Mod.Random.Next(0, 9); // Number of indexes in the themeConfig
                    ThemeConfig themeConfig = ModState.CurrentTheme;
                    DamageType damageType = themeConfig.MechTable[randIdx];

                    switch (damageType)
                    {
                        case DamageType.Skill:
                            PilotSkillHits++;
                            isResolved = true;
                            Mod.Log.Debug($"  {i} is damageType: {damageType}.");
                            break;
                        case DamageType.Gyro:
                            // Only accept 1 gyro hit, then fallback
                            if (compSummary.GyroParts.Count > 0 && gyroHits < compSummary.MaxGyroHits - 1)
                            {
                                MechComponent gyroComp = compSummary.GyroParts.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(gyroComp);
                                compSummary.GyroParts.Remove(gyroComp);
                                gyroHits++;
                                isResolved = true;
                                Mod.Log.Debug($"  {i} is damageType: {damageType}.");
                            }
                            break;
                        case DamageType.Engine:
                            // Only accept 2 engine hits, then fallback
                            if (compSummary.EngineParts.Count > 0 && engineHits < compSummary.MaxEngineHits - 1)
                            {
                                MechComponent engineComp = compSummary.EngineParts.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(engineComp);
                                compSummary.EngineParts.Remove(engineComp);
                                engineHits++;
                                isResolved = true;
                                Mod.Log.Debug($"  {i} is damageType: {damageType}.");
                            }
                            break;
                        case DamageType.HeatSink:
                            if (compSummary.HeatSinks.Count > 0)
                            {
                                MechComponent heatSink = compSummary.HeatSinks.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(heatSink);
                                compSummary.HeatSinks.Remove(heatSink);
                                isResolved = true;
                                Mod.Log.Debug($"  {i} is damageType: {damageType}.");
                            }
                            break;
                        case DamageType.AmmoBox:
                            if (compSummary.AmmoBoxes.Count > 0)
                            {
                                AmmunitionBox ammoBox = compSummary.AmmoBoxes.GetRandomElement<AmmunitionBox>();
                                DamagedComponents.Add(ammoBox);
                                compSummary.AmmoBoxes.Remove(ammoBox);
                                isResolved = true;
                                Mod.Log.Debug($"  {i} is damageType: {damageType}.");
                            }
                            break;
                        case DamageType.Component:
                            if (compSummary.Components.Count > 0)
                            {
                                MechComponent component = compSummary.Components.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(component);
                                compSummary.Components.Remove(component);
                                isResolved = true;
                                Mod.Log.Debug($"  {i} is damageType: {damageType}.");
                            }
                            break;
                        case DamageType.Weapon:
                            // Always leave at least one weapon 
                            if (compSummary.Weapons.Count > 1)
                            {
                                MechComponent weapon = compSummary.Weapons.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(weapon);
                                compSummary.Weapons.Remove(weapon);
                                isResolved = true;
                                Mod.Log.Debug($"  {i} is damageType: {damageType}.");
                            }
                            break;
                        case DamageType.Structure:
                            StructureHits++;
                            isResolved = true;
                            Mod.Log.Debug($"  {i} is damageType: {damageType}.");
                            break;
                        case DamageType.Armor:
                            ArmorHits++;
                            isResolved = true;
                            Mod.Log.Debug($"  {i} is damageType: {damageType}.");
                            break;
                    }

                    killSwitch++;

                    if (killSwitch > 30)
                    {
                        Mod.Log.Info("Too many iterating, stopping and moving forward.");
                    }
                }
            }

        }

        private static ComponentSummary AnalyzeComponents(Mech targetMech)
        {
            ComponentSummary compSummary = new ComponentSummary();
            foreach (MechComponent mc in targetMech.allComponents)
            {
                Mod.Log.Debug($"Checking component: {mc.Name} / {mc.UIName} / {mc.Description.UIName}");


                bool isBlacklisted = false;
                foreach (string category in Mod.Config.CustomComponentCategories.Blacklisted)
                {
                    if (mc.componentDef.IsCategory(category)) isBlacklisted = true;
                }

                if (mc.componentDef.CriticalComponent)
                {
                    Mod.Log.Debug($"  - Skipping critical component: {mc.Description.UIName} in location: {(ChassisLocations)mc.Location}");
                }
                else if (isBlacklisted)
                {
                    Mod.Log.Debug($"  - Skipping blacklisted component: {mc.Description.UIName}");
                }
                else if (mc.componentDef.IsCategory(Mod.Config.CustomComponentCategories.Gyros))
                {
                    Mod.Log.Debug($"  - Found gyro: {mc.Description.UIName}");
                    compSummary.GyroParts.Add(mc);
                    if (mc.componentDef.Is<CriticalEffects>(out CriticalEffects meCritEffects) && 
                        meCritEffects.MaxHits > compSummary.MaxGyroHits)
                    {
                        compSummary.MaxGyroHits = meCritEffects.MaxHits;
                        Mod.Log.Debug($"      gyro has maxhits: {compSummary.MaxGyroHits}");
                    }
                }
                else if (mc.componentDef.IsCategory(Mod.Config.CustomComponentCategories.EngineParts)) 
                {
                    Mod.Log.Debug($"  - Found engine: {mc.Description.UIName}");
                    compSummary.EngineParts.Add(mc);
                    if (mc.componentDef.Is<MechCriticalEffects>(out MechCriticalEffects meCritEffects) && 
                        meCritEffects.MaxHits > compSummary.MaxEngineHits)
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
                    // Check weapons for volatile? If we don't apply effects, do we care?                    
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
}
