using System;
using System.Collections.Generic;

namespace FieldRepairs {

    public static class ModStats {
        public const string TestStat = "IRFR_TestStat";
        public const string CU_Pseudo_Vehicle = "fake_vehicle_chassis"; // Units with this chassisTag are pseudo-mechs representing vehicles

    }

    public class ModConfig {

        public class SkirmishConfig {
            /* A tag to apply to enemy units during skirmish matches. Can be one of the vanilla tags for now:
             * spawn_poorly_maintained_25
             * spawn_poorly_maintained_50
             * spawn_poorly_maintained_75
            */
            public string Tag = "spawn_poorly_maintained_50";
        }
        public SkirmishConfig Skirmish = new SkirmishConfig();

        public class StateConfig {
            public int Skew = 1;
            public int NumRollsDefault = 6;
            public int NumRolls25Effect = 5;
            public int NumRolls50Effect = 8;
            public int NumRolls75Effect = 11;
        }
        public StateConfig State = new StateConfig();

        public class ThemeConfig {
            public const int MaxWeightItems = 10;

            public string Label;

            public string[] MechWeights;
            public DamageType[] MechTable = new DamageType[10];
            public SortedSet<DamageType> MechFallbackTable;

            public string[] VehicleWeights;
            public DamageType[] VehicleTable;

            public string[] TurretWeights;
            public DamageType[] TurretTable;

            public override string ToString() { return Label; }
        }

        public List<ThemeConfig> Themes = new List<ThemeConfig>
        {
            new ThemeConfig
            {
                Label = "Patched",
                MechWeights = new string[] { "Armor", "Structure", "Structure", "Structure", "Component", "Component", "Weapon", "Weapon", "AmmoBox", "Skill" },
                VehicleWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
                TurretWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
            },
            new ThemeConfig
            {
                Label = "Exhausted",
                MechWeights = new string[] { "Armor", "Armor", "Armor", "Structure", "Weapon", "AmmoBox", "AmmoBox", "HeatSink", "Skill", "Skill" },
                VehicleWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
                TurretWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
            },
            new ThemeConfig
            {
                Label = "Mothballed",
                MechWeights = new string[] { "Structure", "Component", "Component", "Weapon", "Weapon", "Weapon", "Engine", "Engine", "Gyro", "Gyro" },
                VehicleWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
                TurretWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
            },
            new ThemeConfig
            {
                Label = "Scavenged",
                MechWeights = new string[] { "Armor", "Structure", "Component", "Component", "Weapon", "Weapon", "AmmoBox", "HeatSink", "Engine", "Gyro" },
                VehicleWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
                TurretWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
            },

        };

        public class CCCategories
        {
            public string Gyros = "Gyro";
            public string EngineParts = "EnginePart";
            public List<string> Blacklisted = new List<string> { "Armor", "Structure", "CASE", "PositiveQuirk", "Cockpit" };
        }
        public CCCategories CustomComponentCategories = new CCCategories();

        // If true, many logs will be printed
        public bool Debug = false;
        // If true, all logs will be printed
        public bool Trace = false;

        public float MinArmorLossPerHit = 0.2f;
        public float MaxArmorLossPerHit = 0.5f;
        public float MinStructureLossPerHit = 0.1f;
        public float MaxStructureLossPerHit = 0.3f;

        public int MinSkillPenaltyPerHit = 1;
        public int MaxSkillPenaltyPerHit = 3;

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

        private void WeightToDamageType(ThemeConfig theme) {

            SortedSet<DamageType> fallback = new SortedSet<DamageType>();
            for (int i = 0; i < ThemeConfig.MaxWeightItems; i++) {
                string damageTypeId = theme.MechWeights[i];
                DamageType damageType = (DamageType)Enum.Parse(typeof(DamageType), damageTypeId);
                theme.MechTable[i] = damageType;
                fallback.Add(damageType);

                // TODO: Add vehicle weights
                // TODO: Add turret weights
            }

            theme.MechFallbackTable = new SortedSet<DamageType>(fallback.Reverse());

        }

    }
}
