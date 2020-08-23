using BattleTech;

namespace FieldRepairs.Helper {

    public static class LocationHelper
    {
        // 0-1   == 1-2 = head -> 2
        // 2-21  == 3-22 = center torso -> 20
        // 22-37 == 23-38 = left torso -> 16
        // 38-53 == 39-54 = right torso -> 16
        // 54-63 == 55-64 = left arm -> 10
        // 64-73 == 65-74 = right arm -> 10
        // 74-86 == 75-87 = left leg -> 13
        // 87-99 == 88-100 = right leg -> 13

        public static ArmorLocation GetRandomMechArmorLocation()
        {
            ArmorLocation location = ArmorLocation.CenterTorso;

            bool isFront = Mod.Random.Next(0, 100) < 80;
            int locationIdx = Mod.Random.Next(0, 100);

            if (locationIdx <= 1) location = ArmorLocation.Head;
            else if (locationIdx <= 21) location = isFront ? ArmorLocation.CenterTorso : ArmorLocation.CenterTorsoRear;
            else if (locationIdx <= 37) location = isFront ? ArmorLocation.LeftTorso : ArmorLocation.LeftTorsoRear;
            else if (locationIdx <= 53) location = isFront ? ArmorLocation.RightTorso : ArmorLocation.RightTorsoRear;
            else if (locationIdx <= 63) location = ArmorLocation.LeftArm;
            else if (locationIdx <= 73) location = ArmorLocation.RightArm;
            else if (locationIdx <= 86) location = ArmorLocation.LeftLeg;
            else if (locationIdx <= 99) location = ArmorLocation.RightLeg;

            Mod.Log.Trace?.Write($" - Returning random location: {location}");
            return location;
        }

        public static ChassisLocations GetRandomMechStructureLocation()
        {
            ChassisLocations location = ChassisLocations.CenterTorso;

            int locationIdx = Mod.Random.Next(0, 100);

            if (locationIdx <= 1) location = ChassisLocations.Head;
            else if (locationIdx <= 21) location = ChassisLocations.CenterTorso;
            else if (locationIdx <= 37) location = ChassisLocations.LeftTorso;
            else if (locationIdx <= 53) location = ChassisLocations.RightTorso;
            else if (locationIdx <= 63) location = ChassisLocations.LeftArm;
            else if (locationIdx <= 73) location = ChassisLocations.RightArm;
            else if (locationIdx <= 86) location = ChassisLocations.LeftLeg;
            else if (locationIdx <= 99) location = ChassisLocations.RightLeg;

            Mod.Log.Trace?.Write($" - Returning random location: {location}");
            return location;
        }

        // 0-19  == 1-20 = left side  -> 20
        // 20-39 == 21-40 = right side -> 20 
        // 40-83 == 41-85 = front / rear -> 45
        // 84-99 == 85-100 = turret -> 15
        public static VehicleChassisLocations GetRandomVehicleLocation()
        {
            VehicleChassisLocations location = VehicleChassisLocations.Front;

            bool isFront = Mod.Random.Next(0, 100) < 80;
            int locationIdx = Mod.Random.Next(0, 100);

            if (locationIdx <= 19) location = VehicleChassisLocations.Left;
            else if (locationIdx <= 39) location = VehicleChassisLocations.Right;
            else if (locationIdx <= 83) location = isFront ? VehicleChassisLocations.Front : VehicleChassisLocations.Rear;
            else if (locationIdx <= 99) location = VehicleChassisLocations.Turret;

            Mod.Log.Trace?.Write($" - Returning random location: {location}");
            return location;
        }

    }

}
