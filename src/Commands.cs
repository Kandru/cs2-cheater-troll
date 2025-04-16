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
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER, minArgs: 0, usage: "[player]")]
        public void CommandAddCheater(CCSPlayerController player, CommandInfo command)
        {
            string playerName = command.GetArg(1);
            if (playerName == null || playerName == "") return;
            List<CCSPlayerController> availablePlayers = [];
            foreach (CCSPlayerController entry in Utilities.GetPlayers()
                .Where(p => p.IsValid
                    && !p.IsBot
                    && !p.IsHLTV
                    && p.PlayerName.ToLower().Contains(playerName.ToLower())))
                availablePlayers.Add(entry);
            if (availablePlayers.Count == 0)
            {
                command.ReplyToCommand(Localizer["command.noplayers"]);
            }
            else if (availablePlayers.Count == 1)
            {
                if (!_onlineCheaters.ContainsKey(availablePlayers[0].NetworkIDString))
                {
                    // add cheater to active cheater list
                    _onlineCheaters.Add(
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
                    // initialize listeners
                    InitializeListener();
                    command.ReplyToCommand(Localizer["command.addedplayer"].Value.Replace("{player}", availablePlayers[0].PlayerName));
                }
                else
                {
                    // remove cheater from active cheater list
                    _onlineCheaters.Remove(availablePlayers[0].NetworkIDString);
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
