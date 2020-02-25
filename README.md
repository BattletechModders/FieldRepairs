# Field Repairs
This mod for the [HBS BattleTech](http://battletechgame.com/) game attempts to make battle damage tell a story. In the base game, certain units can be marked with a _Poorly Maintained_ status. This reduces their armor, but has no other effects. This makes little sense from a lore perspective, where armor is often described as something easily salvaged and quickly applied, even in the field. Internal structure, weapons, and other components are much harder to repair in the field or to keep at a high state of repair. It should be more common to find enemies with damaged weapons, empty ammo bins, or decreased movement or accuracy. This mod attempts to implement a lore-centric view by altering the effects of the Poorly Maintained status.

At the start of combat, a _theme_ for the poorly maintained units are determined. A theme determines what type of effects will be applied to the unit. 

* **Battle Scars** - the units recently participated in a battle and has yet to be fully repaired. Whole limbs may be done, armor largely destroyed and structure damage, and the units may be on their last breath.
* **Mothballed** - the units were unused until recently. While structure and armor is intact, internal components are likely damaged or destroyed from their long storage.
* **Exhaustion** - the units have been on a long patrol or been through light skirmishes. Their ammo is depleted and their some armor damage will be present. 
* **Scavenged** - the units were pulled from a scrapyard and are heavily damaged. 

A theme is determined for each faction on the battlefield. Your units can have one theme, your allies another, and your opfor yet another. Each unit will have a status icon indicating what theme was applied, and the relevant effects.

This mod allows you to tag lances with specific themes, but also overrides the default poorly maintained lance tags (`spawn_poorly_maintained_25`, `spawn_poorly_maintained_50`, `spawn_poorly_maintained_75`). When no theme tags are present, a random theme will be selected for each faction on the battlefield. Your units can have one theme, your allies another, and the opfor yet another.

While each theme has similar effects, the poorly maintained tag also defines a relative strength of the effects that should applied. Units can be tagged with `spawn_poorly_maintained_25`, `spawn_poorly_maintained_50`, and `spawn_poorly_maintained_75`, which correspond to a 25% armor reduction, 50% armor reduction, or 75% armor reduction. In _FieldRepairs_, these strengths are instead classified as _moderate_, _strong_, or _extreme_. 

| Theme        | Moderate (25%)                                               | Strong (50%)                                                 | Extreme (75%)                                                |
| ------------ | ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| Battle Scars | 20% structure damage to a random location<br /><br />Arm crippled (actuators damaged)<br />Leg crippled (actuators damaged)<br />Component damaged<br />Weapon damaged | 50% structure damage to a random location<br />Arm crippled (actuators destroyed)<br />Leg crippled (actuators destroyed)<br />Component destroyed<br />Weapon damaged | 80% structure damage to a random location<br />Arm destroyed<br />Leg destroyed (only one allowed)<br /><br />Component destroyed<br />Weapon destroyed |
| Mothballed   | non-critical component damaged<br />ammo box damaged<br />heat sink damaged<br /> | non-critical component destroyed<br />ammo box destroyed<br />heat sink destroyed<br /><br />engine damaged | 80% chance of component damage (non-critical)<br />ammo box destroyed<br />heat sink destroyed<br /><br />gyro damaged<br />engine damaged |
| Exhaustion   | -1 to a skill [Pilot, Guts, Gunnery, Tactics] (min of 1)<br />Ammo box reduced by 50%<br /><br />15% armor damage to a random location (never structure)<br /> | -2 to a skill [Pilot, Guts, Gunnery, Tactics] (min of 1)<br />Ammo box reduced by 80%<br />25% armor damage to a random location (never structure) | -4 to a skill [Pilot, Guts, Gunnery, Tactics] (min of 1)<br />Ammo box reduced by 100%<br />35% armor damage to a random location (never structure) |
| Scavenged    |                                                              |                                                              |                                                              |

When a unit is spawned, the mod determines a number of flaws for each unit by making a random roll. The roll is based upon the strength of the tag, and is configurable in the mod settings. By default moderate damage will roll between 1-3 effects, strong will roll between 2-4, and extreme will roll between 3-6. For each flaw, the mod will roll once against all possibilities in the column defined above.

Units will **never** be spawned and immediately die. All theme effects ensure the unit won't be completely destroyed by the loss.

 Each unit will have a status icon indicating what theme was applied, and the relevant effects.
