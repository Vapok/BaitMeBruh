using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Vapok.Common.Abstractions;
using Vapok.Common.Managers.Configuration;

namespace BaitMeBruh.Content.Factories;

internal class FoodFactory : AssetFactory
{
    private static bool _registered;

    private static ConfigurableRecipe _chowderRecipe;
    private static readonly List<ConfigurableRecipe> _cauldronRecipes = new List<ConfigurableRecipe>();

    private static ConfigEntry<float> _chowderHealth;
    private static ConfigEntry<float> _chowderStamina;
    private static ConfigEntry<float> _chowderBurnTime;
    private static ConfigEntry<float> _chowderHealthRegen;

    private static ConfigEntry<float> _chowderSneakModifier;
    private static ConfigEntry<float> _chowderEffectDuration;

    private static ConfigEntry<float> _campfireCookTime;

    private static CustomItem _customChowder;
    private static SE_Stats _sneakEffect;
    private static readonly List<CookingStation.ItemConversion> _cookingConversions = new List<CookingStation.ItemConversion>();

    internal static ConfigurableRecipe ChowderRecipe => _chowderRecipe;
    internal static IReadOnlyList<ConfigurableRecipe> CauldronRecipes => _cauldronRecipes;

    internal FoodFactory(ILogIt logger, ConfigSyncBase configs) : base(logger, configs)
    {
    }

    internal override void CreateAssets()
    {
        RegisterConfigurations();

        PrefabManager.OnVanillaPrefabsAvailable += RegisterAllCulinaryContent;
    }

    private void RegisterConfigurations()
    {
        if (_chowderRecipe == null)
        {
            _chowderRecipe = new ConfigurableRecipe(
                "Recipe: Trollfish Chowder",
                "Recipe_TrollfishChowder",
                "TrollfishChowder",
                "piece_cauldron",
                1,
                1,
                "Fish5:1,MushroomYellow:2");
        }

        if (_cauldronRecipes.Count == 0)
        {
            _cauldronRecipes.Add(new ConfigurableRecipe(
                "Recipe: Frostbite Mead Base",
                "Recipe_FrostBiteMeadBase",
                "MeadBaseFrostResist",
                "piece_cauldron",
                1,
                1,
                "Fish4_cave:1,Honey:10,Thistle:5,WolfFang:1"));

            _cauldronRecipes.Add(new ConfigurableRecipe(
                "Recipe: Black Soup (Fish Broth)",
                "Recipe_SwampFishBroth",
                "BlackSoup",
                "piece_cauldron",
                2,
                2,
                "Fish6:1,Honey:2,Turnip:2"));

            _cauldronRecipes.Add(new ConfigurableRecipe(
                "Recipe: Mariner's Mead Base",
                "Recipe_MarinersMeadBase",
                "MeadBaseSwimmer",
                "piece_cauldron",
                2,
                1,
                "Fish3:1,Honey:10,Cloudberry:2"));

            _cauldronRecipes.Add(new ConfigurableRecipe(
                "Recipe: Puffer Poison Mead Base",
                "Recipe_PufferPoisonMeadBase",
                "MeadBasePoisonResist",
                "piece_cauldron",
                2,
                1,
                "Fish12:1,Honey:10,Thistle:4"));

            _cauldronRecipes.Add(new ConfigurableRecipe(
                "Recipe: Glowfin Eitr Mead Base",
                "Recipe_GlowfinEitrMeadBase",
                "MeadBaseEitrMinor",
                "piece_cauldron",
                4,
                1,
                "Fish9:1,Honey:10,Magecap:2"));
        }

        int foodOrder = 10;
        ConfigSyncBase.SyncedConfig(
            "Food: Trollfish Chowder",
            "Health",
            15f,
            new ConfigDescription("Health bonus granted by Trollfish Chowder.", new AcceptableValueRange<float>(1f, 250f), new ConfigurationManagerAttributes { Order = --foodOrder }),
            ref _chowderHealth);

        ConfigSyncBase.SyncedConfig(
            "Food: Trollfish Chowder",
            "Stamina",
            45f,
            new ConfigDescription("Stamina bonus granted by Trollfish Chowder.", new AcceptableValueRange<float>(1f, 250f), new ConfigurationManagerAttributes { Order = --foodOrder }),
            ref _chowderStamina);

        ConfigSyncBase.SyncedConfig(
            "Food: Trollfish Chowder",
            "Duration",
            1200f,
            new ConfigDescription("Duration in seconds the food remains active in the stomach (1200s = 20 minutes).", new AcceptableValueRange<float>(60f, 3600f), new ConfigurationManagerAttributes { Order = --foodOrder }),
            ref _chowderBurnTime);

        ConfigSyncBase.SyncedConfig(
            "Food: Trollfish Chowder",
            "Health Regen",
            2f,
            new ConfigDescription("Health regeneration per tick provided by Trollfish Chowder.", new AcceptableValueRange<float>(0.5f, 10f), new ConfigurationManagerAttributes { Order = --foodOrder }),
            ref _chowderHealthRegen);

        _chowderHealth.SettingChanged += OnFoodStatsChanged;
        _chowderStamina.SettingChanged += OnFoodStatsChanged;
        _chowderBurnTime.SettingChanged += OnFoodStatsChanged;
        _chowderHealthRegen.SettingChanged += OnFoodStatsChanged;

        int effectOrder = 10;
        ConfigSyncBase.SyncedConfig(
            "Effect: Trollfish Chowder (Troll's Guile)",
            "Sneak Skill Modifier",
            10f,
            new ConfigDescription("Sneak skill level modifier granted while the effect is active.", new AcceptableValueRange<float>(1f, 100f), new ConfigurationManagerAttributes { Order = --effectOrder }),
            ref _chowderSneakModifier);

        ConfigSyncBase.SyncedConfig(
            "Effect: Trollfish Chowder (Troll's Guile)",
            "Effect Duration",
            1200f,
            new ConfigDescription("Duration in seconds of the Troll's Guile sneak bonus effect.", new AcceptableValueRange<float>(60f, 3600f), new ConfigurationManagerAttributes { Order = --effectOrder }),
            ref _chowderEffectDuration);

        _chowderSneakModifier.SettingChanged += OnEffectConfigChanged;
        _chowderEffectDuration.SettingChanged += OnEffectConfigChanged;

        ConfigSyncBase.SyncedConfig(
            "Cooking: Campfire Fish",
            "Cook Time",
            25f,
            new ConfigDescription("Cooking time in seconds required to spit-roast whole fish on a cooking station.", new AcceptableValueRange<float>(5f, 120f), new ConfigurationManagerAttributes { Order = 1 }),
            ref _campfireCookTime);

        _campfireCookTime.SettingChanged += OnCampfireCookingChanged;
    }

