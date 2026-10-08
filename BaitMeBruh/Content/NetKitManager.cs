using UnityEngine;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;

namespace BaitMeBruh.Content;

public static class NetKitManager
{
    private static bool _registered;

    public static void Initialize()
    {
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
            CraftingStation = "piece_workbench",
            MinStationLevel = 1,
            Icons = icon != null ? new[] { icon } : null,
            Requirements = new[]
            {
                new RequirementConfig { Item = "RoundLog", Amount = 15, Recover = true },
                new RequirementConfig { Item = "FineWood", Amount = 10, Recover = true },
                new RequirementConfig { Item = "Bronze", Amount = 4, Recover = true },
                new RequirementConfig { Item = "BronzeNails", Amount = 8, Recover = true },
                new RequirementConfig { Item = "TrollHide", Amount = 4, Recover = true },
                new RequirementConfig { Item = "Stone", Amount = 4, Recover = true }
            }
        };

        CustomItem coastalKit = new CustomItem("ItemFishnetCoastal", "LinenThread", coastalConfig);
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
            CraftingStation = "forge",
            MinStationLevel = 1,
            Icons = icon != null ? new[] { icon } : null,
            Requirements = new[]
            {
                new RequirementConfig { Item = "ElderBark", Amount = 15, Recover = true },
                new RequirementConfig { Item = "Iron", Amount = 6, Recover = true },
                new RequirementConfig { Item = "IronNails", Amount = 12, Recover = true },
                new RequirementConfig { Item = "Chain", Amount = 4, Recover = true },
                new RequirementConfig { Item = "Guck", Amount = 4, Recover = true }
            }
        };

        CustomItem deepKit = new CustomItem("ItemFishnetDeep", "LinenThread", deepConfig);
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

    private static void SetupCrateVisual(GameObject itemPrefab, AssetBundle bundle, string cratePrefabName, Vector3 colSize, Vector3 colCenter)
    {
        if (itemPrefab == null || bundle == null)
        {
            return;
        }

        Renderer[] renderers = itemPrefab.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = false;
        }

        GameObject cratePrefab = bundle.LoadAsset<GameObject>(cratePrefabName);
        if (cratePrefab != null)
        {
            GameObject crateVisual = Object.Instantiate(cratePrefab, itemPrefab.transform, false);
            crateVisual.name = "CrateVisual";
            crateVisual.transform.localPosition = Vector3.zero;
            crateVisual.transform.localRotation = Quaternion.identity;
            crateVisual.transform.localScale = Vector3.one;

            SetupVisualShaders(crateVisual);
        }

        Collider existingCol = itemPrefab.GetComponent<Collider>();
        if (existingCol != null && !(existingCol is BoxCollider))
        {
            existingCol.enabled = false;
        }

        BoxCollider boxCol = itemPrefab.GetComponent<BoxCollider>();
        if (boxCol == null)
        {
            boxCol = itemPrefab.AddComponent<BoxCollider>();
        }

        boxCol.enabled = true;
        boxCol.size = colSize;
        boxCol.center = colCenter;
    }

    private static void SetupVisualShaders(GameObject visual)
    {
        int itemLayer = LayerMask.NameToLayer("item");
        if (itemLayer >= 0)
        {
            visual.layer = itemLayer;
            foreach (Transform child in visual.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = itemLayer;
            }
        }

        Shader pieceShader = Shader.Find("Custom/Piece");
        if (pieceShader != null)
        {
            Renderer[] visualRenderers = visual.GetComponentsInChildren<Renderer>(true);
            for (int r = 0; r < visualRenderers.Length; r++)
            {
                Material[] mats = visualRenderers[r].materials;
                for (int m = 0; m < mats.Length; m++)
                {
                    if (mats[m] != null)
                    {
                        mats[m].shader = pieceShader;
                    }
                }
                visualRenderers[r].materials = mats;
            }
        }
    }
}
