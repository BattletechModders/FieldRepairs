
using static FieldRepairs.ModConfig;

namespace FieldRepairs {

    public static class ModState {

        public static StateTheme CurrentTheme = StateTheme.Patched;

        public static void Reset() {
            // Reinitialize state

            CurrentTheme = StateTheme.Patched;

        }

        public static ThemeConfig CurrentThemeConfig() {
            if (CurrentTheme == StateTheme.Patched) {
                return Mod.Config.Patched;
            } else if (CurrentTheme == StateTheme.Exhausted) {
                return Mod.Config.Exhausted;
            } else if (CurrentTheme == StateTheme.Mothballed) {
                return Mod.Config.Mothballed;
            } else {
                return Mod.Config.Scavenged;
            }
        }
    }

}


