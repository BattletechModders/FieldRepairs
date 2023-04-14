using System.Collections.Generic;
using static FieldRepairs.ModConfig;

namespace FieldRepairs
{

    public class TurretRepairState : RepairState
    {
        public readonly Turret Target;

        public readonly int ArmorHits;
        public readonly int StructureHits;
        public readonly int PilotSkillHits;

        public List<MechComponent> DamagedComponents = new List<MechComponent>();

        public TurretRepairState(PoorlyMaintainedEffect effect, Turret targetTurret) : base(effect, Mod.Config.DamageRollsConfig.TurretRolls)
        {
            this.Target = targetTurret;

            // See https://github.com/BattletechModders/MechEngineer/issues/181

            // Parse the list of components on the target. Determine the max engine hits, engine components, etc
            ComponentSummary compSummary = AnalyzeComponents(targetTurret);

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
                            Mod.Log.Debug?.Write($"  {i} is damageType: {damageType}.");
                            break;
                        case DamageType.HeatSink:
                            if (compSummary.HeatSinks.Count > 0)
                            {
                                MechComponent heatSink = compSummary.HeatSinks.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(heatSink);
                                compSummary.HeatSinks.Remove(heatSink);
                                isResolved = true;
                                Mod.Log.Debug?.Write($"  {i} is damageType: {damageType}.");
                            }
                            break;
                        case DamageType.AmmoBox:
                            if (compSummary.AmmoBoxes.Count > 0)
                            {
                                AmmunitionBox ammoBox = compSummary.AmmoBoxes.GetRandomElement<AmmunitionBox>();
                                DamagedComponents.Add(ammoBox);
                                compSummary.AmmoBoxes.Remove(ammoBox);
                                isResolved = true;
                                Mod.Log.Debug?.Write($"  {i} is damageType: {damageType}.");
                            }
                            break;
                        case DamageType.Component:
                            if (compSummary.Components.Count > 0)
                            {
                                MechComponent component = compSummary.Components.GetRandomElement<MechComponent>();
                                DamagedComponents.Add(component);
                                compSummary.Components.Remove(component);
                                isResolved = true;
                                Mod.Log.Debug?.Write($"  {i} is damageType: {damageType}.");
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
                                Mod.Log.Debug?.Write($"  {i} is damageType: {damageType}.");
                            }
                            break;
                        case DamageType.Structure:
                            StructureHits++;
                            isResolved = true;
                            Mod.Log.Debug?.Write($"  {i} is damageType: {damageType}.");
                            break;
                        case DamageType.Armor:
                            ArmorHits++;
                            isResolved = true;
                            Mod.Log.Debug?.Write($"  {i} is damageType: {damageType}.");
                            break;
                    }

                    killSwitch++;

                    if (killSwitch > 30)
                    {
                        Mod.Log.Info?.Write("Too many iterating, stopping and moving forward.");
                    }
                }
            }

        }

        private static ComponentSummary AnalyzeComponents(Turret targetTurret)
        {
            ComponentSummary compSummary = new ComponentSummary();
            foreach (MechComponent mc in targetTurret.allComponents)
            {
                Mod.Log.Debug?.Write($"Checking component: {mc.Name} / {mc.UIName} / {mc.Description.UIName}");

                bool isBlacklisted = false;
                foreach (string category in Mod.Config.CustomComponentCategories.Blacklisted)
                {
                    if (mc.componentDef.IsCategory(category)) isBlacklisted = true;
                }
                if (mc.componentDef.CriticalComponent)
                {
                    Mod.Log.Debug?.Write($"  - Skipping critical component: {mc.Description.UIName} in location: {(ChassisLocations)mc.Location}");
                }
                else if (isBlacklisted)
                {
                    Mod.Log.Debug?.Write($"  - Skipping blacklisted component: {mc.Description.UIName}");
                }
                else if (mc.componentType == ComponentType.AmmunitionBox)
                {
                    Mod.Log.Debug?.Write($"  - Found ammoBox: {mc.Description.UIName}");
                    compSummary.AmmoBoxes.Add((AmmunitionBox)mc);
                }
                else if (mc.componentType == ComponentType.HeatSink)
                {
                    Mod.Log.Debug?.Write($"  - Found heatSink: {mc.Description.UIName}");
                    compSummary.HeatSinks.Add(mc);
                }
                else if (mc.componentType == ComponentType.Weapon)
                {
                    Mod.Log.Debug?.Write($"  - Found weapon: {mc.Description.UIName}");
                    compSummary.Weapons.Add(mc);
                    // Check weapons for volatile? If we don't apply effects, do we care?                    
                    if (mc.componentDef.Is<ComponentExplosion>(out ComponentExplosion compExp))
                    {
                        Mod.Log.Debug?.Write($"      weapon has component explosion: {compExp.ExplosionDamage} / {compExp.HeatDamage} / {compExp.StabilityDamage}");
                    }
                }
                else
                {
                    Mod.Log.Debug?.Write($"  - Found component: {mc.Description.UIName} in location: {mc.Location}");
                    compSummary.Components.Add(mc);
                }
            }

            return compSummary;
        }

        private class ComponentSummary
        {
            // Only lists non-essential components
            public List<MechComponent> Components = new List<MechComponent>();

            public List<AmmunitionBox> AmmoBoxes = new List<AmmunitionBox>();

            public List<MechComponent> HeatSinks = new List<MechComponent>();

            public List<MechComponent> Weapons = new List<MechComponent>();
        }
    }
}
