using System;
using System.Collections.Generic;

namespace FieldRepairs {

    public static class ModStats {
        public const string TestStat = "IRFR_TestStat";

    }


    public class ModConfig {

        public class SkirmishOpts {
            /* A tag to apply to enemy units during skirmish matches. Can be one of the vanilla tags for now:
             * spawn_poorly_maintained_25
             * spawn_poorly_maintained_50
             * spawn_poorly_maintained_75
            */
            public string Tag = "spawn_poorly_maintained_50";
        }
        public SkirmishOpts Skirmish = new SkirmishOpts();

        public class StateOpts {
            public int Skew = 1;
            public int NumRollsDefault = 6;
            public int NumRolls25Effect = 5;
            public int NumRolls50Effect = 8;
            public int NumRolls75Effect = 11;
        }
        public StateOpts State = new StateOpts();

        public class ThemeOpts {
            public string[] MechWeights;
            public DamageType[] MechTable;

            public string[] VehicleWeights;
            public DamageType[] VehicleTable;

            public string[] TurretWeights;
            public DamageType[] TurretTable;
        }
        public ThemeOpts Patched = new ThemeOpts {
            MechWeights = new string[] { "Structure", "Structure", "Structure", "ArmComponent", "ArmComponent", "LegComponent", "LegComponent", "TorsoComponent", "AmmoBox", "Skill" },
            VehicleWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
            TurretWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
        };
        public ThemeOpts Exhausted = new ThemeOpts {
            MechWeights = new string[] { "Armor", "Armor", "Armor", "Structure", "AmmoBox", "AmmoBox", "AmmoBox", "HeatSink", "Skill", "Skill" },
            VehicleWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
            TurretWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
        };
        public ThemeOpts Mothballed = new ThemeOpts {
            MechWeights = new string[] { "ArmComponent", "LegComponent", "LegComponent", "TorsoComponent", "TorsoComponent", "HeadComponent", "Engine", "Engine", "Gyro", "Gyro" },
            VehicleWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
            TurretWeights = new string[] { "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor", "Armor" },
        };
        public ThemeOpts Scavenged = new ThemeOpts {
            MechWeights = new string[] { "Armor", "Structure", "ArmComponent", "LegComponent", "TorsoComponent", "HeadComponent", "AmmoBox", "HeatSink", "Engine", "Gyro" },
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

        public void LogConfig() {
            Mod.Log.Info("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info($"  DEBUG:{this.Debug} Trace:{this.Trace}");

            Mod.Log.Info("=== MOD CONFIG END ===");
        }

        public void Init() {
            WeightToDamageType(Patched);
            WeightToDamageType(Exhausted);
            WeightToDamageType(Mothballed);
            WeightToDamageType(Scavenged);
        }

        private void WeightToDamageType(ThemeOpts theme) {
            for (int i = 0; i < 10; i++) {
                string damageTypeId = theme.MechWeights[i];
                DamageType damageType = (DamageType)Enum.Parse(typeof(DamageType), damageTypeId);
                theme.MechTable[i] = damageType;

                // TODO: Add vehicle weights
                // TODO: Add turret weights
            }

        }

    }
}
