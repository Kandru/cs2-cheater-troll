using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        private bool _RandomPlayerSoundsEnabled = false;

        private void InitializeRandomPlayerSounds()
        {
            if (!Config.RandomPlayerSounds.Enabled) return;
            // skip if already enabled
            if (_RandomPlayerSoundsEnabled) return;
            // register listener
            RegisterEventHandler<EventRoundStart>(OnRoundStart, HookMode.Pre);
            _RandomPlayerSoundsEnabled = true;
            DebugPrint("Plugin RandomPlayerSounds enabled");
        }

        private void ResetRandomPlayerSounds()
        {
            // remove listener
            DeregisterEventHandler<EventRoundStart>(OnRoundStart, HookMode.Pre);
            // disable plug-in
            _RandomPlayerSoundsEnabled = false;
            DebugPrint("Plugin RandomPlayerSounds disabled");
        }

        private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            DebugPrint("OnRoundStart");
            int? currentRound = GetGameRules()?.TotalRoundsPlayed;
            ConVar? mpFreezeTime = ConVar.Find("mp_freezetime");
            if (currentRound == null
                || mpFreezeTime == null) return HookResult.Continue;
            foreach (var kvp in _onlineCheaters)
            {
                if (kvp.Value.RandomPlayerSounds.Enabled)
                {
                    // get player entity
                    CCSPlayerController? player = Utilities.GetPlayerFromSteamId(new SteamID(kvp.Key).SteamId64);
                    if (player == null
                        || !player.IsValid) continue;
                    // wait for freeze time before playing the first sound
                    AddTimer(mpFreezeTime.GetPrimitiveValue<int>() + Config.RandomPlayerSounds.WaitTime, () =>
                    {
                        // make a random player sound
                        MakeRandomPlayerSound(player, (int)currentRound);
                    });
                }
            }
            return HookResult.Continue;
        }

        private void MakeRandomPlayerSound(CCSPlayerController? player, int currentRound, string? lastSound = null)
        {
            // get random sound
            // Avoid repeating the last sound if it exists
            RandomPlayerSoundConfig? sound = null;
            if (Config.RandomPlayerSounds.Sounds.Count > 0)
            {
                // Get all sounds except the last one played
                var availableSounds = Config.RandomPlayerSounds.Sounds
                    .Where(s => lastSound == null || s.Name != lastSound)
                    .ToList();
                // If we have available sounds (or if all sounds are used up), pick one randomly
                if (availableSounds.Count > 0)
                    sound = availableSounds[Random.Shared.Next(availableSounds.Count)];
                else
                    sound = Config.RandomPlayerSounds.Sounds[Random.Shared.Next(Config.RandomPlayerSounds.Sounds.Count)];
            }
            if (sound == null
                || player == null
                || !player.IsValid
                || GetGameRules()?.TotalRoundsPlayed != currentRound
                || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE) return;
            DebugPrint($"MakeRandomPlayerSound -> {sound.Name}");
            // play sound
            for (int i = 0; i < sound.Amount; i++)
            {
                // add timer for the sounds
                AddTimer(sound.Interval * i, () =>
                {
                    if (player == null
                        || !player.IsValid
                        || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null
                        || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE) return;
                    RecipientFilter filter = [player];
                    player.EmitSound(sound.Name, filter);
                });
            }
            // wait random time before playing the next sound
            int randomTime = Random.Shared.Next(Config.RandomPlayerSounds.RandomMinTime, Config.RandomPlayerSounds.RandomMaxTime);
            AddTimer(randomTime, () =>
            {
                // make a random player sound
                MakeRandomPlayerSound(player, currentRound, sound.Name);
            });
        }
    }

}
