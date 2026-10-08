using System;
using System.Collections.Generic;
using UnityEngine;
using BaitMeBruh.Configuration;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Components;

public class PassiveTrap : MonoBehaviour, Hoverable, Interactable
{
    private static readonly int s_trapProgress = "greatcatch_trap_progress".GetStableHashCode();
    private static readonly int s_trapLastTime = "greatcatch_trap_lasttime".GetStableHashCode();
    private static readonly int s_trapItems = "greatcatch_trap_items".GetStableHashCode();
    private static readonly int s_trapFuel = "greatcatch_trap_fuel".GetStableHashCode();
    private static readonly int s_trapBait = "greatcatch_trap_bait".GetStableHashCode();

    private static readonly List<PassiveTrap> s_allTraps = new();

    public TrapType m_trapType = TrapType.BaitCreel;
    public float m_secPerUnit = 300f;
    public int m_maxCapacity = 3;
    public float m_minDepth = 0.1f;
    public float m_maxDepth = 2.0f;

    private const int MaxChumTails = 5;

    private ZNetView m_nview;

    public static bool IsFishingBait(string prefabName)
    {
        switch (prefabName)
        {
            case "FishingBait":
            case "FishingBaitForest":
            case "FishingBaitSwamp":
            case "FishingBaitCave":
            case "FishingBaitPlains":
            case "FishingBaitOcean":
            case "FishingBaitMistlands":
            case "FishingBaitAshlands":
            case "FishingBaitDeepNorth":
                return true;
            default:
                return false;
        }
    }

    public static Heightmap.Biome GetValidBiomeForBait(string baitPrefab)
    {
        switch (baitPrefab)
        {
            case "FishingBaitForest":
                return Heightmap.Biome.BlackForest;
            case "FishingBaitSwamp":
                return Heightmap.Biome.Swamp;
            case "FishingBaitCave":
                return Heightmap.Biome.Mountain;
            case "FishingBaitPlains":
                return Heightmap.Biome.Plains;
            case "FishingBaitOcean":
                return Heightmap.Biome.Ocean;
            case "FishingBaitMistlands":
                return Heightmap.Biome.Mistlands;
            case "FishingBaitAshlands":
                return Heightmap.Biome.AshLands;
            case "FishingBaitDeepNorth":
                return Heightmap.Biome.DeepNorth;
            case "FishingBait":
            default:
                return Heightmap.Biome.Meadows;
        }
    }

    public string GetLoadedBait()
    {
        if (m_nview == null || m_nview.GetZDO() == null)
        {
            return string.Empty;
        }

        return m_nview.GetZDO().GetString(s_trapBait, string.Empty);
    }

    public string GetLoadedBaitDisplayName()
    {
        string baitPrefab = GetLoadedBait();
        if (string.IsNullOrEmpty(baitPrefab))
        {
            return string.Empty;
        }

        GameObject prefab = ObjectDB.instance != null ? ObjectDB.instance.GetItemPrefab(baitPrefab) : null;
        if (prefab != null)
        {
            ItemDrop itemDrop = prefab.GetComponent<ItemDrop>();
            if (itemDrop != null)
            {
                return itemDrop.m_itemData.m_shared.m_name;
            }
        }

        return "$item_fishingbait";
    }

    public string EvaluateBait(string baitPrefab, out bool converted)
    {
        converted = false;
        if (baitPrefab == "FishingBait")
        {
            return "FishingBait";
        }

        Heightmap.Biome currentBiome = Heightmap.FindBiome(transform.position);
        Heightmap.Biome requiredBiome = GetValidBiomeForBait(baitPrefab);

        if (currentBiome == requiredBiome)
        {
            return baitPrefab;
        }

        converted = true;
        return "FishingBait";
    }

    public int GetUnitsPerFuel()
    {
        switch (m_trapType)
        {
            case TrapType.BaitCreel:
                return ConfigRegistry.CreelBaitPerChum != null ? ConfigRegistry.CreelBaitPerChum.Value : 3;
            case TrapType.CoastalNet:
                return 4;
            case TrapType.DeepNet:
                return 4;
            default:
                return 1;
        }
    }

