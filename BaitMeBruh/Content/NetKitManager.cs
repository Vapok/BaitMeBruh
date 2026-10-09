using UnityEngine;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;

namespace BaitMeBruh.Content;

public static class NetKitManager
{
    private static bool _registered;
    private static ConfigurableRecipe _coastalKitRecipe;
    private static ConfigurableRecipe _deepKitRecipe;

    internal static ConfigurableRecipe CoastalKitRecipe => _coastalKitRecipe;
    internal static ConfigurableRecipe DeepKitRecipe => _deepKitRecipe;

    public static void Initialize()
    {
        _coastalKitRecipe = new ConfigurableRecipe(
            "Recipe: Coastal Fish Net Kit",
            "Recipe_ItemFishnetCoastal",
            "ItemFishnetCoastal",
            "piece_workbench",
            1,
            1,
            "RoundLog:15,BronzeNails:18,TrollHide:4,Stone:4");

        _deepKitRecipe = new ConfigurableRecipe(
            "Recipe: Deep-Sea Anchored Net Kit",
            "Recipe_ItemFishnetDeep",
            "ItemFishnetDeep",
            "forge",
            1,
            1,
            "ElderBark:15,IronNails:12,Chain:4,Guck:4");

        PrefabManager.OnVanillaPrefabsAvailable += RegisterNetKits;
    }

    private static void RegisterNetKits()
    {
        if (_registered)
        {
            return;
        }

        _registered = true;

        AssetBundle bundle = TrapAssetManager.GetBundle();
        Sprite coastalIcon = TrapAssetManager.GetSprite("I_FishnetCoastal");
        Sprite deepIcon = TrapAssetManager.GetSprite("I_FishnetDeep");

        RegisterCoastalKit(bundle, coastalIcon);
        RegisterDeepKit(bundle, deepIcon);
    }

    private static void RegisterCoastalKit(AssetBundle bundle, Sprite icon)
    {
        ItemConfig coastalConfig = new ItemConfig
        {
            Name = "$item_fishnet_coastal",
            Description = "$item_fishnet_coastal_desc",
            CraftingStation = _coastalKitRecipe.GetStationString(),
            MinStationLevel = _coastalKitRecipe.MinStationLevel.Value,
            Icons = icon != null ? new[] { icon } : null,
            Requirements = _coastalKitRecipe.GetRequirementConfigs()
        };

        CustomItem coastalKit = new CustomItem("ItemFishnetCoastal", "LinenThread", coastalConfig);
        _coastalKitRecipe.BindCustomItem(coastalKit);

        if (coastalKit.ItemPrefab != null)
        {
            ItemDrop itemDrop = coastalKit.ItemDrop;
            if (itemDrop != null)
            {
                itemDrop.m_itemData.m_shared.m_weight = 25.0f;
                itemDrop.m_itemData.m_shared.m_maxStackSize = 1;
                itemDrop.m_itemData.m_shared.m_itemType = ItemDrop.ItemData.ItemType.Misc;
                if (icon != null)
                {
                    itemDrop.m_itemData.m_shared.m_icons = new Sprite[] { icon };
                }
            }

            SetupCrateVisual(coastalKit.ItemPrefab, bundle, "crate_fishnet_coastal", new Vector3(0.55f, 0.40f, 0.45f), new Vector3(0f, 0.20f, 0f));
        }

        Jotunn.Managers.ItemManager.Instance.AddItem(coastalKit);
    }

