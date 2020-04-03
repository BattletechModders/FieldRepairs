using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldRepairs.Helper {

    public static class LocationHelper
    {
        public static ArmorLocation GetMechArmorLocation()
        {
            ArmorLocation location = ArmorLocation.CenterTorso;

            bool isFront = Mod.Random.Next(0, 100) < 80;
            int locationIdx = Mod.Random.Next(0, 100);
            // 0-1   == 1-2 = head -> 2
            // 2-21  == 3-22 = center torso -> 20
            // 22-37 == 23-38 = left torso -> 16
            // 38-53 == 39-54 = right torso -> 16
            // 54-63 == 55-64 = left arm -> 10
            // 64-73 == 65-74 = right arm -> 10
            // 74-86 == 75-87 = left leg -> 13
            // 87-99 == 88-100 = right leg -> 13
            if (locationIdx <= 1) location = ArmorLocation.Head;
            else if (locationIdx <= 21) location = isFront ? ArmorLocation.CenterTorso : ArmorLocation.CenterTorsoRear;
            else if (locationIdx <= 37) location = isFront ? ArmorLocation.LeftTorso : ArmorLocation.LeftTorsoRear;
            else if (locationIdx <= 53) location = isFront ? ArmorLocation.RightTorso : ArmorLocation.RightTorsoRear;
            else if (locationIdx <= 63) location = ArmorLocation.LeftArm;
            else if (locationIdx <= 73) location = ArmorLocation.RightArm;
            else if (locationIdx <= 86) location = ArmorLocation.LeftLeg;
            else if (locationIdx <= 99) location = ArmorLocation.RightLeg;

            Mod.Log.Debug($" - Returning locaiton: {location}");
            return location;
        }

    }


}