    public int GetMaxFuel()
    {
        switch (m_trapType)
        {
            case TrapType.BaitCreel:
                return GetUnitsPerFuel() * MaxChumTails;
            case TrapType.CoastalNet:
                return 20;
            case TrapType.DeepNet:
                return 20;
            default:
                return 10;
        }
    }

    public float GetSecPerUnit()
    {
        if (m_trapType == TrapType.BaitCreel && ConfigRegistry.CreelMinutesPerBait != null)
        {
            return Mathf.Max(1.0f, ConfigRegistry.CreelMinutesPerBait.Value * 60f);
        }
        return m_secPerUnit;
    }

    private void Awake()
    {
        s_allTraps.Add(this);

        m_nview = GetComponent<ZNetView>();

        if (m_nview == null || m_nview.GetZDO() == null)
        {
            return;
        }

        m_nview.Register("RPC_Extract", RPC_Extract);
        m_nview.Register<string>("RPC_ExtractResponse", RPC_ExtractResponse);
        m_nview.Register<int>("RPC_AddFuel", RPC_AddFuel);
        m_nview.Register<string, int>("RPC_AddBait", RPC_AddBait);

        if (m_nview.IsOwner() && m_nview.GetZDO().GetLong(s_trapLastTime, 0L) == 0L)
        {
            long nowTicks = DateTime.UtcNow.Ticks;
            m_nview.GetZDO().Set(s_trapLastTime, nowTicks);
        }

        InvokeRepeating(nameof(UpdateTrap), UnityEngine.Random.Range(2f, 5f), 10f);
    }

    private void OnDestroy()
    {
        s_allTraps.Remove(this);
    }

    public bool IsCrowded()
    {
        if (m_trapType != TrapType.BaitCreel)
        {
            return false;
        }

        float minDistance = ConfigRegistry.CreelProximityDistance != null ? ConfigRegistry.CreelProximityDistance.Value : 20.0f;
        float minDistanceSq = minDistance * minDistance;
        Vector3 myPos = transform.position;

        for (int i = 0; i < s_allTraps.Count; i++)
        {
            PassiveTrap other = s_allTraps[i];
            if (other == null || other == this || other.m_trapType != TrapType.BaitCreel)
            {
                continue;
            }

            Vector3 diff = other.transform.position - myPos;
            if (diff.sqrMagnitude < minDistanceSq)
            {
                return true;
            }
        }

        return false;
    }

    public int GetFuel()
    {
        if (m_nview == null || m_nview.GetZDO() == null)
        {
            return 0;
        }

        return m_nview.GetZDO().GetInt(s_trapFuel, 0);
    }

    private void AddFuel(int amount)
    {
        if (m_nview != null && m_nview.IsValid())
        {
            if (!m_nview.HasOwner())
            {
                m_nview.ClaimOwnership();
            }
            m_nview.InvokeRPC("RPC_AddFuel", amount);
        }
    }

    private void RPC_AddFuel(long caller, int amount)
    {
        if (m_nview == null || !m_nview.IsValid() || !m_nview.IsOwner())
        {
            return;
        }

        int current = GetFuel();
        m_nview.GetZDO().Set(s_trapFuel, current + amount);
    }

    private void AddBait(string baitPrefab, int fuelAmount)
    {
        if (m_nview != null && m_nview.IsValid())
        {
            if (!m_nview.HasOwner())
            {
                m_nview.ClaimOwnership();
            }
            m_nview.InvokeRPC("RPC_AddBait", baitPrefab, fuelAmount);
        }
    }

    private void RPC_AddBait(long caller, string baitPrefab, int fuelAmount)
    {
        if (m_nview == null || !m_nview.IsValid() || !m_nview.IsOwner())
        {
            return;
        }

        int current = GetFuel();
        m_nview.GetZDO().Set(s_trapBait, baitPrefab);
        m_nview.GetZDO().Set(s_trapFuel, current + fuelAmount);
    }

    private void Start()
    {
        Player localPlayer = Player.m_localPlayer;
        if (localPlayer != null && Vector3.Distance(localPlayer.transform.position, transform.position) < 25.0f)
        {
            switch (m_trapType)
            {
                case TrapType.BaitCreel:
                    HuginTutorialManager.TriggerBaitCreelPlaced();
                    break;
                case TrapType.CoastalNet:
                    HuginTutorialManager.TriggerCoastalNetPlaced();
                    break;
                case TrapType.DeepNet:
                    HuginTutorialManager.TriggerDeepNetPlaced();
                    break;
            }
        }
    }

