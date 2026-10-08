namespace BaitMeBruh.Patches;

internal class FishSpawnerBackup
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
