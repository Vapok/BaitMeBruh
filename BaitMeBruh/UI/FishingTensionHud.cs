using System.Collections.Generic;
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
    private const float BoxHeight = 22.0f;
    private const float Padding = 2.5f;
    private const float DefaultHorizontalOffset = -300.0f;
    private const float DefaultVerticalOffset = -250.0f;

    private const float FadeSpeed = 6.0f;

    private static readonly Color BackgroundColor = new Color(0.04f, 0.05f, 0.07f, 0.90f);
    private static readonly Color TrackColor = new Color(0.10f, 0.13f, 0.17f, 0.65f);
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
    private TextMeshProUGUI _gaugeCenterText;

    private CanvasGroup _promptCanvasGroup;
    private TextMeshProUGUI _promptText;

    private RectTransform _baitTransform;
    private CanvasGroup _baitCanvasGroup;
    private Image _baitIcon;
    private TextMeshProUGUI _baitText;

    private float _targetAlpha;
    private float _promptTargetAlpha;
    private float _baitTargetAlpha;
    private float _maxWidth;
    private float _biteAlertTimer;
    private float _hookedAlertTimer;
    private float _recentCastTimer;

    public bool IsBiteAlertActive => _biteAlertTimer > 0f;
    public bool IsHookedAlertActive => _hookedAlertTimer > 0f;

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
        _rootTransform.sizeDelta = new Vector2(GaugeWidth, BoxHeight * 2.0f);
        UpdatePosition();

        TMP_FontAsset fontAsset = null;
        if (hud != null && hud.m_hoverName != null && hud.m_hoverName.font != null)
        {
            fontAsset = hud.m_hoverName.font;
        }
        else if (hud != null && hud.m_actionName != null && hud.m_actionName.font != null)
        {
            fontAsset = hud.m_actionName.font;
        }

        // 1. Bait Display Widget (Top Box, touching gauge with 0 gap)
        GameObject baitObject = new GameObject("BaitContainer");
        baitObject.transform.SetParent(transform, false);

        _baitTransform = baitObject.AddComponent<RectTransform>();
        _baitTransform.anchorMin = new Vector2(0.5f, 0.5f);
        _baitTransform.anchorMax = new Vector2(0.5f, 0.5f);
        _baitTransform.pivot = new Vector2(0.5f, 0.5f);
        _baitTransform.anchoredPosition = new Vector2(0.0f, BoxHeight / 2.0f);
        _baitTransform.sizeDelta = new Vector2(GaugeWidth, BoxHeight);

        _baitCanvasGroup = baitObject.AddComponent<CanvasGroup>();
        _baitCanvasGroup.alpha = 0.0f;
        _baitCanvasGroup.blocksRaycasts = false;
        _baitCanvasGroup.interactable = false;

        Image baitBg = baitObject.AddComponent<Image>();
        baitBg.color = BackgroundColor;

        GameObject baitTrackObject = new GameObject("BaitTrack");
        baitTrackObject.transform.SetParent(baitObject.transform, false);

        RectTransform baitTrackRect = baitTrackObject.AddComponent<RectTransform>();
        baitTrackRect.anchorMin = Vector2.zero;
        baitTrackRect.anchorMax = Vector2.one;
        baitTrackRect.offsetMin = new Vector2(Padding, Padding);
        baitTrackRect.offsetMax = new Vector2(-Padding, -Padding);

        Image baitTrackImage = baitTrackObject.AddComponent<Image>();
        baitTrackImage.color = TrackColor;

        GameObject iconObject = new GameObject("BaitIcon");
        iconObject.transform.SetParent(baitObject.transform, false);

        RectTransform iconRect = iconObject.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.0f, 0.5f);
        iconRect.anchorMax = new Vector2(0.0f, 0.5f);
        iconRect.pivot = new Vector2(0.0f, 0.5f);
        iconRect.anchoredPosition = new Vector2(16.0f, 0.0f);
        iconRect.sizeDelta = new Vector2(16.0f, 16.0f);

        _baitIcon = iconObject.AddComponent<Image>();
        _baitIcon.preserveAspect = true;
        _baitIcon.enabled = false;

        GameObject textObject = new GameObject("BaitText");
        textObject.transform.SetParent(baitObject.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.0f, 0.0f);
        textRect.anchorMax = new Vector2(1.0f, 1.0f);
        textRect.offsetMin = new Vector2(36.0f, 0.0f);
        textRect.offsetMax = new Vector2(-6.0f, 0.0f);

        _baitText = textObject.AddComponent<TextMeshProUGUI>();
        if (fontAsset != null)
        {
            _baitText.font = fontAsset;
        }

        _baitText.alignment = TextAlignmentOptions.MidlineLeft;
        _baitText.fontSize = 13.0f;
        _baitText.color = Color.white;

        // 2. Gauge Container (Bottom Box, touching bait with 0 gap)
        GameObject gaugeContainer = new GameObject("GaugeContainer");
        gaugeContainer.transform.SetParent(transform, false);

        RectTransform gaugeRect = gaugeContainer.AddComponent<RectTransform>();
        gaugeRect.anchorMin = new Vector2(0.5f, 0.5f);
        gaugeRect.anchorMax = new Vector2(0.5f, 0.5f);
        gaugeRect.pivot = new Vector2(0.5f, 0.5f);
        gaugeRect.anchoredPosition = new Vector2(0.0f, -BoxHeight / 2.0f);
        gaugeRect.sizeDelta = new Vector2(GaugeWidth, BoxHeight);

        _canvasGroup = gaugeContainer.AddComponent<CanvasGroup>();
        _canvasGroup.alpha = 0.0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        Image bgImage = gaugeContainer.AddComponent<Image>();
        bgImage.color = BackgroundColor;

        GameObject trackObject = new GameObject("Track");
        trackObject.transform.SetParent(gaugeContainer.transform, false);

        RectTransform trackRect = trackObject.AddComponent<RectTransform>();
        trackRect.anchorMin = Vector2.zero;
        trackRect.anchorMax = Vector2.one;
        trackRect.offsetMin = new Vector2(Padding, Padding);
        trackRect.offsetMax = new Vector2(-Padding, -Padding);

        Image trackImage = trackObject.AddComponent<Image>();
        trackImage.color = TrackColor;

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(gaugeContainer.transform, false);

        _fillTransform = fillObject.AddComponent<RectTransform>();
        _fillTransform.anchorMin = new Vector2(0.0f, 0.5f);
        _fillTransform.anchorMax = new Vector2(0.0f, 0.5f);
        _fillTransform.pivot = new Vector2(0.0f, 0.5f);
        _fillTransform.anchoredPosition = new Vector2(Padding, 0.0f);

        _maxWidth = GaugeWidth - (Padding * 2.0f);
        float fillHeight = BoxHeight - (Padding * 2.0f);
        _fillTransform.sizeDelta = new Vector2(0.0f, fillHeight);

        _fillImage = fillObject.AddComponent<Image>();
        _fillImage.color = SafeTensionColor;

        GameObject centerTextObject = new GameObject("CenterText");
        centerTextObject.transform.SetParent(gaugeContainer.transform, false);

        RectTransform centerTextRect = centerTextObject.AddComponent<RectTransform>();
        centerTextRect.anchorMin = Vector2.zero;
        centerTextRect.anchorMax = Vector2.one;
        centerTextRect.offsetMin = Vector2.zero;
        centerTextRect.offsetMax = Vector2.zero;

        _gaugeCenterText = centerTextObject.AddComponent<TextMeshProUGUI>();
        if (fontAsset != null)
        {
            _gaugeCenterText.font = fontAsset;
        }
        _gaugeCenterText.alignment = TextAlignmentOptions.Center;
        _gaugeCenterText.fontSize = 12.0f;
        _gaugeCenterText.fontStyle = FontStyles.Bold;
        _gaugeCenterText.color = new Color(0.72f, 0.82f, 0.92f, 0.85f);
        _gaugeCenterText.text = Localization.instance != null ? Localization.instance.Localize("$hud_fishing_ready_to_cast") : "Ready to Cast";

        // 3. Strike / Cast / Reel Prompt (Below gauge)
        GameObject promptObject = new GameObject("StrikePrompt");
        promptObject.transform.SetParent(transform, false);

        RectTransform promptTransform = promptObject.AddComponent<RectTransform>();
        promptTransform.anchorMin = new Vector2(0.5f, 0.5f);
        promptTransform.anchorMax = new Vector2(0.5f, 0.5f);
        promptTransform.pivot = new Vector2(0.5f, 0.5f);
        promptTransform.anchoredPosition = new Vector2(0.0f, -36.0f);
        promptTransform.sizeDelta = new Vector2(320.0f, 26.0f);

        _promptCanvasGroup = promptObject.AddComponent<CanvasGroup>();
        _promptCanvasGroup.alpha = 0.0f;
        _promptCanvasGroup.blocksRaycasts = false;
        _promptCanvasGroup.interactable = false;

        _promptText = promptObject.AddComponent<TextMeshProUGUI>();
        if (fontAsset != null)
        {
            _promptText.font = fontAsset;
        }

        _promptText.alignment = TextAlignmentOptions.Center;
        _promptText.fontSize = 19.0f;
        _promptText.fontStyle = FontStyles.Bold;
        _promptText.color = new Color(1.0f, 0.84f, 0.0f, 1.0f);
        _promptText.text = "STRIKE!";
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

        if (_recentCastTimer > 0f)
        {
            _recentCastTimer -= Time.deltaTime;
        }

        if (InventoryGui.IsVisible())
        {
            SetAlpha(0.0f);
            return;
        }

        Player localPlayer = Player.m_localPlayer;
        if (localPlayer == null)
        {
            SetAlpha(0.0f);
            return;
        }

        ItemDrop.ItemData currentWeapon = localPlayer.GetCurrentWeapon();
        bool isFishingRod = currentWeapon != null && currentWeapon.m_dropPrefab != null && currentWeapon.m_dropPrefab.name.StartsWith("FishingRod");
        if (!isFishingRod)
        {
            SetAlpha(0.0f);
            return;
        }

        if (currentWeapon.m_shared != null && currentWeapon.m_shared.m_attack != null)
        {
            bool isPrimitiveWeapon = currentWeapon.m_dropPrefab.name == "FishingRodPrimitive";
            float targetVel = isPrimitiveWeapon ? 22.0f : 26.0f;
            if (currentWeapon.m_shared.m_attack.m_projectileVel != targetVel || currentWeapon.m_shared.m_attack.m_drawDurationMin != 2.8f)
            {
                currentWeapon.m_shared.m_attack.m_projectileVel = targetVel;
                currentWeapon.m_shared.m_attack.m_projectileVelMin = 2.0f;
                currentWeapon.m_shared.m_attack.m_drawDurationMin = 2.8f;
            }
        }

        FishingFloat activeFloat = FishingFloat.FindFloat(localPlayer);
        float drawPercentage = localPlayer.GetAttackDrawPercentage();

        if (activeFloat == null && drawPercentage <= 0.01f)
        {
            bool chatFocused = Chat.instance != null && Chat.instance.HasFocus();
            bool menuOpen = Menu.IsVisible() || TextInput.IsVisible();
            if (!chatFocused && !menuOpen)
            {
                KeyCode switchKey = ConfigRegistry.SwitchBaitKey != null ? ConfigRegistry.SwitchBaitKey.Value : KeyCode.G;
                bool kbTriggered = Input.GetKeyDown(switchKey);

                string gpButton = ConfigRegistry.SwitchBaitGamepadButton != null ? ConfigRegistry.SwitchBaitGamepadButton.Value : "JoyRBumper";
                bool gpTriggered = (!string.IsNullOrEmpty(gpButton) && ZInput.GetButtonDown(gpButton)) || Input.GetKeyDown(KeyCode.JoystickButton5);

                if (kbTriggered || gpTriggered)
                {
                    bool reverse = kbTriggered && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                    BaitSelectionManager.CycleBait(localPlayer, currentWeapon, reverse);
                }
            }
        }

        UpdateBaitDisplay(localPlayer, currentWeapon, activeFloat);

        float fillHeight = BoxHeight - (Padding * 2.0f);

        if (drawPercentage > 0.01f)
        {
            bool isPrimitive = currentWeapon.m_dropPrefab.name == "FishingRodPrimitive";
            float minCastDistance = 4.0f;
            float safeMaxDistance = isPrimitive ? 20.0f : 30.0f;
            float maxCastDistance = safeMaxDistance + 10.0f;
            float estimatedDistance = Mathf.Lerp(minCastDistance, maxCastDistance, drawPercentage);

            _targetAlpha = 1.0f;
            _promptTargetAlpha = 1.0f;

            if (_gaugeCenterText != null)
            {
                _gaugeCenterText.enabled = false;
            }

            _recentCastTimer = 0.6f;

            string morningTag = MorningFishManager.IsMorningFishingHour() ? " <color=#FFD700>• MORNING BITE</color>" : "";
            int estInt = Mathf.RoundToInt(estimatedDistance);
            int safeInt = Mathf.RoundToInt(safeMaxDistance);

            float drawFillRatio = Mathf.Clamp01(drawPercentage);
            _fillTransform.sizeDelta = new Vector2(_maxWidth * drawFillRatio, fillHeight);

            if (estimatedDistance <= safeMaxDistance)
            {
                float safeRatio = (safeMaxDistance - minCastDistance) > 0f ? (estimatedDistance - minCastDistance) / (safeMaxDistance - minCastDistance) : 0f;
                _fillImage.color = Color.Lerp(new Color(0.25f, 0.78f, 1.0f, 1.0f), new Color(0.20f, 0.95f, 0.60f, 1.0f), safeRatio);

                _promptText.text = $"CAST: {estInt}m / {safeInt}m{morningTag}";
                _promptText.color = Color.white;
            }
            else
            {
                float riskRatio = Mathf.Clamp01((estimatedDistance - safeMaxDistance) / 10.0f);
                Color dangerColor = Color.Lerp(new Color(1.0f, 0.60f, 0.0f, 1.0f), new Color(1.0f, 0.20f, 0.20f, 1.0f), riskRatio);
                _fillImage.color = dangerColor;

                string riskWarning = Localization.instance != null ? Localization.instance.Localize("$hud_fishing_line_break_risk") : "LINE BREAK RISK!";
                _promptText.text = $"CAST: {estInt}m / {safeInt}m (<color=#FF3333><b>{riskWarning}</b></color>){morningTag}";
                _promptText.color = dangerColor;
            }

            UpdateFade();
            return;
        }

        if (activeFloat == null)
        {
            GameObject lastProj = currentWeapon.m_lastProjectile;
            bool isCastingInProgress = (lastProj != null) || localPlayer.InAttack() || (_recentCastTimer > 0f);

            if (isCastingInProgress)
            {
                _targetAlpha = 1.0f;
                _promptTargetAlpha = 1.0f;

                if (_gaugeCenterText != null)
                {
                    _gaugeCenterText.enabled = true;
                    _gaugeCenterText.text = Localization.instance != null ? Localization.instance.Localize("$hud_fishing_casting") : "Casting...";
                }

                if (lastProj != null)
                {
                    _recentCastTimer = 0.6f;
                    Transform rodTop = Utils.FindChild(localPlayer.transform, "_RodTop");
                    Vector3 origin = rodTop != null ? rodTop.position : localPlayer.transform.position;
                    float flightDist = Vector3.Distance(origin, lastProj.transform.position);
                    int flightMeters = Mathf.RoundToInt(flightDist);

                    bool isPrimitive = currentWeapon.m_dropPrefab.name == "FishingRodPrimitive";
                    float safeMax = isPrimitive ? 20.0f : 30.0f;
                    float maxRange = safeMax + 10.0f;

                    _fillTransform.sizeDelta = new Vector2(_maxWidth * Mathf.Clamp01(flightDist / maxRange), fillHeight);

                    if (flightDist > safeMax)
                    {
                        _fillImage.color = new Color(1.0f, 0.25f, 0.25f, 1.0f);
                    }
                    else
                    {
                        _fillImage.color = new Color(0.25f, 0.78f, 1.0f, 1.0f);
                    }

                    _promptText.text = $"CASTING • {flightMeters}m";
                    _promptText.color = new Color(0.39f, 0.78f, 0.98f, 1.0f);
                }
                else
                {
                    _fillTransform.sizeDelta = new Vector2(0.0f, fillHeight);
                    _promptText.text = Localization.instance != null ? Localization.instance.Localize("$hud_fishing_casting") : "Casting...";
                    _promptText.color = new Color(0.39f, 0.78f, 0.98f, 1.0f);
                }

                UpdateFade();
                return;
            }

            _targetAlpha = 0.85f;
            _promptTargetAlpha = 0.0f;
            _fillTransform.sizeDelta = new Vector2(0.0f, fillHeight);

            if (_gaugeCenterText != null)
            {
                _gaugeCenterText.enabled = true;
                _gaugeCenterText.text = Localization.instance != null ? Localization.instance.Localize("$hud_fishing_ready_to_cast") : "Ready to Cast";
            }
        }
        else if (activeFloat.GetCatch() != null)
        {
            _targetAlpha = 1.0f;
            _promptTargetAlpha = 1.0f;

            if (_gaugeCenterText != null)
            {
                _gaugeCenterText.enabled = false;
            }

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
                    _promptText.text = $"REELING • {meters}m";
                    _promptText.color = WarningTensionColor;
                }
                else
                {
                    _promptText.text = $"REELING • {meters}m";
                    _promptText.color = SafeTensionColor;
                }
            }
        }
        else if (localPlayer.IsBlocking())
        {
            float currentLineLen = _lineLengthRef(activeFloat);
            int meters = Mathf.Max(0, Mathf.RoundToInt(currentLineLen));
            float maxDist = activeFloat.m_maxDistance;
            float fillRatio = Mathf.Clamp01(currentLineLen / maxDist);

            _targetAlpha = 1.0f;
            _promptTargetAlpha = 1.0f;

            if (_gaugeCenterText != null)
            {
                _gaugeCenterText.enabled = false;
            }

            _fillTransform.sizeDelta = new Vector2(_maxWidth * fillRatio, fillHeight);
            _fillImage.color = new Color(0.39f, 0.78f, 0.98f, 1.0f);

            _promptText.text = $"RETRIEVING • {meters}m";
            _promptText.color = new Color(0.78f, 0.87f, 0.93f, 1.0f);
        }
        else
        {
            _targetAlpha = 0.85f;
            _fillTransform.sizeDelta = new Vector2(0.0f, fillHeight);

            if (_gaugeCenterText != null)
            {
                _gaugeCenterText.enabled = false;
            }

            Fish nibbler = _nibblerRef(activeFloat);
            if (nibbler != null || _biteAlertTimer > 0f)
            {
                _promptText.text = "<b><size=22>STRIKE!</size></b>";
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

    private void UpdateBaitDisplay(Player localPlayer, ItemDrop.ItemData currentWeapon, FishingFloat activeFloat)
    {
        if (activeFloat != null)
        {
            string baitPrefabName = activeFloat.GetBait();
            string lineLabel = Localization.instance != null ? Localization.instance.Localize("$hud_fishing_line") : "Line:";
            if (!string.IsNullOrEmpty(baitPrefabName))
            {
                Sprite icon = BaitSelectionManager.GetBaitIconByPrefabName(baitPrefabName);
                string baitName = BaitSelectionManager.GetBaitNameByPrefabName(baitPrefabName);

                _baitIcon.sprite = icon;
                _baitIcon.enabled = (icon != null);
                _baitText.text = $"<color=#70A0C0>{lineLabel}</color>   <color=#FFD700><b>{baitName}</b></color>";
            }
            else
            {
                string noBait = Localization.instance != null ? Localization.instance.Localize("$hud_fishing_no_bait") : "No Bait";
                _baitIcon.enabled = false;
                _baitText.text = $"<color=#70A0C0>{lineLabel}</color>   <color=#A0A0A0>{noBait}</color>";
            }
            _baitTargetAlpha = 1.0f;
        }
        else
        {
            ItemDrop.ItemData currentBait = BaitSelectionManager.GetCurrentBait(localPlayer, currentWeapon);

            if (currentBait != null)
            {
                Sprite icon = currentBait.GetIcon();
                string baitName = Localization.instance != null ? Localization.instance.Localize(currentBait.m_shared.m_name) : currentBait.m_shared.m_name;
                int count = BaitSelectionManager.GetBaitTotalCount(localPlayer, currentBait);

                _baitIcon.sprite = icon;
                _baitIcon.enabled = (icon != null);
                _baitText.text = $"<color=#FFD700><b>{baitName}</b></color>    <color=#D0E0F0>({count})</color>";
            }
            else
            {
                string noBaitBags = Localization.instance != null ? Localization.instance.Localize("$hud_fishing_no_bait_in_bags") : "No Bait in Bags";
                _baitIcon.enabled = false;
                _baitText.text = $"<color=#FF4D4D><b>{noBaitBags}</b></color>";
            }
            _baitTargetAlpha = 1.0f;
        }
    }

    private void UpdateGauge(FishingFloat activeFloat)
    {
        FishingLineState lineState = activeFloat.GetComponent<FishingLineState>();
        float tension = lineState != null ? lineState.CurrentTension : 0.0f;

        float clampedTension = Mathf.Clamp01(tension);
        float fillHeight = BoxHeight - (Padding * 2.0f);
        _fillTransform.sizeDelta = new Vector2(_maxWidth * clampedTension, fillHeight);

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

        if (_baitCanvasGroup != null)
        {
            if (Mathf.Abs(_baitCanvasGroup.alpha - _baitTargetAlpha) > 0.01f)
            {
                _baitCanvasGroup.alpha = Mathf.MoveTowards(_baitCanvasGroup.alpha, _baitTargetAlpha, Time.deltaTime * FadeSpeed);
            }
            else
            {
                _baitCanvasGroup.alpha = _baitTargetAlpha;
            }
        }
    }

    private void SetAlpha(float alpha)
    {
        _targetAlpha = alpha;
        _promptTargetAlpha = alpha;
        _baitTargetAlpha = alpha;

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = alpha;
        }

        if (_promptCanvasGroup != null)
        {
            _promptCanvasGroup.alpha = alpha;
        }

        if (_baitCanvasGroup != null)
        {
            _baitCanvasGroup.alpha = alpha;
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
