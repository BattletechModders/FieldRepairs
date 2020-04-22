using System;
using System.Collections.Generic;

namespace FieldRepairs {

    public static class ModStats 
    {
        public const string TestStat = "IRFR_TestStat";
    }

    public class ModConfig {

        public class SkirmishConfig 
        {
            /* A tag to apply to enemy units during skirmish matches. Can be one of the vanilla tags for now:
             * spawn_poorly_maintained_25
             * spawn_poorly_maintained_50
             * spawn_poorly_maintained_75
            */
            public string Tag = "spawn_poorly_maintained_50";
        }
        public SkirmishConfig Skirmish = new SkirmishConfig();

        public class UnitRollCfg
        {
            public int PM25_MinRolls = 1;
            public int PM25_MaxRolls = 4;

            public int PM50_MinRolls = 2;
            public int PM50_MaxRolls = 6;

            public int PM75_MinRolls = 3;
            public int PM75_MaxRolls = 8;
        }
        public class DamageRollCfg
        {
            public UnitRollCfg MechRolls = new UnitRollCfg()
            {
                PM25_MinRolls = 1,
                PM25_MaxRolls = 4,
                PM50_MinRolls = 2,
                PM50_MaxRolls = 6,
                PM75_MinRolls = 3,
                PM75_MaxRolls = 8
            };

            public UnitRollCfg VehicleRolls = new UnitRollCfg()
            {
                PM25_MinRolls = 1,
                PM25_MaxRolls = 4,
                PM50_MinRolls = 2,
                PM50_MaxRolls = 5,
                PM75_MinRolls = 3,
                PM75_MaxRolls = 6

            };

            public UnitRollCfg TurretRolls = new UnitRollCfg() {
                PM25_MinRolls = 1,
                PM25_MaxRolls = 3,
                PM50_MinRolls = 1,
                PM50_MaxRolls = 4,
                PM75_MinRolls = 2,
                PM75_MaxRolls = 5
            };
        }
        public DamageRollCfg DamageRollsConfig = new DamageRollCfg();

        public class ThemeConfig 
        {
            public const int MaxWeightItems = 10;

            public string Label;
            public override string ToString() { return Label; }

            public string[] MechWeights;
            public DamageType[] MechTable = new DamageType[MaxWeightItems];

            public string[] VehicleWeights;
            public DamageType[] VehicleTable = new DamageType[MaxWeightItems];

            public string[] TurretWeights;
            public DamageType[] TurretTable = new DamageType[MaxWeightItems];
        }

        public List<ThemeConfig> Themes = new List<ThemeConfig> { };

        public class CCCategories
        {
            public string Gyros = "Gyro";
            public string EngineParts = "EnginePart";
            public List<string> Blacklisted = new List<string> { "Armor", "Structure", "CASE", "PositiveQuirk", "Cockpit" };
        }
        public CCCategories CustomComponentCategories = new CCCategories();


        public class HitPenalties
        {
            public float MinArmorLoss = 0.2f;
            public float MaxArmorLoss = 0.5f;
            public float MinStructureLoss = 0.1f;
            public float MaxStructureLoss = 0.3f;

            public int MinSkillPenalty = 1;
            public int MaxSkillPenalty = 3;
        }
        public HitPenalties PerHitPenalties = new HitPenalties();

        // If true, many logs will be printed
        public bool Debug = false;
        // If true, all logs will be printed
        public bool Trace = false;

        // Localizations
        public const string LT_TT_DAMAGE_COMP = "COMP_DAMAGE";
        public const string LT_TT_DAMAGE_PILOT = "PILOT_DAMAGE";
        public const string LT_TT_DAMAGE_SKILL = "SKILL_DAMAGE";

        public const string LT_TT_SKILL_GUNNERY = "SKILL_GUNNERY";
        public const string LT_TT_SKILL_GUTS = "SKILL_GUTS";
        public const string LT_TT_SKILL_PILOTING = "SKILL_PILOTING";
        public const string LT_TT_SKILL_TACTICS = "SKILL_TACTICS";

        public const string LT_TT_PILOT_HEALTH = "PILOT_HEALTH";
        public const string LT_TT_PILOT_BONUS_HEALTH = "PILOT_BONUS_HEALTH";

        public Dictionary<string, string> LocalizedText = new Dictionary<string, string>()
        {
            { LT_TT_DAMAGE_COMP, "<color=#FF0000>COMPONENT DAMAGE</color>\n" },
            { LT_TT_DAMAGE_PILOT, "<color=#FF0000>HEALTH DAMAGE</color>\n" },
            { LT_TT_DAMAGE_SKILL, "<color=#FF0000>SKILL PENALTY</color>\n" },

            { LT_TT_SKILL_GUNNERY, " - Gunnery: -{0}\n" },
            { LT_TT_SKILL_GUTS, " - Guts: -{0}\n" },
            { LT_TT_SKILL_PILOTING, " - Piloting: -{0}\n" },
            { LT_TT_SKILL_TACTICS, " - Tactics: -{0}\n" },

            { LT_TT_PILOT_HEALTH, " - Health: -{0}\n" },
            { LT_TT_PILOT_BONUS_HEALTH, " - Bonus Health: -{0}\n" },
        };

