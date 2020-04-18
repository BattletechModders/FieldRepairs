using BattleTech;
using Localize;
using System.Text;
using us.frostraptor.modUtils;

namespace FieldRepairs.Helper
{
    public static class PilotHelper
    {
        public static void ApplyPilotHealthDamage(AbstractActor target, WeaponHitInfo hitInfo, int headHits, out string tooltipText)
        {
            StringBuilder pilotDamageSB = new StringBuilder();

            if (target == null || !target.IsPilotable || headHits == 0)
            {
                tooltipText = null;
                return;
            }

            int healthDamage = headHits;
            if (target.GetPilot().BonusHealth > 0)
            {
                int absorbedDamage;
                if (target.GetPilot().BonusHealth >= healthDamage)
                {
                    absorbedDamage = healthDamage;
                    healthDamage = 0;
                }
                else
                {
                    absorbedDamage = target.GetPilot().BonusHealth;
                    healthDamage = healthDamage - target.GetPilot().BonusHealth;
                }

                Mod.Log.Debug($"Bonus health aborbs: {absorbedDamage} leaving: {healthDamage} healthDamage.");
                target.GetPilot().StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID,
                    "BonusHealth", StatCollection.StatOperation.Int_Subtract, absorbedDamage, -1, true);
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_PILOT_BONUS_HEALTH], new object[] { absorbedDamage });
                pilotDamageSB.Append(localText.ToString());
            }

            if (healthDamage > (target.GetPilot().Health - 1))
            {
                Mod.Log.Debug($"Health damage: {healthDamage} would kill pilot, reducing to maxHealth: {target.GetPilot().Health} - 1");
                healthDamage = target.GetPilot().Health - 1;
            }

            if (healthDamage > 0)
            {
                Mod.Log.Debug($"Adding {healthDamage} to {CombatantUtils.Label(target)}");
                target.GetPilot().StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID,
                    "Injuries", StatCollection.StatOperation.Int_Add, healthDamage, -1, true);
                Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_PILOT_HEALTH], new object[] { healthDamage });
                pilotDamageSB.Append(localText.ToString());
            }

            tooltipText = pilotDamageSB.ToString();
        }

        public static void ApplyPilotSkillDamage(AbstractActor target, WeaponHitInfo hitInfo, int skillDamageHits, out string tooltipText)
        {
            StringBuilder pilotSkillSB = new StringBuilder();

            if (target == null || !target.IsPilotable || skillDamageHits == 0)
            {
                tooltipText = null;
                return;
            }

            Mod.Log.Debug($"Applying {skillDamageHits} hits to pilot skills to: {CombatantUtils.Label(target)}");
            int totalMod = 0;
            for (int i = 0; i < skillDamageHits; i++)
            {
                totalMod += Mod.Random.Next(Mod.Config.PerHitPenalties.MinSkillPenalty, Mod.Config.PerHitPenalties.MaxSkillPenalty);
            }
            Mod.Log.Debug($"  A total penalty of -{totalMod} will be applied to all pilot skills");

            Pilot targetPilot = target.GetPilot();

            int pilotingMod = targetPilot.Piloting - totalMod >= 1 ? totalMod : targetPilot.Piloting - 1;
            int gunneryMod = targetPilot.Gunnery - totalMod >= 1 ? totalMod : targetPilot.Gunnery - 1;
            int tacticsMod = targetPilot.Tactics - totalMod >= 1 ? totalMod : targetPilot.Tactics - 1;
            int gutsMod = targetPilot.Guts - totalMod >= 1 ? totalMod : targetPilot.Guts - 1;

            Mod.Log.Debug($"  reducing piloting: -{pilotingMod}  gunnery: -{gunneryMod}  tactics: -{tacticsMod}  guts: -{gutsMod}"); ;

            targetPilot.StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID,
                "Piloting", StatCollection.StatOperation.Int_Subtract, pilotingMod, -1, true);
            Text localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_SKILL_PILOTING], new object[] { pilotingMod });
            pilotSkillSB.Append(localText.ToString());

            targetPilot.StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID,
                "Gunnery", StatCollection.StatOperation.Int_Subtract, gunneryMod, -1, true);
            localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_SKILL_GUNNERY], new object[] { gunneryMod });
            pilotSkillSB.Append(localText.ToString());

            targetPilot.StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID,
                "Tactics", StatCollection.StatOperation.Int_Subtract, tacticsMod, -1, true);
            localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_SKILL_TACTICS], new object[] { tacticsMod });
            pilotSkillSB.Append(localText.ToString());

            targetPilot.StatCollection.ModifyStat<int>(hitInfo.attackerId, hitInfo.stackItemUID,
                "Guts", StatCollection.StatOperation.Int_Subtract, gutsMod, -1, true);
            localText = new Text(Mod.Config.LocalizedText[ModConfig.LT_TT_SKILL_GUTS], new object[] { gutsMod });
            pilotSkillSB.Append(localText.ToString());


            tooltipText = pilotSkillSB.ToString();
        }
    }
}
