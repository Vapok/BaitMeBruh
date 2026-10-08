using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace GreatCatchBruh.Content;

public static class RodManager
{
    private static bool _registered;

    public static void Initialize()
    {
        PrefabManager.OnVanillaPrefabsAvailable += RegisterRods;
    }

    private static void RegisterRods()
    {
        if (_registered)
        {
            return;
        }

        _registered = true;

        ItemConfig rodConfig = new ItemConfig
        {
            Name = "$item_fishingrod_primitive",
            Description = "$item_fishingrod_primitive_desc",
            CraftingStation = "piece_workbench",
            MinStationLevel = 1,
            Requirements = new[]
            {
                new RequirementConfig { Item = "Wood", Amount = 5, Recover = true },
                new RequirementConfig { Item = "LeatherScraps", Amount = 4, Recover = true },
                new RequirementConfig { Item = "BoneFragments", Amount = 2, Recover = true }
            }
        };

        CustomItem primitiveRod = new CustomItem("FishingRodPrimitive", "FishingRod", rodConfig);
        GameObject prefab = primitiveRod.ItemPrefab;

        if (prefab != null)
        {
            ItemDrop itemDrop = prefab.GetComponent<ItemDrop>();
            if (itemDrop != null)
            {
                itemDrop.m_itemData.m_shared.m_attack.m_projectileVel = 16.0f;
                itemDrop.m_itemData.m_shared.m_attack.m_projectileVelMin = 6.0f;
                itemDrop.m_itemData.m_shared.m_weight = 1.0f;
            }

            Transform attach = prefab.transform.Find("attach");
            if (attach != null)
            {
                attach.localScale = new Vector3(0.78f, 0.78f, 0.78f);
            }

            Transform attachBack = prefab.transform.Find("attach_back");
            if (attachBack != null)
            {
                attachBack.localScale = new Vector3(0.78f, 0.78f, 0.78f);
            }

            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    Material mat = materials[i];
                    mat.color = new Color(0.65f, 0.58f, 0.48f, 1.0f);
                    if (mat.HasProperty("_Metallic"))
                    {
                        mat.SetFloat("_Metallic", 0.0f);
                    }
                    if (mat.HasProperty("_Glossiness"))
                    {
                        mat.SetFloat("_Glossiness", 0.10f);
                    }
                }
                renderer.materials = materials;
            }
        }

        Jotunn.Managers.ItemManager.Instance.AddItem(primitiveRod);
    }
}
