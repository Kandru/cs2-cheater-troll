using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Extensions;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        [ConsoleCommand("cheater", "Set or unset player as a cheater")]
        [RequiresPermissions("@cheatertroll/admin")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY, minArgs: 1, usage: "[player]")]
        public void CommandGiveDice(CCSPlayerController player, CommandInfo command)
        {
            string playerName = command.GetArg(1);
            if (playerName == null || playerName == "") return;
            List<CCSPlayerController> availablePlayers = [];
            foreach (CCSPlayerController entry in Utilities.GetPlayers())
            {
                if (entry.PlayerName.Contains(playerName, StringComparison.OrdinalIgnoreCase)) availablePlayers.Add(entry);
            }
            if (availablePlayers.Count == 0)
            {
                command.ReplyToCommand(Localizer["command.noplayers"]);
            }
            else if (availablePlayers.Count == 1)
            {
                if (!_cheaters.ContainsKey(availablePlayers[0].NetworkIDString))
                {
                    // add cheater to active cheater list
                    _cheaters.Add(
                        availablePlayers[0].NetworkIDString,
                        new CheaterConfig
                        {
                            InvisibleEnemies = true
                        }
                    );
                    // add cheater to cheaters in config if they do not already exist
                    Config.Cheater.Add(
                        availablePlayers[0].NetworkIDString,
                        new CheaterConfig
                        {
                            InvisibleEnemies = true
                        }
                    );
                    command.ReplyToCommand(Localizer["command.addedplayer"].Value.Replace("{player}", availablePlayers[0].PlayerName));
                }
                else
                {
                    // remove cheater from active cheater list
                    _cheaters.Remove(availablePlayers[0].NetworkIDString);
                    // remove cheater from cheaters in config
                    Config.Cheater.Remove(availablePlayers[0].NetworkIDString);
                    command.ReplyToCommand(Localizer["command.removedplayer"].Value.Replace("{player}", availablePlayers[0].PlayerName));
                }
                // update config
                Config.Update();
            }
            else
            {
                command.ReplyToCommand(Localizer["command.toomanyplayers"]);
            }
        }
    }
}
