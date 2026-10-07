using System;
using UnityEngine;

namespace GreatCatchBruh.Components;

public class PassiveTrap : MonoBehaviour
{
    private static readonly int s_trapProgress = "greatcatch_trap_progress".GetStableHashCode();
    private static readonly int s_trapLastTime = "greatcatch_trap_lasttime".GetStableHashCode();

    public TrapType m_trapType = TrapType.BaitCreel;
    public float m_secPerUnit = 360f;
    public int m_maxCapacity = 20;
    public float m_minDepth = 0.5f;
    public float m_maxDepth = 2.0f;

    private ZNetView m_nview;
    private Container m_container;

    private void Awake()
    {
        m_nview = GetComponent<ZNetView>();
        m_container = GetComponent<Container>();

        if (m_nview == null || m_nview.GetZDO() == null)
        {
            return;
        }

        if (m_nview.IsOwner() && m_nview.GetZDO().GetLong(s_trapLastTime, 0L) == 0L)
        {
            long nowTicks = ZNet.instance != null ? ZNet.instance.GetTime().Ticks : DateTime.UtcNow.Ticks;
            m_nview.GetZDO().Set(s_trapLastTime, nowTicks);
        }

        InvokeRepeating(nameof(UpdateTrap), UnityEngine.Random.Range(2f, 5f), 10f);
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

        float liquid = Floating.GetLiquidLevel(position, 1f, LiquidType.All);
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

        if (m_container != null)
        {
            Inventory inventory = m_container.GetInventory();
            if (inventory != null && GetTotalItemCount(inventory) >= m_maxCapacity)
            {
                return "<color=yellow>$msg_trap_full</color>";
            }
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

        if (m_container == null)
        {
            return;
        }

        Inventory inventory = m_container.GetInventory();
        if (inventory == null)
        {
            return;
        }

        int currentCount = GetTotalItemCount(inventory);
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
                if (GetTotalItemCount(inventory) >= m_maxCapacity)
                {
                    break;
                }

                SpawnHarvest(inventory);
            }

            m_container.Save();
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

    private int GetTotalItemCount(Inventory inventory)
    {
        int count = 0;
        foreach (ItemDrop.ItemData item in inventory.GetAllItems())
        {
            count += item.m_stack;
        }
        return count;
    }

    private void SpawnHarvest(Inventory inventory)
    {
        Heightmap.Biome biome = Heightmap.FindBiome(transform.position);

        if (m_trapType == TrapType.BaitCreel)
        {
            string baitPrefab = GetBaitForBiome(biome);
            inventory.AddItem(baitPrefab, 1, 1, 0, 0L, string.Empty, false, false);
            return;
        }

        string fishPrefab = m_trapType == TrapType.CoastalNet ? GetCoastalFishForBiome(biome) : GetDeepFishForBiome(biome);
        float qRoll = UnityEngine.Random.value;
        int quality = 1;

        if (m_trapType == TrapType.CoastalNet)
        {
            quality = qRoll < 0.80f ? 1 : (qRoll < 0.95f ? 2 : 3);
        }
        else
        {
            quality = qRoll < 0.70f ? 1 : (qRoll < 0.90f ? 2 : 3);
        }

        inventory.AddItem(fishPrefab, 1, quality, 0, 0L, string.Empty, false, false);
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
