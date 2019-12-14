using System.Collections.Generic;

namespace FieldRepairs {

    public static class ModStats {
        public const string TestStat = "IRFR_TestStat";

    }

    public class ModConfig {

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
    }
}
