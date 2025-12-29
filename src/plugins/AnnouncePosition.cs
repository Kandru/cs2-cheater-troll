using System.Text;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;
using Microsoft.Extensions.Localization;

namespace CheaterTroll.Plugins
{
    public class AnnouncePosition : PluginBlueprint
    {
        public override string Description => "Announce Position";
        public override string ClassName => "AnnouncePosition";
        public override List<string> Events => [
            "EventPlayerFootstep",
        ];

        private readonly Dictionary<CCSPlayerController, string> _playerPositions = [];

        public AnnouncePosition(PluginConfig GlobalConfig, IStringLocalizer Localizer, bool IshotReloaded) : base(GlobalConfig, Localizer, IshotReloaded)
        {
            Console.WriteLine(_localizer["plugins.class.initialize"].Value.Replace("{name}", ClassName));
        }

        public override void Remove(CCSPlayerController player)
        {
            _ = _players.Remove(player);
            _ = _playerPositions.Remove(player);
        }

        public override void Reset()
        {
            _players.Clear();
            _playerPositions.Clear();
        }

        public HookResult EventPlayerFootstep(EventPlayerFootstep @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.IsHLTV
                || !_players.ContainsKey(player)
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.AbsOrigin == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)
            {
                return HookResult.Continue;
            }

            string currentPosition = player.PlayerPawn.Value.LastPlaceName;

            if (_playerPositions.TryGetValue(player, out string? lastPosition) && lastPosition == currentPosition)
            {
                return HookResult.Continue;
            }

            _playerPositions[player] = currentPosition;
            string spacedPosition = SpaceOutCapitals(currentPosition);
            int randomNumber = Random.Shared.Next(1, 15);
            UserMessage message = UserMessage.FromPartialName("SayText");
            string messageText = "[ALL] " + player.PlayerName + ": " + _localizer[$"heavyknife.chat.position.{randomNumber}"].Value
                .Replace("{name}", spacedPosition);
            message.SetString("text", messageText);
            // add all players except the cheater
            message.Recipients.AddAllPlayers();
            _ = message.Recipients.Remove(player);
            message.Send();

            return HookResult.Continue;
        }

        private static string SpaceOutCapitals(string input)
        {
            StringBuilder result = new(input.Length + 10);
            foreach (char c in input)
            {
                if (char.IsUpper(c) && result.Length > 0)
                {
                    _ = result.Append(' ');
                }

                _ = result.Append(c);
            }
            return result.ToString();
        }
    }
}