    private static void RegisterAllCulinaryContent()
    {
        if (_registered)
        {
            return;
        }

        _registered = true;

        RegisterTrollfishChowder();
        RegisterCauldronRecipes();
        ConfigureCampfireCooking();
        UpdateItemDescriptions();
    }

    private static void RegisterTrollfishChowder()
    {
        Sprite chowderIcon = LoadEmbeddedSprite("BaitMeBruh.Assets.Icons.trollfish_chowder.png");
        AssetBundle bundle = TrapAssetManager.GetBundle();

        _sneakEffect = ScriptableObject.CreateInstance<SE_Stats>();
        _sneakEffect.name = "SE_TrollfishChowder";
        _sneakEffect.m_name = "$se_trollfish_chowder";
        _sneakEffect.m_icon = chowderIcon;
        _sneakEffect.m_ttl = _chowderEffectDuration != null ? _chowderEffectDuration.Value : 1200f;
        _sneakEffect.m_tooltip = "$se_trollfish_chowder_tooltip";
        _sneakEffect.m_skillLevel = Skills.SkillType.Sneak;
        _sneakEffect.m_skillLevelModifier = _chowderSneakModifier != null ? _chowderSneakModifier.Value : 10f;

        Jotunn.Managers.ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(_sneakEffect, false));

        ItemConfig chowderItemConfig = new ItemConfig
        {
            Name = "$item_trollfish_chowder",
            Description = "$item_trollfish_chowder_desc",
            CraftingStation = _chowderRecipe.GetStationString(),
            MinStationLevel = _chowderRecipe.MinStationLevel.Value,
            Icons = chowderIcon != null ? new[] { chowderIcon } : null,
            Requirements = _chowderRecipe.GetRequirementConfigs()
        };

        _customChowder = new CustomItem("TrollfishChowder", "CarrotSoup", chowderItemConfig);
        _chowderRecipe.BindCustomItem(_customChowder);

        if (_customChowder.ItemPrefab != null)
        {
            ItemDrop itemDrop = _customChowder.ItemDrop;
            if (itemDrop != null)
            {
                ItemDrop.ItemData.SharedData shared = itemDrop.m_itemData.m_shared;
                shared.m_name = "$item_trollfish_chowder";
                shared.m_description = "$item_trollfish_chowder_desc";
                shared.m_itemType = ItemDrop.ItemData.ItemType.Consumable;
                shared.m_food = _chowderHealth != null ? _chowderHealth.Value : 15f;
                shared.m_foodStamina = _chowderStamina != null ? _chowderStamina.Value : 45f;
                shared.m_foodBurnTime = _chowderBurnTime != null ? _chowderBurnTime.Value : 1200f;
                shared.m_foodRegen = _chowderHealthRegen != null ? _chowderHealthRegen.Value : 2f;
                shared.m_weight = 1.0f;
                shared.m_maxStackSize = 10;
                shared.m_consumeStatusEffect = _sneakEffect;
                if (chowderIcon != null)
                {
                    shared.m_icons = new[] { chowderIcon };
                }

                RegisterItemStandSupport(itemDrop);
            }

            SetupChowderDropVisual(_customChowder.ItemPrefab, bundle);
        }

