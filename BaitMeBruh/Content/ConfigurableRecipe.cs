using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Vapok.Common.Managers.Configuration;

namespace BaitMeBruh.Content;

public class ConfigurableRecipe
{
    private readonly string _configSection;
    private readonly string _recipeName;
    private readonly string _resultItem;
    private readonly string _defaultStation;
    private readonly int _defaultMinLevel;
    private readonly int _defaultAmount;
    private readonly string _defaultRequirements;

    private CustomRecipe _customRecipe;
    private CustomItem _boundCustomItem;

    public ConfigEntry<bool> Enabled;
    public ConfigEntry<string> CraftingStation;
    public ConfigEntry<int> MinStationLevel;
    public ConfigEntry<string> CraftingCosts;
    public ConfigEntry<int> CraftAmount;

    public string RecipeName => _recipeName;
    public string ResultItem => _resultItem;
    public string ConfigSection => _configSection;
    public CustomRecipe CustomRecipe => _customRecipe;

    public ConfigurableRecipe(
        string configSection,
        string recipeName,
        string resultItem,
        string defaultStation,
        int defaultMinLevel,
        int defaultAmount,
        string defaultRequirements)
    {
        _configSection = configSection;
        _recipeName = recipeName;
        _resultItem = resultItem;
        _defaultStation = defaultStation;
        _defaultMinLevel = defaultMinLevel;
        _defaultAmount = defaultAmount;
        _defaultRequirements = defaultRequirements;

        RegisterConfiguration();
    }

    private void RegisterConfiguration()
    {
        int order = 10;

        ConfigSyncBase.SyncedConfig(
            _configSection,
            "Enabled",
            true,
            new ConfigDescription("Enables or disables this crafting recipe.", null, new ConfigurationManagerAttributes { Order = --order }),
            ref Enabled);

        ConfigSyncBase.SyncedConfig(
            _configSection,
            "Crafting Station",
            _defaultStation,
            new ConfigDescription("Crafting station required to craft this item (e.g. piece_workbench, piece_cauldron, forge, none).", null, new ConfigurationManagerAttributes { Order = --order }),
            ref CraftingStation);

        ConfigSyncBase.SyncedConfig(
            _configSection,
            "Min Station Level",
            _defaultMinLevel,
            new ConfigDescription("Minimum crafting station level required.", new AcceptableValueRange<int>(1, 10), new ConfigurationManagerAttributes { Order = --order }),
            ref MinStationLevel);

        ConfigSyncBase.SyncedConfig(
            _configSection,
            "Craft Amount",
            _defaultAmount,
            new ConfigDescription("Number of items produced per craft.", new AcceptableValueRange<int>(1, 100), new ConfigurationManagerAttributes { Order = --order }),
            ref CraftAmount);

        ConfigSyncBase.SyncedConfig(
            _configSection,
            "Crafting Costs",
            _defaultRequirements,
            new ConfigDescription("Item costs to craft in ItemName:Amount format, separated by commas (e.g. Wood:5,Stone:2).", null, new ConfigurationManagerAttributes { Order = --order }),
            ref CraftingCosts);

        Enabled.SettingChanged += OnSettingChanged;
        CraftingStation.SettingChanged += OnSettingChanged;
        MinStationLevel.SettingChanged += OnSettingChanged;
        CraftAmount.SettingChanged += OnSettingChanged;
        CraftingCosts.SettingChanged += OnSettingChanged;
    }

    public void RegisterWithJotunn()
    {
        RequirementConfig[] reqConfigs = ParseToRequirementConfigs(CraftingCosts.Value);

        RecipeConfig recipeConfig = new RecipeConfig
        {
            Name = _recipeName,
            Item = _resultItem,
            Amount = CraftAmount.Value,
            CraftingStation = GetStationString(),
            MinStationLevel = MinStationLevel.Value,
            Enabled = Enabled.Value,
            Requirements = reqConfigs
        };

        _customRecipe = new CustomRecipe(recipeConfig);
        Jotunn.Managers.ItemManager.Instance.AddRecipe(_customRecipe);
    }

    public void BindCustomItem(CustomItem customItem)
    {
        _boundCustomItem = customItem;
    }

