
using static FieldRepairs.ModConfig;

namespace FieldRepairs {

    public static class ModState {

        public static ThemeConfig CurrentTheme;

        public static void Reset() {
            // Reinitialize state

            CurrentTheme = null;

        }

    }

}