    public string GetHoverName()
    {
        Piece piece = GetComponent<Piece>();
        return piece != null ? piece.m_name : "$piece_bait_creel";
    }

    public string GetHoverText()
    {
        if (!PrivateArea.CheckAccess(transform.position, 0f, flash: false))
        {
            return Localization.instance.Localize(GetHoverName() + "\n$piece_noaccess");
        }

        int currentCount = GetTrapLevel();
        string status = GetStatusHoverText();
        string hoverName = Localization.instance.Localize(GetHoverName());

        string text = hoverName;
        if (currentCount > 0)
        {
            string harvestName = Localization.instance.Localize(GetHarvestItemDisplayName());
            text += $" ( {harvestName} x {currentCount} / {m_maxCapacity} )\n[<color=yellow><b>$KEY_Use</b></color>] $piece_beehive_extract";
        }
        else
        {
            text += " ( $piece_container_empty )";
        }

        int fuel = GetFuel();
        int maxFuel = GetMaxFuel();

        if (m_trapType == TrapType.BaitCreel)
        {
            if (fuel > 0)
            {
                text += $"\n$msg_trap_chum: {fuel} / {maxFuel}";
            }
            else
            {
                text += "\n<color=orange>$msg_trap_needs_chum</color>";
            }

            int perChum = GetUnitsPerFuel();
            if (fuel + perChum <= maxFuel)
            {
                Player localPlayer = Player.m_localPlayer;
                bool hasItem = false;
                if (localPlayer != null && localPlayer.GetInventory() != null)
                {
                    hasItem = localPlayer.GetInventory().HaveItem("$item_necktail") ||
                              localPlayer.GetInventory().GetAllItems().Exists(i => i.m_dropPrefab != null && i.m_dropPrefab.name == "NeckTail");
                }

                if (hasItem && currentCount == 0)
                {
                    text += "\n[<color=yellow><b>$KEY_Use</b></color>] $msg_trap_add_chum";
                }
                else
                {
                    text += "\n[<color=yellow><b>1-8</b></color>] $msg_trap_add_chum";
                }
            }
        }
        else
        {
            if (fuel > 0)
            {
                string baitDisplayName = Localization.instance.Localize(GetLoadedBaitDisplayName());
                text += $"\n$msg_trap_bait: {baitDisplayName} ({fuel} / {maxFuel})";
            }
            else
            {
                text += "\n<color=orange>$msg_trap_needs_bait</color>";
            }

            int perBait = GetUnitsPerFuel();
            if (fuel + perBait <= maxFuel)
            {
                Player localPlayer = Player.m_localPlayer;
                bool hasBait = false;
                if (localPlayer != null && localPlayer.GetInventory() != null)
                {
                    string loadedBait = GetLoadedBait();
                    if (!string.IsNullOrEmpty(loadedBait) && fuel > 0)
                    {
                        hasBait = localPlayer.GetInventory().GetAllItems().Exists(i => i.m_dropPrefab != null && i.m_dropPrefab.name == loadedBait);
                    }
                    else
                    {
                        hasBait = localPlayer.GetInventory().GetAllItems().Exists(i => i.m_dropPrefab != null && IsFishingBait(i.m_dropPrefab.name));
                    }
                }

                if (hasBait && currentCount == 0)
                {
                    text += "\n[<color=yellow><b>$KEY_Use</b></color>] $msg_trap_add_bait";
                }
                else
                {
                    text += "\n[<color=yellow><b>1-8</b></color>] $msg_trap_add_bait";
                }
            }
        }

        if (!string.IsNullOrEmpty(status))
        {
            text += "\n" + status;
        }

        return Localization.instance.Localize(text);
    }

    public float GetHoverOffset()
    {
        return 0f;
    }