        Jotunn.Managers.ItemManager.Instance.AddItem(_customChowder);
    }

    private static void RegisterCauldronRecipes()
    {
        foreach (ConfigurableRecipe recipe in _cauldronRecipes)
        {
            recipe.RegisterWithJotunn();
        }
    }

    private static void ConfigureCampfireCooking()
    {
        GameObject stationPrefab = PrefabManager.Cache.GetPrefab<GameObject>("piece_cookingstation");
        if (stationPrefab == null)
        {
            return;
        }

        CookingStation cs = stationPrefab.GetComponent<CookingStation>();
        if (cs == null || cs.m_conversion == null)
        {
            return;
        }

        ItemDrop fishCooked = PrefabManager.Cache.GetPrefab<ItemDrop>("FishCooked");
        ItemDrop fish1 = PrefabManager.Cache.GetPrefab<ItemDrop>("Fish1");
        ItemDrop fish2 = PrefabManager.Cache.GetPrefab<ItemDrop>("Fish2");

        if (fishCooked == null || fish1 == null || fish2 == null)
        {
            return;
        }

        float cookTime = _campfireCookTime != null ? _campfireCookTime.Value : 25f;

        CookingStation.ItemConversion conv1 = new CookingStation.ItemConversion
        {
            m_from = fish1,
            m_to = fishCooked,
            m_cookTime = cookTime
        };

        CookingStation.ItemConversion conv2 = new CookingStation.ItemConversion
        {
            m_from = fish2,
            m_to = fishCooked,
            m_cookTime = cookTime
        };

        cs.m_conversion.Add(conv1);
        cs.m_conversion.Add(conv2);

        _cookingConversions.Add(conv1);
        _cookingConversions.Add(conv2);
    }

    private static void OnFoodStatsChanged(object sender, EventArgs args)
    {
        if (_customChowder == null || _customChowder.ItemDrop == null)
        {
            return;
        }

        ItemDrop.ItemData.SharedData shared = _customChowder.ItemDrop.m_itemData.m_shared;
        if (shared != null)
        {
            shared.m_food = _chowderHealth != null ? _chowderHealth.Value : 15f;
            shared.m_foodStamina = _chowderStamina != null ? _chowderStamina.Value : 45f;
            shared.m_foodBurnTime = _chowderBurnTime != null ? _chowderBurnTime.Value : 1200f;
            shared.m_foodRegen = _chowderHealthRegen != null ? _chowderHealthRegen.Value : 2f;
        }
    }

    private static void OnEffectConfigChanged(object sender, EventArgs args)
    {
        if (_sneakEffect != null)
        {
            _sneakEffect.m_skillLevelModifier = _chowderSneakModifier != null ? _chowderSneakModifier.Value : 10f;
            _sneakEffect.m_ttl = _chowderEffectDuration != null ? _chowderEffectDuration.Value : 1200f;
        }
    }

    private static void OnCampfireCookingChanged(object sender, EventArgs args)
    {
        float newCookTime = _campfireCookTime != null ? _campfireCookTime.Value : 25f;
        foreach (CookingStation.ItemConversion conv in _cookingConversions)
        {
            conv.m_cookTime = newCookTime;
        }
    }

    private static void SetupChowderDropVisual(GameObject itemPrefab, AssetBundle bundle)
    {
        if (itemPrefab == null || bundle == null)
        {
            return;
        }

        GameObject visualPrefab = bundle.LoadAsset<GameObject>("drop_trollfish_chowder");
        if (visualPrefab == null)
        {
            BaitMeBruh.Log.Warning("[FoodFactory] 'drop_trollfish_chowder' prefab not found in bundle.");
            return;
        }

        Transform existingVisual = itemPrefab.transform.Find("visual");
        Material carrotSoupMaterial = null;
        if (existingVisual != null)
        {
            Renderer existingRenderer = existingVisual.GetComponentInChildren<Renderer>(true);
            if (existingRenderer != null)
            {
                carrotSoupMaterial = existingRenderer.sharedMaterial;
            }
            UnityEngine.Object.Destroy(existingVisual.gameObject);
        }

        GameObject visualInstance = UnityEngine.Object.Instantiate(visualPrefab, itemPrefab.transform);
        visualInstance.name = "visual";
        visualInstance.transform.localPosition = Vector3.zero;
        visualInstance.transform.localRotation = Quaternion.identity;
        visualInstance.transform.localScale = Vector3.one;

        Shader litShader = FindGameShader("Custom/Piece");
        if (litShader == null)
        {
            litShader = FindGameShader("Standard");
        }

        SetupVisualShaders(visualInstance, litShader, carrotSoupMaterial);

        Collider col = itemPrefab.GetComponent<Collider>();
        if (col != null)
        {
            UnityEngine.Object.Destroy(col);
        }

        BoxCollider box = itemPrefab.AddComponent<BoxCollider>();
        box.size = new Vector3(0.36f, 0.165f, 0.36f);
        box.center = new Vector3(0f, 0.0825f, 0f);

        Transform attach = itemPrefab.transform.Find("attach");
        if (attach == null)
        {
            GameObject attachObj = new GameObject("attach");
            attachObj.transform.SetParent(itemPrefab.transform, false);
            attachObj.transform.localPosition = new Vector3(0f, 0.005f, 0f);
            attachObj.transform.localRotation = Quaternion.identity;
        }
        else
        {
            attach.localPosition = new Vector3(0f, 0.005f, 0f);
        }
    }

    private static void SetupVisualShaders(GameObject visual, Shader litShader, Material sourceMaterial)
    {
        if (visual == null)
        {
            return;
        }

        int itemLayer = LayerMask.NameToLayer("item");
        if (itemLayer >= 0)
        {
            visual.layer = itemLayer;
            foreach (Transform child in visual.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = itemLayer;
            }
        }

        Renderer[] visualRenderers = visual.GetComponentsInChildren<Renderer>(true);
        for (int r = 0; r < visualRenderers.Length; r++)
        {
            visualRenderers[r].enabled = true;
            Material[] mats = visualRenderers[r].sharedMaterials;
            for (int m = 0; m < mats.Length; m++)
            {
                if (mats[m] != null)
                {
                    if (litShader != null)
                    {
                        mats[m].shader = litShader;
                    }
                    else if (sourceMaterial != null && sourceMaterial.shader != null)
                    {
                        mats[m].shader = sourceMaterial.shader;
                    }

                    if (visualRenderers[r].gameObject.name == "Soup")
                    {
                        mats[m].color = new Color(1f, 0.98f, 0.92f, 1f);
                    }
                    else if (visualRenderers[r].gameObject.name == "Bowl")
                    {
                        mats[m].color = new Color(1f, 0.96f, 0.90f, 1f);
                    }
                    else
                    {
                        mats[m].color = Color.white;
                    }
                }
            }
            visualRenderers[r].sharedMaterials = mats;
        }
    }

    private static Shader FindGameShader(string shaderName)
    {
        Shader[] shaders = Resources.FindObjectsOfTypeAll<Shader>();
        for (int i = 0; i < shaders.Length; i++)
        {
            if (shaders[i] != null && shaders[i].name == shaderName)
            {
                return shaders[i];
            }
        }
        return Shader.Find(shaderName);
    }

    private static void RegisterItemStandSupport(ItemDrop itemDrop)
    {
        if (itemDrop == null)
        {
            return;
        }

        GameObject standH = PrefabManager.Instance.GetPrefab("itemstandh");
        if (standH != null)
        {
            ItemStand standComp = standH.GetComponent<ItemStand>();
            if (standComp != null && standComp.m_supportedItems != null)
            {
                if (!standComp.m_supportedItems.Contains(itemDrop))
                {
                    standComp.m_supportedItems.Add(itemDrop);
                }
            }
        }
    }

    private static Sprite LoadEmbeddedSprite(string resourceName)
    {
        System.Reflection.Assembly assembly = typeof(FoodFactory).Assembly;
        using System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            return null;
        }

        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);

        Texture2D texture = Jotunn.Utils.AssetUtils.LoadImage(buffer);
        if (texture != null)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        }

        return null;
    }

    private static void UpdateItemDescriptions()
    {
        ItemDrop fish1 = PrefabManager.Cache.GetPrefab<ItemDrop>("Fish1");
        if (fish1 != null)
        {
            fish1.m_itemData.m_shared.m_description = "$item_fish1_desc";
        }

        ItemDrop fish2 = PrefabManager.Cache.GetPrefab<ItemDrop>("Fish2");
        if (fish2 != null)
        {
            fish2.m_itemData.m_shared.m_description = "$item_fish2_desc";
        }

        ItemDrop bait = PrefabManager.Cache.GetPrefab<ItemDrop>("FishingBait");
        if (bait != null)
        {
            bait.m_itemData.m_shared.m_description = "$item_fishingbait_desc";
        }
    }
}
