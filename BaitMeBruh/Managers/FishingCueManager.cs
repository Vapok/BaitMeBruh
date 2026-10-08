using UnityEngine;
using BaitMeBruh.Components;
using BaitMeBruh.UI;

namespace BaitMeBruh.Managers;

public static class FishingCueManager
{
    private static readonly Color FlareWarmGold = new Color(1.0f, 0.85f, 0.35f);
    private static readonly Color FlareBrightGold = new Color(1.0f, 0.92f, 0.50f);

    public static void TriggerBiteCue(FishingFloat fishingFloat, Character owner)
    {
        if (fishingFloat == null || owner == null)
        {
            return;
        }

        if (fishingFloat.m_nibbleEffect != null)
        {
            fishingFloat.m_nibbleEffect.Create(owner.transform.position, Quaternion.identity);
            fishingFloat.m_nibbleEffect.Create(fishingFloat.transform.position, Quaternion.identity, null, 2.5f);
        }

        SpawnWaterLightFlare(fishingFloat.transform.position, 9.0f, 4.5f, 0.85f, FlareWarmGold);

        if (FishingTensionHud.Instance != null)
        {
            FishingTensionHud.Instance.OnBiteAlert();
        }
    }

    public static void TriggerHookedCue(FishingFloat fishingFloat, Character owner, Fish fish)
    {
        if (fishingFloat == null || owner == null)
        {
            return;
        }

        if (fishingFloat.m_nibbleEffect != null)
        {
            fishingFloat.m_nibbleEffect.Create(owner.transform.position, Quaternion.identity, null, 2.0f);
        }

        if (owner is Player player)
        {
            if (player.m_perfectDodgeEffects != null)
            {
                player.m_perfectDodgeEffects.Create(owner.transform.position, Quaternion.identity);
            }

            if (player.m_autopickupEffects != null)
            {
                player.m_autopickupEffects.Create(owner.transform.position, Quaternion.identity);
            }
        }

        if (fish != null && fish.m_jumpEffects != null)
        {
            fish.m_jumpEffects.Create(fishingFloat.transform.position, Quaternion.identity);
        }

        SpawnWaterLightFlare(fishingFloat.transform.position, 12.0f, 6.0f, 0.65f, FlareBrightGold);

        owner.Message(MessageHud.MessageType.Center, "<color=#FFD700><b><size=28>$msg_fishing_hooked</size></b></color>");

        if (FishingTensionHud.Instance != null)
        {
            FishingTensionHud.Instance.OnHookedAlert();
        }
    }

    private static void SpawnWaterLightFlare(Vector3 position, float range, float intensity, float duration, Color color)
    {
        GameObject flareObject = new GameObject("FishingCueFlare");
        flareObject.transform.position = position + (Vector3.up * 0.45f);

        Light flareLight = flareObject.AddComponent<Light>();
        flareLight.type = LightType.Point;
        flareLight.range = range;
        flareLight.intensity = intensity;
        flareLight.color = color;
        flareLight.shadows = LightShadows.None;

        TransientLightFader fader = flareObject.AddComponent<TransientLightFader>();
        fader.Duration = duration;
    }
}
