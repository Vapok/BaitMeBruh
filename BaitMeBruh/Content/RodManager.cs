using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace BaitMeBruh.Content;

public static class RodManager
{
    private static bool _registered;
    private static ConfigurableRecipe _primitiveRodRecipe;

    internal static ConfigurableRecipe PrimitiveRodRecipe => _primitiveRodRecipe;

    public static void Initialize()
    {
        _primitiveRodRecipe = new ConfigurableRecipe(
            "Recipe: Primitive Fishing Rod",
            "Recipe_FishingRodPrimitive",
            "FishingRodPrimitive",
            "piece_workbench",
            1,
            1,
            "Wood:5,LeatherScraps:4,BoneFragments:2");

        PrefabManager.OnVanillaPrefabsAvailable += RegisterRods;
    }

    private static void RegisterRods()
    {
        if (_registered)
        {
            return;
        }

        _registered = true;

        Sprite primitiveIcon = TrapAssetManager.GetSprite("I_FishingRodPrimitive");

        ItemConfig rodConfig = new ItemConfig
        {
            Name = "$item_fishingrod_primitive",
            Description = "$item_fishingrod_primitive_desc",
            CraftingStation = _primitiveRodRecipe.GetStationString(),
            MinStationLevel = _primitiveRodRecipe.MinStationLevel.Value,
            Requirements = _primitiveRodRecipe.GetRequirementConfigs(),
            Icons = primitiveIcon != null ? new Sprite[] { primitiveIcon } : null
        };

        CustomItem primitiveRod = new CustomItem("FishingRodPrimitive", "FishingRod", rodConfig);
        _primitiveRodRecipe.BindCustomItem(primitiveRod);

        GameObject prefab = primitiveRod.ItemPrefab;

        if (prefab != null)
        {
            ItemDrop itemDrop = prefab.GetComponent<ItemDrop>();
            if (itemDrop != null)
            {
                itemDrop.m_itemData.m_shared.m_attack.m_projectileVel = 22.0f;
                itemDrop.m_itemData.m_shared.m_attack.m_projectileVelMin = 2.0f;
                itemDrop.m_itemData.m_shared.m_attack.m_drawDurationMin = 2.8f;
                itemDrop.m_itemData.m_shared.m_weight = 1.0f;

                if (primitiveIcon != null)
                {
                    itemDrop.m_itemData.m_shared.m_icons = new Sprite[] { primitiveIcon };
                }
            }

            GameObject vanillaRodPrefab = PrefabManager.Instance.GetPrefab("FishingRod");
            if (vanillaRodPrefab != null)
            {
                ItemDrop vanillaDrop = vanillaRodPrefab.GetComponent<ItemDrop>();
                if (vanillaDrop != null)
                {
                    vanillaDrop.m_itemData.m_shared.m_attack.m_projectileVel = 26.0f;
                    vanillaDrop.m_itemData.m_shared.m_attack.m_projectileVelMin = 2.0f;
                    vanillaDrop.m_itemData.m_shared.m_attack.m_drawDurationMin = 2.8f;
                }
            }

            GameObject rodAssetPrefab = TrapAssetManager.GetPrefab("primitive_fishing_rod");

            Transform attach = prefab.transform.Find("attach");
            if (attach != null)
            {
                attach.localScale = Vector3.one;
                attach.localPosition = new Vector3(0.00f, 0.05f, -0.85f);

                Transform vanillaDefault = attach.Find("default");
                if (vanillaDefault != null)
                {
                    vanillaDefault.gameObject.SetActive(false);
                }

                if (rodAssetPrefab != null)
                {
                    GameObject newModel = UnityEngine.Object.Instantiate(rodAssetPrefab, attach);
                    newModel.name = "primitive_model";
                    newModel.transform.localPosition = Vector3.zero;
                    newModel.transform.localRotation = Quaternion.identity;
                    newModel.transform.localScale = Vector3.one;

                    Transform vanillaRodTop = attach.Find("_RodTop");
                    Transform newRodTop = newModel.transform.Find("_RodTop");
                    if (vanillaRodTop != null && newRodTop != null)
                    {
                        vanillaRodTop.localPosition = newRodTop.localPosition;
                    }
                }

                BoxCollider collider = attach.GetComponent<BoxCollider>();
                if (collider == null)
                {
                    collider = attach.gameObject.AddComponent<BoxCollider>();
                }
                collider.center = new Vector3(0.0f, -0.05f, 0.85f);
                collider.size = new Vector3(0.08f, 0.08f, 2.45f);
            }

            Transform attachBack = prefab.transform.Find("attach_back");
            if (attachBack != null)
            {
                attachBack.localScale = Vector3.one;
            }
        }

        Jotunn.Managers.ItemManager.Instance.AddItem(primitiveRod);
    }
}
