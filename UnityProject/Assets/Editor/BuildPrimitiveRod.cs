using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildPrimitiveRod
{
    private const string PrefabsDir = "Assets/Prefabs";
    private const string MaterialsDir = "Assets/Materials";
    private const string TexturesDir = "Assets/Textures";
    private const string BundleName = "greatcatch";

    public static void Build()
    {
        EnsureDirectories();

        // 1. Generate dedicated textures for primitive rod
        Texture2D barkTex = GenerateBarkTexture();
        Texture2D twineTex = GenerateTwineTexture();

        // 2. Create materials
        Material woodMat = CreateBarkMaterial(barkTex);
        Material cordMat = CreateTwineMaterial(twineTex);

        // 3. Create combined multi-material primitive rod mesh
        // Submesh 0: Rustic crooked wooden stick branch
        // Submesh 1: Leather handle wrap, lashings, loose sagging twine, tip knot, hanging string & bone hook
        Vector3 tipPos;
        Mesh rodMesh = CreatePrimitiveRodMesh(out tipPos);
        AssetDatabase.CreateAsset(rodMesh, "Assets/Prefabs/primitive_rod_mesh.asset");

        // 4. Build Prefab with visual mesh and _RodTop transform for line attachment
        GameObject rodPrefab = new GameObject("primitive_fishing_rod");
        MeshFilter mf = rodPrefab.AddComponent<MeshFilter>();
        mf.sharedMesh = rodMesh;

        MeshRenderer mr = rodPrefab.AddComponent<MeshRenderer>();
        mr.sharedMaterials = new Material[] { woodMat, cordMat };

        // Tip transform for FishingFloat line connection
        GameObject rodTopObj = new GameObject("_RodTop");
        rodTopObj.transform.SetParent(rodPrefab.transform, false);
        rodTopObj.transform.localPosition = tipPos;

        string prefabPath = Path.Combine(PrefabsDir, "primitive_fishing_rod.prefab");
        PrefabUtility.SaveAsPrefabAsset(rodPrefab, prefabPath);
        UnityEngine.Object.DestroyImmediate(rodPrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 5. Tag asset bundle
        TagAssetBundle("Assets/Prefabs/primitive_rod_mesh.asset", BundleName);
        TagAssetBundle(prefabPath, BundleName);
        TagAssetBundle(AssetDatabase.GetAssetPath(barkTex), BundleName);
        TagAssetBundle(AssetDatabase.GetAssetPath(twineTex), BundleName);
        TagAssetBundle(AssetDatabase.GetAssetPath(woodMat), BundleName);
        TagAssetBundle(AssetDatabase.GetAssetPath(cordMat), BundleName);

        Debug.Log($"Primitive Fishing Rod model built successfully! (Tip position: {tipPos})");
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

    private static Texture2D GenerateBarkTexture()
    {
        int size = 512;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);

        Color deepBark = new Color(0.23f, 0.16f, 0.10f, 1f);
        Color midBark = new Color(0.38f, 0.28f, 0.18f, 1f);
        Color lightBark = new Color(0.52f, 0.40f, 0.26f, 1f);
        Color mossTint = new Color(0.30f, 0.35f, 0.22f, 1f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float nx = (float)x / size;
                float ny = (float)y / size;

                // Longitudinal grain (stretched vertically)
                float grainNoise = Mathf.PerlinNoise(nx * 14f, ny * 1.5f);
                float fineGrain = Mathf.PerlinNoise(nx * 42f, ny * 4.0f) * 0.4f;

                // Bark fissure grooves
                float fissure = Mathf.Abs(Mathf.Sin(nx * Mathf.PI * 8f + grainNoise * 4f));
                fissure = Mathf.Pow(fissure, 3.5f);

                // Organic moss / weathered patina patches
                float mossNoise = Mathf.PerlinNoise(nx * 3.5f, ny * 3.5f);
                bool hasMoss = (mossNoise > 0.68f);

                Color c = Color.Lerp(deepBark, midBark, grainNoise);
                c = Color.Lerp(c, lightBark, fineGrain);
                c = Color.Lerp(deepBark * 0.6f, c, fissure);

                if (hasMoss)
                {
                    c = Color.Lerp(c, mossTint, (mossNoise - 0.68f) * 2.5f);
                }

                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        string path = Path.Combine(TexturesDir, "T_PrimitiveRodBark.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static Texture2D GenerateTwineTexture()
    {
        int size = 256;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);

        Color ropeBase = new Color(0.54f, 0.44f, 0.30f, 1f);
        Color ropeLight = new Color(0.72f, 0.62f, 0.46f, 1f);
        Color ropeDark = new Color(0.28f, 0.21f, 0.13f, 1f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float twist = Mathf.Sin((x * 0.5f + y * 0.9f)) * 0.5f + 0.5f;
                float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) * 0.25f;

                Color c = Color.Lerp(ropeDark, ropeBase, twist);
                c = Color.Lerp(c, ropeLight, noise);
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        string path = Path.Combine(TexturesDir, "T_PrimitiveRodTwine.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static Material CreateBarkMaterial(Texture2D tex)
    {
        Shader shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.name = "M_PrimitiveRodWood";
        mat.mainTexture = tex;
        mat.SetFloat("_Glossiness", 0.08f);
        mat.SetFloat("_Metallic", 0.0f);
        string path = Path.Combine(MaterialsDir, "M_PrimitiveRodWood.mat");
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static Material CreateTwineMaterial(Texture2D tex)
    {
        Shader shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.name = "M_PrimitiveRodCord";
        mat.mainTexture = tex;
        mat.SetFloat("_Glossiness", 0.04f);
        mat.SetFloat("_Metallic", 0.0f);
        string path = Path.Combine(MaterialsDir, "M_PrimitiveRodCord.mat");
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static Mesh CreatePrimitiveRodMesh(out Vector3 tipPosition)
    {
        Mesh mesh = new Mesh { name = "PrimitiveRodMesh" };

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        List<int> woodTris = new List<int>();
        List<int> ropeTris = new List<int>();

        // Generate stick spine points
        // Butt at z = -0.38m, player grip at z = 0.0m (y = -0.05m), rod tip at z = 2.05m (~2.43m total length)
        int spineCount = 48;
        float startZ = -0.38f;
        float endZ = 2.05f;

        List<Vector3> spine = new List<Vector3>();
        List<float> radii = new List<float>();

        for (int i = 0; i < spineCount; i++)
        {
            float t = (float)i / (spineCount - 1);
            float z = Mathf.Lerp(startZ, endZ, t);

            // Natural organic curvature: slight gentle curve upwards, subtle lateral s-bend
            float tForward = Mathf.Max(0f, (z - 0.10f) / (endZ - 0.10f));
            float curveY = 0.065f * (tForward * tForward);
            float wobbleX = 0.012f * Mathf.Sin(t * Mathf.PI * 4f);

            // Grip palm offset: -0.05m Y matches closed fist in m_rightHand
            float yPos = -0.05f + curveY;

            // Knots / branch node swellings
            float knot1 = Mathf.Exp(-Mathf.Pow((z - 0.55f) / 0.04f, 2f)) * 0.005f;
            float knot2 = Mathf.Exp(-Mathf.Pow((z - 1.25f) / 0.04f, 2f)) * 0.004f;
            float knot3 = Mathf.Exp(-Mathf.Pow((z - 1.75f) / 0.03f, 2f)) * 0.003f;

            // Radius tapers from 2.2cm at butt to 0.65cm at tip
            float baseRadius = Mathf.Lerp(0.022f, 0.0065f, t) + knot1 + knot2 + knot3;

            spine.Add(new Vector3(wobbleX, yPos, z));
            radii.Add(baseRadius);
        }

        tipPosition = spine[spineCount - 1];

        // 1. Build Branch Stick Mesh (Submesh 0)
        int radialSegs = 10;
        int stickStartVert = verts.Count;

        for (int i = 0; i < spineCount; i++)
        {
            Vector3 center = spine[i];
            float r = radii[i];
            float v = (float)i / (spineCount - 1) * 6.0f;

            Vector3 forward = (i < spineCount - 1) ? (spine[i + 1] - center).normalized : (center - spine[i - 1]).normalized;
            Vector3 up = Vector3.up;
            Vector3 right = Vector3.Cross(up, forward).normalized;
            up = Vector3.Cross(forward, right).normalized;

            for (int s = 0; s <= radialSegs; s++)
            {
                float u = (float)s / radialSegs;
                float angle = u * Mathf.PI * 2f;

                float barkBump = 0.0012f * Mathf.Sin(angle * 6f + i * 0.8f);
                float rad = r + barkBump;

                Vector3 dir = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)).normalized;
                Vector3 pos = center + dir * rad;

                verts.Add(pos);
                norms.Add(dir);
                uvs.Add(new Vector2(u * 1.5f, v));
            }
        }

        // Stick cylinder triangles
        for (int i = 0; i < spineCount - 1; i++)
        {
            int row1 = stickStartVert + i * (radialSegs + 1);
            int row2 = stickStartVert + (i + 1) * (radialSegs + 1);

            for (int s = 0; s < radialSegs; s++)
            {
                woodTris.Add(row1 + s); woodTris.Add(row2 + s); woodTris.Add(row1 + s + 1);
                woodTris.Add(row1 + s + 1); woodTris.Add(row2 + s); woodTris.Add(row2 + s + 1);
            }
        }

        // End caps on stick (butt and tip)
        AddCap(verts, norms, uvs, woodTris, stickStartVert, radialSegs, -Vector3.forward);
        AddCap(verts, norms, uvs, woodTris, stickStartVert + (spineCount - 1) * (radialSegs + 1), radialSegs, Vector3.forward);

        // 2. Leather Handle Wrap (Submesh 1)
        AddHandleWrap(verts, norms, uvs, ropeTris, spine, radii, -0.32f, 0.08f);

        // 3. Cord Lashings & Guide Eyelets along the rod (Submesh 1)
        float[] lashingZ = new float[] { 0.42f, 0.90f, 1.40f, 1.85f };
        List<Vector3> eyeletPoints = new List<Vector3>();

        for (int l = 0; l < lashingZ.Length; l++)
        {
            Vector3 eyelet = AddLashing(verts, norms, uvs, ropeTris, spine, radii, lashingZ[l]);
            eyeletPoints.Add(eyelet);
        }

        // 4. Loose String / Fishing Line & Tip Dangling String (Submesh 1)
        AddFishingLineWithSlack(verts, norms, uvs, ropeTris, spine, radii, eyeletPoints);

        // Assemble mesh with 2 submeshes
        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.subMeshCount = 2;
        mesh.SetTriangles(woodTris, 0);
        mesh.SetTriangles(ropeTris, 1);
        mesh.RecalculateBounds();

        return mesh;
    }

    private static void AddCap(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, int ringStart, int radialSegs, Vector3 normal)
    {
        Vector3 center = Vector3.zero;
        for (int s = 0; s < radialSegs; s++)
        {
            center += verts[ringStart + s];
        }
        center /= radialSegs;

        int centerIdx = verts.Count;
        verts.Add(center);
        norms.Add(normal);
        uvs.Add(new Vector2(0.5f, 0.5f));

        for (int s = 0; s < radialSegs; s++)
        {
            if (normal.z > 0f)
            {
                tris.Add(centerIdx);
                tris.Add(ringStart + s);
                tris.Add(ringStart + s + 1);
            }
            else
            {
                tris.Add(centerIdx);
                tris.Add(ringStart + s + 1);
                tris.Add(ringStart + s);
            }
        }
    }

    private static void AddHandleWrap(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, List<Vector3> spine, List<float> radii, float startZ, float endZ)
    {
        int spiralTurns = 14;
        int steps = spiralTurns * 12;
        float wrapRadiusOffset = 0.0022f;

        List<Vector3> outerPath = new List<Vector3>();
        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            float z = Mathf.Lerp(startZ, endZ, t);

            Vector3 center = Vector3.zero;
            float r = 0.02f;
            for (int k = 0; k < spine.Count - 1; k++)
            {
                if (z >= spine[k].z && z <= spine[k + 1].z)
                {
                    float localT = (z - spine[k].z) / (spine[k + 1].z - spine[k].z);
                    center = Vector3.Lerp(spine[k], spine[k + 1], localT);
                    r = Mathf.Lerp(radii[k], radii[k + 1], localT) + wrapRadiusOffset;
                    break;
                }
            }

            float angle = t * Mathf.PI * 2f * spiralTurns;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0f);
            outerPath.Add(center + offset);
        }

        for (int i = 0; i < outerPath.Count - 1; i++)
        {
            AddSegmentTube(verts, norms, uvs, tris, outerPath[i], outerPath[i + 1], 0.0035f, 5, 2.0f);
        }

        AddRingTorus(verts, norms, uvs, tris, spine[2], radii[2] + 0.003f, 0.0045f, 10);
        AddRingTorus(verts, norms, uvs, tris, spine[10], radii[10] + 0.003f, 0.0045f, 10);
    }

    private static Vector3 AddLashing(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, List<Vector3> spine, List<float> radii, float targetZ)
    {
        int idx = 0;
        for (int i = 0; i < spine.Count - 1; i++)
        {
            if (spine[i].z <= targetZ && spine[i + 1].z >= targetZ)
            {
                idx = i;
                break;
            }
        }

        Vector3 center = spine[idx];
        float r = radii[idx] + 0.0015f;

        AddRingTorus(verts, norms, uvs, tris, center, r, 0.0035f, 10);
        AddRingTorus(verts, norms, uvs, tris, center + new Vector3(0f, 0f, 0.008f), r, 0.0035f, 10);

        Vector3 eyeletBase = center + Vector3.up * r;
        Vector3 eyeletApex = eyeletBase + Vector3.up * 0.015f;

        AddSegmentTube(verts, norms, uvs, tris, eyeletBase + Vector3.right * 0.005f, eyeletApex, 0.0018f, 4, 1.0f);
        AddSegmentTube(verts, norms, uvs, tris, eyeletApex, eyeletBase - Vector3.right * 0.005f, 0.0018f, 4, 1.0f);

        return eyeletApex;
    }

    private static void AddFishingLineWithSlack(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, List<Vector3> spine, List<float> radii, List<Vector3> eyelets)
    {
        float lineRadius = 0.0016f;

        Vector3 lineStart = spine[10] + new Vector3(0.015f, 0.018f, 0f);

        List<Vector3> lineNodes = new List<Vector3>();
        lineNodes.Add(lineStart);
        lineNodes.AddRange(eyelets);

        Vector3 tip = spine[spine.Count - 1];
        float tipRadius = radii[radii.Count - 1] + 0.001f;
        Vector3 tipTie = tip + Vector3.up * tipRadius;
        lineNodes.Add(tipTie);

        // Loose sagging catenary loops between eyelets
        for (int seg = 0; seg < lineNodes.Count - 1; seg++)
        {
            Vector3 p0 = lineNodes[seg];
            Vector3 p1 = lineNodes[seg + 1];

            int subSegs = 8;
            Vector3 prevPt = p0;

            float sagAmount = (seg > 0) ? 0.022f : 0.010f;

            for (int s = 1; s <= subSegs; s++)
            {
                float t = (float)s / subSegs;
                Vector3 pt = Vector3.Lerp(p0, p1, t);

                float sag = Mathf.Sin(t * Mathf.PI) * sagAmount;
                pt.y -= sag;

                AddSegmentTube(verts, norms, uvs, tris, prevPt, pt, lineRadius, 4, 1.0f);
                prevPt = pt;
            }
        }

        // Tip binding knot
        for (int k = 0; k < 4; k++)
        {
            Vector3 knotPos = tip - new Vector3(0f, 0f, (float)k * 0.007f);
            AddRingTorus(verts, norms, uvs, tris, knotPos, tipRadius, 0.0035f, 8);
        }

        // Dangling loose string off tip with bone hook
        Vector3 dangleStart = tipTie;
        int dangleSteps = 10;
        Vector3 prevDangle = dangleStart;

        for (int d = 1; d <= dangleSteps; d++)
        {
            float t = (float)d / dangleSteps;
            float dy = -0.16f * t;
            float dx = 0.010f * Mathf.Sin(t * Mathf.PI * 2f);
            float dz = 0.015f * Mathf.Cos(t * Mathf.PI * 1.5f);

            Vector3 nextDangle = dangleStart + new Vector3(dx, dy, dz);
            AddSegmentTube(verts, norms, uvs, tris, prevDangle, nextDangle, lineRadius * 0.9f, 4, 1.0f);
            prevDangle = nextDangle;
        }

        // Small carved bone hook
        Vector3 hookTop = prevDangle;
        Vector3 hookBend = hookTop + new Vector3(0f, -0.035f, 0f);
        Vector3 hookTip = hookBend + new Vector3(0.018f, 0.020f, 0f);

        AddSegmentTube(verts, norms, uvs, tris, hookTop, hookBend, 0.0022f, 5, 0.5f);
        AddSegmentTube(verts, norms, uvs, tris, hookBend, hookTip, 0.0018f, 5, 0.5f);
    }

    private static void AddSegmentTube(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, Vector3 p0, Vector3 p1, float radius, int segments = 6, float uvScale = 1.0f)
    {
        Vector3 dir = p1 - p0;
        float length = dir.magnitude;
        if (length < 0.0001f) return;

        Vector3 forward = dir.normalized;
        Vector3 up = (Mathf.Abs(forward.y) < 0.95f) ? Vector3.up : Vector3.right;
        Vector3 right = Vector3.Cross(up, forward).normalized;
        up = Vector3.Cross(forward, right).normalized;

        int startIdx = verts.Count;

        for (int s = 0; s <= segments; s++)
        {
            float u = (float)s / segments;
            float angle = u * Mathf.PI * 2f;
            Vector3 normal = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)).normalized;

            verts.Add(p0 + normal * radius);
            norms.Add(normal);
            uvs.Add(new Vector2(u, 0f));

            verts.Add(p1 + normal * radius);
            norms.Add(normal);
            uvs.Add(new Vector2(u, length * uvScale));
        }

        for (int s = 0; s < segments; s++)
        {
            int r0 = startIdx + s * 2;
            int r1 = startIdx + (s + 1) * 2;

            tris.Add(r0); tris.Add(r0 + 1); tris.Add(r1);
            tris.Add(r1); tris.Add(r0 + 1); tris.Add(r1 + 1);
        }
    }

    private static void AddRingTorus(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, Vector3 center, float majorR, float minorR, int segments = 10)
    {
        int crossSegs = 5;
        int startIdx = verts.Count;

        for (int i = 0; i <= segments; i++)
        {
            float u = (float)i / segments;
            float angle = u * Mathf.PI * 2f;
            Vector3 ringCenter = center + new Vector3(Mathf.Cos(angle) * majorR, Mathf.Sin(angle) * majorR, 0f);
            Vector3 outDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f).normalized;

            for (int j = 0; j <= crossSegs; j++)
            {
                float v = (float)j / crossSegs;
                float crossAngle = v * Mathf.PI * 2f;
                Vector3 normal = (outDir * Mathf.Cos(crossAngle) + Vector3.forward * Mathf.Sin(crossAngle)).normalized;

                verts.Add(ringCenter + normal * minorR);
                norms.Add(normal);
                uvs.Add(new Vector2(u * 2f, v));
            }
        }

        for (int i = 0; i < segments; i++)
        {
            int row1 = startIdx + i * (crossSegs + 1);
            int row2 = startIdx + (i + 1) * (crossSegs + 1);

            for (int j = 0; j < crossSegs; j++)
            {
                tris.Add(row1 + j); tris.Add(row2 + j); tris.Add(row1 + j + 1);
                tris.Add(row1 + j + 1); tris.Add(row2 + j); tris.Add(row2 + j + 1);
            }
        }
    }
}
