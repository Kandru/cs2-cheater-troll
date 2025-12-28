using CounterStrikeSharp.API.Core;

namespace CheaterTroll.Utils
{
    public enum MenuLevel { Main, PlayerConfig, ConfigEntry }

    public class PlayerMenuState
    {
        public MenuLevel CurrentLevel { get; set; } = MenuLevel.Main;
        public CCSPlayerController? SelectedPlayer { get; set; }
        public string? SelectedConfigProperty { get; set; }
        public string? EditingPropertyName { get; set; }
    }

    public class ClientConsoleMenu
    {
        private readonly Dictionary<CCSPlayerController, PlayerMenuState> _playerStates = [];

        public PlayerMenuState GetPlayerState(CCSPlayerController player)
        {
            if (!_playerStates.ContainsKey(player))
            {
                _playerStates[player] = new PlayerMenuState();
            }
            return _playerStates[player];
        }

        public void ResetPlayerState(CCSPlayerController player)
        {
            if (_playerStates.ContainsKey(player))
            {
                _playerStates[player] = new PlayerMenuState();
            }
        }

        public void RemovePlayerState(CCSPlayerController player)
        {
            _ = _playerStates.Remove(player);
        }

        public void ClearAllStates()
        {
            _playerStates.Clear();
        }
    }
}