    public bool Interact(Humanoid character, bool repeat, bool alt)
    {
        if (repeat)
        {
            return false;
        }

        if (!PrivateArea.CheckAccess(transform.position))
        {
            return true;
        }

        int currentCount = GetTrapLevel();
        if (currentCount > 0)
        {
            Extract();
            return true;
        }

        int maxFuel = GetMaxFuel();
        int currentFuel = GetFuel();

        if (m_trapType == TrapType.BaitCreel)
        {
            int perChum = GetUnitsPerFuel();
            if (currentFuel + perChum <= maxFuel && character != null)
            {
                Inventory inventory = character.GetInventory();
                if (inventory != null)
                {
                    ItemDrop.ItemData fuelItem = inventory.GetItem("$item_necktail");
                    if (fuelItem == null)
                    {
                        fuelItem = inventory.GetAllItems().Find(i => i.m_dropPrefab != null && i.m_dropPrefab.name == "NeckTail");
                    }

                    if (fuelItem != null)
                    {
                        inventory.RemoveOneItem(fuelItem);
                        AddFuel(perChum);
                        character.Message(MessageHud.MessageType.Center, $"$msg_trap_chum_added ($msg_trap_chum: {currentFuel + perChum} / {maxFuel})");
                        return true;
                    }
                }
            }
        }
        else
        {
            int perBait = GetUnitsPerFuel();
            if (currentFuel + perBait <= maxFuel && character != null)
            {
                Inventory inventory = character.GetInventory();
                if (inventory != null)
                {
                    ItemDrop.ItemData baitItem = null;
                    string currentBait = GetLoadedBait();

                    if (currentFuel > 0 && !string.IsNullOrEmpty(currentBait))
                    {
                        baitItem = inventory.GetAllItems().Find(i => i.m_dropPrefab != null && i.m_dropPrefab.name == currentBait);
                    }
                    else
                    {
                        Heightmap.Biome currentBiome = Heightmap.FindBiome(transform.position);
                        string idealBait = GetBaitForBiome(currentBiome);
                        baitItem = inventory.GetAllItems().Find(i => i.m_dropPrefab != null && i.m_dropPrefab.name == idealBait);

                        if (baitItem == null)
                        {
                            baitItem = inventory.GetAllItems().Find(i => i.m_dropPrefab != null && i.m_dropPrefab.name == "FishingBait");
                        }

                        if (baitItem == null)
                        {
                            baitItem = inventory.GetAllItems().Find(i => i.m_dropPrefab != null && IsFishingBait(i.m_dropPrefab.name));
                        }
                    }

                    if (baitItem != null)
                    {
                        string effectiveBait = EvaluateBait(baitItem.m_dropPrefab.name, out bool converted);
                        inventory.RemoveOneItem(baitItem);
                        AddBait(effectiveBait, perBait);

                        if (converted)
                        {
                            character.Message(MessageHud.MessageType.Center, "$msg_trap_bait_converted_basic");
                        }
                        else
                        {
                            character.Message(MessageHud.MessageType.Center, $"$msg_trap_bait_added ($msg_trap_bait: {currentFuel + perBait} / {maxFuel})");
                        }
                        return true;
                    }
                }
            }
        }

        if (IsCrowded())
        {
            character.Message(MessageHud.MessageType.Center, "$msg_trap_crowded");
            return true;
        }

        float depth = GetWaterDepth();
        if (depth < m_minDepth)
        {
            character.Message(MessageHud.MessageType.Center, "$msg_trap_too_shallow");
        }
        else if (depth > m_maxDepth)
        {
            character.Message(MessageHud.MessageType.Center, "$msg_trap_too_deep");
        }
        else if (GetFuel() <= 0)
        {
            character.Message(MessageHud.MessageType.Center, m_trapType == TrapType.BaitCreel ? "$msg_trap_needs_chum" : "$msg_trap_needs_bait");
        }
        else
        {
            character.Message(MessageHud.MessageType.Center, $"$msg_trap_active ($msg_trap_depth: {depth:F1}m)");
        }

        return true;
    }

