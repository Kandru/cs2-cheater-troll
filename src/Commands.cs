using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CheaterTroll.Enums;
using CheaterTroll.Utils;
using CheaterTroll.Plugins;
using System.Text;
using System.Reflection;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {

        private readonly ClientConsoleMenu _clientConsoleMenu = new();
        private PlayerMenuState _serverConsoleState = new();

        private static readonly PropertyInfo[] _cheaterConfigProperties = typeof(CheaterConfig).GetProperties();
        private static readonly PropertyInfo[] _cheaterConfigFeaturesProperties = [.. _cheaterConfigProperties.Where(static p => p.Name != "Name")];
        private static readonly Dictionary<string, PropertyInfo[]> _typePropertiesCache = [];
        private static readonly object _propertiesCacheLock = new();

        private PlayerMenuState GetPlayerState(CCSPlayerController? player)
        {
            return player != null ? _clientConsoleMenu.GetPlayerState(player) : _serverConsoleState;
        }

        private PropertyInfo[] GetPropertiesByType(object configObject, Type propertyType)
        {
            Type objectType = configObject.GetType();
            string cacheKey = $"{objectType.FullName}_{propertyType.Name}";

            lock (_propertiesCacheLock)
            {
                if (!_typePropertiesCache.TryGetValue(cacheKey, out PropertyInfo[]? props))
                {
                    props = propertyType == typeof(bool)
                        ? [.. objectType.GetProperties().Where(static p => p.PropertyType == typeof(bool) && p.Name != "Enabled")]
                        : [.. objectType.GetProperties().Where(p => p.PropertyType == propertyType)];
                    _typePropertiesCache[cacheKey] = props;
                }
                return props;
            }
        }

        private PropertyInfo[] GetAllConfigProperties(object? configObject)
        {
            if (configObject == null)
            {
                return [];
            }

            PropertyInfo[] boolProps = GetPropertiesByType(configObject, typeof(bool));
            PropertyInfo[] floatProps = GetPropertiesByType(configObject, typeof(float));
            PropertyInfo[] intProps = GetPropertiesByType(configObject, typeof(int));
            PropertyInfo[] stringProps = GetPropertiesByType(configObject, typeof(string));

            return [.. boolProps, .. floatProps, .. intProps, .. stringProps];
        }

        [ConsoleCommand("cheater", "Set or unset player as a cheater")]
        [RequiresPermissions("@cheatertroll/admin")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER, minArgs: 0, usage: "<player>")]
        public void CommandManageCheater(CCSPlayerController? player, CommandInfo command)
        {
            bool isClientConsole = player != null && command.CallingContext == CommandCallingContext.Console;
            bool isServerConsole = player == null && command.CallingContext == CommandCallingContext.Console;

            if (!isClientConsole && !isServerConsole)
            {
                return;
            }

            if (isClientConsole && player != null)
            {
                HandleClientConsoleCommand(player, command);
            }
            else if (isServerConsole)
            {
                HandleServerConsoleCommand(command);
            }
        }

        private void HandleClientConsoleCommand(CCSPlayerController player, CommandInfo command)
        {
            string? arg = command.GetArg(1);
            PlayerMenuState state = _clientConsoleMenu.GetPlayerState(player);

            if (string.IsNullOrEmpty(arg))
            {
                _clientConsoleMenu.ResetPlayerState(player);
                ShowMainMenu(command, player);
            }
            else if (!string.IsNullOrEmpty(state.EditingPropertyName))
            {
                HandlePropertyEdit(command, player, state, command.GetCommandString[7..].TrimStart());
            }
            else if (int.TryParse(arg, out int choice))
            {
                HandleMenuChoice(command, player, choice);
            }
        }

        private void HandleServerConsoleCommand(CommandInfo command)
        {
            string? arg = command.GetArg(1);

            if (string.IsNullOrEmpty(arg))
            {
                _serverConsoleState = new PlayerMenuState();
                ShowMainMenu(command, null);
            }
            else if (!string.IsNullOrEmpty(_serverConsoleState.EditingPropertyName))
            {
                HandlePropertyEdit(command, null, _serverConsoleState, command.GetCommandString[7..].TrimStart());
            }
            else if (int.TryParse(arg, out int choice))
            {
                HandleMenuChoice(command, null, choice);
            }
        }

        private void HandleMenuChoice(CommandInfo command, CCSPlayerController? player, int choice)
        {
            PlayerMenuState state = GetPlayerState(player);

            if (choice == 0)
            {
                GoUpMenu(command, player, state);
                return;
            }

            switch (state.CurrentLevel)
            {
                case MenuLevel.Main:
                    HandleMainMenuChoice(command, player, state, choice);
                    break;
                case MenuLevel.PlayerConfig:
                    HandlePlayerConfigChoice(command, player, state, choice);
                    break;
                case MenuLevel.ConfigEntry:
                    HandleConfigEntryChoice(command, player, state, choice);
                    break;
                default:
                    break;
            }
        }

        private void GoUpMenu(CommandInfo command, CCSPlayerController? player, PlayerMenuState state)
        {
            if (state.CurrentLevel == MenuLevel.Main)
            {
                command.ReplyToCommand("Already at main menu.");
                return;
            }

            if (state.CurrentLevel == MenuLevel.ConfigEntry)
            {
                state.SelectedConfigProperty = null;
                state.CurrentLevel = MenuLevel.PlayerConfig;
                ShowPlayerConfigMenu(command, player, state.SelectedPlayerSteamId!);
            }
            else if (state.CurrentLevel == MenuLevel.PlayerConfig)
            {
                state.SelectedPlayer = null;
                state.SelectedPlayerSteamId = null;
                state.CurrentLevel = MenuLevel.Main;
                ShowMainMenu(command, player);
            }
        }

        private void HandleMainMenuChoice(CommandInfo command, CCSPlayerController? player, PlayerMenuState state, int choice)
        {
            List<(string steamId, CCSPlayerController? controller, Dictionary<PlayerData, string>? playerData)> allCheaters = GetAllCheatersForMenu();
            if (choice < 1 || choice > allCheaters.Count)
            {
                command.ReplyToCommand($"Invalid choice. Please select between 1 and {allCheaters.Count}.");
                ShowMainMenu(command, player);
                return;
            }

            (string steamId, CCSPlayerController? controller, Dictionary<PlayerData, string>? playerData) = allCheaters[choice - 1];
            state.SelectedPlayer = controller;
            state.SelectedPlayerSteamId = steamId;
            state.CurrentLevel = MenuLevel.PlayerConfig;
            ShowPlayerConfigMenu(command, player, steamId, playerData);
        }

        private void HandlePlayerConfigChoice(CommandInfo command, CCSPlayerController? player, PlayerMenuState state, int choice)
        {
            string steamId = state.SelectedPlayerSteamId!;

            if (choice == 1)
            {
                bool isExistingCheater = Config.Cheater.ContainsKey(steamId);

                if (isExistingCheater)
                {
                    _ = Config.Cheater.Remove(steamId);
                    if (state.SelectedPlayer != null)
                    {
                        DeleteCheaterConfig(state.SelectedPlayer);
                        RemovePlayerCheats(state.SelectedPlayer);
                    }
                    command.ReplyToCommand("✓ Cheater disabled");
                }
                else
                {
                    Config.Cheater.Add(steamId, new CheaterConfig());
                    if (state.SelectedPlayer != null)
                    {
                        EnablePlayerCheats(state.SelectedPlayer);
                    }
                    command.ReplyToCommand("✓ Cheater enabled");
                }

                ShowPlayerConfigMenu(command, player, steamId);
                return;
            }

            bool isCheater = Config.Cheater.ContainsKey(steamId);

            if (!isCheater)
            {
                command.ReplyToCommand("Player is not a cheater yet. Enable cheater mode first.");
                ShowPlayerConfigMenu(command, player, steamId);
                return;
            }

            if (choice < 2 || choice > _cheaterConfigFeaturesProperties.Length + 1)
            {
                command.ReplyToCommand($"Invalid choice.");
                ShowPlayerConfigMenu(command, player, steamId);
                return;
            }

            state.SelectedConfigProperty = _cheaterConfigFeaturesProperties[choice - 2].Name;
            state.CurrentLevel = MenuLevel.ConfigEntry;
            ShowConfigEntryMenu(command, player, steamId, state.SelectedConfigProperty);
        }

        private void HandleConfigEntryChoice(CommandInfo command, CCSPlayerController? player, PlayerMenuState state, int choice)
        {
            string steamId = state.SelectedPlayerSteamId!;
            object? configObject = typeof(CheaterConfig).GetProperty(state.SelectedConfigProperty!)!
                .GetValue(Config.Cheater[steamId]);

            if (choice == 1)
            {
                PropertyInfo enabledProperty = configObject!.GetType().GetProperty("Enabled")!;
                bool isEnabled = (bool)enabledProperty.GetValue(configObject)!;
                enabledProperty.SetValue(configObject, !isEnabled);
                if (state.SelectedPlayer != null)
                {
                    EnablePlayerCheats(state.SelectedPlayer);
                }
                command.ReplyToCommand($"✓ {state.SelectedConfigProperty} {(!isEnabled ? "enabled" : "disabled")}");
                ShowConfigEntryMenu(command, player, steamId, state.SelectedConfigProperty!);
                return;
            }

            PropertyInfo[] allProperties = GetAllConfigProperties(configObject);

            if (choice < 2 || choice > allProperties.Length + 1)
            {
                command.ReplyToCommand($"Invalid choice.");
                ShowConfigEntryMenu(command, player, steamId, state.SelectedConfigProperty!);
                return;
            }

            PropertyInfo selectedProp = allProperties[choice - 2];

            if (selectedProp.PropertyType == typeof(bool))
            {
                bool value = (bool)selectedProp.GetValue(configObject)!;
                selectedProp.SetValue(configObject, !value);
                if (state.SelectedPlayer != null)
                {
                    EnablePlayerCheats(state.SelectedPlayer);
                }
                command.ReplyToCommand($"✓ {selectedProp.Name} {(!value ? "enabled" : "disabled")}");
                ShowConfigEntryMenu(command, player, steamId, state.SelectedConfigProperty!);
            }
            else if (IsEnumStringProperty(selectedProp))
            {
                object currentValue = selectedProp.GetValue(configObject)!;
                Type enumType = GetEnumTypeForStringProperty(selectedProp)!;
                string[] enumValues = Enum.GetNames(enumType);

                state.EditingPropertyName = selectedProp.Name;
                command.ReplyToCommand($"Current value of {selectedProp.Name}: {currentValue}");
                command.ReplyToCommand($"Valid options: {string.Join(", ", enumValues)}");
                command.ReplyToCommand($"Enter new value (or type 'cancel' to abort):");
            }
            else
            {
                object currentValue = selectedProp.GetValue(configObject)!;
                state.EditingPropertyName = selectedProp.Name;
                command.ReplyToCommand($"Current value of {selectedProp.Name}: {currentValue}");
                if (selectedProp.PropertyType == typeof(float))
                {
                    command.ReplyToCommand($"Example: 5.5 (or type 'cancel' to abort):");
                }
                else if (selectedProp.PropertyType == typeof(int))
                {
                    command.ReplyToCommand($"Example: 42 (or type 'cancel' to abort):");
                }
                else
                {
                    command.ReplyToCommand($"Example: value (or type 'cancel' to abort):");
                }
            }
        }

        private void HandlePropertyEdit(CommandInfo command, CCSPlayerController? player, PlayerMenuState state, string input)
        {
            string steamId = state.SelectedPlayerSteamId!;

            if (input.Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                state.EditingPropertyName = null;
                command.ReplyToCommand("✓ Cancelled");
                ShowConfigEntryMenu(command, player, steamId, state.SelectedConfigProperty!);
                return;
            }
            object? configObject = typeof(CheaterConfig).GetProperty(state.SelectedConfigProperty!)!
                .GetValue(Config.Cheater[steamId]);

            PropertyInfo propertyInfo = configObject!.GetType().GetProperty(state.EditingPropertyName!)!;

            try
            {
                if (propertyInfo.PropertyType == typeof(float))
                {
                    if (!float.TryParse(input, out float floatValue))
                    {
                        command.ReplyToCommand("✗ Invalid float value. Please try again.");
                        command.ReplyToCommand($"Example: 5.5 (or type 'cancel' to abort):");
                        return;
                    }
                    propertyInfo.SetValue(configObject, floatValue);
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    if (!int.TryParse(input, out int intValue))
                    {
                        command.ReplyToCommand("✗ Invalid integer value. Please try again.");
                        command.ReplyToCommand($"Example: 42 (or type 'cancel' to abort):");
                        return;
                    }
                    propertyInfo.SetValue(configObject, intValue);
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    if (IsEnumStringProperty(propertyInfo))
                    {
                        Type enumType = GetEnumTypeForStringProperty(propertyInfo)!;
                        string[] validEnumNames = Enum.GetNames(enumType);
                        bool isValidEnum = validEnumNames.Any(n => n.Equals(input, StringComparison.OrdinalIgnoreCase));

                        if (!isValidEnum)
                        {
                            string validOptions = string.Join(", ", validEnumNames);
                            command.ReplyToCommand($"✗ Invalid value. Valid options: {validOptions}");
                            command.ReplyToCommand($"Enter new value (or type 'cancel' to abort):");
                            return;
                        }
                    }
                    propertyInfo.SetValue(configObject, input);
                }
                else
                {
                    command.ReplyToCommand($"✗ Unsupported property type: {propertyInfo.PropertyType.Name}");
                    state.EditingPropertyName = null;
                    ShowConfigEntryMenu(command, player, steamId, state.SelectedConfigProperty!);
                    return;
                }

                if (state.SelectedPlayer != null)
                {
                    EnablePlayerCheats(state.SelectedPlayer);
                }
                command.ReplyToCommand($"✓ {state.EditingPropertyName} set to: {propertyInfo.GetValue(configObject)}");
                state.EditingPropertyName = null;
                ShowConfigEntryMenu(command, player, steamId, state.SelectedConfigProperty!);
            }
            catch (Exception ex)
            {
                command.ReplyToCommand($"✗ Error setting property: {ex.Message}");
                state.EditingPropertyName = null;
                ShowConfigEntryMenu(command, player, steamId, state.SelectedConfigProperty!);
            }
        }

        private void ShowMainMenu(CommandInfo command, CCSPlayerController? player)
        {
            StringBuilder menu = new();
            _ = menu.AppendLine("=== Overview ===");

            List<(string steamId, CCSPlayerController? controller, Dictionary<PlayerData, string>? playerData)> allCheaters = GetAllCheatersForMenu();
            int index = 1;
            foreach ((string steamId, CCSPlayerController? controller, Dictionary<PlayerData, string>? playerData) in allCheaters)
            {
                bool isCheater = Config.Cheater.ContainsKey(steamId);
                string status = isCheater ? "✓" : "X";
                string displayText = controller != null && playerData != null
                    ? FormatPlayerOption(playerData)
                    : FormatOfflinePlayerOption(steamId);
                _ = menu.AppendLine($"{index}. {status} {displayText}");
                index++;
            }

            _ = menu.AppendLine("0. Exit");
            command.ReplyToCommand(menu.ToString());

            if (player != null)
            {
                PlayerMenuState state = _clientConsoleMenu.GetPlayerState(player);
                state.CurrentLevel = MenuLevel.Main;
            }
        }

        private List<(string steamId, CCSPlayerController? controller, Dictionary<PlayerData, string>? playerData)> GetAllCheatersForMenu()
        {
            List<(string steamId, CCSPlayerController controller, Dictionary<PlayerData, string> playerData, double timeOnline)> onlineCheaters = [];
            List<(string steamId, string playerName)> offlineCheaters = [];

            Dictionary<string, (CCSPlayerController, Dictionary<PlayerData, string>)> onlinePlayersBySteamId = [];
            foreach (KeyValuePair<CCSPlayerController, Dictionary<PlayerData, string>> kvp in _connectedPlayers)
            {
                string steamId = kvp.Value[PlayerData.STEAM_ID];
                onlinePlayersBySteamId[steamId] = (kvp.Key, kvp.Value);
            }

            foreach (string steamId in Config.Cheater.Keys)
            {
                if (onlinePlayersBySteamId.TryGetValue(steamId, out (CCSPlayerController, Dictionary<PlayerData, string>) playerData))
                {
                    double timeOnline = CalculateTimeOnline(playerData.Item2[PlayerData.TIMESTAMP]);
                    onlineCheaters.Add((steamId, playerData.Item1, playerData.Item2, timeOnline));
                }
                else
                {
                    offlineCheaters.Add((steamId, GetCheaterName(steamId)));
                }
            }

            onlineCheaters.Sort(static (a, b) => a.timeOnline.CompareTo(b.timeOnline));
            offlineCheaters.Sort(static (a, b) => a.playerName.CompareTo(b.playerName));

            List<(string, CCSPlayerController?, Dictionary<PlayerData, string>?)> result = [];
            foreach ((string? steamId, CCSPlayerController? controller, Dictionary<PlayerData, string>? playerData, double _) in onlineCheaters)
            {
                result.Add((steamId, controller, playerData));
            }

            foreach ((string? steamId, string _) in offlineCheaters)
            {
                result.Add((steamId, null, null));
            }

            return result;
        }

        private string GetCheaterName(string steamId)
        {
            foreach (KeyValuePair<CCSPlayerController, Dictionary<PlayerData, string>> p in _connectedPlayers)
            {
                if (p.Value[PlayerData.STEAM_ID] == steamId)
                {
                    return p.Value[PlayerData.PLAYER_NAME];
                }
            }
            return steamId;
        }

        private void ShowPlayerConfigMenu(CommandInfo command, CCSPlayerController? player, string steamId, Dictionary<PlayerData, string>? offlinePlayerData = null)
        {
            StringBuilder menu = new();
            bool isExistingCheater = Config.Cheater.ContainsKey(steamId);
            string playerName = offlinePlayerData != null ? offlinePlayerData[PlayerData.PLAYER_NAME] : "Unknown";

            if (offlinePlayerData == null && player != null)
            {
                PlayerMenuState state = _clientConsoleMenu.GetPlayerState(player);
                playerName = state.SelectedPlayer?.PlayerName ?? GetCheaterName(steamId);
            }

            _ = menu.AppendLine($"=== Overview -> {playerName} ===");

            if (isExistingCheater)
            {
                _ = menu.AppendLine("1. Disable Cheater");

                int index = 2;
                foreach (PropertyInfo? feature in _cheaterConfigFeaturesProperties)
                {
                    CheaterConfig configObject = Config.Cheater[steamId];
                    object? featureValue = feature.GetValue(configObject);
                    PropertyInfo? enabledProp = featureValue?.GetType().GetProperty("Enabled");
                    bool isEnabled = enabledProp != null && (bool)enabledProp.GetValue(featureValue)!;
                    string status = isEnabled ? "✓" : "X";
                    _ = menu.AppendLine($"{index}. {status} {feature.Name}");
                    index++;
                }
            }
            else
            {
                _ = menu.AppendLine("1. Enable Cheater");
            }

            _ = menu.AppendLine("0. Back");
            command.ReplyToCommand(menu.ToString());

            if (player != null)
            {
                PlayerMenuState state = _clientConsoleMenu.GetPlayerState(player);
                state.CurrentLevel = MenuLevel.PlayerConfig;
            }
        }

        private void ShowConfigEntryMenu(CommandInfo command, CCSPlayerController? player, string steamId, string configProperty)
        {
            object? configObject = typeof(CheaterConfig).GetProperty(configProperty)!
                .GetValue(Config.Cheater[steamId]);

            string playerName = GetCheaterName(steamId);
            StringBuilder menu = new();
            _ = menu.AppendLine($"=== Overview -> {playerName} -> {configProperty} ===");

            PropertyInfo enabledProperty = configObject!.GetType().GetProperty("Enabled")!;
            bool isEnabled = (bool)enabledProperty.GetValue(configObject)!;

            _ = menu.AppendLine($"1. {(isEnabled ? "Disable" : "Enable")}");

            PropertyInfo[] boolProperties = GetPropertiesByType(configObject, typeof(bool));
            PropertyInfo[] floatProperties = GetPropertiesByType(configObject, typeof(float));
            PropertyInfo[] intProperties = GetPropertiesByType(configObject, typeof(int));
            PropertyInfo[] stringProperties = GetPropertiesByType(configObject, typeof(string));

            int index = 2;
            foreach (PropertyInfo? prop in boolProperties)
            {
                bool value = (bool)prop.GetValue(configObject)!;
                string status = value ? "✓" : "X";
                _ = menu.AppendLine($"{index}. {status} {prop.Name}");
                index++;
            }

            foreach (PropertyInfo? prop in floatProperties)
            {
                float value = (float)prop.GetValue(configObject)!;
                _ = menu.AppendLine($"{index}. {prop.Name}: {value}");
                index++;
            }

            foreach (PropertyInfo? prop in intProperties)
            {
                int value = (int)prop.GetValue(configObject)!;
                _ = menu.AppendLine($"{index}. {prop.Name}: {value}");
                index++;
            }

            foreach (PropertyInfo? prop in stringProperties)
            {
                string value = (string?)prop.GetValue(configObject) ?? "null";
                string displayValue = value;

                if (IsEnumStringProperty(prop))
                {
                    Type? enumType = GetEnumTypeForStringProperty(prop);
                    string enumValues = string.Join("|", Enum.GetNames(enumType!));
                    displayValue = $"{value} ({enumValues})";
                }

                _ = menu.AppendLine($"{index}. {prop.Name}: {displayValue}");
                index++;
            }

            _ = menu.AppendLine("0. Back");
            command.ReplyToCommand(menu.ToString());

            PlayerMenuState state = GetPlayerState(player);
            state.CurrentLevel = MenuLevel.ConfigEntry;
            state.EditingPropertyName = null;
        }



        private string FormatPlayerOption(Dictionary<PlayerData, string> playerData)
        {
            string name = playerData[PlayerData.PLAYER_NAME];
            string steamId = playerData[PlayerData.STEAM_ID];
            double timeOnline = CalculateTimeOnline(playerData[PlayerData.TIMESTAMP]);

            return $"{name} ({steamId}, {timeOnline} min online)";
        }

        private string FormatOfflinePlayerOption(string steamId)
        {
            string playerName = GetCheaterName(steamId);
            return $"{playerName} ({steamId}, offline)";
        }

        private double CalculateTimeOnline(string timestamp)
        {
            return double.TryParse(timestamp, out double timestampValue) ? Math.Round((Server.CurrentTime - timestampValue) / 60, 2) : 0;
        }

        private bool IsEnumStringProperty(PropertyInfo prop)
        {
            if (prop.PropertyType != typeof(string))
            {
                return false;
            }

            Type? enumType = GetEnumTypeForStringProperty(prop);
            return enumType != null;
        }

        private Type? GetEnumTypeForStringProperty(PropertyInfo prop)
        {
            string propName = prop.Name;

            if (propName.EndsWith("String", StringComparison.OrdinalIgnoreCase))
            {
                string enumPropertyName = propName[..^6];
                PropertyInfo? enumProperty = prop.DeclaringType?.GetProperty(enumPropertyName);

                if (enumProperty?.PropertyType.IsEnum == true)
                {
                    return enumProperty.PropertyType;
                }
            }

            return null;
        }

        private void EnableAllPlayerCheats()
        {
            foreach (KeyValuePair<CCSPlayerController, Dictionary<PlayerData, string>> kvp in _connectedPlayers)
            {
                if (CheckIfCheater(kvp.Key))
                {
                    EnablePlayerCheats(kvp.Key);
                }
            }
        }

        private void EnablePlayerCheats(CCSPlayerController player)
        {
            if (!CheckIfCheater(player))
            {
                return;
            }
            string steamid = player.SteamID.ToString();
            // remove all cheats
            RemovePlayerCheats(player);
            // add active cheats
            foreach (PluginBlueprint plugin in _plugins)
            {
                PropertyInfo? configProperty = typeof(CheaterConfig).GetProperty(plugin.ClassName);
                // ignore if no config exists for this plugin
                if (configProperty == null)
                {
                    continue;
                }
                object? pluginConfig = configProperty.GetValue(Config.Cheater[steamid]);
                // check if player has given config entry
                if (pluginConfig == null)
                {
                    continue;
                }
                // check if cheat is enabled for given player
                PropertyInfo? enabledProperty = pluginConfig.GetType().GetProperty("Enabled");
                if (enabledProperty != null && (bool)enabledProperty.GetValue(pluginConfig)!)
                {
                    plugin.Add(player, Config.Cheater[steamid]);
                }
            }
        }

        private void RemovePlayerCheats(CCSPlayerController player)
        {
            foreach (PluginBlueprint plugin in _plugins)
            {
                if (plugin._players.ContainsKey(player))
                {
                    plugin.Remove(player);
                }
            }
        }
    }
}
