using UnityEngine;

namespace BaitMeBruh.Components;

public class TransientLightFader : MonoBehaviour
{
    private Light _light;
    private float _initialIntensity;
    private float _duration = 0.75f;
    private float _elapsedTime;

    public float Duration
    {
        get => _duration;
        set => _duration = Mathf.Max(0.1f, value);
    }

    private void Awake()
    {
        _light = GetComponent<Light>();
        if (_light != null)
        {
            _initialIntensity = _light.intensity;
        }
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_elapsedTime / _duration);

        if (_light != null)
        {
            _light.intensity = Mathf.Lerp(_initialIntensity, 0.0f, progress);
        }

        if (progress >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