    public bool UseItem(Humanoid user, ItemDrop.ItemData item)
    {
        if (item == null || user == null)
        {
            return false;
        }

        if (m_trapType == TrapType.BaitCreel)
        {
            bool isNeckTail = (item.m_dropPrefab != null && item.m_dropPrefab.name == "NeckTail") || item.m_shared.m_name == "$item_necktail";
            if (!isNeckTail)
            {
                return false;
            }

            int perChum = GetUnitsPerFuel();
            int maxFuel = GetMaxFuel();
            int currentFuel = GetFuel();

            if (currentFuel + perChum > maxFuel)
            {
                user.Message(MessageHud.MessageType.Center, "$msg_trap_fuel_full");
                return true;
            }

            user.GetInventory().RemoveOneItem(item);
            AddFuel(perChum);
            user.Message(MessageHud.MessageType.Center, $"$msg_trap_chum_added ($msg_trap_chum: {currentFuel + perChum} / {maxFuel})");
            return true;
        }
        else
        {
            string itemPrefab = item.m_dropPrefab != null ? item.m_dropPrefab.name : string.Empty;
            if (!IsFishingBait(itemPrefab))
            {
                return false;
            }

            int perBait = GetUnitsPerFuel();
            int maxFuel = GetMaxFuel();
            int currentFuel = GetFuel();

            if (currentFuel + perBait > maxFuel)
            {
                user.Message(MessageHud.MessageType.Center, "$msg_trap_fuel_full");
                return true;
            }

            string currentLoaded = GetLoadedBait();
            string effectiveBait = EvaluateBait(itemPrefab, out bool converted);

            if (currentFuel > 0 && !string.IsNullOrEmpty(currentLoaded) && currentLoaded != effectiveBait)
            {
                user.Message(MessageHud.MessageType.Center, "$msg_trap_bait_mismatch");
                return true;
            }

            user.GetInventory().RemoveOneItem(item);
            AddBait(effectiveBait, perBait);

            if (converted)
            {
                user.Message(MessageHud.MessageType.Center, "$msg_trap_bait_converted_basic");
            }
            else
            {
                user.Message(MessageHud.MessageType.Center, $"$msg_trap_bait_added ($msg_trap_bait: {currentFuel + perBait} / {maxFuel})");
            }
            return true;
        }
    }

    private void Extract()
    {
        if (m_nview != null && m_nview.IsValid())
        {
            if (!m_nview.HasOwner())
            {
                m_nview.ClaimOwnership();
            }
            m_nview.InvokeRPC("RPC_Extract");
        }
    }

    private void RPC_Extract(long caller)
    {
        if (m_nview == null || !m_nview.IsValid() || !m_nview.IsOwner())
        {
            return;
        }

        string items = m_nview.GetZDO().GetString(s_trapItems, string.Empty);
        if (string.IsNullOrEmpty(items))
        {
            return;
        }

        m_nview.GetZDO().Set(s_trapItems, string.Empty);
        m_nview.InvokeRPC(caller, "RPC_ExtractResponse", items);
    }

    private void RPC_ExtractResponse(long caller, string items)
    {
        if (string.IsNullOrEmpty(items))
        {
            return;
        }

        Player localPlayer = Player.m_localPlayer;
        if (localPlayer == null)
        {
            return;
        }

        string[] itemPrefabs = items.Split(';');
        Dictionary<string, int> grouped = new();

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            string prefabName = itemPrefabs[i];
            if (string.IsNullOrEmpty(prefabName))
            {
                continue;
            }

            if (grouped.ContainsKey(prefabName))
            {
                grouped[prefabName]++;
            }
            else
            {
                grouped[prefabName] = 1;
            }
        }

        Inventory inventory = localPlayer.GetInventory();
        Vector3 dropPos = localPlayer.transform.position + Vector3.up * 0.5f;

