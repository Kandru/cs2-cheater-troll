using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        private bool _InvisibleEnemiesEnabled = false;
        private Dictionary<string, List<CCSPlayerController>> _InvisibleEnemiesModeRandom = [];
        private float _InvisibleEnemiesModeRandomNextChange = 0;

        private void InitializeInvisibleEnemies()
        {
            // check type of InvisibleEnemies for each player and initialize accordingly
            foreach (KeyValuePair<string, CheaterConfig> entry in _onlineCheaters)
            {
                if (entry.Value.InvisibleEnemies.Mode == InvisibleEnemiesMode.Random) _InvisibleEnemiesModeRandom.Add(entry.Key, []);
            }
            // skip if already enabled
            if (_InvisibleEnemiesEnabled) return;
            // register listener
            RegisterListener<Listeners.CheckTransmit>(EventInvisibleEnemiesCheckTransmit);
            _InvisibleEnemiesEnabled = true;
        }

        private void ResetInvisibleEnemies()
        {
            // remove listener
            RemoveListener<Listeners.CheckTransmit>(EventInvisibleEnemiesCheckTransmit);
            // disable plug-in
            _InvisibleEnemiesEnabled = false;
            _InvisibleEnemiesModeRandom.Clear();
            _InvisibleEnemiesModeRandomNextChange = 0;
        }

        private void EventInvisibleEnemiesCheckTransmit(CCheckTransmitInfoList infoList)
        {
            // remove listener if no players to save resources
            if (_onlineCheaters.Count() == 0)
            {
                ResetInvisibleEnemies();
                return;
            }
            // check if it is time to change invisible enemies for players
            bool isTimeToChangeRandom = false;
            if (_InvisibleEnemiesModeRandom.Count > 0
                && _InvisibleEnemiesModeRandomNextChange <= Server.CurrentTime)
            {
                // Set next change time to a random value between MinTime and MaxTime
                _InvisibleEnemiesModeRandomNextChange = Server.CurrentTime
                    + (float)Random.Shared.NextDouble()
                    * (Config.InvisibleEnemies.RandomMaxTime
                    - Config.InvisibleEnemies.RandomMinTime)
                    + Config.InvisibleEnemies.RandomMinTime;
                isTimeToChangeRandom = true;
            }
            // worker
            foreach ((CCheckTransmitInfo info, CCSPlayerController? player) in infoList)
            {
                // check if player is in config and has InvisibleEnemies enabled
                if (player == null
                    || !player.IsValid
                    || string.IsNullOrEmpty(player.NetworkIDString)
                    || !_onlineCheaters.ContainsKey(player.NetworkIDString)
                    || !_onlineCheaters[player.NetworkIDString].InvisibleEnemies.Enabled
                    || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null
                    || player.PlayerPawn.Value.AbsOrigin == null
                    || (player.TeamNum != (int)CsTeam.Terrorist && player.TeamNum != (int)CsTeam.CounterTerrorist)
                    || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE) continue;
                // check if player has random invisible enemies
                if (_onlineCheaters[player.NetworkIDString].InvisibleEnemies.Mode == InvisibleEnemiesMode.Random && isTimeToChangeRandom)
                {
                    // Get all valid enemies who are alive
                    var enemies = Utilities.GetPlayers()
                        .Where(entry => entry.IsValid
                            && !entry.IsHLTV
                            && (entry.TeamNum == (int)CsTeam.Terrorist || entry.TeamNum == (int)CsTeam.CounterTerrorist)
                            && entry.TeamNum != player.TeamNum
                            && entry.PlayerPawn != null
                            && entry.PlayerPawn.IsValid
                            && entry.PlayerPawn.Value != null
                            && entry.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE)
                        .ToList();
                    // Calculate how many enemies to make invisible based on RandomPercentage
                    int invisibleCount = (int)Math.Ceiling(enemies.Count * (Config.InvisibleEnemies.RandomPercentage / 100.0f));
                    // Randomly select which enemies to make invisible
                    _InvisibleEnemiesModeRandom[player.NetworkIDString] = [.. enemies
                        .OrderBy(_ => Random.Shared.Next())
                        .Take(invisibleCount)
                        .Select(e => e)];
                }
                // check which enemies not to transmit (only when they are alive)
                foreach (CCSPlayerController entry in Utilities.GetPlayers())
                {
                    // check if player is in enemy team and alive
                    if ((entry.TeamNum == (int)CsTeam.Terrorist || entry.TeamNum == (int)CsTeam.CounterTerrorist)
                        && !string.IsNullOrEmpty(entry.NetworkIDString)
                        && entry.TeamNum != player.TeamNum
                        && entry.PlayerPawn != null
                        && entry.PlayerPawn.IsValid
                        && entry.PlayerPawn.Value != null
                        && entry.PlayerPawn.Value.AbsOrigin != null
                        && entry.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE
                        // check of specific type of InvisibleEnemies
                        && (
                            // mode "Full"
                            _onlineCheaters[player.NetworkIDString].InvisibleEnemies.Mode == InvisibleEnemiesMode.Full
                            // mode "Random"
                            || _onlineCheaters[player.NetworkIDString].InvisibleEnemies.Mode == InvisibleEnemiesMode.Random
                                && _InvisibleEnemiesModeRandom[player.NetworkIDString].Contains(entry)
                            // mode "Distance"
                            || _onlineCheaters[player.NetworkIDString].InvisibleEnemies.Mode == InvisibleEnemiesMode.Distance
                                && GetVectorDistance(player.PlayerPawn.Value.AbsOrigin, entry.PlayerPawn.Value.AbsOrigin) < Config.InvisibleEnemies.Distance
                        ))
                    {
                        // do not transmit ;)
                        info.TransmitEntities.Remove(entry.PlayerPawn.Value);
                    }
                }
            }
        }
    }
}
