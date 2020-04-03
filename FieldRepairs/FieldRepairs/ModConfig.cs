using System;
using System.Collections.Generic;

namespace FieldRepairs {

    public static class ModStats {
        public const string TestStat = "IRFR_TestStat";

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
            public string[] MechWeights;
            public DamageType[] MechTable = new DamageType[10];
            public SortedSet<DamageType> MechFallbackTable;

            public string[] VehicleWeights;
            public DamageType[] VehicleTable;

            public string[] TurretWeights;
            public DamageType[] TurretTable;
        }
        public ThemeConfig Patched = new ThemeConfig {
            MechWeights = new string[] { "Armor", "Structure", "Structure", "Structure", "Component", "Component", "Weapon", "Weapon", "AmmoBox", "Skill" },
            VehicleWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
            TurretWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
        };
        public ThemeConfig Exhausted = new ThemeConfig {
            MechWeights = new string[] { "Armor", "Armor", "Armor", "Structure", "Weapon", "AmmoBox", "AmmoBox", "HeatSink", "Skill", "Skill" },
            VehicleWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
            TurretWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
        };
        public ThemeConfig Mothballed = new ThemeConfig {
            MechWeights = new string[] { "Structure", "Component", "Component", "Weapon", "Weapon", "Weapon", "Engine", "Engine", "Gyro", "Gyro" },
            VehicleWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
            TurretWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
        };
        public ThemeConfig Scavenged = new ThemeConfig {
            MechWeights = new string[] { "Armor", "Structure", "Component", "Component", "Weapon", "Weapon", "AmmoBox", "HeatSink", "Engine", "Gyro" },
            VehicleWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
            TurretWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
        };

        // If true, many logs will be printed
        public bool Debug = false;
        // If true, all logs will be printed
        public bool Trace = false;

        // The effect armorMod will be multiplied this number, then converted to an int. 0.3 armorMod * 20 => 6 rolls
        public float ArmorEffectToRollsMulti = 20f;

        // The effect ammoMod will be multiplied by this number, then converted to an int. 0.4 ammoMod * 10 => 4 rolls
        public float AmmoEffectToRollsMulti = 10f;

        public string GyroCCCategory = "Gyro";
        public string EnginePartCCCategory = "EnginePart";

        public float MinArmorLossPerHit = 0.2f;
        public float MaxArmorLossPerHit = 0.5f;
        public float MinStructureLossPerHit = 0.1f;
        public float MaxStructureLossPerHit = 0.3f;

        public int MinSkillPenaltyPerHit = 1;
        public int MaxSkillPenaltyPerHit = 3;

        public void LogConfig() {
            Mod.Log.Info("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info($"  DEBUG:{this.Debug} Trace:{this.Trace}");

            Mod.Log.Info("=== MOD CONFIG END ===");
        }

        public void Init() {
            Mod.Log.Debug(" == Initializing Configuration");
            Mod.Log.Debug(" -- Patched.");
            WeightToDamageType(Patched);

            Mod.Log.Debug(" -- Exhausted.");
            WeightToDamageType(Exhausted);

            Mod.Log.Debug(" -- Mothballed.");
            WeightToDamageType(Mothballed);

            Mod.Log.Debug(" -- Scavenged.");
            WeightToDamageType(Scavenged);

            Mod.Log.Debug(" == Configuration Initialized");
        }

        private void WeightToDamageType(ThemeConfig theme) {

            SortedSet<DamageType> fallback = new SortedSet<DamageType>();
            for (int i = 0; i < 10; i++) {
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