        foreach (KeyValuePair<string, int> entry in grouped)
        {
            string prefabName = entry.Key;
            int totalCount = entry.Value;

            GameObject prefab = ObjectDB.instance != null ? ObjectDB.instance.GetItemPrefab(prefabName) : null;
            if (prefab == null)
            {
                continue;
            }

            ItemDrop itemDrop = prefab.GetComponent<ItemDrop>();
            if (itemDrop == null)
            {
                continue;
            }

            int addedCount = 0;
            for (int i = 0; i < totalCount; i++)
            {
                if (inventory.AddItem(prefab, 1))
                {
                    addedCount++;
                }
                else
                {
                    break;
                }
            }

            if (addedCount > 0)
            {
                localPlayer.ShowPickupMessage(itemDrop.m_itemData, addedCount);
                HuginTutorialManager.TriggerBaitHarvested(prefabName);
            }

            int remaining = totalCount - addedCount;
            if (remaining > 0)
            {
                localPlayer.Message(MessageHud.MessageType.Center, "$msg_noroom");
                ItemDrop dropped = ItemDrop.DropItem(itemDrop.m_itemData, remaining, dropPos, localPlayer.transform.rotation);
                if (dropped != null)
                {
                    Rigidbody rb = dropped.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.up * 3f;
                    }
                }
            }
        }
    }

    public int GetTrapLevel()
    {
        if (m_nview == null || m_nview.GetZDO() == null)
        {
            return 0;
        }

        string items = m_nview.GetZDO().GetString(s_trapItems, string.Empty);
        if (string.IsNullOrEmpty(items))
        {
            return 0;
        }

        string[] parts = items.Split(';');
        int count = 0;
        for (int i = 0; i < parts.Length; i++)
        {
            if (!string.IsNullOrEmpty(parts[i]))
            {
                count++;
            }
        }
        return count;
    }

    public float GetWaterDepth()
    {
        Vector3 position = transform.position;
        float groundHeight = position.y;
        bool hasGround = false;

        if (ZoneSystem.instance != null && ZoneSystem.instance.GetGroundHeight(position, out float height))
        {
            groundHeight = height;
            hasGround = true;
        }

        if (!hasGround)
        {
            return 0f;
        }

        float liquid = Floating.GetLiquidLevel(position + Vector3.up * 0.5f, 1f, LiquidType.All);
        if (liquid <= -9000f)
        {
            liquid = Floating.GetLiquidLevel(position, 1f, LiquidType.All);
        }

        float waterSurface = liquid > -9000f ? liquid : (ZoneSystem.instance != null ? ZoneSystem.instance.m_waterLevel : 30f);

        if (waterSurface > groundHeight)
        {
            return waterSurface - groundHeight;
        }

        return 0f;
    }

    public string GetStatusHoverText()
    {
        if (IsCrowded())
        {
            return "<color=orange>$msg_trap_crowded</color>";
        }

        float depth = GetWaterDepth();
        if (depth < m_minDepth)
        {
            return "<color=orange>$msg_trap_too_shallow</color>";
        }

        if (depth > m_maxDepth)
        {
            return "<color=orange>$msg_trap_too_deep</color>";
        }

        if (GetFuel() <= 0)
        {
            return "<color=yellow>$msg_trap_dormant</color>";
        }

        if (GetTrapLevel() >= m_maxCapacity)
        {
            return "<color=yellow>$msg_trap_full</color>";
        }

        return $"<color=#80d0ff>$msg_trap_active</color> ($msg_trap_depth: {depth:F1}m)";
    }

    private void UpdateTrap()
    {
        if (m_nview == null || !m_nview.IsValid())
        {
            return;
        }

        if (!m_nview.HasOwner())
        {
            m_nview.ClaimOwnership();
        }

        if (!m_nview.IsOwner())
        {
            return;
        }

        if (IsCrowded())
        {
            return;
        }

        float depth = GetWaterDepth();
        if (depth < m_minDepth || depth > m_maxDepth)
        {
            return;
        }

        int currentCount = GetTrapLevel();
        if (currentCount >= m_maxCapacity)
        {
            return;
        }

        if (GetFuel() <= 0)
        {
            return;
        }

        float deltaSeconds = GetTimeSinceLastUpdate();
        if (deltaSeconds <= 0f)
        {
            return;
        }

        float currentProgress = m_nview.GetZDO().GetFloat(s_trapProgress, 0f) + deltaSeconds;

        float secPerUnit = GetSecPerUnit();
        if (currentProgress >= secPerUnit)
        {
            int unitsProduced = (int)(currentProgress / secPerUnit);
            currentProgress %= secPerUnit;

            for (int i = 0; i < unitsProduced; i++)
            {
                if (GetTrapLevel() >= m_maxCapacity)
                {
                    break;
                }

                int fuel = GetFuel();
                if (fuel <= 0)
                {
                    break;
                }

                int remainingFuel = fuel - 1;
                m_nview.GetZDO().Set(s_trapFuel, remainingFuel);
                if (remainingFuel <= 0 && m_trapType != TrapType.BaitCreel)
                {
                    m_nview.GetZDO().Set(s_trapBait, string.Empty);
                }

                SpawnHarvest();
            }
        }

        m_nview.GetZDO().Set(s_trapProgress, currentProgress);
    }

    private float GetTimeSinceLastUpdate()
    {
        if (m_nview == null || m_nview.GetZDO() == null)
        {
            return 0f;
        }

        DateTime now = DateTime.UtcNow;
        long lastTicks = m_nview.GetZDO().GetLong(s_trapLastTime, now.Ticks);
        DateTime lastTime = new DateTime(lastTicks, DateTimeKind.Utc);
        TimeSpan delta = now - lastTime;

        m_nview.GetZDO().Set(s_trapLastTime, now.Ticks);

        if (delta.TotalDays > 1.0)
        {
            return 0f;
        }

        float seconds = (float)delta.TotalSeconds;
        if (seconds < 0f)
        {
            seconds = 0f;
        }

        return seconds;
    }

    private void SpawnHarvest()
    {
        Heightmap.Biome biome = Heightmap.FindBiome(transform.position);
        string prefabName = string.Empty;

        if (m_trapType == TrapType.BaitCreel)
        {
            prefabName = GetBaitForBiome(biome);
        }
        else
        {
            string loadedBait = GetLoadedBait();
            if (string.IsNullOrEmpty(loadedBait))
            {
                loadedBait = "FishingBait";
            }
            prefabName = GetFishForBait(loadedBait);
        }

        if (string.IsNullOrEmpty(prefabName))
        {
            return;
        }

        string current = m_nview.GetZDO().GetString(s_trapItems, string.Empty);
        string updated = string.IsNullOrEmpty(current) ? prefabName : current + ";" + prefabName;
        m_nview.GetZDO().Set(s_trapItems, updated);
    }

    private string GetFishForBait(string baitPrefab)
    {
        float roll = UnityEngine.Random.value;
        switch (baitPrefab)
        {
            case "FishingBaitForest":
                return roll < 0.70f ? "Fish3" : "Fish2";
            case "FishingBaitSwamp":
                return roll < 0.70f ? "Fish5" : "Fish12";
            case "FishingBaitCave":
                return "Fish4_cave";
            case "FishingBaitPlains":
                return roll < 0.70f ? "Fish7" : "Fish12";
            case "FishingBaitOcean":
                return roll < 0.60f ? "Fish6" : "Fish11";
            case "FishingBaitMistlands":
                return roll < 0.60f ? "Fish8" : "Fish11";
            case "FishingBaitAshlands":
                return "Fish9";
            case "FishingBaitDeepNorth":
                return "Fish10";
            case "FishingBait":
            default:
                return roll < 0.70f ? "Fish1" : "Fish2";
        }
    }

    private string GetHarvestItemDisplayName()
    {
        string items = m_nview != null && m_nview.GetZDO() != null ? m_nview.GetZDO().GetString(s_trapItems, string.Empty) : string.Empty;
        if (!string.IsNullOrEmpty(items))
        {
            string[] parts = items.Split(';');
            for (int i = 0; i < parts.Length; i++)
            {
                if (!string.IsNullOrEmpty(parts[i]))
                {
                    GameObject prefab = ObjectDB.instance != null ? ObjectDB.instance.GetItemPrefab(parts[i]) : null;
                    if (prefab != null)
                    {
                        ItemDrop itemDrop = prefab.GetComponent<ItemDrop>();
                        if (itemDrop != null)
                        {
                            return itemDrop.m_itemData.m_shared.m_name;
                        }
                    }
                    break;
                }
            }
        }

        return m_trapType == TrapType.BaitCreel ? "$item_fishingbait" : "$item_fish_raw";
    }

    private string GetBaitForBiome(Heightmap.Biome biome)
    {
        switch (biome)
        {
            case Heightmap.Biome.Meadows:
                return "FishingBait";
            case Heightmap.Biome.BlackForest:
                return "FishingBaitForest";
            case Heightmap.Biome.Swamp:
                return "FishingBaitSwamp";
            case Heightmap.Biome.Mountain:
                return "FishingBaitCave";
            case Heightmap.Biome.Plains:
                return "FishingBaitPlains";
            case Heightmap.Biome.Ocean:
                return "FishingBaitOcean";
            case Heightmap.Biome.Mistlands:
                return "FishingBaitMistlands";
            case Heightmap.Biome.AshLands:
                return "FishingBaitAshlands";
            case Heightmap.Biome.DeepNorth:
                return "FishingBaitDeepNorth";
            default:
                return "FishingBait";
        }
    }
}
