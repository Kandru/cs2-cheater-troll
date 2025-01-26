# CounterstrikeSharp - Cheater Troll

[![UpdateManager Compatible](https://img.shields.io/badge/CS2-UpdateManager-darkgreen)](https://github.com/Kandru/cs2-update-manager/)
[![Discord Support](https://img.shields.io/discord/289448144335536138?label=Discord%20Support&color=darkgreen)](https://discord.gg/bkuF8xKHUt)
[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-cheater-troll?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-cheater-troll/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-cheater-troll](https://img.shields.io/github/issues/Kandru/cs2-cheater-troll?color=darkgreen)](https://github.com/Kandru/cs2-cheater-troll/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)

This plugin will make the life for cheaters harder by applying some server-side features to their ingame experience they will not like. Why? Because I had a lot of fun doing this back in the days on my Counter-Strike:Source Server. Whenever I found someone to be cheating (e.g. by seeing through walls, only headshots, spin-bots, etc) I activated one or more random stuff on their side. They simply gave up on their own and I banned them afterwards. But this was much more enjoyable for everyone on the server (except the cheater).

## Current Features against cheater

- None yet

## Road Map

- Make players invisible for the cheater (either all or randomly)
- Make cheater glow for everyone else
- Jam cheater weapons
- Give cheater "butter fingers" (they drop weapons from time to time)
- Disable headshots for cheater
- Reduce damage to 1hp for cheater
- Make grenade damage significantly lower for cheater
- Invert movement for cheater
- Random mode which cycles between one or multiple of the features
- Announcement for every other player that a specific player is a cheater and what is used against him currently
- Slower movement for cheater
- Shake screen of cheater
- Blind cheater at random times

## Plugin Installation

1. Download and extract the latest release from the [GitHub releases page](https://github.com/Kandru/cs2-cheater-troll/releases/).
2. Move the "CheaterTroll" folder to the `/addons/counterstrikesharp/configs/plugins/` directory of your gameserver.
3. Restart the server.

## Plugin Update

Simply overwrite all plugin files and they will be reloaded automatically or just use the [Update Manager](https://github.com/Kandru/cs2-update-manager/) itself for an easy automatic or manual update by using the *um update CheaterTroll* command.

## Commands

There is currently no client-side command.

## Configuration

This plugin automatically creates a readable JSON configuration file. This configuration file can be found in `/addons/counterstrikesharp/configs/plugins/CheaterTroll/CheaterTroll.json`.

```json
{
  "enabled": true,
  "debug": false,
  "ConfigVersion": 1
}
```

You can either disable the complete CheaterTroll Plugin by simply setting the *enable* boolean to *false* or specify a specific map where you want this plugin to be disabled. This allows for a maximum customizability.

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
