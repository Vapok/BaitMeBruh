using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(SpawnSystem), "UpdateSpawnList")]
internal static class SpawnSystemUpdateSpawnListPatch
{
    private class FishSpawnerBackup
    {
        public SpawnSystem.SpawnData Spawner;
        public float OriginalSpawnChance;
        public int OriginalMaxSpawned;
        public int OriginalGroupSizeMin;
        public int OriginalGroupSizeMax;
        public float OriginalSpawnInterval;
        public bool OriginalSpawnAtDay;
        public bool OriginalSpawnAtNight;
    }

    [HarmonyPrefix]
    private static void Prefix(List<SpawnSystem.SpawnData> spawners, ref List<FishSpawnerBackup> __state)
    {
        if (spawners == null || !MorningFishManager.IsMorningFishingHour())
        {
            __state = null;
            return;
        }

        List<FishSpawnerBackup> backups = new();

        foreach (SpawnSystem.SpawnData spawner in spawners)
        {
            if (spawner == null || spawner.m_prefab == null)
            {
                continue;
            }

            Fish fish = spawner.m_prefab.GetComponent<Fish>();
            if (fish == null)
            {
                continue;
            }

            FishSpawnerBackup backup = new()
            {
                Spawner = spawner,
                OriginalSpawnChance = spawner.m_spawnChance,
                OriginalMaxSpawned = spawner.m_maxSpawned,
                OriginalGroupSizeMin = spawner.m_groupSizeMin,
                OriginalGroupSizeMax = spawner.m_groupSizeMax,
                OriginalSpawnInterval = spawner.m_spawnInterval,
                OriginalSpawnAtDay = spawner.m_spawnAtDay,
                OriginalSpawnAtNight = spawner.m_spawnAtNight
            };
            backups.Add(backup);

            spawner.m_spawnChance = 100f;
            spawner.m_maxSpawned = Mathf.Max(spawner.m_maxSpawned * 2, 8);
            spawner.m_groupSizeMin = Mathf.Max(spawner.m_groupSizeMin + 1, 2);
            spawner.m_groupSizeMax = Mathf.Max(spawner.m_groupSizeMax * 2, 4);
            spawner.m_spawnInterval = Mathf.Min(spawner.m_spawnInterval * 0.5f, 10f);
            spawner.m_spawnAtDay = true;
            spawner.m_spawnAtNight = true;
        }

        __state = backups;
    }

    [HarmonyPostfix]
    private static void Postfix(List<FishSpawnerBackup> __state)
    {
        if (__state == null)
        {
            return;
        }

        foreach (FishSpawnerBackup backup in __state)
        {
            backup.Spawner.m_spawnChance = backup.OriginalSpawnChance;
            backup.Spawner.m_maxSpawned = backup.OriginalMaxSpawned;
            backup.Spawner.m_groupSizeMin = backup.OriginalGroupSizeMin;
            backup.Spawner.m_groupSizeMax = backup.OriginalGroupSizeMax;
            backup.Spawner.m_spawnInterval = backup.OriginalSpawnInterval;
            backup.Spawner.m_spawnAtDay = backup.OriginalSpawnAtDay;
            backup.Spawner.m_spawnAtNight = backup.OriginalSpawnAtNight;
        }
    }
}
