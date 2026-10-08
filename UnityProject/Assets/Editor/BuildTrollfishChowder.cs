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
        Material soupMat = CreateMaterial("M_TrollfishSoup.mat", soupTex, 0.65f, 0.05f);
        Material garnishMat = CreateMaterial("M_TrollfishGarnish.mat", garnishTex, 0.45f, 0.05f);

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

        int radialSegments = 24;

        // Profile points (radius, height): from outer bottom center to outer rim, then inner rim to inner bottom
        Vector2[] profile = new Vector2[]
        {
            new Vector2(0.00f, 0.000f), // 0: bottom center
            new Vector2(0.08f, 0.000f), // 1: foot rim bottom
            new Vector2(0.09f, 0.015f), // 2: foot flare
            new Vector2(0.14f, 0.050f), // 3: lower bowl wall
            new Vector2(0.18f, 0.095f), // 4: belly widest point
            new Vector2(0.17f, 0.140f), // 5: upper curve
            new Vector2(0.175f, 0.160f), // 6: outer rim lip
            new Vector2(0.165f, 0.165f), // 7: rim crest rounded
            new Vector2(0.152f, 0.160f), // 8: inner rim lip
            new Vector2(0.145f, 0.130f), // 9: inner upper cavity
            new Vector2(0.120f, 0.070f), // 10: inner mid cavity
            new Vector2(0.070f, 0.035f), // 11: inner bottom rim
            new Vector2(0.000f, 0.030f)  // 12: inner bottom center
        };

        // Calculate total profile length for V texture coordinate
        float totalProfileLen = 0f;
        float[] profileV = new float[profile.Length];
        profileV[0] = 0f;
        for (int i = 1; i < profile.Length; i++)
        {
            totalProfileLen += Vector2.Distance(profile[i], profile[i - 1]);
            profileV[i] = totalProfileLen;
        }
        for (int i = 0; i < profile.Length; i++)
        {
            profileV[i] /= totalProfileLen;
        }

        // Generate rings of vertices
        int rings = profile.Length;
        for (int r = 0; r < rings; r++)
        {
            float rad = profile[r].x;
            float y = profile[r].y;
            float v = profileV[r];

            // Outer surface normals vs inner normals
            bool isInner = (r >= 8);

            for (int s = 0; s <= radialSegments; s++)
            {
                float u = (float)s / radialSegments;
                float angle = u * Mathf.PI * 2f;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                Vector3 pos = new Vector3(cos * rad, y, sin * rad);
                verts.Add(pos);

                // Approximate normal
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
                    Vector2 tangent = profile[r + 1] - profile[r - 1];
                    Vector2 n2 = new Vector2(-tangent.y, tangent.x).normalized;
                    normal = new Vector3(cos * n2.x, n2.y, sin * n2.x);
                    if (isInner)
                    {
                        normal = -normal;
                    }
                }

                norms.Add(normal);
                uvs.Add(new Vector2(u * 2f, v));
            }
        }

        // Stitch quads between consecutive rings
        int vertsPerRing = radialSegments + 1;
        for (int r = 0; r < rings - 1; r++)
        {
            bool isInner = (r >= 8);
            for (int s = 0; s < radialSegments; s++)
            {
                int curr = r * vertsPerRing + s;
                int next = curr + 1;
                int currAbove = (r + 1) * vertsPerRing + s;
                int nextAbove = currAbove + 1;

                if (isInner)
                {
                    tris.Add(curr);
                    tris.Add(nextAbove);
                    tris.Add(next);

                    tris.Add(curr);
                    tris.Add(currAbove);
                    tris.Add(nextAbove);
                }
                else
                {
                    tris.Add(curr);
                    tris.Add(next);
                    tris.Add(nextAbove);

                    tris.Add(curr);
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

        int radialSegments = 24;
        float soupRadius = 0.150f;
        float soupCenterY = 0.138f;
        float soupRimY = 0.142f; // Subtle meniscus at bowl edge

        // Center vertex
        verts.Add(new Vector3(0f, soupCenterY, 0f));
        norms.Add(Vector3.up);
        uvs.Add(new Vector2(0.5f, 0.5f));

        // Rim vertices
        for (int s = 0; s <= radialSegments; s++)
        {
            float angle = ((float)s / radialSegments) * Mathf.PI * 2f;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            verts.Add(new Vector3(cos * soupRadius, soupRimY, sin * soupRadius));
            norms.Add(Vector3.up);

            // Planar circular UV mapping
            float u = 0.5f + (cos * 0.5f);
            float v = 0.5f + (sin * 0.5f);
            uvs.Add(new Vector2(u, v));
        }

        for (int s = 1; s <= radialSegments; s++)
        {
            tris.Add(0);
            tris.Add(s);
            tris.Add(s + 1);
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

        // 1. Mushroom 1 (Yellow Mushroom cap floating on the right)
        AddMushroomCap(verts, norms, uvs, tris, new Vector3(0.055f, 0.141f, 0.040f), 0.024f, 0.014f, 15f);

        // 2. Mushroom 2 (Smaller Yellow Mushroom cap on the left)
        AddMushroomCap(verts, norms, uvs, tris, new Vector3(-0.065f, 0.141f, -0.035f), 0.018f, 0.010f, -25f);

        // 3. Mushroom 3 (Small cap near top)
        AddMushroomCap(verts, norms, uvs, tris, new Vector3(-0.020f, 0.141f, -0.080f), 0.015f, 0.009f, 40f);

        // 4. 3D Trollfish Dorsal Fin emerging from broth
        AddTrollfishFin(verts, norms, uvs, tris, new Vector3(-0.01f, 0.138f, 0.045f));

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static void AddMushroomCap(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, Vector3 center, float radius, float height, float tiltAngle)
    {
        int segs = 10;
        int centerIdx = verts.Count;
        Quaternion tilt = Quaternion.Euler(tiltAngle * 0.5f, tiltAngle, 0f);

        // Apex vertex
        Vector3 apex = center + tilt * new Vector3(0f, height, 0f);
        verts.Add(apex);
        norms.Add(tilt * Vector3.up);
        uvs.Add(new Vector2(0.5f, 0.5f));

        // Rim vertices
        for (int i = 0; i <= segs; i++)
        {
            float a = ((float)i / segs) * Mathf.PI * 2f;
            float cos = Mathf.Cos(a);
            float sin = Mathf.Sin(a);

            Vector3 rimLocal = new Vector3(cos * radius, 0f, sin * radius);
            verts.Add(center + tilt * rimLocal);

            Vector3 n = tilt * (new Vector3(cos, 0.6f, sin).normalized);
            norms.Add(n);
            uvs.Add(new Vector2(0.5f + cos * 0.45f, 0.5f + sin * 0.45f));
        }

        for (int i = 1; i <= segs; i++)
        {
            tris.Add(centerIdx);
            tris.Add(centerIdx + i);
            tris.Add(centerIdx + i + 1);
        }
    }

    private static void AddTrollfishFin(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, Vector3 basePos)
    {
        int startIdx = verts.Count;

        // An organic fan-shaped dorsal fin emerging from the broth at ~30 deg angle
        // Fin has thickness (front and back faces)
        Vector3[] spinePoints = new Vector3[]
        {
            basePos + new Vector3(-0.025f, 0.000f, -0.015f), // 0: base front
            basePos + new Vector3(-0.010f, 0.025f,  0.005f), // 1: mid front
            basePos + new Vector3( 0.005f, 0.055f,  0.025f), // 2: tip high point
            basePos + new Vector3( 0.025f, 0.035f,  0.015f), // 3: trailing edge mid
            basePos + new Vector3( 0.035f, 0.000f, -0.005f)  // 4: base rear
        };

        float halfThick = 0.0025f;

        // Front face (+Z offset)
        for (int i = 0; i < spinePoints.Length; i++)
        {
            verts.Add(spinePoints[i] + new Vector3(0f, 0f, halfThick));
            norms.Add(new Vector3(0.2f, 0.3f, 0.9f).normalized);
            uvs.Add(new Vector2((float)i / (spinePoints.Length - 1), 0.8f));
        }

        // Back face (-Z offset)
        for (int i = 0; i < spinePoints.Length; i++)
        {
            verts.Add(spinePoints[i] - new Vector3(0f, 0f, halfThick));
            norms.Add(new Vector3(-0.2f, 0.3f, -0.9f).normalized);
            uvs.Add(new Vector2((float)i / (spinePoints.Length - 1), 0.2f));
        }

        // Front triangles: 0,1,2 and 0,2,3 and 0,3,4
        tris.Add(startIdx + 0); tris.Add(startIdx + 1); tris.Add(startIdx + 2);
        tris.Add(startIdx + 0); tris.Add(startIdx + 2); tris.Add(startIdx + 3);
        tris.Add(startIdx + 0); tris.Add(startIdx + 3); tris.Add(startIdx + 4);

        // Back triangles (reverse winding)
        int b = startIdx + 5;
        tris.Add(b + 0); tris.Add(b + 2); tris.Add(b + 1);
        tris.Add(b + 0); tris.Add(b + 3); tris.Add(b + 2);
        tris.Add(b + 0); tris.Add(b + 4); tris.Add(b + 3);

        // Edge quads along top: 1-2, 2-3
        AddQuad(verts, norms, uvs, tris,
            startIdx + 1, startIdx + 2, b + 2, b + 1,
            Vector3.up, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f));
        AddQuad(verts, norms, uvs, tris,
            startIdx + 2, startIdx + 3, b + 3, b + 2,
            Vector3.up, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f));
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

        // Rigidbody & BoxCollider for freeform drop physics
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = 1.0f;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.082f, 0f);
        col.size = new Vector3(0.36f, 0.165f, 0.36f);

        // Visual child container
        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(root.transform, false);
        visual.layer = root.layer;

        CreateSubObject("Bowl", visual.transform, bowlMesh, bowlMat, root.layer);
        CreateSubObject("Soup", visual.transform, soupMesh, soupMat, root.layer);
        CreateSubObject("Garnish", visual.transform, garnishMesh, garnishMat, root.layer);

        // attach child for ItemStand support (itemstandh)
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
