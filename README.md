# CounterstrikeSharp - Cheater Troll

[![UpdateManager Compatible](https://img.shields.io/badge/CS2-UpdateManager-darkgreen)](https://github.com/Kandru/cs2-update-manager/)
[![Discord Support](https://img.shields.io/discord/289448144335536138?label=Discord%20Support&color=darkgreen)](https://discord.gg/bkuF8xKHUt)
[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-cheater-troll?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-cheater-troll/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-cheater-troll](https://img.shields.io/github/issues/Kandru/cs2-cheater-troll?color=darkgreen)](https://github.com/Kandru/cs2-cheater-troll/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)


Tired of cheaters ruining your server? This plugin turns the tables by making their lives **miserable** with clever server-side tricks. It's inspired by the golden days of my community-driven Counter-Strike:Source server, where admin justice came with a side of entertainment. Whenever a cheater was spotted (wallhacking, spinbots, suspiciously perfect headshots), a little chaos was unleashed on *their* side only. They'd eventually rage quit or confess, and everyone else had a laugh.

Special thanks to [ScriptKid](https://www.youtube.com/watch?v=fYYPH4ZUFNo) from whom I got the idea years ago initially. You did something really funny and back then that made me laugh so hard. Now, I've brought this concept into CS2!

**Best part?** It works out of the box with zero configuration needed. But if you want to fine-tune the chaos, all settings are fully customizable.

## Current Features

A growing arsenal of server-side tricks to make cheaters regret their life choices:

- **Invisible Enemies** — Cheaters can't see anyone while alive (their aimbot is blind too!). Once they die, they get full visibility. Works with configurable distance limits or unlimited range.
- **Phantom Bomb Plants** — They think they're planting the bomb, but nothing actually happens. Frustration guaranteed.
- **Psychological Warfare** — Random sounds play throughout the match (knives, grenades, smokes) to keep them on edge and paranoid.
- **Grenade Backfire** — Their own grenades hurt them. No exceptions.
- **Stuck Doors** — All doors lock down or snap shut when they approach. Escape routes? Not anymore.
- **Make cheater glow** — Everyone else sees them highlighted like a Christmas tree
- **Impossible Bomb Plant** — They think they're planting the bomb, but nothing actually happens. Frustration guaranteed.
- **Visible On Radar** - Watch him moving via your radar
- **Heavy Knife** - walk initially slower with the knife in hand
- **AnnouncePosition** - the cheater announces his position to other player
- **CrouchJump** - the cheater is pushed a little bit back when crouching during airtime

## Plugin Installation

1. Download and extract the latest release from the [GitHub releases page](https://github.com/Kandru/cs2-cheater-troll/releases/).
2. Move the "CheaterTroll" folder to the `/addons/counterstrikesharp/plugins/` directory of your gameserver.
3. Restart the server.

## Plugin Update

Simply overwrite all plugin files and they will be reloaded automatically or just use the [Update Manager](https://github.com/Kandru/cs2-update-manager/) itself for an easy automatic or manual update by using the *um update CheaterTroll* command.

## Commands

### *cheater <argument>*

Permission: *@cheatertroll/admin*

#### Quick Tutorial

The `cheater` command is an interactive menu system to enable cheater punishments for individual players. You can run it from either the **server console** or **client console**:

1. **Type `cheater`** (without arguments) to list all players on the server, for actions simply prepend the command with the player index / value to change (e.g., `cheater 1`)
2. **Choose a player** by typing the menu number next to their name
3. **Toggle features on/off** using the numbers shown in the submenu
4. **Configure settings** like distance limits or modes for each enabled feature
5. **Return to previous menu** by selecting `cheater 0` (Back)

Example command line:

```c
> cheater
[Client] === Overview ===
[Client] 1. X kalle (765611XXXXXXXXXXX, 0.26 min online)
[Client] 0. Exit
> 
> cheater 1
[Client] === Overview -> kalle (765611XXXXXXXXXXX) ===
[Client] 1. Enable Cheater
[Client] 0. Back
> 
> cheater 1
[Client] ✓ Cheater enabled
[Client] === Overview -> kalle (765611XXXXXXXXXXX) ===
[Client] 1. Disable Cheater
[Client] 2. ✓ DoorGate
[Client] 3. ✓ DamageControl
[Client] 4. X PlayerGlow
[Client] 5. X GrenadeSelfDamage
[Client] 6. ✓ ImpossibleBombPlant
[Client] 7. X InvisibleEnemies
[Client] 8. X RandomPlayerSounds
[Client] 9. X VisibleOnRadar
[Client] 0. Back
> 
> cheater 4
[Client] === Overview -> kalle (765611XXXXXXXXXXX) -> PlayerGlow ===
[Client] 1. Enable
[Client] 0. Back
> 
> cheater 1
[Client] ✓ PlayerGlow enabled
[Client] === Overview -> kalle (765611XXXXXXXXXXX) -> PlayerGlow ===
[Client] 1. Disable
[Client] 0. Back
> 
> cheater 0
[Client] === Overview -> kalle (765611XXXXXXXXXXX) ===
[Client] 1. Disable Cheater
[Client] 2. ✓ DoorGate
[Client] 3. ✓ DamageControl
[Client] 4. ✓ PlayerGlow
[Client] 5. X GrenadeSelfDamage
[Client] 6. ✓ ImpossibleBombPlant
[Client] 7. X InvisibleEnemies
[Client] 8. X RandomPlayerSounds
[Client] 9. X VisibleOnRadar
[Client] 0. Back
> 
> cheater 0
[Client] === Overview ===
[Client] 1. ✓ kalle (765611XXXXXXXXXXX, 0.77 min online)
[Client] 0. Exit
```

## Configuration

This plugin automatically creates a readable JSON configuration file. This configuration file can be found in `/addons/counterstrikesharp/configs/plugins/CheaterTroll/CheaterTroll.json`.

```json
{
  "enabled": true,
  "debug": false,
  "global_config": {
    "always_door_closed": {
      "enabled": true,
      "enabled_for_new_cheater": true,
      "sound": "Saysound.Knock",
      "speed": 60,
      "delay": 0.1
    },
    "announce_position": {
      "enabled": true,
      "enabled_for_new_cheater": true
    },
    "crouch_jump": {
      "enabled": true,
      "enabled_for_new_cheater": true
    },
    "damage_control": {
      "enabled": true,
      "enabled_for_new_cheater": true
    },
    "glow": {
      "enabled": true,
      "enabled_for_new_cheater": false,
      "color": "#ff00a2"
    },
    "grenade_self_damage": {
      "enabled": true,
      "enabled_for_new_cheater": false,
      "flashbang_duration": 5
    },
    "heavy_knife": {
      "enabled": true,
      "enabled_for_new_cheater": true
    },
    "impossible_bomb_plant": {
      "enabled": true,
      "enabled_for_new_cheater": true
    },
    "invisible_enemies": {
      "enabled": true,
      "enabled_for_new_cheater": false,
      "mode": "Full"
    },
    "random_player_sounds": {
      "enabled": true,
      "enabled_for_new_cheater": false,
      "sounds": [
        {
          "name": "Weapon_Knife.Deploy",
          "amount": 1,
          "interval": 1
        },
        {
          "name": "Weapon_Knife.Slash",
          "amount": 1,
          "interval": 1
        },
        {
          "name": "BaseSmokeEffect.Sound",
          "amount": 1,
          "interval": 1
        },
        {
          "name": "HEGrenade.PullPin_Grenade",
          "amount": 1,
          "interval": 1
        },
        {
          "name": "Weapon_Taser.Single",
          "amount": 1,
          "interval": 1
        }
      ],
      "wait_time": 5,
      "min_time": 5,
      "max_time": 30
    },
    "visible_on_radar": {
      "enabled": true,
      "enabled_for_new_cheater": false
    }
  },
  "cheater": {},
  "ConfigVersion": 1
}
```

### global_config

The global config section contains all available options for each module. Each option has its own subsection containing various parameters such as enabling/disabling specific features, setting thresholds, etc.

#### enabled <true/false>

Whether or not this modules is enabled or disabled globally.

#### enabled_for_new_cheater <true/false>

Whether or not a newly marked cheater should have this feature enabled by default. This should be set to `false` unless you want to make it obvious to a new cheater that he is being punished. Per default everything to obvious is disabled.

## Compile Yourself

Clone the project:

```bash
git clone https://github.com/Kandru/cs2-cheater-troll.git
```

Go to the project directory

```bash
  cd cs2-cheater-troll
```

Install dependencies

```bash
  dotnet restore
```

Build debug files (to use on a development game server)

```bash
  dotnet build
```

Build release files (to use on a production game server)

```bash
  dotnet publish
```

## License

Released under [GPLv3](/LICENSE) by [@Kandru](https://github.com/Kandru).

## Authors

- [@derkalle4](https://www.github.com/derkalle4)
