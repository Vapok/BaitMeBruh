using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BaitMeBruh.Components;
using BaitMeBruh.Configuration;
using BaitMeBruh.Managers;

namespace BaitMeBruh.UI;

public class FishingTensionHud : MonoBehaviour
{
    private const float GaugeWidth = 240.0f;
    private const float GaugeHeight = 16.0f;
    private const float Padding = 2.5f;
    private const float DefaultHorizontalOffset = -180.0f;
    private const float DefaultVerticalOffset = -46.0f;

    private const float FadeSpeed = 6.0f;

    private static readonly Color BackgroundColor = new Color(0.04f, 0.05f, 0.07f, 0.90f);
    private static readonly Color SafeTensionColor = new Color(0.29f, 0.87f, 0.50f, 1.0f);
    private static readonly Color WarningTensionColor = new Color(0.98f, 0.80f, 0.08f, 1.0f);
    private static readonly Color CriticalTensionColor = new Color(0.94f, 0.27f, 0.27f, 1.0f);

    private static readonly AccessTools.FieldRef<FishingFloat, Fish> _nibblerRef =
        AccessTools.FieldRefAccess<FishingFloat, Fish>("m_nibbler");

    private static readonly AccessTools.FieldRef<FishingFloat, float> _lineLengthRef =
        AccessTools.FieldRefAccess<FishingFloat, float>("m_lineLength");

    public static FishingTensionHud Instance { get; private set; }

    private RectTransform _rootTransform;
    private CanvasGroup _canvasGroup;
    private RectTransform _fillTransform;
    private Image _fillImage;

    private CanvasGroup _promptCanvasGroup;
    private TextMeshProUGUI _promptText;

    private float _targetAlpha;
    private float _promptTargetAlpha;
    private float _maxWidth;
    private float _biteAlertTimer;
    private float _hookedAlertTimer;

    public void OnBiteAlert()
    {
        _biteAlertTimer = 1.25f;
    }

    public void OnHookedAlert()
    {
        _hookedAlertTimer = 1.0f;
    }

    public static void EnsureInitialized(Hud hud)
    {
        if (Instance != null || hud == null || hud.m_crosshair == null)
        {
            return;
        }

        try
        {
            Transform parent = hud.m_crosshair.transform.parent;
            if (parent == null)
            {
                return;
            }

            GameObject gaugeObject = new GameObject("FishingTensionGauge");
            gaugeObject.transform.SetParent(parent, false);

            Instance = gaugeObject.AddComponent<FishingTensionHud>();
            Instance.BuildUi(hud);
        }
        catch (System.Exception ex)
        {
            BaitMeBruh.Log.Error($"[FishingTensionHud] Failed to initialize gauge: {ex.Message}");
        }
    }

    private void BuildUi(Hud hud)
    {
        _rootTransform = gameObject.GetComponent<RectTransform>();
        if (_rootTransform == null)
        {
            _rootTransform = gameObject.AddComponent<RectTransform>();
        }

        _rootTransform.anchorMin = new Vector2(0.5f, 0.5f);
        _rootTransform.anchorMax = new Vector2(0.5f, 0.5f);
        _rootTransform.pivot = new Vector2(0.5f, 0.5f);
        _rootTransform.sizeDelta = new Vector2(GaugeWidth, GaugeHeight);
        UpdatePosition();

        GameObject gaugeContainer = new GameObject("GaugeContainer");
        gaugeContainer.transform.SetParent(transform, false);

        RectTransform gaugeRect = gaugeContainer.AddComponent<RectTransform>();
        gaugeRect.anchorMin = Vector2.zero;
        gaugeRect.anchorMax = Vector2.one;
        gaugeRect.offsetMin = Vector2.zero;
        gaugeRect.offsetMax = Vector2.zero;

        _canvasGroup = gaugeContainer.AddComponent<CanvasGroup>();
        _canvasGroup.alpha = 0.0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        Image bgImage = gaugeContainer.AddComponent<Image>();
        bgImage.color = BackgroundColor;

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(gaugeContainer.transform, false);

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

        GameObject promptObject = new GameObject("StrikePrompt");
        promptObject.transform.SetParent(transform, false);

        RectTransform promptTransform = promptObject.AddComponent<RectTransform>();
        promptTransform.anchorMin = new Vector2(0.5f, 0.5f);
        promptTransform.anchorMax = new Vector2(0.5f, 0.5f);
        promptTransform.pivot = new Vector2(0.5f, 0.5f);
        promptTransform.anchoredPosition = new Vector2(0.0f, -28.0f);
        promptTransform.sizeDelta = new Vector2(320.0f, 32.0f);

        _promptCanvasGroup = promptObject.AddComponent<CanvasGroup>();
        _promptCanvasGroup.alpha = 0.0f;
        _promptCanvasGroup.blocksRaycasts = false;
        _promptCanvasGroup.interactable = false;

        _promptText = promptObject.AddComponent<TextMeshProUGUI>();

        if (hud != null && hud.m_hoverName != null && hud.m_hoverName.font != null)
        {
            _promptText.font = hud.m_hoverName.font;
        }
        else if (hud != null && hud.m_actionName != null && hud.m_actionName.font != null)
        {
            _promptText.font = hud.m_actionName.font;
        }

        _promptText.alignment = TextAlignmentOptions.Center;
        _promptText.fontSize = 20f;
        _promptText.fontStyle = FontStyles.Bold;
        _promptText.color = new Color(1.0f, 0.84f, 0.0f, 1.0f);
        _promptText.text = "[RMB] STRIKE!";
    }

