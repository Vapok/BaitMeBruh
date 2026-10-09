using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BaitMeBruh.Components;
using BaitMeBruh.Configuration;
using BaitMeBruh.UI;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(KeyHints), "UpdateHints")]
internal static class KeyHintsUpdateHintsPatch
{
    private static readonly AccessTools.FieldRef<FishingFloat, Fish> _nibblerRef =
        AccessTools.FieldRefAccess<FishingFloat, Fish>("m_nibbler");

    private static GameObject _switchBaitHintObjectKB;
    private static TextMeshProUGUI _switchBaitLabelKB;
    private static TextMeshProUGUI _switchBaitKeyKB;
    private static TextMeshProUGUI _pullTextKB;

    private static GameObject _switchBaitHintObjectGP;
    private static TextMeshProUGUI _switchBaitLabelGP;
    private static TextMeshProUGUI _switchBaitKeyGP;
    private static TextMeshProUGUI _pullTextGP;

    private static void Postfix(KeyHints __instance)
    {
        if (__instance == null || __instance.m_fishingHints == null)
        {
            return;
        }

        if (!__instance.m_fishingHints.activeSelf)
        {
            return;
        }

        Player localPlayer = Player.m_localPlayer;
        if (localPlayer == null)
        {
            return;
        }

        Transform hintsTransform = __instance.m_fishingHints.transform;

        Transform rogueKB = hintsTransform.Find("BaitMeBruh_SwitchBaitHint");
        if (rogueKB != null)
        {
            Object.Destroy(rogueKB.gameObject);
        }

        Transform rogueGP = hintsTransform.Find("BaitMeBruh_SwitchBaitHintGP");
        if (rogueGP != null)
        {
            Object.Destroy(rogueGP.gameObject);
        }

        Transform keyboardTransform = hintsTransform.Find("Keyboard");
        Transform gamepadTransform = hintsTransform.Find("Gamepad");

        UIInputHint inputHint = __instance.m_fishingHints.GetComponent<UIInputHint>();
        if (inputHint != null)
        {
            if (keyboardTransform == null && inputHint.m_mouseKeyboardHint != null)
            {
                keyboardTransform = inputHint.m_mouseKeyboardHint.transform;
            }
            if (gamepadTransform == null && inputHint.m_gamepadHint != null)
            {
                gamepadTransform = inputHint.m_gamepadHint.transform;
            }
        }

        FishingFloat activeFloat = FishingFloat.FindFloat(localPlayer);
        float drawPercentage = localPlayer.GetAttackDrawPercentage();
        bool showSwitchHint = activeFloat == null && drawPercentage <= 0.01f;

        string switchBaitLocalized = Localization.instance != null
            ? Localization.instance.Localize("$hud_fishing_switch_bait")
            : "Switch Bait";

        if (keyboardTransform != null)
        {
            if (_switchBaitHintObjectKB == null || _switchBaitHintObjectKB.transform.parent != keyboardTransform)
            {
                Transform existing = keyboardTransform.Find("BaitMeBruh_SwitchBaitHintKB") ?? keyboardTransform.Find("BaitMeBruh_SwitchBaitHint");
                if (existing != null)
                {
                    _switchBaitHintObjectKB = existing.gameObject;
                    _switchBaitHintObjectKB.name = "BaitMeBruh_SwitchBaitHintKB";
                }
                else
                {
                    Transform primaryAttack = keyboardTransform.Find("PrimaryAttack");
                    if (primaryAttack != null)
                    {
                        _switchBaitHintObjectKB = Object.Instantiate(primaryAttack.gameObject, keyboardTransform, false);
                        _switchBaitHintObjectKB.name = "BaitMeBruh_SwitchBaitHintKB";
                        _switchBaitHintObjectKB.transform.SetSiblingIndex(1);
                    }
                }

                if (_switchBaitHintObjectKB != null)
                {
                    Transform labelTransform = _switchBaitHintObjectKB.transform.Find("Text");
                    if (labelTransform != null)
                    {
                        _switchBaitLabelKB = labelTransform.GetComponent<TextMeshProUGUI>();
                        Localize loc = labelTransform.GetComponent<Localize>();
                        if (loc != null)
                        {
                            Object.Destroy(loc);
                        }
                    }

                    Transform keyTransform = _switchBaitHintObjectKB.transform.Find("key_bkg/Key") ?? _switchBaitHintObjectKB.transform.Find("Key");
                    if (keyTransform != null)
                    {
                        _switchBaitKeyKB = keyTransform.GetComponent<TextMeshProUGUI>();
                        Localize keyLoc = keyTransform.GetComponent<Localize>();
                        if (keyLoc != null)
                        {
                            Object.Destroy(keyLoc);
                        }
                    }
                }
            }

            if (_switchBaitLabelKB != null)
            {
                _switchBaitLabelKB.text = switchBaitLocalized;
            }

            if (_switchBaitKeyKB != null)
            {
                KeyCode switchKey = ConfigRegistry.SwitchBaitKey != null ? ConfigRegistry.SwitchBaitKey.Value : KeyCode.G;
                _switchBaitKeyKB.text = switchKey.ToString();
            }

            if (_switchBaitHintObjectKB != null && _switchBaitHintObjectKB.activeSelf != showSwitchHint)
            {
                _switchBaitHintObjectKB.SetActive(showSwitchHint);
                if (keyboardTransform is RectTransform kbRt)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(kbRt);
                }
            }

            if (_pullTextKB == null || _pullTextKB.transform.parent == null)
            {
                Transform blockText = keyboardTransform.Find("Block/Text");
                if (blockText != null)
                {
                    _pullTextKB = blockText.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        if (gamepadTransform != null)
        {
            if (_switchBaitHintObjectGP == null || _switchBaitHintObjectGP.transform.parent != gamepadTransform)
            {
                Transform existingGP = gamepadTransform.Find("BaitMeBruh_SwitchBaitHintGP");
                if (existingGP != null)
                {
                    _switchBaitHintObjectGP = existingGP.gameObject;
                }
                else
                {
                    Transform template = gamepadTransform.Find("PrimaryAttack");
                    if (template == null && gamepadTransform.childCount > 0)
                    {
                        template = gamepadTransform.GetChild(0);
                    }

                    if (template != null)
                    {
                        _switchBaitHintObjectGP = Object.Instantiate(template.gameObject, gamepadTransform, false);
                        _switchBaitHintObjectGP.name = "BaitMeBruh_SwitchBaitHintGP";
                        _switchBaitHintObjectGP.transform.SetSiblingIndex(1);
                    }
                }

                if (_switchBaitHintObjectGP != null)
                {
                    Transform labelTransform = _switchBaitHintObjectGP.transform.Find("Text");
                    if (labelTransform != null)
                    {
                        _switchBaitLabelGP = labelTransform.GetComponent<TextMeshProUGUI>();
                        Localize loc = labelTransform.GetComponent<Localize>();
                        if (loc != null)
                        {
                            Object.Destroy(loc);
                        }
                    }

                    Transform keyTransform = _switchBaitHintObjectGP.transform.Find("key_bkg/Key") ?? _switchBaitHintObjectGP.transform.Find("Key");
                    if (keyTransform != null)
                    {
                        _switchBaitKeyGP = keyTransform.GetComponent<TextMeshProUGUI>();
                        Localize keyLoc = keyTransform.GetComponent<Localize>();
                        if (keyLoc != null)
                        {
                            Object.Destroy(keyLoc);
                        }
                    }

                    if (_switchBaitKeyGP == null)
                    {
                        TextMeshProUGUI[] texts = _switchBaitHintObjectGP.GetComponentsInChildren<TextMeshProUGUI>(true);
                        for (int i = 0; i < texts.Length; i++)
                        {
                            TextMeshProUGUI t = texts[i];
                            if (t != _switchBaitLabelGP)
                            {
                                _switchBaitKeyGP = t;
                                Localize keyLoc = t.GetComponent<Localize>();
                                if (keyLoc != null)
                                {
                                    Object.Destroy(keyLoc);
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (_switchBaitLabelGP != null)
            {
                _switchBaitLabelGP.text = switchBaitLocalized;
            }

            if (_switchBaitKeyGP != null)
            {
                string gpKeyStr = null;
                string gpButton = ConfigRegistry.SwitchBaitGamepadButton != null ? ConfigRegistry.SwitchBaitGamepadButton.Value : "JoyRBumper";
                if (ZInput.instance != null && !string.IsNullOrEmpty(gpButton))
                {
                    string bound = ZInput.instance.GetBoundKeyString(gpButton, true);
                    if (!string.IsNullOrEmpty(bound) && !bound.StartsWith("MISSING"))
                    {
                        gpKeyStr = bound;
                    }
                }

                if (string.IsNullOrEmpty(gpKeyStr))
                {
                    gpKeyStr = Localization.instance != null ? Localization.instance.Localize("$KEY_RBumper") : "RB";
                    if (string.IsNullOrEmpty(gpKeyStr) || gpKeyStr.StartsWith("["))
                    {
                        gpKeyStr = "RB";
                    }
                }
                _switchBaitKeyGP.text = gpKeyStr;
            }

            if (_switchBaitHintObjectGP != null && _switchBaitHintObjectGP.activeSelf != showSwitchHint)
            {
                _switchBaitHintObjectGP.SetActive(showSwitchHint);
                if (gamepadTransform is RectTransform gpRt)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(gpRt);
                }
            }

            if (_pullTextGP == null || _pullTextGP.transform.parent == null)
            {
                Transform blockTextGP = gamepadTransform.Find("Block/Text");
                if (blockTextGP != null)
                {
                    _pullTextGP = blockTextGP.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        string pullString = Localization.instance != null ? Localization.instance.Localize("$hud_fishing_pull") : "Pull";
        if (activeFloat != null)
        {
            if (activeFloat.GetCatch() != null)
            {
                FishingLineState lineState = activeFloat.GetComponent<FishingLineState>();
                float tension = lineState != null ? lineState.CurrentTension : 0.0f;
                pullString = (tension >= 0.75f)
                    ? (Localization.instance != null ? Localization.instance.Localize("$hud_fishing_release") : "Release")
                    : (Localization.instance != null ? Localization.instance.Localize("$hud_fishing_reel") : "Reel");
            }
            else
            {
                bool isBite = (FishingTensionHud.Instance != null && FishingTensionHud.Instance.IsBiteAlertActive) || (_nibblerRef(activeFloat) != null);
                pullString = isBite
                    ? (Localization.instance != null ? Localization.instance.Localize("$hud_fishing_strike") : "Strike")
                    : (Localization.instance != null ? Localization.instance.Localize("$hud_fishing_pull") : "Pull");
            }
        }

        if (_pullTextKB != null)
        {
            _pullTextKB.text = pullString;
        }

        if (_pullTextGP != null)
        {
            _pullTextGP.text = pullString;
        }
    }
}
