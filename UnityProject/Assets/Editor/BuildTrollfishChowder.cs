using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildTrollfishChowder
{
    private const string PrefabsDir = "Assets/Prefabs";
    private const string MaterialsDir = "Assets/Materials";
    private const string TexturesDir = "Assets/Textures";
    private const string BundleName = "greatcatch";

    public static void Build()
    {
        EnsureDirectories();

        Texture2D woodTex = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(TexturesDir, "T_BowlWood_HD.png"));
        Texture2D soupTex = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(TexturesDir, "T_TrollfishSoup_HD.png"));
        Texture2D garnishTex = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(TexturesDir, "T_TrollfishGarnish_HD.png"));

        Material bowlMat = CreateMaterial("M_TrollfishBowl.mat", woodTex, 0.20f, 0.02f);
        Material soupMat = CreateMaterial("M_TrollfishSoup.mat", soupTex, 0.85f, 0.02f);
        Material garnishMat = CreateMaterial("M_TrollfishGarnish.mat", garnishTex, 0.50f, 0.05f);

        Mesh bowlMesh = CreateBowlMesh();
        Mesh soupMesh = CreateSoupMesh();
        Mesh garnishMesh = CreateGarnishMesh();

        AssetDatabase.CreateAsset(bowlMesh, "Assets/Prefabs/trollfish_bowl_mesh.asset");
        AssetDatabase.CreateAsset(soupMesh, "Assets/Prefabs/trollfish_soup_mesh.asset");
        AssetDatabase.CreateAsset(garnishMesh, "Assets/Prefabs/trollfish_garnish_mesh.asset");

        BuildPrefab(bowlMesh, soupMesh, garnishMesh, bowlMat, soupMat, garnishMat);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        TagAssetBundle(AssetDatabase.GetAssetPath(bowlMat), BundleName);
        TagAssetBundle(AssetDatabase.GetAssetPath(soupMat), BundleName);
        TagAssetBundle(AssetDatabase.GetAssetPath(garnishMat), BundleName);
        TagAssetBundle("Assets/Textures/T_BowlWood_HD.png", BundleName);
        TagAssetBundle("Assets/Textures/T_TrollfishSoup_HD.png", BundleName);
        TagAssetBundle("Assets/Textures/T_TrollfishGarnish_HD.png", BundleName);
        TagAssetBundle("Assets/Prefabs/drop_trollfish_chowder.prefab", BundleName);

        Debug.Log("Trollfish Chowder Drop Model & HD Materials successfully generated!");
    }

    private static void EnsureDirectories()
    {
        if (!Directory.Exists(PrefabsDir)) Directory.CreateDirectory(PrefabsDir);
        if (!Directory.Exists(MaterialsDir)) Directory.CreateDirectory(MaterialsDir);
        if (!Directory.Exists(TexturesDir)) Directory.CreateDirectory(TexturesDir);
    }

    private static void TagAssetBundle(string path, string bundleName)
    {
        if (!string.IsNullOrEmpty(path))
        {
            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.assetBundleName = bundleName;
            }
        }
    }

    private static Material CreateMaterial(string fileName, Texture2D tex, float gloss, float metallic)
    {
        Shader shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        if (tex != null)
        {
            mat.mainTexture = tex;
        }
        mat.SetFloat("_Glossiness", gloss);
        mat.SetFloat("_Metallic", metallic);
        string path = Path.Combine(MaterialsDir, fileName);
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static Mesh CreateBowlMesh()
    {
        Mesh mesh = new Mesh { name = "TrollfishBowlMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        int radialSegments = 28;

        Vector2[] profile = new Vector2[]
        {
            new Vector2(0.00f, 0.000f), // 0: bottom center
            new Vector2(0.08f, 0.000f), // 1: foot rim bottom
            new Vector2(0.10f, 0.015f), // 2: foot flare
            new Vector2(0.14f, 0.050f), // 3: lower wall
            new Vector2(0.17f, 0.095f), // 4: belly widest
            new Vector2(0.170f, 0.135f),// 5: upper wall
            new Vector2(0.165f, 0.150f),// 6: outer rim
            new Vector2(0.155f, 0.153f),// 7: rim crest rounded
            new Vector2(0.145f, 0.148f),// 8: inner rim lip
            new Vector2(0.140f, 0.125f),// 9: inner upper cavity
            new Vector2(0.115f, 0.065f),// 10: inner mid cavity
            new Vector2(0.065f, 0.035f),// 11: inner bottom rim
            new Vector2(0.000f, 0.030f) // 12: inner bottom center
        };

        float uvDiameter = 0.38f;

        int rings = profile.Length;
        for (int r = 0; r < rings; r++)
        {
            float rad = profile[r].x;
            float y = profile[r].y;
            bool isInner = (r >= 8);

            for (int s = 0; s <= radialSegments; s++)
            {
                float uAngle = (float)s / radialSegments;
                float angle = uAngle * Mathf.PI * 2f;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                Vector3 pos = new Vector3(cos * rad, y, sin * rad);
                verts.Add(pos);

                Vector3 normal;
                if (r == 0)
                {
                    normal = Vector3.down;
                }
                else if (r == rings - 1)
                {
                    normal = Vector3.up;
                }
                else
                {
                    Vector2 pPrev = profile[r - 1];
                    Vector2 pNext = profile[r + 1];
                    Vector2 dir = (pNext - pPrev).normalized;
                    Vector2 n2D = new Vector2(-dir.y, dir.x);
                    normal = new Vector3(cos * n2D.x, n2D.y, sin * n2D.x).normalized;
                }
                norms.Add(normal);

                float uvX = 0.5f + (cos * rad / uvDiameter);
                float uvY = 0.5f + (sin * rad / uvDiameter);
                uvs.Add(new Vector2(uvX, uvY));
            }
        }

        for (int r = 0; r < rings - 1; r++)
        {
            int rowStart = r * (radialSegments + 1);
            int nextRowStart = (r + 1) * (radialSegments + 1);
            bool isInner = (r >= 7);

            for (int s = 0; s < radialSegments; s++)
            {
                int curr = rowStart + s;
                int next = rowStart + s + 1;
                int currAbove = nextRowStart + s;
                int nextAbove = nextRowStart + s + 1;

                if (!isInner)
                {
                    tris.Add(curr);
                    tris.Add(currAbove);
                    tris.Add(next);

                    tris.Add(next);
                    tris.Add(currAbove);
                    tris.Add(nextAbove);
                }
                else
                {
                    tris.Add(curr);
                    tris.Add(next);
                    tris.Add(currAbove);

                    tris.Add(next);
                    tris.Add(nextAbove);
                    tris.Add(currAbove);
                }
            }
        }

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateSoupMesh()
    {
        Mesh mesh = new Mesh { name = "TrollfishSoupMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        int radialSegments = 32;
        float soupRadius = 0.144f;
        float soupCenterY = 0.134f;
        float soupRimY = 0.137f;

        verts.Add(new Vector3(0f, soupCenterY, 0f));
        norms.Add(Vector3.up);
        uvs.Add(new Vector2(0.5f, 0.5f));

        for (int s = 0; s <= radialSegments; s++)
        {
            float angle = ((float)s / radialSegments) * Mathf.PI * 2f;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            verts.Add(new Vector3(cos * soupRadius, soupRimY, sin * soupRadius));
            norms.Add(Vector3.up);

            float u = 0.5f + (cos * 0.44f);
            float v = 0.5f + (sin * 0.44f);
            uvs.Add(new Vector2(u, v));
        }

        for (int s = 1; s <= radialSegments; s++)
        {
            tris.Add(0);
            tris.Add(s + 1);
            tris.Add(s);
        }

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateGarnishMesh()
    {
        Mesh mesh = new Mesh { name = "TrollfishGarnishMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        AddYellowMushroom(verts, norms, uvs, tris, new Vector3(0.050f, 0.137f, 0.040f), 0.032f, 0.016f, Quaternion.Euler(5f, 30f, -8f));
        AddYellowMushroom(verts, norms, uvs, tris, new Vector3(-0.060f, 0.137f, -0.030f), 0.028f, 0.014f, Quaternion.Euler(-8f, 75f, 10f));
        AddYellowMushroom(verts, norms, uvs, tris, new Vector3(-0.010f, 0.137f, -0.070f), 0.024f, 0.012f, Quaternion.Euler(10f, 160f, -5f));

        AddTrollfishFin(verts, norms, uvs, tris, new Vector3(0.010f, 0.136f, 0.025f));

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static void AddYellowMushroom(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris,
        Vector3 center, float radius, float height, Quaternion tilt)
    {
        int segs = 14;
        int centerIdx = verts.Count;

        Vector3 apex = center + tilt * new Vector3(0f, height, 0f);
        verts.Add(apex);
        norms.Add(tilt * Vector3.up);
        uvs.Add(new Vector2(0.25f, 0.75f));

        for (int i = 0; i <= segs; i++)
        {
            float a = ((float)i / segs) * Mathf.PI * 2f;
            float cos = Mathf.Cos(a);
            float sin = Mathf.Sin(a);

            Vector3 rimLocal = new Vector3(cos * radius, 0f, sin * radius);
            verts.Add(center + tilt * rimLocal);

            Vector3 n = tilt * (new Vector3(cos, 0.7f, sin).normalized);
            norms.Add(n);
            uvs.Add(new Vector2(0.25f + cos * 0.18f, 0.75f + sin * 0.18f));
        }

        for (int i = 1; i <= segs; i++)
        {
            tris.Add(centerIdx);
            tris.Add(centerIdx + i + 1);
            tris.Add(centerIdx + i);
        }

        int underIdx = verts.Count;
        verts.Add(center + tilt * new Vector3(0f, 0.002f, 0f));
        norms.Add(tilt * Vector3.down);
        uvs.Add(new Vector2(0.25f, 0.40f));

        for (int i = 0; i <= segs; i++)
        {
            float a = ((float)i / segs) * Mathf.PI * 2f;
            float cos = Mathf.Cos(a);
            float sin = Mathf.Sin(a);

            Vector3 rimLocal = new Vector3(cos * radius * 0.95f, 0.002f, sin * radius * 0.95f);
            verts.Add(center + tilt * rimLocal);
            norms.Add(tilt * Vector3.down);
            uvs.Add(new Vector2(0.25f + cos * 0.10f, 0.40f + sin * 0.10f));
        }

        for (int i = 1; i <= segs; i++)
        {
            tris.Add(underIdx);
            tris.Add(underIdx + i);
            tris.Add(underIdx + i + 1);
        }
    }

    private static void AddTrollfishFin(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, Vector3 basePos)
    {
        int startIdx = verts.Count;

        Vector3[] spinePoints = new Vector3[]
        {
            basePos + new Vector3(-0.030f, 0.000f, -0.018f),
            basePos + new Vector3(-0.015f, 0.035f,  0.005f),
            basePos + new Vector3( 0.005f, 0.068f,  0.030f),
            basePos + new Vector3( 0.030f, 0.045f,  0.020f),
            basePos + new Vector3( 0.045f, 0.000f, -0.005f)
        };

        float halfThick = 0.003f;

        for (int i = 0; i < spinePoints.Length; i++)
        {
            verts.Add(spinePoints[i] + new Vector3(0f, 0f, halfThick));
            norms.Add(new Vector3(0.15f, 0.25f, 0.95f).normalized);
            float u = 0.55f + ((float)i / (spinePoints.Length - 1)) * 0.40f;
            uvs.Add(new Vector2(u, 0.85f));
        }

        for (int i = 0; i < spinePoints.Length; i++)
        {
            verts.Add(spinePoints[i] - new Vector3(0f, 0f, halfThick));
            norms.Add(new Vector3(-0.15f, 0.25f, -0.95f).normalized);
            float u = 0.55f + ((float)i / (spinePoints.Length - 1)) * 0.40f;
            uvs.Add(new Vector2(u, 0.25f));
        }

        tris.Add(startIdx + 0); tris.Add(startIdx + 1); tris.Add(startIdx + 2);
        tris.Add(startIdx + 0); tris.Add(startIdx + 2); tris.Add(startIdx + 3);
        tris.Add(startIdx + 0); tris.Add(startIdx + 3); tris.Add(startIdx + 4);

        int b = startIdx + 5;
        tris.Add(b + 0); tris.Add(b + 2); tris.Add(b + 1);
        tris.Add(b + 0); tris.Add(b + 3); tris.Add(b + 2);
        tris.Add(b + 0); tris.Add(b + 4); tris.Add(b + 3);

        AddQuad(verts, norms, uvs, tris,
            startIdx + 1, startIdx + 2, b + 2, b + 1,
            Vector3.up, new Vector2(0.6f, 0.9f), new Vector2(0.8f, 0.9f));
        AddQuad(verts, norms, uvs, tris,
            startIdx + 2, startIdx + 3, b + 3, b + 2,
            Vector3.up, new Vector2(0.8f, 0.9f), new Vector2(0.95f, 0.9f));
    }

    private static void AddQuad(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris,
        int v0, int v1, int v2, int v3, Vector3 norm, Vector2 uv0, Vector2 uv1)
    {
        int idx = verts.Count;
        verts.Add(verts[v0]); norms.Add(norm); uvs.Add(uv0);
        verts.Add(verts[v1]); norms.Add(norm); uvs.Add(uv1);
        verts.Add(verts[v2]); norms.Add(norm); uvs.Add(uv1);
        verts.Add(verts[v3]); norms.Add(norm); uvs.Add(uv0);

        tris.Add(idx); tris.Add(idx + 1); tris.Add(idx + 2);
        tris.Add(idx); tris.Add(idx + 2); tris.Add(idx + 3);
    }

    private static void BuildPrefab(Mesh bowlMesh, Mesh soupMesh, Mesh garnishMesh,
        Material bowlMat, Material soupMat, Material garnishMat)
    {
        GameObject root = new GameObject("drop_trollfish_chowder");
        root.layer = LayerMask.NameToLayer("item") >= 0 ? LayerMask.NameToLayer("item") : 0;

        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = 1.0f;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.082f, 0f);
        col.size = new Vector3(0.36f, 0.165f, 0.36f);

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(root.transform, false);
        visual.layer = root.layer;

        CreateSubObject("Bowl", visual.transform, bowlMesh, bowlMat, root.layer);
        CreateSubObject("Soup", visual.transform, soupMesh, soupMat, root.layer);
        CreateSubObject("Garnish", visual.transform, garnishMesh, garnishMat, root.layer);

        GameObject attach = new GameObject("attach");
        attach.transform.SetParent(root.transform, false);
        attach.transform.localPosition = Vector3.zero;
        attach.transform.localRotation = Quaternion.identity;
        attach.layer = root.layer;

        CreateSubObject("Bowl", attach.transform, bowlMesh, bowlMat, root.layer);
        CreateSubObject("Soup", attach.transform, soupMesh, soupMat, root.layer);
        CreateSubObject("Garnish", attach.transform, garnishMesh, garnishMat, root.layer);

        string prefabPath = Path.Combine(PrefabsDir, "drop_trollfish_chowder.prefab");
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        TagAssetBundle(prefabPath, BundleName);
    }

    private static GameObject CreateSubObject(string name, Transform parent, Mesh mesh, Material mat, int layer)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        go.layer = layer;

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;

        return go;
    }
}
