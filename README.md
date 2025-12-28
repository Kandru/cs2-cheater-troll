# CounterstrikeSharp - Cheater Troll

[![UpdateManager Compatible](https://img.shields.io/badge/CS2-UpdateManager-darkgreen)](https://github.com/Kandru/cs2-update-manager/)
[![Discord Support](https://img.shields.io/discord/289448144335536138?label=Discord%20Support&color=darkgreen)](https://discord.gg/bkuF8xKHUt)
[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-cheater-troll?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-cheater-troll/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-cheater-troll](https://img.shields.io/github/issues/Kandru/cs2-cheater-troll?color=darkgreen)](https://github.com/Kandru/cs2-cheater-troll/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)

This plugin will make the life for cheaters harder by applying some server-side features to their ingame experience they will not like. Why? Because I had a lot of fun doing this back in the days on my Counter-Strike:Source Server. Whenever I found someone to be cheating (e.g. by seeing through walls, only headshots, spin-bots, etc) I activated one or more random stuff on their side. They simply gave up on their own and I banned them afterwards. But this was much more enjoyable for everyone on the server (except the cheater).

This plug-in works out of the box. It does not require any additional configuration. You can however change the default settings to your liking.

## Current Features against cheater

- InvisibleEnemies -> Player won't see enemies as long as he is alive (not transmitted by the server to avoid his Aimbot to detect/see enemeis through the wall). Whenever he is not alive he will see everyone. Either within a given distance or regardless of the distance.
- ImpossibleBombPlant -> fakes the planting of the bomb but does not actually plant it.
- RandomPlayerSounds -> will play sounds every now and then to make players uncomfortable while playing.
- GrenadeSelfDamage -> Players will get damage by their own grenades.
- DoorGate -> Players will not be able to open doors and doors will close when a player is nearby.

## Road Map

- Make cheater glow for everyone else
- Jam cheater weapons
- Give cheater "butter fingers" (they drop weapons from time to time)
- Disable headshots for cheater
- Reduce damage to 1hp for cheater
- Make grenade damage significantly lower for cheater
- Invert movement for cheater
- Slower movement for cheater
- Shake screen of cheater

## Plugin Installation

1. Download and extract the latest release from the [GitHub releases page](https://github.com/Kandru/cs2-cheater-troll/releases/).
2. Move the "CheaterTroll" folder to the `/addons/counterstrikesharp/plugins/` directory of your gameserver.
3. Restart the server.

## Plugin Update

Simply overwrite all plugin files and they will be reloaded automatically or just use the [Update Manager](https://github.com/Kandru/cs2-update-manager/) itself for an easy automatic or manual update by using the *um update CheaterTroll* command.

## Commands

### *cheater <argument>*

Permission: *@cheatertroll/admin*

This command (without any argument) lists you all current players on the server. You can then go ahead and change the config of each player individually. It will show a menu with numbers and you simply write the command *cheater* and append the menu number afterwards. You can either run this via the server console or the client console.

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

...
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
