using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;
using Microsoft.Extensions.Localization;
using System.Text;

namespace CheaterTroll.Plugins
{
    public class AnnouncePosition : PluginBlueprint
    {
        public override string Description => "Announce Position";
        public override string ClassName => "AnnouncePosition";
        public override List<string> Listeners => [
            "OnTick",
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

        public void OnTick()
        {
            if (_players.Count == 0
                || Server.TickCount % 64 != 0)
            {
                return;
            }
            foreach (CCSPlayerController player in _players.Keys.ToList())
            {
                if (player == null
                    || !player.IsValid
                    || player.PlayerPawn == null
                    || !player.PlayerPawn.IsValid
                    || player.PlayerPawn.Value == null
                    || player.PlayerPawn.Value.AbsOrigin == null
                    || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)
                {
                    continue;
                }
                string currentPosition = player.PlayerPawn.Value.LastPlaceName;
                if (_playerPositions.TryGetValue(player, out string? lastPosition)
                    && (lastPosition == currentPosition
                    || lastPosition == string.Empty
                    || lastPosition == null))
                {
                    continue;
                }
                // update current position
                _playerPositions[player] = currentPosition;
                // add spaces in between
                string spacedPosition = SpaceOutCapitals(currentPosition);
                // get random localizer message
                int randomNumber = Random.Shared.Next(1, 15);
                UserMessage message = UserMessage.FromId(118);
                // prepare chat message
                string messageText = _localizer[$"heavyknife.chat.position.{randomNumber}"].Value
                    .Replace("{name}", spacedPosition.ToLower());
                messageText = HumanizeText(messageText);
                message.SetInt("entityindex", (int)player.Index);
                message.SetBool("chat", true);
                message.SetString("messagename", "Cstrike_Chat_All");
                message.SetString("param1", player.PlayerName);
                message.SetString("param2", messageText);
                // add all players except the cheater
                message.Recipients.AddAllPlayers();
                _ = message.Recipients.Remove(player);
                // go for it
                message.Send();
            }
        }

        private static string HumanizeText(string text)
        {
            StringBuilder result = new();
            Random random = Random.Shared;
            string[] words = text.Split(' ');

            for (int w = 0; w < words.Length; w++)
            {
                string word = words[w];

                // Occasional word capitalization issues
                if (word.Length > 0 && random.Next(100) < 8)
                {
                    word = random.Next(2) == 0
                        ? word.ToUpper()
                        : char.ToLower(word[0]) + word[1..];
                }

                // Add occasional typos within the word
                if (word.Length > 2 && random.Next(100) < 12)
                {
                    int typoType = random.Next(3);
                    int charPos = random.Next(word.Length);

                    switch (typoType)
                    {
                        case 0: // Swap adjacent characters
                            if (charPos < word.Length - 1)
                            {
                                word = word[..charPos] +
                                       word[charPos + 1] +
                                       word[charPos] +
                                       word[(charPos + 2)..];
                            }
                            break;
                        case 1: // Drop a character
                            if (charPos > 0 && charPos < word.Length)
                            {
                                word = word[..charPos] +
                                       word[(charPos + 1)..];
                            }
                            break;
                        case 2: // Add nearby key typo (just append random letter)
                            word = word.Insert(charPos, ((char)('a' + random.Next(26))).ToString());
                            break;
                        default:
                            break;
                    }
                }

                _ = result.Append(word);

                if (w < words.Length - 1)
                {
                    _ = result.Append(' ');
                    // Occasional double space
                    if (random.Next(100) < 3)
                    {
                        _ = result.Append(' ');
                    }
                }
            }

            return result.ToString();
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
