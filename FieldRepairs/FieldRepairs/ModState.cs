
using FieldRepairs.State;

namespace FieldRepairs {

    public static class ModState {

        public static StateTheme CurrentTheme = StateTheme.Patched;

        public static void Reset() {
            // Reinitialize state

            CurrentTheme = StateTheme.Patched;

        }
    }

}


