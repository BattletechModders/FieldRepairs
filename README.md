# Field Repairs
This mod for the [HBS BattleTech](http://battletechgame.com/) game attempts to make battle damage tell a story. In the base game, certain units can be marked with a _Poorly Maintained_ status. This reduces their armor, but has no other effects. This makes little sense from a lore perspective, where armor is often described as something easily salvaged and quickly applied, even in the field. Internal structure, weapons, and other components are much harder to repair in the field or to keep at a high state of repair. It should be more common to find enemies with damaged weapons, empty ammo bins, or decreased movement or accuracy. This mod implements a more lore-centric view by replacing the default behavior of the *poorly maintained* lance tags (`spawn_poorly_maintained_25`, `spawn_poorly_maintained_50`, `spawn_poorly_maintained_75`). 

Units will **never** be spawned and immediately die. All effects ensure the unit won't be completely destroyed by any applied damage. 

Each unit will have a status icon indicating what theme was applied, the strength of the effects, and any component or skill damage that was applied.

This mod has dependencies to the following mods. These are enforced through `mod.json` dependencies, and this mod will not load if they are not present.

* [CustomComponents](https://github.com/battletechmodders/customcomponents/) provides component categories
* [MechEngineer](https://github.com/battletechmodders/mechengineer/) provides Engine and Gyro customization
* [IRBTModUtils](https://github.com/iceraptor/irbtmodutils/) provides common logging and utility functions

## Themes

At the start of combat, a _theme_ for the poorly maintained units are determined. A theme is determined for each faction on the battlefield. Your units can have one theme, your allies another, and the opposing force yet another. Each unit will have a status icon indicating what theme was applied, and the relevant effects that were applied. The theme determines what type of effects will be applied to the unit:

* **Patched** - the units recently participated in a battle and has yet to be fully repaired. Armor has been hastily scavenged and reapplied, though underlying structure damage has not. Some components and weapons may have been damaged or destroyed.
* **Exhausted** - the units have been on a long patrol or been through light skirmishes. Their ammo is depleted and their some armor damage will be present. 
* **Mothballed** - the units were unused until recently. While structure and armor is intact, internal components are likely damaged or destroyed from their long storage.
* **Scavenged** - the units were pulled from a scrapyard and are heavily damaged. 

Note that themes are completely customizable and can be user-defined. You are free to add whatever themes you like, by customizing the Theme Weights and providing a unique label. The only requirement is that the Theme Weights (`MechWeights`, `VehicleWeights`, `TurretWeights`) have exactly 10 effects.

### Theme Weights

Each theme is a unique weighting of different types of damage. At the start of combat, the mod rolls to determine how many damage 'hits' to apply to each unit. Each hit will be applied to one of the following damage types:

| Damage Type  | Game Effect | Example |
| ------------ | ----------- | ----------- |
| Armor Damage  | A random armor location will suffer damage (see random locations, below). A percentage between `HitPenalties.MinArmorLossPerHit` and `HitPenalties.MaxArmorLossPerHit` will be calculated. These values default to 0.2 and 0.5 respectively. The maximum armor of the location will be multiplied by this percentage, and then reduced by the result. Armor can be reduced to 0 if a location suffers multiple hits. | If a location has 20 armor, and the percentage rolled is 0.35, the location would be reduced by 20 * .35 = 7, and would end up as 20 - 7 = 13. |
| Structure Damage| A random structure location will suffer damage (see random locations, below). A percentage between `HitPenalties.MinStructureLossPerHit` and `HitPenalties.MaxStructureLossPerHit` will be calculated. These values default to 0.1 and 0.3 respectively. The maximum structure of the location will be multiplied by this percentage, and then reduced by the result. Structure will never be reduced to less than 1 in a location. | If a location has 10 structure, and the percentage rolled is 0.20, the location would be reduced by 20 * .2= 4, and would end up as 10 - 4 = 6. |
| Component Damage| A random component will be damaged, preventing it from being used. Engines, Gyros, Weapons, HeatSinks, AmmoBoxes, and blacklisted components as well as components marked as `criticalComponent=true` are excluded from this damage type. This will NOT cause explosions or damage effects to fire. | |
| Weapon Damage| A random weapon will be damaged, preventing it from being used. This will NOT cause explosions or damage effects to fire. If this would reduce a mech to 0 un-damaged weapons, the hit will be re-rolled. | |
| Ammo Box Damage| A random ammo box will be damaged, preventing it from being used. This will NOT cause explosions or damage effects to fire. | |
| Heat Sink Damage| A random heat sink will be damaged, preventing it from being used. This will NOT cause explosions or damage effects to fire. | |
| Engine Damage | A random engine part will be damaged. If this effect would cause the engine to be destroyed, it will be re-rolled. Engine parts are identified through [Custom Components](http://github.com/battletechmodders/customcomponents/) categories - see below. | |
| Gyro Damage | A random gyro component will be damaged. If this effect would cause the gyro to be destroyed, it will be re-rolled. Gyros are identified through [Custom Components](http://github.com/battletechmodders/customcomponents/) categories - see below. | |
| Skill Penalty | A random penalty between `HitPenalties.MinSkillPenaltyPerHit` and `HitPenalties.MaxSkillPenaltyPerHit` is calculated, and then applied to every skill for the Pilot. This will not reduce a pilot's skill levels below 1. These values default to 1 and 3 respectively. | For a penalty of 3 and a pilot with Piloting 4, Guts 1, Gunnery 5, Tactics 3, the result will be Piloting 1, Guts 1, Gunnery 2, Tactics 1. |

Important notes:

* Units can be rendered weaponless if they eliminate all but one ammo-using weapon, and the ammo-box was also damaged. 

The default theme effects are listed below, where a + symbol represents one 'weight' on the scale. Damage types with more + symbols are more likely to happen than a damage type with a single +.

#### Mech Effects

| Damage                 | Patched | **Exhausted** | **Mothballed** | **Scavenged** |
| ---------------------- | ------- | ------------- | -------------- | ------------- |
| Armor Damage           | + | +++          |                | +             |
| Structure Damage       | +++     | +             | + | +             |
| Component Damage | ++ | | ++ |++|
| Weapon Damage | ++ | + | +++ |++|
| Ammo Box Damage        | +       | ++           |                | +             |
| Heat Sink Damage       |         | +             |                | +             |
| Engine Damage          |         |               | ++             | +             |
| Gyro Damage            |         |               | ++             | +             |
| Skill Penalty          | +       | ++            |                |               |

#### Turret and Vehicle

| Damage           | Patched | **Exhausted** | **Mothballed** | **Scavenged** |
| ---------------- | ------- | ------------- | -------------- | ------------- |
| Armor Damage     | +++     | ++++          | ++             | ++            |
| Structure Damage | +++     | +             | ++             | ++            |
| Component Damage | +       |               | +++            | ++            |
| Weapon Damage    | +       | +             | +++            | ++            |
| Ammo Box Damage  | +       | +++           |                | ++            |
| Heat Sink Damage |         |               |                |               |
| Engine Damage    |         |               |                |               |
| Gyro Damage      |         |               |                |               |
| Skill Penalty    | +       | ++            |                |               |

### Damage Rolls

The value of the poorly maintained tag defines the number of damage effects that should be applied. Units can be tagged with `spawn_poorly_maintained_25`, `spawn_poorly_maintained_50`, and `spawn_poorly_maintained_75`, which correspond to a 25% armor reduction, 50% armor reduction, or 75% armor reduction in the vanilla game. In _FieldRepairs_ each value is associated with a minimum and maximum number of damage rolls that will be applied. The table below provides the ranges provided by the default `mod.json`.

| Effect Value               | Mech Rolls | Vehicle Rolls | Turret Rolls |
| -------------------------- | ---------- | ------------- | ------------ |
| spawn_poorly_maintained_25 | 1-4 | 1-4 | 1-3 |
| spawn_poorly_maintained_50 | 2-6 | 2-5 | 1-4 |
| spawn_poorly_maintained_75 | 3-8 | 3-6 | 2-5 |

These values are configurable, and controlled through the mod.json setting `DamageRollsConfig.MechRolls`, `DamageRollsConfig.VehicleRolls`, and  `DamageRollsConfig.TurretRolls`.

## Component Categories

MechEngineer and Custom Components provide mod authors with significant flexibility. To accommodate the variety of mods available, *FieldRepairs* has to be configured to identity engine and gyro components for special processing. The `CustomComponentCategories.Gyros` and `CustomComponentCategories.EngineParts` values MUST be set to the values you specified for ME gyros and engine parts. If this is not set properly, engine components may be destroyed improperly and leave units crippled at the start of the game.

In addition, components that should NOT be damaged as part of *FieldRepairs* processing must be identified in the `CustomComponentCategories.Blacklisted` array. By default we assume that components with the CC Category "Armor", "Structure", "CASE", "PositiveQuirk", or "Cockpit" should not be destroyed and thus are excluded from processing.

## Skirmish Tags

Because it can be frustrating to find contracts that are marked with poorly_maintained tags, *FieldRepairs* includes an option to apply the tag in Skirmish matches. If the `Skirmish.Tag`value is non-null, *FieldRepairs* will apply it to any op-for unit in the match. This can be used to apply `spawn_poorly_maintained_50` for instance and play with custom themes in a more controlled fashion.