        public void LogConfig() {
            Mod.Log.Info("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info($"  DEBUG:{this.Debug} Trace:{this.Trace}");
            
            Mod.Log.Info(" --- SKIRMISH ---");
            Mod.Log.Info($"  TAG: {this.Skirmish.Tag}");

            Mod.Log.Info(" --- DAMAGE ROLLS ---");
            Mod.Log.Info($"  MECH ROLLS: ");
            Mod.Log.Info($"    poorly_maintained_25 -> min: {this.DamageRollsConfig.MechRolls.PM25_MinRolls} / max: {this.DamageRollsConfig.MechRolls.PM25_MaxRolls}");
            Mod.Log.Info($"    poorly_maintained_50 -> min: {this.DamageRollsConfig.MechRolls.PM50_MinRolls} / max: {this.DamageRollsConfig.MechRolls.PM50_MaxRolls}");
            Mod.Log.Info($"    poorly_maintained_75 -> min: {this.DamageRollsConfig.MechRolls.PM75_MinRolls} / max: {this.DamageRollsConfig.MechRolls.PM75_MaxRolls}");
            Mod.Log.Info($"  VEHICLE ROLLS: ");
            Mod.Log.Info($"    poorly_maintained_25 -> min: {this.DamageRollsConfig.VehicleRolls.PM25_MinRolls} / max: {this.DamageRollsConfig.VehicleRolls.PM25_MaxRolls}");
            Mod.Log.Info($"    poorly_maintained_50 -> min: {this.DamageRollsConfig.VehicleRolls.PM50_MinRolls} / max: {this.DamageRollsConfig.VehicleRolls.PM50_MaxRolls}");
            Mod.Log.Info($"    poorly_maintained_75 -> min: {this.DamageRollsConfig.VehicleRolls.PM75_MinRolls} / max: {this.DamageRollsConfig.VehicleRolls.PM75_MaxRolls}");
            Mod.Log.Info($"  TURRET ROLLS: ");
            Mod.Log.Info($"    poorly_maintained_25 -> min: {this.DamageRollsConfig.TurretRolls.PM25_MinRolls} / max: {this.DamageRollsConfig.TurretRolls.PM25_MaxRolls}");
            Mod.Log.Info($"    poorly_maintained_50 -> min: {this.DamageRollsConfig.TurretRolls.PM50_MinRolls} / max: {this.DamageRollsConfig.TurretRolls.PM50_MaxRolls}");
            Mod.Log.Info($"    poorly_maintained_75 -> min: {this.DamageRollsConfig.TurretRolls.PM75_MinRolls} / max: {this.DamageRollsConfig.TurretRolls.PM75_MaxRolls}");

            Mod.Log.Info(" --- CUSTOM COMPONENTS CATEGORIES ---");
            Mod.Log.Info($"  Gyros: {this.CustomComponentCategories.Gyros}  " +
                $"EngineParts: {this.CustomComponentCategories.EngineParts}  " +
                $"Blacklisted: {String.Join(", ", this.CustomComponentCategories.Blacklisted)}");

            Mod.Log.Info(" --- PER HIT PENALTIES ---");
            Mod.Log.Info($"  ArmorLoss =>  min: {this.PerHitPenalties.MinArmorLoss} max: {this.PerHitPenalties.MaxArmorLoss }");
            Mod.Log.Info($"  StructureLoss =>  min: {this.PerHitPenalties.MinStructureLoss} max: {this.PerHitPenalties.MaxStructureLoss }");
            Mod.Log.Info($"  SkillPenalty =>  min: {this.PerHitPenalties.MinSkillPenalty} max: {this.PerHitPenalties.MaxSkillPenalty}");

            Mod.Log.Info(" --- THEMES ---");
            foreach (ThemeConfig theme in this.Themes)
            {
                Mod.Log.Info($"  THEME: {theme.Label}");
                Mod.Log.Info($"    MECH WEIGHTS: {String.Join(", ", theme.MechWeights)}");
                Mod.Log.Info($"    VEHICLE WEIGHTS: {String.Join(", ", theme.VehicleWeights)}");
                Mod.Log.Info($"    TURRET WEIGHTS: {String.Join(", ", theme.TurretWeights)}");
                Mod.Log.Info($" ");
            }

            Mod.Log.Info("=== MOD CONFIG END ===");
        }

        public void Init() {
            Mod.Log.Debug(" == Initializing Configuration");

            foreach (ThemeConfig themeConfig in Themes)
            {
                Mod.Log.Debug($" -- {themeConfig.Label} ");
                WeightToDamageType(themeConfig);
            }

            Mod.Log.Debug(" == Configuration Initialized");
        }

        // Translate the strings in the config to enum types
        private void WeightToDamageType(ThemeConfig theme) {

            for (int i = 0; i < ThemeConfig.MaxWeightItems; i++) {
                string mechDamageTypeId = theme.MechWeights[i];
                DamageType mechDamageType = (DamageType)Enum.Parse(typeof(DamageType), mechDamageTypeId);
                theme.MechTable[i] = mechDamageType;

                string vehicleDamageTypeId = theme.VehicleWeights[i];
                DamageType vehicleDamageType = (DamageType)Enum.Parse(typeof(DamageType), vehicleDamageTypeId);
                theme.VehicleTable[i] = vehicleDamageType;

                string turretDamageTypeId = theme.TurretWeights[i];
                DamageType turretDamageType = (DamageType)Enum.Parse(typeof(DamageType), turretDamageTypeId);
                theme.TurretTable[i] = turretDamageType;
            }

        }

    }
}
