using System;
using BepInEx.Configuration;
using UnityEngine;
using Vapok.Common.Abstractions;
using Vapok.Common.Managers.Configuration;

namespace BaitMeBruh.Configuration;

public class ConfigRegistry : ConfigSyncBase
{
    internal static ConfigEntry<bool> Enabled;
    public static ConfigEntry<float> HudHorizontalOffset;
    public static ConfigEntry<float> HudVerticalOffset;
    public static ConfigEntry<KeyCode> SwitchBaitKey;
    public static ConfigEntry<float> CreelProximityDistance;
    public static ConfigEntry<int> CreelBaitPerChum;
    public static ConfigEntry<float> CreelMinutesPerBait;

    public static Waiting Waiter;

    public ConfigRegistry(IPluginInfo mod, bool enableLockedConfigs = false) : base(mod, enableLockedConfigs)
    {
        Waiter = new Waiting();

        InitializeConfigurationSettings();
    }

    public sealed override void InitializeConfigurationSettings()
    {
        if (_config == null)
            return;

        SyncedConfig("Server Settings", "Enable BaitMeBruh", true,
            new ConfigDescription("If enabled, enables BaitMeBruh features.",
                null, new ConfigurationManagerAttributes { IsAdminOnly = true, Order = 1 }), ref Enabled);

        SyncedConfig("UI Settings", "HUD Horizontal Offset", -300.0f,
            new ConfigDescription("Horizontal pixel offset of the fishing tension and retrieval HUD relative to screen center. Negative values offset to the left.",
                null, new ConfigurationManagerAttributes { Order = 2 }), ref HudHorizontalOffset, synchronizedSetting: false);

        SyncedConfig("UI Settings", "HUD Vertical Offset", -250.0f,
            new ConfigDescription("Vertical pixel offset of the fishing tension and retrieval HUD relative to screen center. Negative values offset downward.",
                null, new ConfigurationManagerAttributes { Order = 3 }), ref HudVerticalOffset, synchronizedSetting: false);

        SyncedConfig("UI Settings", "Switch Bait Key", KeyCode.G,
            new ConfigDescription("Key to cycle through fishing bait types in inventory while holding a fishing rod before casting.",
                null, new ConfigurationManagerAttributes { Order = 4 }), ref SwitchBaitKey, synchronizedSetting: false);

        SyncedConfig("Trap Settings", "Bait Creel Proximity Distance", 20.0f,
            new ConfigDescription("Minimum distance (in meters) required between Bait Creels before waters become overcrowded.",
                null, new ConfigurationManagerAttributes { IsAdminOnly = true, Order = 4 }), ref CreelProximityDistance);

        SyncedConfig("Trap Settings", "Bait Produced Per Chum", 3,
            new ConfigDescription("Number of bait yields produced per Neck Tail added as chum to the Bait Creel.",
                null, new ConfigurationManagerAttributes { IsAdminOnly = true, Order = 5 }), ref CreelBaitPerChum);

        SyncedConfig("Trap Settings", "Bait Creel Minutes Per Bait", 5.0f,
            new ConfigDescription("Real-world minutes required for the Bait Creel to produce one unit of bait.",
                null, new ConfigurationManagerAttributes { IsAdminOnly = true, Order = 6 }), ref CreelMinutesPerBait);
    }
}

public class Waiting
{
    public void ConfigurationComplete(bool configDone)
    {
        if (configDone)
            StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler StatusChanged;
}
