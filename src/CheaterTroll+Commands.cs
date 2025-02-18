using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

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
                if (!_cheaters.ContainsKey(availablePlayers[0]))
                {
                    // statically add invisible enemy punishment for now
                    _cheaters.Add(availablePlayers[0], ["invisible_enemies"]);
                    command.ReplyToCommand(Localizer["command.addedplayer"].Value.Replace("{player}", availablePlayers[0].PlayerName));
                }
                else
                {
                    _cheaters.Remove(availablePlayers[0]);
                    command.ReplyToCommand(Localizer["command.removedplayer"].Value.Replace("{player}", availablePlayers[0].PlayerName));
                }
            }
            else
            {
                command.ReplyToCommand(Localizer["command.toomanyplayers"]);
            }
        }
    }
}
