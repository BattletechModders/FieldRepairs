# Field Repairs
This mod for the [HBS BattleTech](http://battletechgame.com/) game attempts to make battle damage tell a story. In the base game, certain units can be marked with a _Poorly Maintained_ status. This reduces their armor, but has no other effects. This makes little sense from a lore perspective, where armor is often described as something easily salvaged and quickly applied, even in the field. Internal structure, weapons, and other components are much harder to repair in the field or to keep at a high state of repair. It should be more common to find enemies with damaged weapons, empty ammo bins, or decreased movement or accuracy. This mod attempts to implement a lore-centric view by altering the effects of the Poorly Maintained status.

At the start of combat, a _theme_ for the poorly maintained units are determined. A theme determines what type of effects will be applied to the unit. 

* **Patched** - the units recently participated in a battle and has yet to be fully repaired. Armor has been hastily scavenged and reapplied, though underlying structure damage has not. Some components and weapons may have been damaged or destroyed.
* **Exhausted** - the units have been on a long patrol or been through light skirmishes. Their ammo is depleted and their some armor damage will be present. 
* **Mothballed** - the units were unused until recently. While structure and armor is intact, internal components are likely damaged or destroyed from their long storage.
* **Scavenged** - the units were pulled from a scrapyard and are heavily damaged. 

A theme is determined for each faction on the battlefield. Your units can have one theme, your allies another, and your opfor yet another. Each unit will have a status icon indicating what theme was applied, and the relevant effects.

This mod allows you to tag lances with specific themes, but also overrides the default poorly maintained lance tags (`spawn_poorly_maintained_25`, `spawn_poorly_maintained_50`, `spawn_poorly_maintained_75`). When no theme tags are present, a random theme will be selected for each faction on the battlefield. Your units can have one theme, your allies another, and the opfor yet another.

While each theme has similar effects, the poorly maintained tag also defines a relative strength of the effects that should applied. Units can be tagged with `spawn_poorly_maintained_25`, `spawn_poorly_maintained_50`, and `spawn_poorly_maintained_75`, which correspond to a 25% armor reduction, 50% armor reduction, or 75% armor reduction. In _FieldRepairs_, these strengths are instead classified as _moderate_, _strong_, or _extreme_. 

| Damage                 | Patched | **Exhausted** | **Mothballed** | **Scavenged** |
| ---------------------- | ------- | ------------- | -------------- | ------------- |
| Armor Damage           | + | +++           |                | +             |
| Structure Damage       | +++     | +             |                | +             |
| Component Damage | +++ | | +++ |++|
| Weapon Damage | ++ | | +++ |++|
| Ammo Box Damage        | +       | +++           |                | +             |
| Heat Sink Damage       |         | +             |                | +             |
| Engine Damage          |         |               | ++             | +             |
| Gyro Damage            |         |               | ++             | +             |
| Skill Penalty          | +       | ++            |                |               |

When a unit is spawned, the mod determines a number of flaws for each unit by making a random roll. The roll is based upon the strength of the tag, and is configurable in the mod settings. With default settings:

* Moderate damage will result in 4-6 rolls (25% armor = 8 armor damage 'hits')
* Strong damage will result in 7-9 rolls (50% armor = 16 armor damage 'hits')
* Extreme damage will result in 10-12 rolls (70% armor = 24 armor damage 'hits')

Units will **never** be spawned and immediately die. All theme effects ensure the unit won't be completely destroyed by the loss.

 Each unit will have a status icon indicating what theme was applied, and the relevant effects.
