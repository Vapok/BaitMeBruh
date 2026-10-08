using System.Collections.Generic;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace BaitMeBruh.Content;

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

        RegisterCustomFoods();
        ConfigureCampfireFishCooking();
        RegisterWholeFishRecipes();
        UpdateItemDescriptions();
    }

    private static void RegisterCustomFoods()
    {
        Sprite chowderIcon = LoadEmbeddedSprite("BaitMeBruh.Assets.Icons.trollfish_chowder.png");

        SE_Stats sneakEffect = ScriptableObject.CreateInstance<SE_Stats>();
        sneakEffect.name = "SE_TrollfishChowder";
        sneakEffect.m_name = "$se_trollfish_chowder";
        sneakEffect.m_icon = chowderIcon;
        sneakEffect.m_ttl = 1200f;
        sneakEffect.m_tooltip = "$se_trollfish_chowder_tooltip";
        sneakEffect.m_skillLevel = Skills.SkillType.Sneak;
        sneakEffect.m_skillLevelModifier = 10f;

        Jotunn.Managers.ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(sneakEffect, false));

        ItemConfig chowderConfig = new ItemConfig
        {
            Name = "$item_trollfish_chowder",
            Description = "$item_trollfish_chowder_desc",
            CraftingStation = "piece_cauldron",
            MinStationLevel = 1,
            Icons = chowderIcon != null ? new[] { chowderIcon } : null,
            Requirements = new[]
            {
                new RequirementConfig { Item = "Fish5", Amount = 1, Recover = false },
                new RequirementConfig { Item = "MushroomYellow", Amount = 2, Recover = false }
            }
        };

        CustomItem customChowder = new CustomItem("TrollfishChowder", "CarrotSoup", chowderConfig);
        if (customChowder.ItemPrefab != null)
        {
            ItemDrop itemDrop = customChowder.ItemDrop;
            if (itemDrop != null)
            {
                ItemDrop.ItemData.SharedData shared = itemDrop.m_itemData.m_shared;
                shared.m_name = "$item_trollfish_chowder";
                shared.m_description = "$item_trollfish_chowder_desc";
                shared.m_itemType = ItemDrop.ItemData.ItemType.Consumable;
                shared.m_food = 15f;
                shared.m_foodStamina = 45f;
                shared.m_foodBurnTime = 1200f;
                shared.m_foodRegen = 2f;
                shared.m_weight = 1.0f;
                shared.m_maxStackSize = 10;
                shared.m_consumeStatusEffect = sneakEffect;
                if (chowderIcon != null)
                {
                    shared.m_icons = new[] { chowderIcon };
                }
            }
        }

        Jotunn.Managers.ItemManager.Instance.AddItem(customChowder);
    }

    private static Sprite LoadEmbeddedSprite(string resourceName)
    {
        System.Reflection.Assembly assembly = typeof(FishCulinaryManager).Assembly;
        using System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            return null;
        }

        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);

        Texture2D texture = Jotunn.Utils.AssetUtils.LoadImage(buffer);
        if (texture != null)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        }

        return null;
    }

    private static void UpdateItemDescriptions()
    {
        ItemDrop fish1 = PrefabManager.Cache.GetPrefab<ItemDrop>("Fish1");
        if (fish1 != null)
        {
            fish1.m_itemData.m_shared.m_description = "$item_fish1_desc";
        }

        ItemDrop fish2 = PrefabManager.Cache.GetPrefab<ItemDrop>("Fish2");
        if (fish2 != null)
        {
            fish2.m_itemData.m_shared.m_description = "$item_fish2_desc";
        }

        ItemDrop bait = PrefabManager.Cache.GetPrefab<ItemDrop>("FishingBait");
        if (bait != null)
        {
            bait.m_itemData.m_shared.m_description = "$item_fishingbait_desc";
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
                new RequirementConfig { Item = "Fish6", Amount = 1, Recover = false },
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
                new RequirementConfig { Item = "Fish3", Amount = 1, Recover = false },
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
                new RequirementConfig { Item = "Fish12", Amount = 1, Recover = false },
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
                new RequirementConfig { Item = "Fish9", Amount = 1, Recover = false },
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
