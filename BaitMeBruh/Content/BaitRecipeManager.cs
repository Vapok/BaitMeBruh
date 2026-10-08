using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;

namespace BaitMeBruh.Content;

public static class BaitRecipeManager
{
    private static bool _registered;

    public static void Initialize()
    {
        PrefabManager.OnVanillaPrefabsAvailable += RegisterRecipes;
    }

    private static void RegisterRecipes()
    {
        if (_registered)
        {
            return;
        }

        _registered = true;

        RegisterRecipe("Recipe_FishingBait_Alt", "FishingBait", 20, "piece_workbench", 1, new[]
        {
            new RequirementConfig { Item = "NeckTail", Amount = 5 },
            new RequirementConfig { Item = "Honey", Amount = 2 },
            new RequirementConfig { Item = "BoneFragments", Amount = 10 }
        });

        RegisterRecipe("Recipe_FishingBaitForest_Alt", "FishingBaitForest", 20, "piece_cauldron", 1, new[]
        {
            new RequirementConfig { Item = "FishingBait", Amount = 20 },
            new RequirementConfig { Item = "TrollHide", Amount = 4 },
            new RequirementConfig { Item = "AncientSeed", Amount = 2 }
        });

        RegisterRecipe("Recipe_FishingBaitSwamp_Alt", "FishingBaitSwamp", 20, "piece_cauldron", 2, new[]
        {
            new RequirementConfig { Item = "FishingBait", Amount = 20 },
            new RequirementConfig { Item = "Bloodbag", Amount = 4 },
            new RequirementConfig { Item = "Entrails", Amount = 4 }
        });

        RegisterRecipe("Recipe_FishingBaitCave_Alt", "FishingBaitCave", 20, "piece_cauldron", 2, new[]
        {
            new RequirementConfig { Item = "FishingBait", Amount = 20 },
            new RequirementConfig { Item = "WolfFang", Amount = 6 },
            new RequirementConfig { Item = "FreezeGland", Amount = 2 }
        });

        RegisterRecipe("Recipe_FishingBaitPlains_Alt", "FishingBaitPlains", 20, "piece_cauldron", 3, new[]
        {
            new RequirementConfig { Item = "FishingBait", Amount = 20 },
            new RequirementConfig { Item = "Needle", Amount = 4 },
            new RequirementConfig { Item = "Cloudberry", Amount = 4 }
        });

        RegisterRecipe("Recipe_FishingBaitOcean_Alt", "FishingBaitOcean", 20, "piece_cauldron", 3, new[]
        {
            new RequirementConfig { Item = "FishingBait", Amount = 20 },
            new RequirementConfig { Item = "Chitin", Amount = 6 },
            new RequirementConfig { Item = "SerpentMeat", Amount = 2 }
        });

        RegisterRecipe("Recipe_FishingBaitMistlands_Alt", "FishingBaitMistlands", 20, "piece_cauldron", 4, new[]
        {
            new RequirementConfig { Item = "FishingBait", Amount = 20 },
            new RequirementConfig { Item = "Carapace", Amount = 2 },
            new RequirementConfig { Item = "RoyalJelly", Amount = 2 }
        });

        RegisterRecipe("Recipe_FishingBaitAshlands_Alt", "FishingBaitAshlands", 20, "piece_cauldron", 5, new[]
        {
            new RequirementConfig { Item = "FishingBait", Amount = 20 },
            new RequirementConfig { Item = "CharredBone", Amount = 4 },
            new RequirementConfig { Item = "SulfurStone", Amount = 2 }
        });

        RegisterRecipe("Recipe_FishingBaitDeepNorth_Alt", "FishingBaitDeepNorth", 20, "piece_cauldron", 5, new[]
        {
            new RequirementConfig { Item = "FishingBait", Amount = 20 },
            new RequirementConfig { Item = "FreezeGland", Amount = 4 },
            new RequirementConfig { Item = "Feathers", Amount = 4 }
        });
    }

    private static void RegisterRecipe(string recipeName, string resultItem, int amount, string station, int minLevel, RequirementConfig[] requirements)
    {
        RecipeConfig config = new RecipeConfig
        {
            Name = recipeName,
            Item = resultItem,
            Amount = amount,
            CraftingStation = station,
            MinStationLevel = minLevel,
            Requirements = requirements
        };

        CustomRecipe customRecipe = new CustomRecipe(config);
        Jotunn.Managers.ItemManager.Instance.AddRecipe(customRecipe);
    }
}