    public string GetStationString()
    {
        string station = CraftingStation != null ? CraftingStation.Value : _defaultStation;
        if (string.IsNullOrEmpty(station) || string.Equals(station, "none", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (string.Equals(station, "disabled", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return station;
    }

    public RequirementConfig[] GetRequirementConfigs()
    {
        string costs = CraftingCosts != null ? CraftingCosts.Value : _defaultRequirements;
        return ParseToRequirementConfigs(costs);
    }

    private void OnSettingChanged(object sender, EventArgs args)
    {
        UpdateActiveRecipe();
    }

    public void UpdateActiveRecipe()
    {
        Recipe activeRecipe = null;
        if (_customRecipe != null && _customRecipe.Recipe != null)
        {
            activeRecipe = _customRecipe.Recipe;
        }
        else if (_boundCustomItem != null && _boundCustomItem.Recipe != null && _boundCustomItem.Recipe.Recipe != null)
        {
            activeRecipe = _boundCustomItem.Recipe.Recipe;
        }

        if (activeRecipe == null)
        {
            return;
        }

        bool isStationDisabled = string.Equals(CraftingStation.Value, "disabled", StringComparison.OrdinalIgnoreCase);
        activeRecipe.m_enabled = Enabled.Value && !isStationDisabled;
        activeRecipe.m_amount = CraftAmount.Value;
        activeRecipe.m_minStationLevel = MinStationLevel.Value;
        activeRecipe.m_craftingStation = ResolveStationComponent(CraftingStation.Value);
        activeRecipe.m_resources = ParseToPieceRequirements(CraftingCosts.Value);

        RefreshInventoryGui();
    }

    public static RequirementConfig[] ParseToRequirementConfigs(string costs)
    {
        if (string.IsNullOrWhiteSpace(costs))
        {
            return Array.Empty<RequirementConfig>();
        }

        List<RequirementConfig> list = new List<RequirementConfig>();
        string[] items = costs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string itemEntry in items)
        {
            string[] parts = itemEntry.Trim().Split(':');
            if (parts.Length == 0)
            {
                continue;
            }

            string itemName = parts[0].Trim();
            int amount = 1;
            if (parts.Length > 1)
            {
                int.TryParse(parts[1].Trim(), out amount);
            }

            if (!string.IsNullOrEmpty(itemName) && amount > 0)
            {
                list.Add(new RequirementConfig { Item = itemName, Amount = amount, Recover = false });
            }
        }

        return list.ToArray();
    }

    public static Piece.Requirement[] ParseToPieceRequirements(string costs)
    {
        if (string.IsNullOrWhiteSpace(costs))
        {
            return Array.Empty<Piece.Requirement>();
        }

        List<Piece.Requirement> list = new List<Piece.Requirement>();
        string[] items = costs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string itemEntry in items)
        {
            string[] parts = itemEntry.Trim().Split(':');
            if (parts.Length == 0)
            {
                continue;
            }

            string itemName = parts[0].Trim();
            int amount = 1;
            if (parts.Length > 1)
            {
                int.TryParse(parts[1].Trim(), out amount);
            }

            if (string.IsNullOrEmpty(itemName) || amount <= 0)
            {
                continue;
            }

            GameObject prefab = PrefabManager.Instance.GetPrefab(itemName);
            if (prefab == null && ObjectDB.instance != null)
            {
                prefab = ObjectDB.instance.GetItemPrefab(itemName);
            }

            if (prefab != null)
            {
                ItemDrop itemDrop = prefab.GetComponent<ItemDrop>();
                if (itemDrop != null)
                {
                    list.Add(new Piece.Requirement
                    {
                        m_resItem = itemDrop,
                        m_amount = amount,
                        m_amountPerLevel = 0,
                        m_recover = false
                    });
                }
            }
            else
            {
                BaitMeBruh.Log.Warning($"[ConfigurableRecipe] Item '{itemName}' not found in PrefabManager or ObjectDB.");
            }
        }

        return list.ToArray();
    }

    public static CraftingStation ResolveStationComponent(string stationName)
    {
        if (string.IsNullOrEmpty(stationName) ||
            string.Equals(stationName, "none", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(stationName, "disabled", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        GameObject stationPrefab = PrefabManager.Instance.GetPrefab(stationName);
        if (stationPrefab != null)
        {
            return stationPrefab.GetComponent<CraftingStation>();
        }

        return null;
    }

    private static void RefreshInventoryGui()
    {
        if (InventoryGui.instance != null && InventoryGui.IsVisible())
        {
            MethodInfo updateMethod = AccessTools.Method(typeof(InventoryGui), "UpdateCraftingPanel");
            if (updateMethod != null)
            {
                updateMethod.Invoke(InventoryGui.instance, new object[] { false });
            }
        }
    }
}
