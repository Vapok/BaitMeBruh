using UnityEngine;

namespace BaitMeBruh.Managers;

public static class MorningFishManager
{
    public const float MorningHourStart = 5f / 24f; // 0.2083f (~5:00 AM)
    public const float MorningHourEnd = 7f / 24f;   // 0.2917f (~7:00 AM)

    public const float MorningDetectionRange = 25.0f;
    public const float StandardDetectionRange = 15.0f;

    public const float MorningHookChance = 0.95f;
    public const float StandardHookChance = 0.50f;

    public static bool IsMorningFishingHour()
    {
        if (EnvMan.instance == null)
        {
            return false;
        }

        float dayFraction = EnvMan.instance.GetDayFraction();
        return dayFraction >= MorningHourStart && dayFraction <= MorningHourEnd;
    }

    public static float GetDetectionRange()
    {
        return IsMorningFishingHour() ? MorningDetectionRange : StandardDetectionRange;
    }

    public static float GetBaseHookChance(float standardChance)
    {
        return IsMorningFishingHour() ? MorningHookChance : standardChance;
    }
}
