using System;
using UnityEngine;

namespace BaitMeBruh.Components;

public class TrapNetWaveSway : MonoBehaviour
{
    private MeshFilter m_meshFilter;
    private Mesh m_dynamicMesh;
    private Vector3[] m_baseVertices;
    private Vector3[] m_animVertices;
    private float[] m_weights;
    private WaterVolume m_waterVolume;
    private bool m_hasLoggedError;

    private const float MaxUpdateDistance = 25f;

    private void Start()
    {
        if (Jotunn.Managers.GUIManager.IsHeadless())
        {
            Destroy(this);
            return;
        }

        Transform netTrans = transform.Find("Visual/Netting");
        if (netTrans == null)
        {
            netTrans = transform.Find("Netting");
        }

        if (netTrans == null)
        {
            return;
        }

        m_meshFilter = netTrans.GetComponent<MeshFilter>();
        if (m_meshFilter == null || m_meshFilter.sharedMesh == null)
        {
            return;
        }

        Mesh sourceMesh = m_meshFilter.sharedMesh;
        m_baseVertices = sourceMesh.vertices;
        m_animVertices = new Vector3[m_baseVertices.Length];
        Array.Copy(m_baseVertices, m_animVertices, m_baseVertices.Length);

        m_dynamicMesh = Instantiate(sourceMesh);
        m_dynamicMesh.name = "Netting_DynamicInstance";
        m_meshFilter.mesh = m_dynamicMesh;

        float maxY = float.MinValue;
        float minY = float.MaxValue;
        for (int i = 0; i < m_baseVertices.Length; i++)
        {
            float y = m_baseVertices[i].y;
            if (y > maxY) maxY = y;
            if (y < minY) minY = y;
        }

        float heightSpan = maxY - minY;
        m_weights = new float[m_baseVertices.Length];
        for (int i = 0; i < m_baseVertices.Length; i++)
        {
            float y = m_baseVertices[i].y;
            if (heightSpan > 0.05f)
            {
                m_weights[i] = Mathf.Clamp01((maxY - y) / heightSpan);
            }
            else
            {
                m_weights[i] = 0f;
            }
        }
    }

    private void Update()
    {
        if (m_dynamicMesh == null || m_baseVertices == null)
        {
            return;
        }

        Camera mainCam = Camera.main;
        if (mainCam != null && Vector3.Distance(transform.position, mainCam.transform.position) > MaxUpdateDistance)
        {
            return;
        }

        try
        {
            float waterLevel = Floating.GetWaterLevel(transform.position, ref m_waterVolume);
            float baseWorldY = transform.position.y;
            float waterRelativeHeight = waterLevel - baseWorldY;

            float time = Time.time;

            float ambientWave = Mathf.Sin(time * 1.8f) * 0.015f;
            float ambientCross = Mathf.Cos(time * 1.3f) * 0.012f;

            float waveSurge = 0f;
            bool isInWater = waterRelativeHeight > -0.2f;
            if (isInWater)
            {
                waveSurge = Mathf.Sin(time * 2.8f) * 0.035f * Mathf.Clamp01(waterRelativeHeight + 0.5f);
            }

            for (int i = 0; i < m_baseVertices.Length; i++)
            {
                float weight = m_weights[i];
                if (weight <= 0.001f)
                {
                    m_animVertices[i] = m_baseVertices[i];
                    continue;
                }

                Vector3 basePos = m_baseVertices[i];
                float vertexWorldY = baseWorldY + basePos.y;
                float submergedDepth = waterLevel - vertexWorldY;

                float vertWave = (submergedDepth > 0f)
                    ? (waveSurge + Mathf.Sin(time * 3.2f + basePos.x * 4f + basePos.z * 4f) * 0.025f)
                    : ambientWave;

                Vector3 radialDir = new Vector3(basePos.x, 0f, basePos.z).normalized;
                Vector3 tangentDir = new Vector3(-radialDir.z, 0f, radialDir.x);

                float radialOffset = vertWave * weight;
                float lateralOffset = ambientCross * weight;
                float verticalOffset = (submergedDepth > 0f ? Mathf.Sin(time * 2.5f) * 0.015f : 0f) * weight;

                m_animVertices[i] = basePos + (radialDir * radialOffset) + (tangentDir * lateralOffset) + (Vector3.up * verticalOffset);
            }

            m_dynamicMesh.vertices = m_animVertices;
            m_dynamicMesh.RecalculateBounds();
        }
        catch (Exception ex)
        {
            if (!m_hasLoggedError)
            {
                m_hasLoggedError = true;
                BaitMeBruh.Log.Warning($"[TrapNetWaveSway] Exception during sway update: {ex.Message}");
            }
        }
    }

    private void OnDestroy()
    {
        if (m_dynamicMesh != null)
        {
            Destroy(m_dynamicMesh);
            m_dynamicMesh = null;
        }
    }
}
