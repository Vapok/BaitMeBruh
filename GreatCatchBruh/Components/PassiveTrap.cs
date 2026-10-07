using System;
using UnityEngine;

namespace GreatCatchBruh.Components;

public class PassiveTrap : MonoBehaviour, Hoverable, Interactable
{
    private static readonly int s_trapProgress = "greatcatch_trap_progress".GetStableHashCode();
    private static readonly int s_trapLastTime = "greatcatch_trap_lasttime".GetStableHashCode();
    private static readonly int s_trapItems = "greatcatch_trap_items".GetStableHashCode();

    public TrapType m_trapType = TrapType.BaitCreel;
    public float m_secPerUnit = 300f;
    public int m_maxCapacity = 3;
    public float m_minDepth = 0.1f;
    public float m_maxDepth = 2.0f;

    private ZNetView m_nview;

    private void Awake()
    {
        m_nview = GetComponent<ZNetView>();

        if (m_nview == null || m_nview.GetZDO() == null)
        {
            return;
        }

        m_nview.Register("RPC_Extract", RPC_Extract);

        if (m_nview.IsOwner() && m_nview.GetZDO().GetLong(s_trapLastTime, 0L) == 0L)
        {
            long nowTicks = ZNet.instance != null ? ZNet.instance.GetTime().Ticks : DateTime.UtcNow.Ticks;
            m_nview.GetZDO().Set(s_trapLastTime, nowTicks);
        }

        InvokeRepeating(nameof(UpdateTrap), UnityEngine.Random.Range(2f, 5f), 10f);
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

        float depth = GetWaterDepth();
        if (depth < m_minDepth)
        {
            character.Message(MessageHud.MessageType.Center, "$msg_trap_too_shallow");
        }
        else if (depth > m_maxDepth)
        {
            character.Message(MessageHud.MessageType.Center, "$msg_trap_too_deep");
        }
        else
        {
            character.Message(MessageHud.MessageType.Center, $"$msg_trap_active ($msg_trap_depth: {depth:F1}m)");
        }

        return true;
    }

    public bool UseItem(Humanoid user, ItemDrop.ItemData item)
    {
        return false;
    }

    private void Extract()
    {
        if (m_nview != null && m_nview.IsValid())
        {
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

        string[] itemPrefabs = items.Split(';');
        Vector3 basePos = transform.position + Vector3.up * 0.6f;

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            string prefabName = itemPrefabs[i];
            if (string.IsNullOrEmpty(prefabName))
            {
                continue;
            }

            GameObject itemPrefab = ObjectDB.instance != null ? ObjectDB.instance.GetItemPrefab(prefabName) : null;
            if (itemPrefab == null)
            {
                continue;
            }

            Vector2 rand = UnityEngine.Random.insideUnitCircle * 0.4f;
            Vector3 spawnPos = basePos + new Vector3(rand.x, 0.2f * i, rand.y);
            GameObject spawned = UnityEngine.Object.Instantiate(itemPrefab, spawnPos, Quaternion.identity);
            ItemDrop itemDrop = spawned.GetComponent<ItemDrop>();
            if (itemDrop != null)
            {
                itemDrop.m_itemData.m_stack = 1;
            }
        }

        m_nview.GetZDO().Set(s_trapItems, string.Empty);
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
        float depth = GetWaterDepth();
        if (depth < m_minDepth)
        {
            return "<color=orange>$msg_trap_too_shallow</color>";
        }

        if (depth > m_maxDepth)
        {
            return "<color=orange>$msg_trap_too_deep</color>";
        }

        if (GetTrapLevel() >= m_maxCapacity)
        {
            return "<color=yellow>$msg_trap_full</color>";
        }

        return $"<color=#80d0ff>$msg_trap_active</color> ($msg_trap_depth: {depth:F1}m)";
    }

