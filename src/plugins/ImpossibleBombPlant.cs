using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;

namespace CheaterTroll.Plugins
{
    public class ImpossibleBombPlant : PluginBlueprint
    {
        public override string Description => "Impossible Bomb Plant";
        public override string ClassName => "ImpossibleBombPlant";
        public override List<string> Events => [
            "EventBombBeginplant",
            "OnBombAbortplant"
        ];
        private float _ImpossibleBombPlantLastPlant = 0;

        public ImpossibleBombPlant(PluginConfig GlobalConfig, IStringLocalizer Localizer) : base(GlobalConfig, Localizer)
        {
            Console.WriteLine(_localizer["plugins.class.initialize"].Value.Replace("{name}", ClassName));
        }

        public HookResult EventBombBeginplant(EventBombBeginplant @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.IsHLTV
                || !_players.ContainsKey(player)
                || !_players[player].ImpossibleBombPlant.Enabled
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.AbsOrigin == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)
            {
                return HookResult.Continue;
            }
            // find all bomb sites
            IEnumerable<CBombTarget> bombSites = Utilities.FindAllEntitiesByDesignerName<CBombTarget>("func_bomb_target");
            if (bombSites == null)
            {
                return HookResult.Continue;
            }

            foreach (CBombTarget bombSite in bombSites)
            {
                if (@event.Site == bombSite.Index)
                {
                    _ImpossibleBombPlantLastPlant = Server.CurrentTime;
                    float currentServerTime = Server.CurrentTime;
                    // stop bomb plant
                    _ = new CounterStrikeSharp.API.Modules.Timers.Timer(3.01f, () =>
                    {
                        if (bombSite == null
                            || !bombSite.IsValid
                            || _ImpossibleBombPlantLastPlant != currentServerTime)
                        {
                            return;
                        }
                        // change player weapon to knife to create the illusion of a bomb plant
                        player.ExecuteClientCommand("slot3");
                        // send player a nice message
                        Server.NextFrame(() =>
                        {
                            if (player == null
                                || !player.IsValid)
                            {
                                return;
                            }

                            player.PrintToCenterHtml("BOMB PLANTED");
                        });
                        // play bomb plant sound
                        RecipientFilter filter = [player];
                        _ = player.EmitSound("Announcer.BombPlanted.CS2_Classic", filter);
                        // get bomb site sounds
                        string beepSound = bombSite.IsBombSiteB ? "C4.PlantSoundB" : "C4.PlantSound";
                        // beep three times
                        _ = new CounterStrikeSharp.API.Modules.Timers.Timer(1f, () =>
                        {
                            if (player == null
                                || !player.IsValid)
                            {
                                return;
                            }

                            _ = player.EmitSound(beepSound, filter);
                            _ = new CounterStrikeSharp.API.Modules.Timers.Timer(1f, () =>
                            {
                                if (player == null
                                    || !player.IsValid)
                                {
                                    return;
                                }

                                _ = player.EmitSound(beepSound, filter);
                                _ = new CounterStrikeSharp.API.Modules.Timers.Timer(1f, () =>
                                {
                                    if (player == null
                                        || !player.IsValid)
                                    {
                                        return;
                                    }

                                    _ = player.EmitSound(beepSound, filter);
                                });
                            });
                        });
                        // disable bomb spot
                        bombSite.AcceptInput("Disable");
                        // enable a second later again :)
                        _ = new CounterStrikeSharp.API.Modules.Timers.Timer(1f, () =>
                        {
                            if (bombSite == null
                                || !bombSite.IsValid)
                            {
                                return;
                            }

                            bombSite.AcceptInput("Enable");
                        });
                    });
                    break;
                }
            }
            return HookResult.Continue;
        }

        public HookResult OnBombAbortplant(EventBombAbortplant @event, GameEventInfo info)
        {
            _ImpossibleBombPlantLastPlant = 0;
            return HookResult.Continue;
        }
    }
}
