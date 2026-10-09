using UnityEngine;

namespace BaitMeBruh.Components;

public class FishingProjectileWatcher : MonoBehaviour
{
    private Character _owner;
    private float _maxDistance;
    private Transform _rodTop;
    private bool _hasBroken;

    public void Setup(Character owner, float maxDistance)
    {
        _owner = owner;
        _maxDistance = maxDistance;
        if (_owner != null)
        {
            _rodTop = Utils.FindChild(_owner.transform, "_RodTop");
        }
    }

    private void FixedUpdate()
    {
        if (_hasBroken || _owner == null)
        {
            return;
        }

        Vector3 origin = _rodTop != null ? _rodTop.position : _owner.transform.position;
        float distance = Vector3.Distance(origin, transform.position);

        if (distance > _maxDistance + 0.5f)
        {
            _hasBroken = true;

            GameObject sfxPrefab = ZNetScene.instance != null ? ZNetScene.instance.GetPrefab("sfx_fishingrod_linebreak") : null;
            if (sfxPrefab != null)
            {
                UnityEngine.Object.Instantiate(sfxPrefab, transform.position, Quaternion.identity);
            }

            if (_owner is Player player)
            {
                player.Message(MessageHud.MessageType.Center, Localization.instance != null ? Localization.instance.Localize("$msg_fishing_linebroke") : "Line broke");
            }

            if (ZNetScene.instance != null)
            {
                ZNetScene.instance.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
