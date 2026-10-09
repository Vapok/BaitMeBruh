using System.Collections.Generic;
using UnityEngine;

namespace BaitMeBruh.Managers;

public static class BaitSelectionManager
{
    public static List<ItemDrop.ItemData> GetAvailableBaitTypes(Player player, ItemDrop.ItemData weapon)
    {
        List<ItemDrop.ItemData> list = new List<ItemDrop.ItemData>();
        if (player == null || weapon == null)
        {
            return list;
        }

        string ammoType = weapon.m_shared.m_ammoType;
        if (string.IsNullOrEmpty(ammoType))
        {
            return list;
        }

        HashSet<string> seenPrefabs = new HashSet<string>();
        List<ItemDrop.ItemData> allItems = player.GetInventory().GetAllItems();

        for (int i = 0; i < allItems.Count; i++)
        {
            ItemDrop.ItemData item = allItems[i];
            if (item == null || item.m_dropPrefab == null || item.m_shared == null)
            {
                continue;
            }

            if (item.m_shared.m_ammoType == ammoType &&
                (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Ammo || item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.AmmoNonEquipable))
            {
                if (seenPrefabs.Add(item.m_dropPrefab.name))
                {
                    list.Add(item);
                }
            }
        }

        return list;
    }

    public static ItemDrop.ItemData GetCurrentBait(Player player, ItemDrop.ItemData weapon)
    {
        if (player == null || weapon == null)
        {
            return null;
        }

        string ammoType = weapon.m_shared.m_ammoType;
        if (string.IsNullOrEmpty(ammoType))
        {
            return null;
        }

        ItemDrop.ItemData ammoItem = player.GetAmmoItem();
        if (ammoItem != null && player.GetInventory().ContainsItem(ammoItem) && ammoItem.m_shared.m_ammoType == ammoType)
        {
            return ammoItem;
        }

        return player.GetInventory().GetAmmoItem(ammoType);
    }

    public static bool CycleBait(Player player, ItemDrop.ItemData weapon, bool reverse = false)
    {
        if (player == null || weapon == null)
        {
            return false;
        }

        List<ItemDrop.ItemData> baits = GetAvailableBaitTypes(player, weapon);
        if (baits.Count <= 1)
        {
            return false;
        }

        ItemDrop.ItemData currentBait = GetCurrentBait(player, weapon);
        int currentIndex = -1;

        if (currentBait != null && currentBait.m_dropPrefab != null)
        {
            for (int i = 0; i < baits.Count; i++)
            {
                if (baits[i].m_dropPrefab != null && baits[i].m_dropPrefab.name == currentBait.m_dropPrefab.name)
                {
                    currentIndex = i;
                    break;
                }
            }
        }

        int nextIndex;
        if (reverse)
        {
            nextIndex = (currentIndex <= 0) ? baits.Count - 1 : currentIndex - 1;
        }
        else
        {
            nextIndex = (currentIndex + 1) % baits.Count;
        }

        ItemDrop.ItemData nextBait = baits[nextIndex];
        return player.EquipItem(nextBait, triggerEquipEffects: false);
    }

    public static int GetBaitTotalCount(Player player, ItemDrop.ItemData bait)
    {
        if (player == null || bait == null || bait.m_shared == null)
        {
            return 0;
        }

        return player.GetInventory().CountItems(bait.m_shared.m_name);
    }

    public static Sprite GetBaitIconByPrefabName(string prefabName)
    {
        if (string.IsNullOrEmpty(prefabName) || ObjectDB.instance == null)
        {
            return null;
        }

        GameObject prefab = ObjectDB.instance.GetItemPrefab(prefabName);
        if (prefab == null)
        {
            return null;
        }

        ItemDrop itemDrop = prefab.GetComponent<ItemDrop>();
        if (itemDrop == null)
        {
            return null;
        }

        return itemDrop.m_itemData.GetIcon();
    }

    public static string GetBaitNameByPrefabName(string prefabName)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            return string.Empty;
        }

        if (ObjectDB.instance != null)
        {
            GameObject prefab = ObjectDB.instance.GetItemPrefab(prefabName);
            if (prefab != null)
            {
                ItemDrop itemDrop = prefab.GetComponent<ItemDrop>();
                if (itemDrop != null && itemDrop.m_itemData.m_shared != null)
                {
                    return Localization.instance != null
                        ? Localization.instance.Localize(itemDrop.m_itemData.m_shared.m_name)
                        : itemDrop.m_itemData.m_shared.m_name;
                }
            }
        }

        return prefabName;
    }
}
