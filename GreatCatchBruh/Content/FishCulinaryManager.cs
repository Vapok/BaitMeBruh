using System.Collections.Generic;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace GreatCatchBruh.Content;

public static class FishCulinaryManager
{
    private static bool _registered;

    public static void Initialize()
    {
        PrefabManager.OnVanillaPrefabsAvailable += RegisterCulinaryContent;
    }

    private static void RegisterCulinaryContent()
    {
        if (_registered)
        {
            return;
        }

        _registered = true;

        ConfigureEarlyButchering();
        ConfigureCampfireFishCooking();
        RegisterWholeFishRecipes();
    }

    private static void ConfigureEarlyButchering()
    {
        Recipe recipeFish1 = PrefabManager.Cache.GetPrefab<Recipe>("Recipe_Fish1");
        if (recipeFish1 != null)
        {
            CraftingStation workbench = PrefabManager.Cache.GetPrefab<CraftingStation>("piece_workbench");
            if (workbench != null)
            {
                recipeFish1.m_craftingStation = workbench;
                recipeFish1.m_minStationLevel = 1;
                recipeFish1.m_amount = 2;
                recipeFish1.m_qualityResultAmountMultiplier = 2.0f;
            }
        }
    }

    private static void ConfigureCampfireFishCooking()
    {
        GameObject stationPrefab = PrefabManager.Cache.GetPrefab<GameObject>("piece_cookingstation");
        if (stationPrefab == null)
        {
            return;
        }

        CookingStation cs = stationPrefab.GetComponent<CookingStation>();
        if (cs == null || cs.m_conversion == null)
        {
            return;
        }

        ItemDrop fishCooked = PrefabManager.Cache.GetPrefab<ItemDrop>("FishCooked");
        ItemDrop fish1 = PrefabManager.Cache.GetPrefab<ItemDrop>("Fish1");
        ItemDrop fish2 = PrefabManager.Cache.GetPrefab<ItemDrop>("Fish2");

        if (fishCooked == null || fish1 == null || fish2 == null)
        {
            return;
        }

        CookingStation.ItemConversion conv1 = new CookingStation.ItemConversion
        {
            m_from = fish1,
            m_to = fishCooked,
            m_cookTime = 25f
        };

        CookingStation.ItemConversion conv2 = new CookingStation.ItemConversion
        {
            m_from = fish2,
            m_to = fishCooked,
            m_cookTime = 25f
        };

        cs.m_conversion.Add(conv1);
        cs.m_conversion.Add(conv2);
    }

    private static void RegisterWholeFishRecipes()
    {
        RegisterRecipe(new RecipeConfig
        {
            Name = "Recipe_TrollfishChowder",
            Item = "FishSoup",
            Amount = 1,
            CraftingStation = "piece_cauldron",
            MinStationLevel = 1,
            Requirements = new[]
            {
                new RequirementConfig { Item = "Fish3", Amount = 1, Recover = false },
                new RequirementConfig { Item = "MushroomYellow", Amount = 2, Recover = false }
            }
        });

        RegisterRecipe(new RecipeConfig
        {
            Name = "Recipe_FrostBiteMeadBase",
            Item = "MeadBaseFrostResist",
            Amount = 1,
            CraftingStation = "piece_cauldron",
            MinStationLevel = 1,
            Requirements = new[]
            {
                new RequirementConfig { Item = "Fish4_cave", Amount = 1, Recover = false },
                new RequirementConfig { Item = "Honey", Amount = 10, Recover = false },
                new RequirementConfig { Item = "Thistle", Amount = 5, Recover = false },
                new RequirementConfig { Item = "WolfFang", Amount = 1, Recover = false }
            }
        });

        RegisterRecipe(new RecipeConfig
        {
            Name = "Recipe_SwampFishBroth",
            Item = "BlackSoup",
            Amount = 1,
            CraftingStation = "piece_cauldron",
            MinStationLevel = 2,
            Requirements = new[]
            {
                new RequirementConfig { Item = "Fish5", Amount = 1, Recover = false },
                new RequirementConfig { Item = "Bloodbag", Amount = 2, Recover = false },
                new RequirementConfig { Item = "Entrails", Amount = 2, Recover = false }
            }
        });

        RegisterRecipe(new RecipeConfig
        {
            Name = "Recipe_MarinersMeadBase",
            Item = "MeadBaseSwimmer",
            Amount = 1,
            CraftingStation = "piece_cauldron",
            MinStationLevel = 2,
            Requirements = new[]
            {
                new RequirementConfig { Item = "Fish7", Amount = 1, Recover = false },
                new RequirementConfig { Item = "Honey", Amount = 10, Recover = false },
                new RequirementConfig { Item = "Cloudberry", Amount = 2, Recover = false }
            }
        });

        RegisterRecipe(new RecipeConfig
        {
            Name = "Recipe_PufferPoisonMeadBase",
            Item = "MeadBasePoisonResist",
            Amount = 1,
            CraftingStation = "piece_cauldron",
            MinStationLevel = 2,
            Requirements = new[]
            {
                new RequirementConfig { Item = "Fish11", Amount = 1, Recover = false },
                new RequirementConfig { Item = "Honey", Amount = 10, Recover = false },
                new RequirementConfig { Item = "Thistle", Amount = 4, Recover = false }
            }
        });

        RegisterRecipe(new RecipeConfig
        {
            Name = "Recipe_GlowfinEitrMeadBase",
            Item = "MeadBaseEitrMinor",
            Amount = 1,
            CraftingStation = "piece_cauldron",
            MinStationLevel = 4,
            Requirements = new[]
            {
                new RequirementConfig { Item = "Fish8", Amount = 1, Recover = false },
                new RequirementConfig { Item = "Honey", Amount = 10, Recover = false },
                new RequirementConfig { Item = "Magecap", Amount = 2, Recover = false }
            }
        });
    }

    private static void RegisterRecipe(RecipeConfig config)
    {
        CustomRecipe customRecipe = new CustomRecipe(config);
        Jotunn.Managers.ItemManager.Instance.AddRecipe(customRecipe);
    }
}
