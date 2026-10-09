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

    private static GameObject _switchBaitHintObject;
    private static TextMeshProUGUI _switchBaitLabel;
    private static TextMeshProUGUI _switchBaitKey;
    private static TextMeshProUGUI _pullText;

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

        // Clean up any rogue hint objects created in earlier versions directly under FishingHints
        Transform rogue = hintsTransform.Find("BaitMeBruh_SwitchBaitHint");
        if (rogue != null)
        {
            Object.Destroy(rogue.gameObject);
        }

        Transform keyboardTransform = hintsTransform.Find("Keyboard");
        if (keyboardTransform == null)
        {
            return;
        }

        // Initialize Switch Bait hint under Keyboard row
        if (_switchBaitHintObject == null || _switchBaitHintObject.transform.parent != keyboardTransform)
        {
            Transform existing = keyboardTransform.Find("BaitMeBruh_SwitchBaitHint");
            if (existing != null)
            {
                _switchBaitHintObject = existing.gameObject;
            }
            else
            {
                Transform primaryAttack = keyboardTransform.Find("PrimaryAttack");
                if (primaryAttack != null)
                {
                    _switchBaitHintObject = Object.Instantiate(primaryAttack.gameObject, keyboardTransform, false);
                    _switchBaitHintObject.name = "BaitMeBruh_SwitchBaitHint";
                    _switchBaitHintObject.transform.SetSiblingIndex(1);
                }
            }

            if (_switchBaitHintObject != null)
            {
                Transform labelTransform = _switchBaitHintObject.transform.Find("Text");
                if (labelTransform != null)
                {
                    _switchBaitLabel = labelTransform.GetComponent<TextMeshProUGUI>();
                }

                Transform keyTransform = _switchBaitHintObject.transform.Find("key_bkg/Key");
                if (keyTransform != null)
                {
                    _switchBaitKey = keyTransform.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        if (_switchBaitLabel != null)
        {
            _switchBaitLabel.text = Localization.instance != null ? Localization.instance.Localize("$hud_fishing_switch_bait") : "Switch Bait";
        }

        if (_switchBaitKey != null)
        {
            KeyCode switchKey = ConfigRegistry.SwitchBaitKey != null ? ConfigRegistry.SwitchBaitKey.Value : KeyCode.G;
            _switchBaitKey.text = switchKey.ToString();
        }

        FishingFloat activeFloat = FishingFloat.FindFloat(localPlayer);
        float drawPercentage = localPlayer.GetAttackDrawPercentage();

        // Switch bait is available whenever holding a fishing rod before casting
        bool showSwitchHint = activeFloat == null && drawPercentage <= 0.01f;

        if (_switchBaitHintObject != null && _switchBaitHintObject.activeSelf != showSwitchHint)
        {
            _switchBaitHintObject.SetActive(showSwitchHint);
            if (keyboardTransform is RectTransform kbRt)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(kbRt);
            }
        }

        // Context-sensitive Pull / Strike / Reel text
        if (_pullText == null || _pullText.transform.parent == null)
        {
            Transform blockText = keyboardTransform.Find("Block/Text");
            if (blockText != null)
            {
                _pullText = blockText.GetComponent<TextMeshProUGUI>();
            }
        }

        if (_pullText != null)
        {
            if (activeFloat != null)
            {
                if (activeFloat.GetCatch() != null)
                {
                    FishingLineState lineState = activeFloat.GetComponent<FishingLineState>();
                    float tension = lineState != null ? lineState.CurrentTension : 0.0f;
                    _pullText.text = (tension >= 0.75f) ? "Release" : "Reel";
                }
                else
                {
                    bool isBite = (FishingTensionHud.Instance != null && FishingTensionHud.Instance.IsBiteAlertActive) || (_nibblerRef(activeFloat) != null);
                    _pullText.text = isBite ? "Strike" : "Pull";
                }
            }
            else
            {
                _pullText.text = "Pull";
            }
        }
    }
}
