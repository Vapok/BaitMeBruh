using System.Collections.Generic;
using UnityEngine;

namespace BaitMeBruh.Managers;

public static class HuginTutorialManager
{
    public const string TutorialPrimitiveRod = "GC_Tutorial_PrimitiveRod";
    public const string TutorialBaitCreel = "GC_Tutorial_BaitCreel";
    public const string TutorialFirstFish = "GC_Tutorial_FirstFish";
    public const string TutorialCoastalNet = "GC_Tutorial_CoastalNet";
    public const string TutorialDeepNet = "GC_Tutorial_DeepNet";

    public const string TutorialBaitMeadows = "GC_Tutorial_Bait_Meadows";
    public const string TutorialBaitForest = "GC_Tutorial_Bait_Forest";
    public const string TutorialBaitSwamp = "GC_Tutorial_Bait_Swamp";
    public const string TutorialBaitCave = "GC_Tutorial_Bait_Cave";
    public const string TutorialBaitOcean = "GC_Tutorial_Bait_Ocean";
    public const string TutorialBaitPlains = "GC_Tutorial_Bait_Plains";
    public const string TutorialBaitMistlands = "GC_Tutorial_Bait_Mistlands";
    public const string TutorialBaitAshlands = "GC_Tutorial_Bait_Ashlands";
    public const string TutorialBaitDeepNorth = "GC_Tutorial_Bait_DeepNorth";

    public static void RegisterTutorials(Tutorial tutorial)
    {
        if (tutorial == null || tutorial.m_texts == null)
        {
            return;
        }

        AddTutorial(tutorial.m_texts, TutorialPrimitiveRod, "$tutorial_gc_primitiverod_topic", "$tutorial_gc_primitiverod_label", "$tutorial_gc_primitiverod_text");
        AddTutorial(tutorial.m_texts, TutorialBaitCreel, "$tutorial_gc_baitcreel_topic", "$tutorial_gc_baitcreel_label", "$tutorial_gc_baitcreel_text");
        AddTutorial(tutorial.m_texts, TutorialCoastalNet, "$tutorial_gc_coastalnet_topic", "$tutorial_gc_coastalnet_label", "$tutorial_gc_coastalnet_text");
        AddTutorial(tutorial.m_texts, TutorialDeepNet, "$tutorial_gc_deepnet_topic", "$tutorial_gc_deepnet_label", "$tutorial_gc_deepnet_text");
        AddTutorial(tutorial.m_texts, TutorialFirstFish, "$tutorial_gc_firstfish_topic", "$tutorial_gc_firstfish_label", "$tutorial_gc_firstfish_text");

        AddTutorial(tutorial.m_texts, TutorialBaitMeadows, "$tutorial_gc_bait_meadows_topic", "$item_fishingbait", "$tutorial_gc_bait_meadows_text");
        AddTutorial(tutorial.m_texts, TutorialBaitForest, "$tutorial_gc_bait_forest_topic", "$item_fishingbait_forest", "$tutorial_gc_bait_forest_text");
        AddTutorial(tutorial.m_texts, TutorialBaitSwamp, "$tutorial_gc_bait_swamp_topic", "$item_fishingbait_swamp", "$tutorial_gc_bait_swamp_text");
        AddTutorial(tutorial.m_texts, TutorialBaitCave, "$tutorial_gc_bait_cave_topic", "$item_fishingbait_cave", "$tutorial_gc_bait_cave_text");
        AddTutorial(tutorial.m_texts, TutorialBaitOcean, "$tutorial_gc_bait_ocean_topic", "$item_fishingbait_ocean", "$tutorial_gc_bait_ocean_text");
        AddTutorial(tutorial.m_texts, TutorialBaitPlains, "$tutorial_gc_bait_plains_topic", "$item_fishingbait_plains", "$tutorial_gc_bait_plains_text");
        AddTutorial(tutorial.m_texts, TutorialBaitMistlands, "$tutorial_gc_bait_mistlands_topic", "$item_fishingbait_mistlands", "$tutorial_gc_bait_mistlands_text");
        AddTutorial(tutorial.m_texts, TutorialBaitAshlands, "$tutorial_gc_bait_ashlands_topic", "$item_fishingbait_ashlands", "$tutorial_gc_bait_ashlands_text");
        AddTutorial(tutorial.m_texts, TutorialBaitDeepNorth, "$tutorial_gc_bait_deepnorth_topic", "$item_fishingbait_deepnorth", "$tutorial_gc_bait_deepnorth_text");
    }

    private static void AddTutorial(List<Tutorial.TutorialText> list, string key, string topic, string label, string text)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].m_name == key)
            {
                return;
            }
        }

        Tutorial.TutorialText tutorialText = new()
        {
            m_name = key,
            m_topic = topic,
            m_label = label,
            m_text = text,
            m_isMunin = false
        };

        list.Add(tutorialText);
    }

    public static void TriggerPrimitiveRodCrafted(Player player)
    {
        if (player == null || player.HaveSeenTutorial(TutorialPrimitiveRod))
        {
            return;
        }

        player.ShowTutorial(TutorialPrimitiveRod);
    }

    public static void TriggerBaitCreelPlaced()
    {
        Player localPlayer = Player.m_localPlayer;
        if (localPlayer == null || localPlayer.HaveSeenTutorial(TutorialBaitCreel))
        {
            return;
        }

        localPlayer.ShowTutorial(TutorialBaitCreel);
    }

    public static void TriggerCoastalNetPlaced()
    {
        Player localPlayer = Player.m_localPlayer;
        if (localPlayer == null || localPlayer.HaveSeenTutorial(TutorialCoastalNet))
        {
            return;
        }

        localPlayer.ShowTutorial(TutorialCoastalNet);
    }

    public static void TriggerDeepNetPlaced()
    {
        Player localPlayer = Player.m_localPlayer;
        if (localPlayer == null || localPlayer.HaveSeenTutorial(TutorialDeepNet))
        {
            return;
        }

        localPlayer.ShowTutorial(TutorialDeepNet);
    }

    public static void TriggerFirstFishCaught(Player player)
    {
        if (player == null || player.HaveSeenTutorial(TutorialFirstFish))
        {
            return;
        }

        player.ShowTutorial(TutorialFirstFish);
    }

    public static void TriggerBaitHarvested(string baitPrefabName)
    {
        Player localPlayer = Player.m_localPlayer;
        if (localPlayer == null || string.IsNullOrEmpty(baitPrefabName))
        {
            return;
        }

        string tutorialKey = GetTutorialKeyForBait(baitPrefabName);
        if (string.IsNullOrEmpty(tutorialKey) || localPlayer.HaveSeenTutorial(tutorialKey))
        {
            return;
        }

        localPlayer.ShowTutorial(tutorialKey);
    }

    private static string GetTutorialKeyForBait(string baitPrefabName)
    {
        switch (baitPrefabName)
        {
            case "FishingBait":
                return TutorialBaitMeadows;
            case "FishingBaitForest":
                return TutorialBaitForest;
            case "FishingBaitSwamp":
                return TutorialBaitSwamp;
            case "FishingBaitCave":
                return TutorialBaitCave;
            case "FishingBaitOcean":
                return TutorialBaitOcean;
            case "FishingBaitPlains":
                return TutorialBaitPlains;
            case "FishingBaitMistlands":
                return TutorialBaitMistlands;
            case "FishingBaitAshlands":
                return TutorialBaitAshlands;
            case "FishingBaitDeepNorth":
                return TutorialBaitDeepNorth;
            default:
                return string.Empty;
        }
    }
}
