using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Localization;

namespace CheaterTroll.Plugins
{
    public class PluginBlueprint(PluginConfig GlobalConfig, IStringLocalizer Localizer)
    {
        public readonly PluginConfig _globalConfig = GlobalConfig;
        public readonly IStringLocalizer _localizer = Localizer;
        public readonly Dictionary<CCSPlayerController, CheaterConfig> _players = [];
        public virtual string Description { get; private set; } = "PluginBlueprint";
        public virtual string ClassName => "PluginBlueprint";
        public virtual List<string> Events => [];
        public virtual List<string> Listeners => [];
        public virtual Dictionary<int, HookMode> UserMessages => [];
        public virtual List<string> Precache => [];

        public virtual void Add(CCSPlayerController player, CheaterConfig config)
        {
            // check if player is valid and has a pawn
            if (player == null
                || !player.IsValid
                || player.Pawn?.Value == null
                || !player.Pawn.Value.IsValid)
            {
                return;
            }
            _players.Add(player, config);
        }

        public virtual void Remove(CCSPlayerController player)
        {
            _ = _players.Remove(player);
        }

        public virtual void Reset()
        {
            _players.Clear();
        }

        public virtual void Destroy()
        {
            Reset();
        }
    }
}