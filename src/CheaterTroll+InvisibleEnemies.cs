﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        private bool _InvisibleEnemiesEnabled = false;

        private void InitializeInvisibleEnemies()
        {
            if (_InvisibleEnemiesEnabled) return;
            RegisterListener<Listeners.CheckTransmit>(EventInvisibleEnemiesCheckTransmit);
            _InvisibleEnemiesEnabled = true;
        }

        private void ResetInvisibleEnemies()
        {
            RemoveListener<Listeners.CheckTransmit>(EventInvisibleEnemiesCheckTransmit);
            _InvisibleEnemiesEnabled = false;
        }

        private void EventInvisibleEnemiesCheckTransmit(CCheckTransmitInfoList infoList)
        {
            // remove listener if no players to save resources
            if (_cheaters.Count() == 0)
            {
                ResetInvisibleEnemies();
                return;
            }
            // worker
            foreach ((CCheckTransmitInfo info, CCSPlayerController? player) in infoList)
            {
                if (player == null
                    || player.Pawn == null
                    || player.Pawn.Value == null
                    || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null
                    || (player.TeamNum != (int)CsTeam.Terrorist && player.TeamNum != (int)CsTeam.CounterTerrorist)
                    || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE
                    || !_cheaters.ContainsKey(player.NetworkIDString)
                    || !_cheaters[player.NetworkIDString].InvisibleEnemies) continue;
                // do not transmit enemies that are alive
                foreach (CCSPlayerController entry in Utilities.GetPlayers())
                {
                    // check if player is in enemy team and alive
                    if ((entry.TeamNum == (int)CsTeam.Terrorist || entry.TeamNum == (int)CsTeam.CounterTerrorist)
                        && entry.TeamNum != player.TeamNum
                        && entry.PlayerPawn != null
                        && entry.PlayerPawn.IsValid
                        && entry.PlayerPawn.Value != null
                        && entry.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE)
                    {
                        // do not transmit ;)
                        info.TransmitEntities.Remove(entry);
                    }
                }
            }
        }
    }
}