    private static void RegisterDeepKit(AssetBundle bundle, Sprite icon)
    {
        ItemConfig deepConfig = new ItemConfig
        {
            Name = "$item_fishnet_deep",
            Description = "$item_fishnet_deep_desc",
            CraftingStation = _deepKitRecipe.GetStationString(),
            MinStationLevel = _deepKitRecipe.MinStationLevel.Value,
            Icons = icon != null ? new[] { icon } : null,
            Requirements = _deepKitRecipe.GetRequirementConfigs()
        };

        CustomItem deepKit = new CustomItem("ItemFishnetDeep", "LinenThread", deepConfig);
        _deepKitRecipe.BindCustomItem(deepKit);

        if (deepKit.ItemPrefab != null)
        {
            ItemDrop itemDrop = deepKit.ItemDrop;
            if (itemDrop != null)
            {
                itemDrop.m_itemData.m_shared.m_weight = 50.0f;
                itemDrop.m_itemData.m_shared.m_maxStackSize = 1;
                itemDrop.m_itemData.m_shared.m_itemType = ItemDrop.ItemData.ItemType.Misc;
                if (icon != null)
                {
                    itemDrop.m_itemData.m_shared.m_icons = new Sprite[] { icon };
                }
            }

            SetupCrateVisual(deepKit.ItemPrefab, bundle, "crate_fishnet_deep", new Vector3(0.65f, 0.45f, 0.55f), new Vector3(0f, 0.22f, 0f));
        }

        Jotunn.Managers.ItemManager.Instance.AddItem(deepKit);
    }

    private static void SetupCrateVisual(GameObject itemPrefab, AssetBundle bundle, string modelName, Vector3 colliderSize, Vector3 colliderCenter)
    {
        if (bundle == null)
        {
            return;
        }

        GameObject modelPrefab = bundle.LoadAsset<GameObject>(modelName);
        if (modelPrefab == null)
        {
            return;
        }

        Transform existingVisual = itemPrefab.transform.Find("visual");
        if (existingVisual != null)
        {
            UnityEngine.Object.Destroy(existingVisual.gameObject);
        }

        GameObject visualInstance = UnityEngine.Object.Instantiate(modelPrefab, itemPrefab.transform);
        visualInstance.name = "visual";
        visualInstance.transform.localPosition = Vector3.zero;
        visualInstance.transform.localRotation = Quaternion.identity;
        visualInstance.transform.localScale = Vector3.one;

        int itemLayer = LayerMask.NameToLayer("item");
        if (itemLayer >= 0)
        {
            visualInstance.layer = itemLayer;
            foreach (Transform child in visualInstance.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = itemLayer;
            }
        }

        Shader pieceShader = Shader.Find("Custom/Piece");
        if (pieceShader == null)
        {
            pieceShader = Shader.Find("Standard");
        }

        if (pieceShader != null)
        {
            Renderer[] visualRenderers = visualInstance.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in visualRenderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    if (mat != null)
                    {
                        mat.shader = pieceShader;
                    }
                }
            }
        }

        BoxCollider boxCol = itemPrefab.GetComponent<BoxCollider>();
        if (boxCol == null)
        {
            boxCol = itemPrefab.AddComponent<BoxCollider>();
        }

        boxCol.size = colliderSize;
        boxCol.center = colliderCenter;

        Floating floating = itemPrefab.GetComponent<Floating>();
        if (floating == null)
        {
            floating = itemPrefab.AddComponent<Floating>();
        }

        floating.m_waterLevelOffset = 0.2f;
        floating.m_force = 0.5f;
        floating.m_balanceForceFraction = 0.05f;
        floating.m_damping = 0.05f;
        floating.m_forceDistance = 1.0f;

        Transform attach = itemPrefab.transform.Find("attach");
        if (attach == null)
        {
            GameObject attachObj = new GameObject("attach");
            attachObj.transform.SetParent(itemPrefab.transform, false);
            attachObj.transform.localPosition = Vector3.zero;
            attachObj.transform.localRotation = Quaternion.identity;
        }

        Transform attachBack = itemPrefab.transform.Find("attach_back");
        if (attachBack == null)
        {
            GameObject attachBackObj = new GameObject("attach_back");
            attachBackObj.transform.SetParent(itemPrefab.transform, false);
            attachBackObj.transform.localPosition = Vector3.zero;
            attachBackObj.transform.localRotation = Quaternion.identity;
        }
    }
}
