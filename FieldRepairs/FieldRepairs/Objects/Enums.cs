using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldRepairs {
    public enum StateTheme {
        Patched,
        Mothballed,
        Exhausted,
        Scavenged
    }

    public static class StateThemeExtensions
    {
        public static string Title(this StateTheme theme)
        {
            switch (theme)
            {
                case StateTheme.Patched:
                    return "PATCHED";
                case StateTheme.Mothballed:
                    return "MOTH-BALLED";
                case StateTheme.Exhausted:
                    return "EXHAUSTED";
                case StateTheme.Scavenged:
                    return "SCAVENGED";
                default:
                    return "UNKNOWN";
            }
        }
    }

    public enum DamageType {
        Armor,
        Structure,
        Component,
        Weapon,
        AmmoBox,
        HeatSink,
        Engine,
        Gyro,
        Skill
    }
}
