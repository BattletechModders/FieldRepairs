using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldRepairs.State {

    public class RepairsHelper {
        public static BuildingRepairState CalculateRepairState(PoorlyMaintainedEffect effect, Building target) {
            return new BuildingRepairState(effect, target);
        }

        public static MechRepairState CalculateRepairState(PoorlyMaintainedEffect effect, Mech target) {
            MechRepairState mechRepairState = new MechRepairState(effect, target);


            return mechRepairState;
        }

        public static TurretRepairState CalculateRepairState(PoorlyMaintainedEffect effect, Turret target) {
            return new TurretRepairState(effect, target);
        }

        public static VehicleRepairState CalculateRepairState(PoorlyMaintainedEffect effect, Vehicle target) {
            return new VehicleRepairState(effect, target);
        }


    }
}
