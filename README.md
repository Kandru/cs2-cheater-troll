# CounterstrikeSharp - Cheater Troll

[![UpdateManager Compatible](https://img.shields.io/badge/CS2-UpdateManager-darkgreen)](https://github.com/Kandru/cs2-update-manager/)
[![Discord Support](https://img.shields.io/discord/289448144335536138?label=Discord%20Support&color=darkgreen)](https://discord.gg/bkuF8xKHUt)
[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-cheater-troll?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-cheater-troll/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-cheater-troll](https://img.shields.io/github/issues/Kandru/cs2-cheater-troll?color=darkgreen)](https://github.com/Kandru/cs2-cheater-troll/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)

## 🎭 What is This Plugin?

Tired of cheaters ruining your server? This plugin turns the tables by making their lives **miserable** with clever server-side tricks. It's inspired by the golden days of my community-driven Counter-Strike:Source server, where admin justice came with a side of entertainment. Whenever a cheater was spotted (wallhacking, spinbots, suspiciously perfect headshots), a little chaos was unleashed on *their* side only. They'd eventually rage quit or confess, and everyone else had a laugh.

**Best part?** It works out of the box with zero configuration needed. But if you want to fine-tune the chaos, all settings are fully customizable.

## ✨ Current Features

A growing arsenal of server-side tricks to make cheaters regret their life choices:

- **Invisible Enemies** — Cheaters can't see anyone while alive (their aimbot is blind too!). Once they die, they get full visibility. Works with configurable distance limits or unlimited range.
- **Phantom Bomb Plants** — They think they're planting the bomb, but nothing actually happens. Frustration guaranteed.
- **Psychological Warfare** — Random sounds play throughout the match (knives, grenades, smokes) to keep them on edge and paranoid.
- **Grenade Backfire** — Their own grenades hurt them. No exceptions.
- **Stuck Doors** — All doors lock down or snap shut when they approach. Escape routes? Not anymore.
- **Make cheater glow** — Everyone else sees them highlighted like a Christmas tree
- **Impossible Bomb Plant** — They think they're planting the bomb, but nothing actually happens. Frustration guaranteed.
- **Visible On Radar** - Watch him moving via your radar

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

1. **Type `cheater`** (without arguments) to list all players on the server
2. **Choose a player** by typing the menu number next to their name
3. **Toggle features on/off** using the numbers shown in the submenu
4. **Configure settings** like distance limits or modes for each enabled feature
5. **Return to previous menu** by selecting `0` (Back)

Example:

```
cheater
=== Overview ===
1. X kalle (7656XXXXXXXXXXXXXX, 2.26 min online)
0. Exit

cheater 1
=== Overview -> kalle ===
1. Enable Cheater
0. Back

cheater 1
✓ Cheater enabled
=== Overview -> kalle ===
1. Disable Cheater
2. X InvisibleEnemies
3. X GrenadeSelfDamage
4. X ImpossibleBombPlant
5. X RandomPlayerSounds
6. ✓ DoorGate
0. Back

cheater 2
=== Overview -> kalle -> InvisibleEnemies ===
1. Enable
0. Back

cheater 1
✓ InvisibleEnemies enabled
=== Overview -> kalle -> InvisibleEnemies ===
1. Disable
2. Distance: 750
3. ModeString: Full (Full|Distance)
0. Back

```

## Configuration

This plugin automatically creates a readable JSON configuration file. This configuration file can be found in `/addons/counterstrikesharp/configs/plugins/CheaterTroll/CheaterTroll.json`.

```json
{
  "enabled": true,
  "debug": false,
  "global_config": {
    "name": "",
    "invisible_enemies": {
      "enabled": true,
      "mode": "Full"
    },
    "grenade_self_damage": {
      "enabled": true,
      "flashbang_duration": 5
    },
    "impossible_bomb_plant": {
      "enabled": true
    },
    "random_player_sounds": {
      "enabled": true,
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
      "min_time": 20,
      "max_time": 60
    },
    "always_door_closed": {
      "enabled": true,
      "sound": "Saysound.Knock",
      "speed": 60,
      "delay": 0.1
    }
  },
  "cheater": {},
  "ConfigVersion": 1
}
```

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