    private void UpdateTrap()
    {
        if (m_nview == null || !m_nview.IsValid() || !m_nview.IsOwner())
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

        float deltaSeconds = GetTimeSinceLastUpdate();
        if (deltaSeconds <= 0f)
        {
            return;
        }

        float currentProgress = m_nview.GetZDO().GetFloat(s_trapProgress, 0f) + deltaSeconds;

        if (currentProgress >= m_secPerUnit)
        {
            int unitsProduced = (int)(currentProgress / m_secPerUnit);
            currentProgress %= m_secPerUnit;

            for (int i = 0; i < unitsProduced; i++)
            {
                if (GetTrapLevel() >= m_maxCapacity)
                {
                    break;
                }

                SpawnHarvest();
            }
        }

        m_nview.GetZDO().Set(s_trapProgress, currentProgress);
    }

    private float GetTimeSinceLastUpdate()
    {
        if (m_nview == null || m_nview.GetZDO() == null || ZNet.instance == null)
        {
            return 0f;
        }

        DateTime now = ZNet.instance.GetTime();
        long lastTicks = m_nview.GetZDO().GetLong(s_trapLastTime, now.Ticks);
        DateTime lastTime = new DateTime(lastTicks);
        TimeSpan delta = now - lastTime;

        m_nview.GetZDO().Set(s_trapLastTime, now.Ticks);

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
            prefabName = m_trapType == TrapType.CoastalNet ? GetCoastalFishForBiome(biome) : GetDeepFishForBiome(biome);
        }

        if (string.IsNullOrEmpty(prefabName))
        {
            return;
        }

        string current = m_nview.GetZDO().GetString(s_trapItems, string.Empty);
        string updated = string.IsNullOrEmpty(current) ? prefabName : current + ";" + prefabName;
        m_nview.GetZDO().Set(s_trapItems, updated);
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

    private string GetCoastalFishForBiome(Heightmap.Biome biome)
    {
        float roll = UnityEngine.Random.value;
        switch (biome)
        {
            case Heightmap.Biome.Meadows:
                return roll < 0.70f ? "Fish1" : "Fish2";
            case Heightmap.Biome.BlackForest:
                return roll < 0.40f ? "Fish2" : "Fish3";
            case Heightmap.Biome.Swamp:
                return roll < 0.70f ? "Fish5" : "Fish12";
            case Heightmap.Biome.Mountain:
                return "Fish4_cave";
            case Heightmap.Biome.Plains:
                return "Fish7";
            case Heightmap.Biome.Ocean:
                return roll < 0.60f ? "Fish6" : "Fish11";
            case Heightmap.Biome.Mistlands:
                return roll < 0.60f ? "Fish8" : "Fish11";
            case Heightmap.Biome.AshLands:
                return "Fish9";
            case Heightmap.Biome.DeepNorth:
                return "Fish10";
            default:
                return "Fish1";
        }
    }

    private string GetDeepFishForBiome(Heightmap.Biome biome)
    {
        float roll = UnityEngine.Random.value;
        switch (biome)
        {
            case Heightmap.Biome.Ocean:
                if (roll < 0.50f) return "Fish6";
                if (roll < 0.80f) return "Fish11";
                return "Fish1";
            case Heightmap.Biome.Swamp:
                return roll < 0.60f ? "Fish12" : "Fish5";
            case Heightmap.Biome.Plains:
                return roll < 0.70f ? "Fish7" : "Fish6";
            case Heightmap.Biome.Mistlands:
                return roll < 0.70f ? "Fish8" : "Fish11";
            case Heightmap.Biome.AshLands:
                return "Fish9";
            case Heightmap.Biome.DeepNorth:
                return "Fish10";
            case Heightmap.Biome.BlackForest:
                return roll < 0.50f ? "Fish3" : "Fish2";
            case Heightmap.Biome.Meadows:
                return roll < 0.50f ? "Fish2" : "Fish1";
            case Heightmap.Biome.Mountain:
                return "Fish4_cave";
            default:
                return "Fish6";
        }
    }
}
