# Subsystem

Subsystem is a mod loader for _Homeworld: Deserts of Kharak_. It aims to have a safe, auditable and data-oriented design.

### Disclaimer

Subsystem is currently in alpha. It may be extremely unstable, and it's not likely to provide useful debugging information. Use at your own risk, and please try to dig into logs on your own before reporting crashes or other odd behavior.

## Installation

Place `Subsystem.dll` and `BBI.Unity.Game.dll` in the `Deserts of Kharak/Data/Managed/` folder. 

`BBI.Unity.Game.dll` already exists and should be overwritten. You can make a backup of this file if you'd like.

## Usage

Create a JSON file like this:

```json
{
  "Entities": {
      "C_Escort_MP": {
        "WeaponAttributes": {
          "C_Escort_Weapon_G2G_MP": {
            "BaseDamagePerRound": 14,
            "ExcludeFromHeightAdvantage": true
          }
        },
      },
      "G_Baserunner_MP": {
        "UnitAttributes": {
          "MaxHealth": 3500,
          "Armour": 3,
          "Resource1Cost": 225,
          "ProductionTime": 18.0
        }
      },
      "Tier_3_C_Artillery": {
        "ResearchItemAttributes": {
          "Resource2Cost": 150,
          "ResearchTime": 45.0,
          "Dependencies": [
            "Tier_2_C_ArmouredAssault"
          ]
        }
      }
  }
}
```

Save the file and place it at `Deserts of Kharak/Data/patch.json`. The filename and location must match exactly.

Note that the keys (`Entities`, `UnitAttributes`, etc) are case-sensitive.

The stats are reloaded at the beginning of every game, 

### Multiplayer Notes

To use this in multiplayer, all players in the game **must** have the same version of Subsystem *and* the same `patch.json` file contents. If any player has different data, the game is liable to crash with a synchronization error.

### Modifiable Attributes

See these source files for a list of attributes that can be modified:

* [AbilityAttributesPatch.cs](https://github.com/Majiir/Subsystem/blob/develop/Subsystem/AbilityAttributesPatch.cs)
* [ResearchItemAttributesPatch.cs](https://github.com/Majiir/Subsystem/blob/develop/Subsystem/ResearchItemAttributesPatch.cs)
* [UnitAttributesPatch.cs](https://github.com/Majiir/Subsystem/blob/develop/Subsystem/UnitAttributesPatch.cs)
* [WeaponAttributesPatch.cs](https://github.com/Majiir/Subsystem/blob/develop/Subsystem/WeaponAttributesPatch.cs)

### Entity Names

This gist shows a dump of all named entity components: https://gist.github.com/Majiir/1c78930ad70a16e1bd9a116948f55409

### Serialization Notes

* Some enums are marked with `[Flags]`. You must use a particular syntax in order to assign multiple flags. For example, to set a unit's `Class` to multiple values:

```json
...
"UnitAttributes": {
  "Class": "Hover, Carrier"
}
...
```

* Non-flags enums should be set as strings. (Example: `"HackableProperties": "InstantHackable"`)

* `Fixed64` values are set as Numbers (equivalent to `double`) in JSON. This is technically a lossy conversion, but it will work well in most circumstances.