    private void UpdatePosition()
    {
        if (_rootTransform == null)
        {
            return;
        }

        float posX = ConfigRegistry.HudHorizontalOffset != null ? ConfigRegistry.HudHorizontalOffset.Value : DefaultHorizontalOffset;
        float posY = ConfigRegistry.HudVerticalOffset != null ? ConfigRegistry.HudVerticalOffset.Value : DefaultVerticalOffset;

        Vector2 currentPos = _rootTransform.anchoredPosition;
        if (Mathf.Abs(currentPos.x - posX) > 0.01f || Mathf.Abs(currentPos.y - posY) > 0.01f)
        {
            _rootTransform.anchoredPosition = new Vector2(posX, posY);
        }
    }

    private void Update()
    {
        UpdatePosition();

        if (_hookedAlertTimer > 0f)
        {
            _hookedAlertTimer -= Time.deltaTime;
        }

        if (_biteAlertTimer > 0f)
        {
            _biteAlertTimer -= Time.deltaTime;
        }

        Player localPlayer = Player.m_localPlayer;
        if (localPlayer == null)
        {
            SetAlpha(0.0f);
            return;
        }

        ItemDrop.ItemData currentWeapon = localPlayer.GetCurrentWeapon();
        bool isFishingRod = currentWeapon != null && currentWeapon.m_dropPrefab != null && currentWeapon.m_dropPrefab.name.StartsWith("FishingRod");
        float drawPercentage = localPlayer.GetAttackDrawPercentage();

        if (isFishingRod && drawPercentage > 0.01f)
        {
            bool isPrimitive = currentWeapon.m_dropPrefab.name == "FishingRodPrimitive";
            float minCastDistance = 6.0f;
            float maxCastDistance = isPrimitive ? 20.0f : 30.0f;
            float estimatedDistance = Mathf.Lerp(minCastDistance, maxCastDistance, drawPercentage);

            _targetAlpha = 1.0f;
            _promptTargetAlpha = 1.0f;

            _fillTransform.sizeDelta = new Vector2(_maxWidth * drawPercentage, _fillTransform.sizeDelta.y);
            _fillImage.color = Color.Lerp(new Color(0.25f, 0.78f, 1.0f, 1.0f), new Color(0.20f, 0.95f, 0.60f, 1.0f), drawPercentage);

            string morningTag = MorningFishManager.IsMorningFishingHour() ? " <color=#FFD700>• MORNING BITE</color>" : "";
            if (drawPercentage >= 0.95f)
            {
                _promptText.text = $"[LMB] CAST: {Mathf.RoundToInt(estimatedDistance)}m (MAX){morningTag}";
                _promptText.color = new Color(0.20f, 0.95f, 0.60f, 1.0f);
            }
            else
            {
                _promptText.text = $"[LMB] CAST: {Mathf.RoundToInt(estimatedDistance)}m / {Mathf.RoundToInt(maxCastDistance)}m{morningTag}";
                _promptText.color = Color.white;
            }

            UpdateFade();
            return;
        }

        FishingFloat activeFloat = FishingFloat.FindFloat(localPlayer);
        if (activeFloat == null)
        {
            _targetAlpha = 0.0f;
            _promptTargetAlpha = 0.0f;
        }
        else if (activeFloat.GetCatch() != null)
        {
            _targetAlpha = 1.0f;
            _promptTargetAlpha = 1.0f;
            UpdateGauge(activeFloat);

            if (_hookedAlertTimer > 0f)
            {
                _promptText.text = "<color=#FFD700><b><size=22>HOOKED!</size></b></color>";
            }
            else
            {
                FishingLineState lineState = activeFloat.GetComponent<FishingLineState>();
                float tension = lineState != null ? lineState.CurrentTension : 0.0f;
                float clampedTension = Mathf.Clamp01(tension);
                float currentLineLen = _lineLengthRef(activeFloat);
                int meters = Mathf.Max(0, Mathf.RoundToInt(currentLineLen));

                if (clampedTension >= 0.75f)
                {
                    _promptText.text = $"RELEASE REEL! • {meters}m";
                    _promptText.color = CriticalTensionColor;
                }
                else if (clampedTension >= 0.40f)
                {
                    _promptText.text = $"REELING [RMB] • {meters}m";
                    _promptText.color = WarningTensionColor;
                }
                else
                {
                    _promptText.text = $"REELING [RMB] • {meters}m";
                    _promptText.color = SafeTensionColor;
                }
            }
        }
        else if (isFishingRod && localPlayer.IsBlocking())
        {
            float currentLineLen = _lineLengthRef(activeFloat);
            int meters = Mathf.Max(0, Mathf.RoundToInt(currentLineLen));
            float maxDist = activeFloat.m_maxDistance;
            float fillRatio = Mathf.Clamp01(currentLineLen / maxDist);

            _targetAlpha = 1.0f;
            _promptTargetAlpha = 1.0f;

            _fillTransform.sizeDelta = new Vector2(_maxWidth * fillRatio, _fillTransform.sizeDelta.y);
            _fillImage.color = new Color(0.39f, 0.78f, 0.98f, 1.0f);

            _promptText.text = $"RETRIEVING • {meters}m";
            _promptText.color = new Color(0.78f, 0.87f, 0.93f, 1.0f);
        }
        else
        {
            _targetAlpha = 0.0f;
            Fish nibbler = _nibblerRef(activeFloat);
            if (nibbler != null || _biteAlertTimer > 0f)
            {
                _promptText.text = "<b><size=22>[RMB] STRIKE!</size></b>";
                _promptText.color = Color.Lerp(new Color(1.0f, 0.85f, 0.0f, 1.0f), new Color(1.0f, 0.45f, 0.0f, 1.0f), Mathf.PingPong(Time.time * 6.0f, 1.0f));
                _promptTargetAlpha = 1.0f;
            }
            else
            {
                _promptTargetAlpha = 0.0f;
            }
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
        if (_canvasGroup != null)
        {
            if (Mathf.Abs(_canvasGroup.alpha - _targetAlpha) > 0.01f)
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, _targetAlpha, Time.deltaTime * FadeSpeed);
            }
            else
            {
                _canvasGroup.alpha = _targetAlpha;
            }
        }

        if (_promptCanvasGroup != null)
        {
            if (Mathf.Abs(_promptCanvasGroup.alpha - _promptTargetAlpha) > 0.01f)
            {
                _promptCanvasGroup.alpha = Mathf.MoveTowards(_promptCanvasGroup.alpha, _promptTargetAlpha, Time.deltaTime * FadeSpeed * 2.0f);
            }
            else
            {
                _promptCanvasGroup.alpha = _promptTargetAlpha;
            }
        }
    }

    private void SetAlpha(float alpha)
    {
        _targetAlpha = alpha;
        _promptTargetAlpha = alpha;
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = alpha;
        }
        if (_promptCanvasGroup != null)
        {
            _promptCanvasGroup.alpha = alpha;
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
