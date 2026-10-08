using UnityEngine;

namespace BaitMeBruh.Components;

public class FishingLineState : MonoBehaviour
{
    private const float OverTensionSnapThreshold = 0.5f;
    private const float MinimumRodDamping = 0.1f;

    public float CurrentTension { get; private set; }
    public float OverTensionDuration { get; private set; }
    public bool IsSnapped { get; private set; }

    private FishingFloat _fishingFloat;

    private void Awake()
    {
        _fishingFloat = GetComponent<FishingFloat>();
    }

    public void ResetTension()
    {
        CurrentTension = 0f;
        OverTensionDuration = 0f;
        IsSnapped = false;
    }

    public void UpdateTension(float fixedDeltaTime, bool isStruggling, bool isReeling, int fishQuality, float skillFactor, float rodDamping, float snapToleranceMultiplier = 1.0f)
    {
        float effectiveDamping = Mathf.Max(MinimumRodDamping, rodDamping);
        float tensionGain;

        if (isStruggling && isReeling)
        {
            tensionGain = 0.65f * ((1.0f + 0.3f * fishQuality) / effectiveDamping);
        }
        else if (isStruggling && !isReeling)
        {
            tensionGain = 0.15f * ((1.0f + 0.1f * fishQuality) / effectiveDamping);
        }
        else if (!isStruggling && isReeling)
        {
            tensionGain = 0.10f * (1.0f / effectiveDamping);
        }
        else
        {
            tensionGain = 0.0f;
        }

        float tensionDissipation;
        if (!isReeling)
        {
            tensionDissipation = 0.40f * (1.0f + 0.5f * skillFactor);
        }
        else
        {
            tensionDissipation = 0.05f;
        }

        float tensionDelta = (tensionGain - tensionDissipation) * fixedDeltaTime;
        CurrentTension = Mathf.Clamp01(CurrentTension + tensionDelta);

        if (CurrentTension >= 1.0f)
        {
            OverTensionDuration += fixedDeltaTime;
            float threshold = OverTensionSnapThreshold * Mathf.Max(0.5f, snapToleranceMultiplier);
            if (OverTensionDuration >= threshold)
            {
                IsSnapped = true;
            }
        }
        else
        {
            OverTensionDuration = Mathf.Max(0.0f, OverTensionDuration - fixedDeltaTime * 2.0f);
        }
    }
}
