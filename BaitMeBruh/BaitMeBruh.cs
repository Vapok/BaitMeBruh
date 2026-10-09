/* BaitMeBruh by Vapok */

using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn.Managers;
using BaitMeBruh.Configuration;
using BaitMeBruh.Content;
using BaitMeBruh.Content.Factories;
using Vapok.Common.Abstractions;
using Vapok.Common.Managers;
using Vapok.Common.Managers.Configuration;
using Vapok.Common.Managers.LocalizationManager;
using Vapok.Common.Managers.Splash;
using Vapok.Common.Tools;

namespace BaitMeBruh;

[BepInPlugin(_pluginId, _displayName, _version)]
[BepInDependency(Jotunn.Main.ModGuid)]
[BepInDependency("com.ValheimModding.YamlDotNetDetector")]
public class BaitMeBruh : BaseUnityPlugin, IPluginInfo
{
    private const string _pluginId = "vapok.mods.BaitMeBruh";
    private const string _displayName = "BaitMeBruh!";
    private const string _version = "1.0.0";
    public static bool ValheimAwake;
    public static Waiting Waiter;

    private static BaitMeBruh _instance;
    private static ConfigSyncBase _config;
    private static ILogIt _log;
    private Harmony _harmony;

    public static ILogIt Log => _log;

    [UsedImplicitly]
    private void Awake()
    {
        _instance = this;

        Waiter = new Waiting();

        Jotunn.Entities.CustomLocalization localization = LocalizationManager.Instance.GetLocalization();

        LogManager.Init(PluginId, out _log);

        Initializer.LoadManagers(localization);

        _config = new ConfigRegistry(_instance);

        ModSplashManager.Register(new ModSplashDossier(_instance)
        {
            Tagline = "An immersive fishing overhaul featuring dynamic line tension, rod progression, coastal traps, and deep-sea angling.",
            ShowOnStartup = ConfigRegistry.ShowSplashOnStartup,
        });

        RodManager.Initialize();
        NetKitManager.Initialize();
        TrapPieceManager.Initialize();

        BaitFactory baitFactory = new BaitFactory(_log, _config);
        baitFactory.CreateAssets();

        FoodFactory foodFactory = new FoodFactory(_log, _config);
        foodFactory.CreateAssets();

        Localizer.Waiter.StatusChanged += InitializeModule;

        _harmony = new Harmony(Info.Metadata.GUID);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());

        if (GUIManager.IsHeadless())
        {
            InitializeModule(this, EventArgs.Empty);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    public string PluginId => _pluginId;
    public string DisplayName => _displayName;
    public string Version => _version;
    public BaseUnityPlugin Instance => _instance;

    public void InitializeModule(object sender, EventArgs args)
    {
        if (ValheimAwake)
            return;

        ConfigRegistry.Waiter.ConfigurationComplete(true);

        ValheimAwake = true;
    }

    public class Waiting
    {
        public void ValheimIsAwake(bool awakeFlag)
        {
            if (awakeFlag)
                StatusChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler StatusChanged;
    }
}
