using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;

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
        Jotunn.Managers.ItemManager.Instance.AddItem(primitiveRod);
    }
}
