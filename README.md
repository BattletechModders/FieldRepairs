# PitCrew
This mod for the [HBS BattleTech](http://battletechgame.com/) game breathes life into the technical crews responsible for keeping your company's BattleMechs in fighting shape. The crew is harder to manage, requires a salary, and can occasionally make mistakes that will set back repairs or damage stored items. They are also mercenaries, and can leave at end of a contract or when someone else makes them a better offer! You'll always have Wang and a handful of his chosen favorites, but that may not help if you have a full mechbay of repairs and customizations!

This mod provides the same functionality as:

* [donZappo's Repair Bays](https://github.com/donZappo/Repair-Bays)
* [donZappo's Monthly Tech Adjustment](https://github.com/donZappo/MonthlyTechAdjustment).
* [Morphyum's Mech Maintenance by Cost](https://github.com/Morphyum/MechMaintenanceByCost/)

You should disable or remove the above mods before activating this mod.

## Feature Overview

 * Your available tech points are strongly tied to the number of MechTechs you can hire. You need a large, skilled crew in order to effectively maintain a company of BattleMechs.
 * MechTechs can be head-hunted. There is small chance someone will offer to pay them more. You'll to find another crew, or meet their high payscale, to keep them.
 * Active Mechs monthly maintenance cost (in c-bills) reflects their tech level, tonnage, and/or rarity.
 * Active Mechs require daily maintenance (in tech points). Maintenance can be failed, which causes components to be broken or destroyed. More complex or rare mechs require more maintenance.
 * Repairs have a chance to fail. Components being repaired will remain broken, or can be destroyed.

## MechTech Crews
By default, your company has Yang and 2-3 MechTechs that are founding members. They take a cut of the profits, and if someone runs off for better pastures, Yang finds a replacement. They are an elite crew, but there's only so much of them to go around. If you suddenly find yourself with multiple mechs in need of repair they will be stretched thin.

Anytime you're at a planet, you can speak with Yang and ask him to hire additional techs. You can choose the size of the crew you want to hire, as well as their expertise.

### Crew Size
A large MechTech crew is a significant boon when multiple lances need repairs or upgrades.

| Crew Size | Members | Restrictions |
| -- | -- | -- |
| Tiny | 1-3 | |
| Small | 3-5 | |
| Medium | 5-8 | Requires Beta Hub |
| Large | 8-12 | Requires Beta Hub |
| Huge | 12-20 |  Requires Gamma Hub |

A larger crew enables more **concurrent repairs**; for each point of concurrent repair (rounded up) the unit's full tech points are applied to the MechLab queue.

| Crew Size | Rookies | Regulars | Veterans | Elites | Legendaries |
| -- | -- | -- | -- | -- | -- |
| Tiny | 0.3 | 1.2 |1.7 | 2.2 | 2.7 |
| Small | 0.7 | 1.7 | 2.2 | 2.7 | 3.2 |
| Medium | 1 | 2.2 | 2.7 | 3.2 | 3.7 |
| Large | 1.2 | 2.7 | 3.2 | 3.7 | 4.2 |
| Huge | 1.7 | 3.2 | 3.7 | 4.2 | 4.7 |
| *Automation 1* | 0.3 | 0.3 | 0.3 | 0.3 | 0.3 |
| *Automation 2* | 0.3 | 0.3 | 0.3 | 0.3 | 0.3 |
| *Repair Harness* | 0.3 | 0.3 | 0.3 | 0.3 | 0.3 |

### Crew Skill
The crew's expertise is represented by their *experience level*. A rookie rating could be techs that have only tinkered with equipment before, or it could be veterans that just don't get along with each other. An elite rating could be all neophytes with one draconian genius driving them to exhaustion.

| Crew Rating | Repair Check% | Poached % | Notes |
| -- | -- | -- | -- |
| Rookie | 80% | 5% | |
| Regular | 90% | 10% |  |
| Veteran | 95% | 15% |  |
| Elite | 97% | 30% | Can only be found on planets with XXX tag |
| Legendary | 99% | 60% | Can only be found on planets with XXX tag |

The crew's experience determines how many tech points are gained. Yang and his crew always provide 5 tech points as a base. A crew adds additional tech points based upon their skill and size, as shown below.

| Crew Size | Rookies | Regulars | Veterans | Elites | Legendaries |
| -- | -- | -- | -- | -- | -- |
| Tiny | 3 | 5 | 8 | 12 | 18 |
| Small | 4 |	7	| 11 | 15 | 22 |
| Medium | 5 | 9 | 13 | 18 | 26 |
| Large | 6 |11 | 16 | 21 | 30 |
| Huge | 7 | 13 | 18 | 24 | 34 |

### Payment
Like any good mercenary, MechTechs expect to be paid for their services upfront and on-time. Each month your crew incurs living expenses and their paycheck. The greater the skill, the more in-demand the 'techs. This translates into a steep invoice for your mercenary company.

| Crew Size | Rookies | Regulars | Veterans | Elites | Legendaries |
| -- | -- | -- | -- | -- | -- |
| Tiny | 2,000 | 8,000 | 32,000 | 128,000 | 256,000 |
| Small | 5,000 | 20,000 | 80,000 | 320,000 | 640,000 |
| Medium | 8,000 | 32,000 | 128,000 | 512,000 | 1,024,000 |
| Large | 12,000 | 48,000 | 192,000 | 768,000 | 1,536,000 |
| Huge | 20,000 | 80,000 | 320,000 | 1,280,000 | 2,560,000 |

### Morale and Lifestyle

MechTechs are people too, and benefit (or suffer!) from their working environment. A company with lots of amenities will make the hired help more productive, and less likely to leave when head-hunted. An operation that's barely able to cut paychecks will have folks looking for a new job.

Your company's lifestyle setting (chosen during the monthly report) defines how well your mercenary MechTechs live. Well-fed and entertained MechTechs will work faster, though they also cost more. Going cheaper saves you some cash, but they cut corners and are more likely to listen to a compelling offer from another firm.

| Lifestyle | TP Multi | Cost Multi | Poaching% |
| -- | -- | -- | -- |
| Extravagant | x1.25 | x1.10 | -35% |
| Generous |  x1.10 | x1.05 | -20% |
| Normal |  x1.0 | x1.0 | +0% |
| Restrictive | x0.9 | x0.95 | +20% |
| Spartan |  -x0.75 | x0.90 | +35% |

### Being Poached

Good MechTech crews are hard to find, and always in high demand. While most planets will have rookies, regular and veteran MechTechs roaming around, elite and legendary crew are in extreme demand and can pretty much name their price. Some employers are more than happy to hire your techs out from underneath you, by offering them more money. When this happens you'll have to choose whether to accept their demands, let them go, or enforce their contract terms. Each approach has benefits and drawbacks that determine how loyal the mercenaries will be in the future.
 * **Counter-Offering** means you pay them a bit more each month, but their loyalty is increased. You gain a -10% poaching modifier, but add 0.05 to their lifestyle cost modifier. Both are cumulative but cap at -30% and 0.15 respectively.
 * **Releasing** means they leave your service and work elsewhere. Word gets around that you're a reasonable boss, meaning folks are more likely to work with you in the future.  You gain a -0.05 to the lifestyle cost modifier, which is cumulative to -0.15.
 * **Contesting** means you drag out the contracts and threaten them with bankruptcy if they leave. They remain in your service, but you gain a +20% poaching modifier. This is cumulative up to +60%.

## Maintenance

Mechs that are ready for action require constant maintenance checks to ensure they can jump into a battle at a moment's notice. Each MechBay with a readied BattleMech costs one or more tech points each day. More complicated 'Mechs increase this cost, with prototype 'Mechs being the most expensive.

Unfortunately even in the best of conditions mistakes can happen. Hurtling through space in the los-tech equivalent of a custom van makes them even more likely. Each day, for each active bay, the crew has a chance to make a mistake during maintenance. If a **maintenance failure** occurs instead of the tech points being subtracted from the queue, they are instead added.

This mechanic is controlled by a standard distribution, centered on 0. Very likely results cluster around the C on the following diagram. Likely results are in light purple, unlikely results are in dark purple.

![Maintenance Check Distribution](maint_check_distribution.png)

TODO: Add critical check success notes

## DEVELOPER NOTES

* Your crew size determines your total # of techpoints.
* Each mechbay with an active mech requires 1 point per day for maintenance
  * This value can be modified by high tech equipment, low test equipment, etc
* Any surplus points can go towards repairs/changes.

* Upgrades provide a flat bonus to every repair/change currently happening
So if you have 20 tech points from crew, who are elites and extravagant, you might get 40 tech points
If you have all 18 bays open with mechs in them that's 40 - 18 = 22 points for repairs/changes
Automation/harness/machine shop would add their bonus directly to each of those,  so a +3 yields 10,10,11 points.
If you drop to 1 active job though, you're only getting 22 + 3 points
ChassisTags will also factor in; omni is either a reduction in cost and/or tech point bonus on that particular model
