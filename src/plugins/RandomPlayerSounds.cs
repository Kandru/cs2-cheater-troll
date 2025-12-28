using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;

namespace CheaterTroll.Plugins
{
    public class RandomPlayerSounds : PluginBlueprint
    {
        public override string Description => "Random Player Sounds";
        public override string ClassName => "RandomPlayerSounds";
        public override List<string> Events => [
            "EventRoundStart",
            "EventRoundEnd"
        ];
        private bool _roundActive = true;

        public RandomPlayerSounds(PluginConfig GlobalConfig, IStringLocalizer Localizer, bool IshotReloaded) : base(GlobalConfig, Localizer, IshotReloaded)
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
            if (_roundActive)
            {
                // make a random player sound
                MakeRandomPlayerSound(player);
            }
        }

        public HookResult EventRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            _roundActive = true;
            ConVar? mpFreezeTime = ConVar.Find("mp_freezetime");
            if (mpFreezeTime == null)
            {
                return HookResult.Continue;
            }

            foreach (KeyValuePair<CCSPlayerController, CheaterConfig> kvp in _players)
            {
                // wait for freeze time before playing the first sound
                _ = new CounterStrikeSharp.API.Modules.Timers.Timer(mpFreezeTime.GetPrimitiveValue<int>() + _globalConfig.Plugins.RandomPlayerSounds.WaitTime, () =>
                {
                    // make a random player sound
                    MakeRandomPlayerSound(kvp.Key);
                });
            }
            return HookResult.Continue;
        }

        public HookResult EventRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            _roundActive = false;
            return HookResult.Continue;
        }

        private void MakeRandomPlayerSound(CCSPlayerController player, string? lastSound = null)
        {
            if (!_roundActive
                || player == null
                || !player.IsValid
                || !_players.ContainsKey(player)
                || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)
            {
                return;
            }
            // get random sound
            // Avoid repeating the last sound if it exists
            RandomPlayerSoundConfig? sound = null;
            if (_globalConfig.Plugins.RandomPlayerSounds.Sounds.Count > 0)
            {
                // Get all sounds except the last one played
                List<RandomPlayerSoundConfig> availableSounds = [.. _globalConfig.Plugins.RandomPlayerSounds.Sounds.Where(s => lastSound == null || s.Name != lastSound)];
                // If we have available sounds (or if all sounds are used up), pick one randomly
                sound = availableSounds.Count > 0
                    ? availableSounds[Random.Shared.Next(availableSounds.Count)]
                    : _globalConfig.Plugins.RandomPlayerSounds.Sounds[Random.Shared.Next(_globalConfig.Plugins.RandomPlayerSounds.Sounds.Count)];
            }
            if (sound == null)
            {
                return;
            }
            // play sound
            for (int i = 0; i < sound.Amount; i++)
            {
                // add timer for the sounds
                _ = new CounterStrikeSharp.API.Modules.Timers.Timer(sound.Interval * i, () =>
                {
                    if (player == null
                        || !player.IsValid
                        || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null
                        || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)
                    {
                        return;
                    }

                    RecipientFilter filter = [player];
                    _ = player.EmitSound(sound.Name, filter);
                });
            }
            // wait random time before playing the next sound
            int randomTime = Random.Shared.Next(_globalConfig.Plugins.RandomPlayerSounds.RandomMinTime, _globalConfig.Plugins.RandomPlayerSounds.RandomMaxTime);
            _ = new CounterStrikeSharp.API.Modules.Timers.Timer(randomTime, () =>
            {
                // make a random player sound
                MakeRandomPlayerSound(player, sound.Name);
            });
        }
    }
}
