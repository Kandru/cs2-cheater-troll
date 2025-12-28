using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CheaterTroll.Enums;
using CheaterTroll.Utils;
using Microsoft.Extensions.Localization;

namespace CheaterTroll.Plugins
{
    public class InvisibleEnemies : PluginBlueprint
    {
        public override string Description => "Invisible Enemies";
        public override string ClassName => "InvisibleEnemies";
        public override List<string> Listeners => [
            "CheckTransmit"
        ];

        public InvisibleEnemies(PluginConfig GlobalConfig, IStringLocalizer Localizer, bool IshotReloaded) : base(GlobalConfig, Localizer, IshotReloaded)
        {
            Console.WriteLine(_localizer["plugins.class.initialize"].Value.Replace("{name}", ClassName));
        }

        public override void Add(CCSPlayerController player, CheaterConfig config)
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

        public override void Remove(CCSPlayerController player)
        {
            _ = _players.Remove(player);
        }

        public override void Reset()
        {
            _players.Clear();
        }

        public override void Destroy()
        {
            Reset();
        }

        public void CheckTransmit(CCheckTransmitInfoList infoList)
        {
            // stop if no players have InvisibleEnemies enabled
            if (_players.Count == 0)
            {
                return;
            }

            // worker
            foreach ((CCheckTransmitInfo info, CCSPlayerController? player) in infoList)
            {
                // check if player is cheater
                if (player == null
                    || !player.IsValid
                    || player.IsBot
                    || player.IsHLTV
                    || !_players.ContainsKey(player)
                    || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null
                    || player.PlayerPawn.Value.AbsOrigin == null
                    || (player.TeamNum != (int)CsTeam.Terrorist && player.TeamNum != (int)CsTeam.CounterTerrorist)
                    || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)
                {
                    continue;
                }
                // check which enemies not to transmit (only when they are alive)
                foreach (CCSPlayerController entry in Utilities.GetPlayers().Where(p =>
                    (p.TeamNum == (int)CsTeam.Terrorist || p.TeamNum == (int)CsTeam.CounterTerrorist)
                    && p.TeamNum != player.TeamNum
                    && p.PlayerPawn != null
                    && p.PlayerPawn.IsValid
                    && p.PlayerPawn.Value != null
                    && p.PlayerPawn.Value.AbsOrigin != null
                    && p.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE
                    && (
                        // mode "Full"
                        _players[player].InvisibleEnemies.Mode == InvisibleEnemiesMode.Full
                        // mode "Distance"
                        || (_players[player].InvisibleEnemies.Mode == InvisibleEnemiesMode.Distance
                            && Vectors.GetDistance(player.PlayerPawn.Value.AbsOrigin, p.PlayerPawn.Value.AbsOrigin) < _players[player].InvisibleEnemies.Distance)
                    )))
                {
                    // do not transmit ;)
                    info.TransmitEntities.Remove(entry.PlayerPawn!.Value!);
                }
            }
        }
    }
}
