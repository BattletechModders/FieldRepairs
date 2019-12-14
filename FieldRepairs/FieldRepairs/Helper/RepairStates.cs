using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldRepairs.Helper {

    public abstract class RepairState {

        // Represents armor and structure damage levels
        public readonly int ArmorDamage;
        // Represents component damage (ammo, weapons, etc)
        public readonly int ComponentDamage;

        public RepairState(PoorlyMaintainedEffect effect) {
            if (effect.EffectData.poorlyMaintainedEffectData != null) {
                ArmorDamage = (int)Math.Ceiling(effect.EffectData.poorlyMaintainedEffectData.armorMod * 5f);
                ComponentDamage = (int)Math.Ceiling(effect.EffectData.poorlyMaintainedEffectData.ammoMod * 10f);
            } else {
                Mod.Log.Warn($"Passed effect '{effect?.EffectData?.Description?.Name}' with no poorMaint data, won't damage target.");
            }
        }

        public abstract void CalculateDamage(int armorHits, int componentHits);
    }

    public class BuildingRepairState : RepairState {
        public readonly Building Target;
        public BuildingRepairState(PoorlyMaintainedEffect effect, Building targetBuilding) : base(effect) {
            Target = targetBuilding;

            // Buildings only have structure
        }

        public override void CalculateDamage(int armorHits, int componentHits) {
            throw new NotImplementedException();
        }
    }

    public class MechRepairState : RepairState {
        public readonly Mech Target;
        public MechRepairState(PoorlyMaintainedEffect effect, Mech targetMech) : base(effect) {
            this.Target = targetMech;

            // Mechs have: structure, armor, components, weapons, ammo, limbs
        }

        public override void CalculateDamage(int armorHits, int componentHits) {
            throw new NotImplementedException();
        }
    }

    public class TurretRepairState : RepairState {
        public readonly Turret Target;
        public TurretRepairState(PoorlyMaintainedEffect effect, Turret targetTurret) : base(effect) {
            this.Target = targetTurret;
            // Turrets have armor, structure, components, weapons, ammo
        }

        public override void CalculateDamage(int armorHits, int componentHits) {
            throw new NotImplementedException();
        }
    }

    public class VehicleRepairState : RepairState {
        public readonly Vehicle Target;
        public VehicleRepairState(PoorlyMaintainedEffect effect, Vehicle targetVehicle) : base(effect) {

        }

        public override void CalculateDamage(int armorHits, int componentHits) {
            throw new NotImplementedException();
        }
    }
}
