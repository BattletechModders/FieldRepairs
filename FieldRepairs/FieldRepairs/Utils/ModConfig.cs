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

        public void LogConfig() {
            Mod.Log.Info("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info($"  DEBUG:{this.Debug} Trace:{this.Trace}");

            Mod.Log.Info("=== MOD CONFIG END ===");
        }
    }
}
