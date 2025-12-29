using CounterStrikeSharp.API.Core;
using CheaterTroll.Utils;
using Microsoft.Extensions.Localization;
using System.Drawing;

namespace CheaterTroll.Plugins
{
    public class PlayerGlow : PluginBlueprint
    {
        public override string Description => "Player Glow";
        public override string ClassName => "PlayerGlow";
        public override List<string> Events => [
            "EventPlayerDeath",
            "EventPlayerSpawn"
        ];
        private readonly Dictionary<CCSPlayerController, (CDynamicProp?, CDynamicProp?)> _glow = [];

        public PlayerGlow(PluginConfig GlobalConfig, IStringLocalizer Localizer, bool IshotReloaded) : base(GlobalConfig, Localizer, IshotReloaded)
        {
            Console.WriteLine(_localizer["plugins.class.initialize"].Value.Replace("{name}", ClassName));
        }

        public override void Add(CCSPlayerController player, CheaterConfig config)
        {
            // check if player is valid and has a pawn
            if (player == null
                || !player.IsValid
                || player.PlayerPawn?.Value == null
                || !player.PlayerPawn.Value.IsValid)
            {
                return;
            }
            _players.Add(player, config);
            _glow.Add(player, Glow.Create(player.PlayerPawn.Value, ColorTranslator.FromHtml(_globalConfig.Plugins.PlayerGlow.Color)));
        }

        public override void Remove(CCSPlayerController player)
        {
            _ = _players.Remove(player);
            if (_glow.ContainsKey(player))
            {
                Glow.RemoveGlow(_glow[player].Item1, _glow[player].Item2);
                _ = _glow.Remove(player);
            }
        }

        public override void Reset()
        {
            foreach (CCSPlayerController? player in _players.Keys.ToList())
            {
                Remove(player);
            }
            _players.Clear();
            _glow.Clear();
        }

        public override void Destroy()
        {
            Reset();
        }

        public HookResult EventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || !_players.ContainsKey(player)
                || !_glow.ContainsKey(player))
            {
                return HookResult.Continue;
            }
            // remove glow
            Glow.RemoveGlow(_glow[player].Item1, _glow[player].Item2);
            _ = _glow.Remove(player);
            return HookResult.Continue;
        }

        public HookResult EventPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.PlayerPawn?.Value == null
                || !player.PlayerPawn.Value.IsValid
                || !_players.ContainsKey(player))
            {
                return HookResult.Continue;
            }
            // add glow again
            if (_glow.ContainsKey(player))
            {
                Glow.RemoveGlow(_glow[player].Item1, _glow[player].Item2);
                _ = _glow.Remove(player);
            }
            _glow.Add(player, Glow.Create(player.PlayerPawn.Value, ColorTranslator.FromHtml(_globalConfig.Plugins.PlayerGlow.Color)));
            return HookResult.Continue;
        }
    }
}
