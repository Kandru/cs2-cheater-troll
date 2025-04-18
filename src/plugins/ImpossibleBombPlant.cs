﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        private bool _ImpossibleBombPlantEnabled = false;
        private float _ImpossibleBombPlantLastPlant = 0;

        private void InitializeImpossibleBombPlant()
        {
            if (!Config.ImpossibleBombPlant.Enabled) return;
            // skip if already enabled
            if (_ImpossibleBombPlantEnabled) return;
            // register listener
            RegisterEventHandler<EventBombBeginplant>(OnBombBeginplant);
            RegisterEventHandler<EventBombAbortplant>(OnBombAbortplant);
            _ImpossibleBombPlantEnabled = true;
            DebugPrint("Plugin ImpossibleBombPlant enabled");
        }

        private void ResetImpossibleBombPlant()
        {
            // remove listener
            DeregisterEventHandler<EventBombBeginplant>(OnBombBeginplant);
            DeregisterEventHandler<EventBombAbortplant>(OnBombAbortplant);
            // disable plug-in
            _ImpossibleBombPlantEnabled = false;
            _ImpossibleBombPlantLastPlant = 0;
            DebugPrint("Plugin ImpossibleBombPlant disabled");
        }

        private HookResult OnBombBeginplant(EventBombBeginplant @event, GameEventInfo info)
        {
            DebugPrint("OnBombBeginplant");
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || string.IsNullOrEmpty(player.NetworkIDString)
                || player.IsBot
                || player.IsHLTV
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.AbsOrigin == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE) return HookResult.Continue;
            // check if player is a cheater
            if (_onlineCheaters.TryGetValue(player.NetworkIDString, out CheaterConfig? cheaterConfig))
            {
                // find all bomb sites
                var bombSites = Utilities.FindAllEntitiesByDesignerName<CBombTarget>("func_bomb_target");
                if (bombSites == null) return HookResult.Continue;
                foreach (var bombSite in bombSites)
                {
                    if (@event.Site == bombSite.Index)
                    {
                        _ImpossibleBombPlantLastPlant = Server.CurrentTime;
                        float currentServerTime = Server.CurrentTime;
                        // stop bomb plant
                        AddTimer(3.01f, () =>
                        {
                            if (bombSite == null
                                || !bombSite.IsValid
                                || _ImpossibleBombPlantLastPlant != currentServerTime) return;
                            // change player weapon to knife to create the illusion of a bomb plant
                            player.ExecuteClientCommand("slot3");
                            // send player a nice message
                            Server.NextFrame(() =>
                            {
                                if (player == null
                                    || !player.IsValid) return;
                                player.PrintToCenterHtml("BOMB PLANTED");
                            });
                            // play bomb plant sound
                            RecipientFilter filter = [player];
                            player.EmitSound("Announcer.BombPlanted.CS2_Classic", filter);
                            // get bomb site sounds
                            string beepSound = bombSite.IsBombSiteB ? "C4.PlantSoundB" : "C4.PlantSound";
                            // beep three times
                            AddTimer(1f, () =>
                            {
                                if (player == null
                                    || !player.IsValid) return;
                                player.EmitSound(beepSound, filter);
                                AddTimer(1f, () =>
                                {
                                    if (player == null
                                        || !player.IsValid) return;
                                    player.EmitSound(beepSound, filter);
                                    AddTimer(1f, () =>
                                    {
                                        if (player == null
                                            || !player.IsValid) return;
                                        player.EmitSound(beepSound, filter);
                                    });
                                });
                            });
                            // disable bomb spot
                            bombSite.AcceptInput("Disable");
                            // enable a second later again :)
                            AddTimer(1f, () =>
                            {
                                if (bombSite == null
                                    || !bombSite.IsValid) return;
                                bombSite.AcceptInput("Enable");
                            });
                        });
                        break;
                    }
                }
            }
            return HookResult.Continue;
        }

        private HookResult OnBombAbortplant(EventBombAbortplant @event, GameEventInfo info)
        {
            _ImpossibleBombPlantLastPlant = 0;
            return HookResult.Continue;
        }
    }
}
