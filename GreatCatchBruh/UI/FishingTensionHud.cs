using UnityEngine;
using UnityEngine.UI;
using GreatCatchBruh.Components;

namespace GreatCatchBruh.UI;

public class FishingTensionHud : MonoBehaviour
{
    private const float GaugeWidth = 120.0f;
    private const float GaugeHeight = 8.0f;
    private const float Padding = 2.0f;
    private const float VerticalOffset = -34.0f;

    private const float FadeSpeed = 6.0f;

    private static readonly Color BackgroundColor = new Color(0.08f, 0.08f, 0.10f, 0.75f);
    private static readonly Color SafeTensionColor = new Color(0.29f, 0.87f, 0.50f, 1.0f);
    private static readonly Color WarningTensionColor = new Color(0.98f, 0.80f, 0.08f, 1.0f);
    private static readonly Color CriticalTensionColor = new Color(0.94f, 0.27f, 0.27f, 1.0f);

    public static FishingTensionHud Instance { get; private set; }

    private CanvasGroup _canvasGroup;
    private RectTransform _fillTransform;
    private Image _fillImage;

    private float _targetAlpha;
    private float _maxWidth;

    public static void EnsureInitialized(Hud hud)
    {
        if (Instance != null || hud == null || hud.m_crosshair == null)
        {
            return;
        }

        Transform parent = hud.m_crosshair.transform.parent;
        if (parent == null)
        {
            return;
        }

        GameObject gaugeObject = new GameObject("FishingTensionGauge");
        gaugeObject.transform.SetParent(parent, false);

        Instance = gaugeObject.AddComponent<FishingTensionHud>();
        Instance.BuildUi();
    }

    private void BuildUi()
    {
        RectTransform rootTransform = gameObject.AddComponent<RectTransform>();
        rootTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rootTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rootTransform.pivot = new Vector2(0.5f, 0.5f);
        rootTransform.anchoredPosition = new Vector2(0.0f, VerticalOffset);
        rootTransform.sizeDelta = new Vector2(GaugeWidth, GaugeHeight);

        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _canvasGroup.alpha = 0.0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        Image bgImage = gameObject.AddComponent<Image>();
        bgImage.color = BackgroundColor;

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(transform, false);

        _fillTransform = fillObject.AddComponent<RectTransform>();
        _fillTransform.anchorMin = new Vector2(0.0f, 0.5f);
        _fillTransform.anchorMax = new Vector2(0.0f, 0.5f);
        _fillTransform.pivot = new Vector2(0.0f, 0.5f);
        _fillTransform.anchoredPosition = new Vector2(Padding, 0.0f);

        _maxWidth = GaugeWidth - (Padding * 2.0f);
        float fillHeight = GaugeHeight - (Padding * 2.0f);
        _fillTransform.sizeDelta = new Vector2(0.0f, fillHeight);

        _fillImage = fillObject.AddComponent<Image>();
        _fillImage.color = SafeTensionColor;
    }

    private void Update()
    {
        Player localPlayer = Player.m_localPlayer;
        if (localPlayer == null)
        {
            SetAlpha(0.0f);
            return;
        }

        FishingFloat activeFloat = FishingFloat.FindFloat(localPlayer);
        if (activeFloat == null || activeFloat.GetCatch() == null)
        {
            _targetAlpha = 0.0f;
        }
        else
        {
            _targetAlpha = 1.0f;
            UpdateGauge(activeFloat);
        }

        UpdateFade();
    }

    private void UpdateGauge(FishingFloat activeFloat)
    {
        FishingLineState lineState = activeFloat.GetComponent<FishingLineState>();
        float tension = lineState != null ? lineState.CurrentTension : 0.0f;

        float clampedTension = Mathf.Clamp01(tension);
        _fillTransform.sizeDelta = new Vector2(_maxWidth * clampedTension, _fillTransform.sizeDelta.y);

        Color targetColor;
        if (clampedTension < 0.40f)
        {
            targetColor = SafeTensionColor;
        }
        else if (clampedTension < 0.75f)
        {
            float t = (clampedTension - 0.40f) / 0.35f;
            targetColor = Color.Lerp(SafeTensionColor, WarningTensionColor, t);
        }
        else
        {
            float t = (clampedTension - 0.75f) / 0.25f;
            targetColor = Color.Lerp(WarningTensionColor, CriticalTensionColor, t);

            if (clampedTension >= 0.85f)
            {
                float pulse = Mathf.PingPong(Time.time * 8.0f, 0.4f) + 0.6f;
                targetColor.a = pulse;
            }
        }

        _fillImage.color = targetColor;
    }

    private void UpdateFade()
    {
        if (_canvasGroup == null)
        {
            return;
        }

        if (Mathf.Abs(_canvasGroup.alpha - _targetAlpha) > 0.01f)
        {
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, _targetAlpha, Time.deltaTime * FadeSpeed);
        }
        else
        {
            _canvasGroup.alpha = _targetAlpha;
        }
    }

    private void SetAlpha(float alpha)
    {
        _targetAlpha = alpha;
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = alpha;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
