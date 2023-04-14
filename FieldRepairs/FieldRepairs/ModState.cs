
using static FieldRepairs.ModConfig;

namespace FieldRepairs
{

    public static class ModState
    {

        public static ThemeConfig CurrentTheme;
        public static bool SuppressShowActorSequences = false;

        public static void Reset()
        {
            // Reinitialize state

            CurrentTheme = null;
            SuppressShowActorSequences = false;

        }

    }

}


