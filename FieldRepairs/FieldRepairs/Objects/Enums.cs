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

    public enum DamageType {
        Armor,
        Structure,
        ArmComponent,
        LegComponent,
        TorsoComponent,
        HeadComponent,
        AmmoBox,
        HeatSink,
        Engine,
        Gyro,
        Skill
    }
}
