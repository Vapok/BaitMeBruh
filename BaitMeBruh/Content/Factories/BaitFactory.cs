using System.Collections.Generic;
using Jotunn.Managers;
using Vapok.Common.Abstractions;
using Vapok.Common.Managers.Configuration;

namespace BaitMeBruh.Content.Factories;

internal class BaitFactory : AssetFactory
{
    private static readonly List<ConfigurableRecipe> _baitRecipes = new List<ConfigurableRecipe>();
    private static bool _registered;

    internal static IReadOnlyList<ConfigurableRecipe> BaitRecipes => _baitRecipes;

    internal BaitFactory(ILogIt logger, ConfigSyncBase configs) : base(logger, configs)
    {
    }

    internal override void CreateAssets()
    {
        if (_baitRecipes.Count == 0)
        {
            _baitRecipes.Add(new ConfigurableRecipe(
                "Recipe: Fishing Bait (Meadows)",
                "Recipe_FishingBait_Alt",
                "FishingBait",
                "piece_workbench",
                1,
                20,
                "NeckTail:5,Honey:2,BoneFragments:10"));

            _baitRecipes.Add(new ConfigurableRecipe(
                "Recipe: Mossy Fishing Bait (Black Forest)",
                "Recipe_FishingBaitForest_Alt",
                "FishingBaitForest",
                "piece_cauldron",
                1,
                20,
                "FishingBait:20,TrollHide:4,AncientSeed:2"));

            _baitRecipes.Add(new ConfigurableRecipe(
                "Recipe: Sticky Fishing Bait (Swamp)",
                "Recipe_FishingBaitSwamp_Alt",
                "FishingBaitSwamp",
                "piece_cauldron",
                2,
                20,
                "FishingBait:20,Bloodbag:4,Entrails:4"));

            _baitRecipes.Add(new ConfigurableRecipe(
                "Recipe: Cold Fishing Bait (Frost Caves)",
                "Recipe_FishingBaitCave_Alt",
                "FishingBaitCave",
                "piece_cauldron",
                2,
                20,
                "FishingBait:20,WolfFang:6,FreezeGland:2"));

            _baitRecipes.Add(new ConfigurableRecipe(
                "Recipe: Stingy Fishing Bait (Plains)",
                "Recipe_FishingBaitPlains_Alt",
                "FishingBaitPlains",
                "piece_cauldron",
                3,
                20,
                "FishingBait:20,Needle:4,Cloudberry:4"));

            _baitRecipes.Add(new ConfigurableRecipe(
                "Recipe: Heavy Fishing Bait (Ocean)",
                "Recipe_FishingBaitOcean_Alt",
                "FishingBaitOcean",
                "piece_cauldron",
                3,
                20,
                "FishingBait:20,SerpentScale:2,Guck:4"));

            _baitRecipes.Add(new ConfigurableRecipe(
                "Recipe: Misty Fishing Bait (Mistlands)",
                "Recipe_FishingBaitMistlands_Alt",
                "FishingBaitMistlands",
                "piece_cauldron",
                4,
                20,
                "FishingBait:20,Carapace:2,RoyalJelly:2"));

            _baitRecipes.Add(new ConfigurableRecipe(
                "Recipe: Hot Fishing Bait (Ashlands)",
                "Recipe_FishingBaitAshlands_Alt",
                "FishingBaitAshlands",
                "piece_cauldron",
                5,
                20,
                "FishingBait:20,CharredBone:4,SulfurStone:2"));

            _baitRecipes.Add(new ConfigurableRecipe(
                "Recipe: Frosty Fishing Bait (Deep North)",
                "Recipe_FishingBaitDeepNorth_Alt",
                "FishingBaitDeepNorth",
                "piece_cauldron",
                5,
                20,
                "FishingBait:20,FreezeGland:4,Feathers:4"));
        }

        PrefabManager.OnVanillaPrefabsAvailable += RegisterRecipes;
    }

    private static void RegisterRecipes()
    {
        if (_registered)
        {
            return;
        }

        _registered = true;

        foreach (ConfigurableRecipe recipe in _baitRecipes)
        {
            recipe.RegisterWithJotunn();
        }
    }
}
