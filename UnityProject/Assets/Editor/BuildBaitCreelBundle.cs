using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildBaitCreelBundle
{
    private const string PrefabsDir = "Assets/Prefabs";
    private const string MaterialsDir = "Assets/Materials";
    private const string TexturesDir = "Assets/Textures";
    private const string BundlesDir = "Assets/AssetBundles";
    private const string TargetModBundleDir = "/home/vapok/Modding/Valheim/GreatCatchBruh/GreatCatchBruh/Assets/Bundles";

    public static void Build()
    {
        Debug.Log("Starting Bait Creel AssetBundle build (Draped Hemp Netting V3 - 3D Normal Mapping & Fabric Folds)...");

        EnsureDirectories();

        Texture2D woodTex = GenerateWoodTexture();
        Texture2D netTex = GenerateNetTexture();
        Texture2D netNormalTex = GenerateNetNormalTexture();
        Texture2D ropeTex = GenerateRopeTexture();

        Material woodMat = CreateWoodMaterial(woodTex);
        Material netMat = CreateNetMaterial(netTex, netNormalTex);
        Material ropeMat = CreateRopeMaterial(ropeTex);

        Mesh barrelMesh = CreateBarrelMesh();
        Mesh nettingMesh = CreateNettingMesh();
        Mesh ropeMesh = CreateRopeMesh();

        AssetDatabase.CreateAsset(barrelMesh, "Assets/Prefabs/barrel_mesh.asset");
        AssetDatabase.CreateAsset(nettingMesh, "Assets/Prefabs/netting_mesh.asset");
        AssetDatabase.CreateAsset(ropeMesh, "Assets/Prefabs/rope_mesh.asset");

        GameObject root = new GameObject("piece_bait_creel");
        root.layer = LayerMask.NameToLayer("piece") >= 0 ? LayerMask.NameToLayer("piece") : 0;

        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, 0.5f, 0f);
        collider.size = new Vector3(0.85f, 1.0f, 0.85f);

        GameObject visualRoot = new GameObject("Visual");
        visualRoot.transform.SetParent(root.transform, false);
        visualRoot.layer = root.layer;

        GameObject barrelGo = new GameObject("Barrel");
        barrelGo.transform.SetParent(visualRoot.transform, false);
        barrelGo.layer = root.layer;
        MeshFilter mfBarrel = barrelGo.AddComponent<MeshFilter>();
        mfBarrel.sharedMesh = barrelMesh;
        MeshRenderer mrBarrel = barrelGo.AddComponent<MeshRenderer>();
        mrBarrel.sharedMaterial = woodMat;

        GameObject netGo = new GameObject("Netting");
        netGo.transform.SetParent(visualRoot.transform, false);
        netGo.layer = root.layer;
        MeshFilter mfNet = netGo.AddComponent<MeshFilter>();
        mfNet.sharedMesh = nettingMesh;
        MeshRenderer mrNet = netGo.AddComponent<MeshRenderer>();
        mrNet.sharedMaterial = netMat;

        GameObject ropeGo = new GameObject("Ropes");
        ropeGo.transform.SetParent(visualRoot.transform, false);
        ropeGo.layer = root.layer;
        MeshFilter mfRope = ropeGo.AddComponent<MeshFilter>();
        mfRope.sharedMesh = ropeMesh;
        MeshRenderer mrRope = ropeGo.AddComponent<MeshRenderer>();
        mrRope.sharedMaterial = ropeMat;

        string prefabPath = Path.Combine(PrefabsDir, "piece_bait_creel.prefab");
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        UnityEngine.Object.DestroyImmediate(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        AssetImporter importer = AssetImporter.GetAtPath(prefabPath);
        importer.assetBundleName = "greatcatch";

        AssetImporter woodMatImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(woodMat));
        woodMatImporter.assetBundleName = "greatcatch";

        AssetImporter netMatImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(netMat));
        netMatImporter.assetBundleName = "greatcatch";

        AssetImporter ropeMatImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ropeMat));
        ropeMatImporter.assetBundleName = "greatcatch";

        string iconPath = Path.Combine(TexturesDir, "I_BaitCreel.png");
        if (File.Exists(iconPath))
        {
            AssetDatabase.ImportAsset(iconPath);
            TextureImporter iconImporter = AssetImporter.GetAtPath(iconPath) as TextureImporter;
            if (iconImporter != null)
            {
                iconImporter.textureType = TextureImporterType.Sprite;
                iconImporter.spriteImportMode = SpriteImportMode.Single;
                iconImporter.alphaIsTransparency = true;
                iconImporter.assetBundleName = "greatcatch";
                iconImporter.SaveAndReimport();
            }
        }

        BuildPipeline.BuildAssetBundles(BundlesDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux64);

        string generatedBundlePath = Path.Combine(BundlesDir, "greatcatch");
        if (File.Exists(generatedBundlePath))
        {
            if (!Directory.Exists(TargetModBundleDir))
            {
                Directory.CreateDirectory(TargetModBundleDir);
            }
            string destination = Path.Combine(TargetModBundleDir, "greatcatch");
            File.Copy(generatedBundlePath, destination, true);
            Debug.Log($"Successfully deployed AssetBundle to: {destination} ({new FileInfo(destination).Length} bytes)");
        }
        else
        {
            Debug.LogError($"AssetBundle not found at {generatedBundlePath}!");
        }

        Debug.Log("Build complete!");
    }

    private static void EnsureDirectories()
    {
        if (!Directory.Exists(PrefabsDir)) Directory.CreateDirectory(PrefabsDir);
        if (!Directory.Exists(MaterialsDir)) Directory.CreateDirectory(MaterialsDir);
        if (!Directory.Exists(TexturesDir)) Directory.CreateDirectory(TexturesDir);
        if (!Directory.Exists(BundlesDir)) Directory.CreateDirectory(BundlesDir);
    }

    private static Texture2D GenerateWoodTexture()
    {
        int size = 512;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
        Color baseWood = new Color(0.38f, 0.26f, 0.15f, 1f);
        Color darkWood = new Color(0.20f, 0.13f, 0.07f, 1f);
        Color highlightWood = new Color(0.48f, 0.35f, 0.22f, 1f);
        Color ironBand = new Color(0.18f, 0.18f, 0.20f, 1f);
        Color ironRivet = new Color(0.45f, 0.40f, 0.30f, 1f);

        for (int y = 0; y < size; y++)
        {
            float normY = (float)y / size;
            bool isHoop = (normY >= 0.10f && normY <= 0.14f) ||
                          (normY >= 0.34f && normY <= 0.38f) ||
                          (normY >= 0.63f && normY <= 0.67f) ||
                          (normY >= 0.86f && normY <= 0.90f);

            for (int x = 0; x < size; x++)
            {
                if (isHoop)
                {
                    float noise = Mathf.PerlinNoise(x * 0.15f, y * 0.15f) * 0.12f;
                    Color c = Color.Lerp(ironBand, Color.black, noise);
                    if ((x % 64 >= 30 && x % 64 <= 34) && (y % 20 >= 8 && y % 20 <= 12))
                    {
                        c = ironRivet;
                    }
                    tex.SetPixel(x, y, c);
                }
                else
                {
                    bool isPlankSeam = (x % 32 == 0 || x % 32 == 1);
                    if (isPlankSeam)
                    {
                        tex.SetPixel(x, y, darkWood);
                    }
                    else
                    {
                        float woodNoise = Mathf.PerlinNoise(x * 0.04f, y * 0.35f);
                        float grain = Mathf.Sin((x + Mathf.PerlinNoise(x * 0.02f, y * 0.1f) * 15f) * 0.8f) * 0.08f;
                        Color c = Color.Lerp(baseWood, darkWood, woodNoise * 0.5f + grain);
                        if (x % 32 == 2) c = Color.Lerp(c, highlightWood, 0.3f);
                        tex.SetPixel(x, y, c);
                    }
                }
            }
        }

        tex.Apply();
        string path = Path.Combine(TexturesDir, "T_BarrelWood.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static Texture2D GenerateNetTexture()
    {
        int size = 512;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
        Color clear = new Color(0f, 0f, 0f, 0f);

        Color ropeHighlight = new Color(0.74f, 0.62f, 0.44f, 1f);
        Color ropeBase = new Color(0.55f, 0.44f, 0.28f, 1f);
        Color ropeDark = new Color(0.28f, 0.20f, 0.12f, 1f);
        Color ropeCrevice = new Color(0.14f, 0.10f, 0.05f, 1f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                tex.SetPixel(x, y, clear);
            }
        }

        int cell = 128;
        int halfCell = cell / 2;
        float ropeRadius = 8.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int modX = x % cell;
                int modY = y % cell;

                float d1 = Mathf.Abs(modX - modY) * 0.7071f;
                float d2 = Mathf.Abs(modX - (cell - modY)) * 0.7071f;

                int dxCorner = Mathf.Min(modX, cell - modX);
                int dyCorner = Mathf.Min(modY, cell - modY);
                float distCorner = Mathf.Sqrt(dxCorner * dxCorner + dyCorner * dyCorner);

                int dxCenter = modX - halfCell;
                int dyCenter = modY - halfCell;
                float distCenter = Mathf.Sqrt(dxCenter * dxCenter + dyCenter * dyCenter);

                float knotDist = Mathf.Min(distCorner, distCenter);

                if (knotDist <= 17f)
                {
                    float knotFactor = 1f - (knotDist / 17f);
                    float fiberNoise = Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * 0.15f;
                    float wrapGroove = Mathf.Sin((x + y) * 0.45f) * 0.15f;
                    Color c = Color.Lerp(ropeDark, ropeBase, knotFactor);
                    if (knotDist < 6f)
                    {
                        c = Color.Lerp(c, ropeHighlight, 0.5f + fiberNoise);
                    }
                    else if (knotDist > 13f)
                    {
                        c = Color.Lerp(c, ropeCrevice, 0.6f);
                    }
                    else
                    {
                        c = Color.Lerp(c, ropeHighlight, wrapGroove);
                    }
                    tex.SetPixel(x, y, c);
                }
                else if (d1 <= ropeRadius || d2 <= ropeRadius)
                {
                    bool useD1 = d1 <= d2;
                    float dist = useD1 ? d1 : d2;
                    float factor = 1f - (dist / ropeRadius);

                    float longCoord = useD1 ? (x + y) * 0.7071f : (x - y) * 0.7071f;
                    float strandWave = Mathf.Sin(longCoord * 0.35f);
                    float fiber = (Mathf.PerlinNoise(x * 0.5f, y * 0.5f) - 0.5f) * 0.12f;

                    Color c = Color.Lerp(ropeDark, ropeBase, factor);
                    if (strandWave > 0.05f)
                    {
                        c = Color.Lerp(c, ropeHighlight, (strandWave - 0.05f) * 0.85f * factor + fiber);
                    }
                    else
                    {
                        c = Color.Lerp(c, ropeDark, (-strandWave) * 0.6f);
                    }

                    if (factor < 0.25f)
                    {
                        c = Color.Lerp(ropeCrevice, c, factor / 0.25f);
                    }

                    tex.SetPixel(x, y, c);
                }
            }
        }

        tex.Apply();
        string path = Path.Combine(TexturesDir, "T_FishNet.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static Texture2D GenerateNetNormalTexture()
    {
        int size = 512;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
        Color flatNormal = new Color(0.5f, 0.5f, 1.0f, 1.0f);

        int cell = 128;
        int halfCell = cell / 2;
        float ropeRadius = 8.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int modX = x % cell;
                int modY = y % cell;

                float d1 = Mathf.Abs(modX - modY) * 0.7071f;
                float d2 = Mathf.Abs(modX - (cell - modY)) * 0.7071f;

                int dxCorner = Mathf.Min(modX, cell - modX);
                int dyCorner = Mathf.Min(modY, cell - modY);
                float distCorner = Mathf.Sqrt(dxCorner * dxCorner + dyCorner * dyCorner);

                int dxCenter = modX - halfCell;
                int dyCenter = modY - halfCell;
                float distCenter = Mathf.Sqrt(dxCenter * dxCenter + dyCenter * dyCenter);

                float knotDist = Mathf.Min(distCorner, distCenter);

                if (knotDist <= 17f)
                {
                    float dxKnot = (distCorner <= distCenter) ? ((modX < halfCell) ? modX : modX - cell) : dxCenter;
                    float dyKnot = (distCorner <= distCenter) ? ((modY < halfCell) ? modY : modY - cell) : dyCenter;

                    float nx = Mathf.Clamp(dxKnot / 17f, -1f, 1f) * 0.85f;
                    float ny = Mathf.Clamp(dyKnot / 17f, -1f, 1f) * 0.85f;
                    float nz = Mathf.Sqrt(Mathf.Max(0.05f, 1f - nx * nx - ny * ny));

                    Vector3 n = new Vector3(nx, ny, nz).normalized;
                    tex.SetPixel(x, y, new Color(n.x * 0.5f + 0.5f, n.y * 0.5f + 0.5f, n.z * 0.5f + 0.5f, 1f));
                }
                else if (d1 <= ropeRadius || d2 <= ropeRadius)
                {
                    bool useD1 = d1 <= d2;
                    float dist = useD1 ? d1 : d2;
                    float acrossT = Mathf.Clamp(dist / ropeRadius, -1f, 1f);

                    float longCoord = useD1 ? (x + y) * 0.7071f : (x - y) * 0.7071f;
                    float strandWave = Mathf.Cos(longCoord * 0.35f) * 0.3f;

                    float nu = strandWave;
                    float nv = (useD1 ? (modX - modY) : (modX + modY - cell)) > 0 ? acrossT : -acrossT;

                    float nx = (useD1 ? (nu - nv) : (nu + nv)) * 0.7071f;
                    float ny = (useD1 ? (nu + nv) : (nv - nu)) * 0.7071f;
                    float nz = Mathf.Sqrt(Mathf.Max(0.05f, 1f - nx * nx - ny * ny));

                    Vector3 n = new Vector3(nx, ny, nz).normalized;
                    tex.SetPixel(x, y, new Color(n.x * 0.5f + 0.5f, n.y * 0.5f + 0.5f, n.z * 0.5f + 0.5f, 1f));
                }
                else
                {
                    tex.SetPixel(x, y, flatNormal);
                }
            }
        }

        tex.Apply();
        string path = Path.Combine(TexturesDir, "T_FishNet_Normal.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.NormalMap;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static Texture2D GenerateRopeTexture()
    {
        int size = 256;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
        Color ropeLight = new Color(0.74f, 0.62f, 0.44f, 1f);
        Color ropeBase = new Color(0.55f, 0.44f, 0.28f, 1f);
        Color ropeDark = new Color(0.28f, 0.20f, 0.12f, 1f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float v = (float)y / size;
                float twist = Mathf.Sin((x * 0.4f + y * 0.8f)) * 0.5f + 0.5f;
                float cylinderFalloff = Mathf.Sin(v * Mathf.PI);

                Color c = Color.Lerp(ropeDark, ropeBase, cylinderFalloff);
                c = Color.Lerp(c, ropeLight, twist * cylinderFalloff * 0.6f);
                tex.SetPixel(x, y, c);
            }
        }
        tex.Apply();
        string path = Path.Combine(TexturesDir, "T_RopeHemp.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static Material CreateWoodMaterial(Texture2D tex)
    {
        Shader shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.mainTexture = tex;
        mat.SetFloat("_Glossiness", 0.15f);
        mat.SetFloat("_Metallic", 0.05f);
        string path = Path.Combine(MaterialsDir, "M_BarrelWood.mat");
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static Material CreateNetMaterial(Texture2D tex, Texture2D normalTex)
    {
        Shader shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.mainTexture = tex;
        mat.SetTexture("_BumpMap", normalTex);
        mat.EnableKeyword("_NORMALMAP");
        mat.SetFloat("_BumpScale", 1.4f);
        mat.SetFloat("_Mode", 1);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.EnableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 2450;
        mat.SetFloat("_Cutoff", 0.35f);
        mat.SetFloat("_Glossiness", 0.04f);
        string path = Path.Combine(MaterialsDir, "M_FishNet.mat");
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static Material CreateRopeMaterial(Texture2D tex)
    {
        Shader shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.mainTexture = tex;
        mat.SetFloat("_Glossiness", 0.08f);
        mat.SetFloat("_Metallic", 0.0f);
        string path = Path.Combine(MaterialsDir, "M_Rope.mat");
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static Mesh CreateBarrelMesh()
    {
        Mesh mesh = new Mesh { name = "BarrelMesh" };
        int radialSegments = 24;
        int verticalSegments = 10;

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        for (int y = 0; y <= verticalSegments; y++)
        {
            float v = (float)y / verticalSegments;
            float heightY = v * 1.0f;
            float r = 0.38f + 0.07f * (1.0f - Mathf.Pow(2f * v - 1f, 2f));

            for (int x = 0; x <= radialSegments; x++)
            {
                float u = (float)x / radialSegments;
                float angle = u * Mathf.PI * 2f;
                float posX = Mathf.Cos(angle) * r;
                float posZ = Mathf.Sin(angle) * r;

                verts.Add(new Vector3(posX, heightY, posZ));
                Vector3 n = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
                norms.Add(n);
                uvs.Add(new Vector2(u * 2f, v));
            }
        }

        for (int y = 0; y < verticalSegments; y++)
        {
            int row1 = y * (radialSegments + 1);
            int row2 = (y + 1) * (radialSegments + 1);

            for (int x = 0; x < radialSegments; x++)
            {
                tris.Add(row1 + x);
                tris.Add(row2 + x);
                tris.Add(row1 + x + 1);

                tris.Add(row1 + x + 1);
                tris.Add(row2 + x);
                tris.Add(row2 + x + 1);
            }
        }

        int bottomCenterIdx = verts.Count;
        verts.Add(new Vector3(0f, 0f, 0f));
        norms.Add(Vector3.down);
        uvs.Add(new Vector2(0.5f, 0.5f));

        for (int x = 0; x <= radialSegments; x++)
        {
            float u = (float)x / radialSegments;
            float angle = u * Mathf.PI * 2f;
            verts.Add(new Vector3(Mathf.Cos(angle) * 0.38f, 0f, Mathf.Sin(angle) * 0.38f));
            norms.Add(Vector3.down);
            uvs.Add(new Vector2(0.5f + Mathf.Cos(angle) * 0.5f, 0.5f + Mathf.Sin(angle) * 0.5f));
        }

        for (int x = 0; x < radialSegments; x++)
        {
            tris.Add(bottomCenterIdx);
            tris.Add(bottomCenterIdx + 1 + x);
            tris.Add(bottomCenterIdx + 1 + x + 1);
        }

        int topCenterIdx = verts.Count;
        verts.Add(new Vector3(0f, 0.96f, 0f));
        norms.Add(Vector3.up);
        uvs.Add(new Vector2(0.5f, 0.5f));

        for (int x = 0; x <= radialSegments; x++)
        {
            float u = (float)x / radialSegments;
            float angle = u * Mathf.PI * 2f;
            verts.Add(new Vector3(Mathf.Cos(angle) * 0.36f, 0.96f, Mathf.Sin(angle) * 0.36f));
            norms.Add(Vector3.up);
            uvs.Add(new Vector2(0.5f + Mathf.Cos(angle) * 0.5f, 0.5f + Mathf.Sin(angle) * 0.5f));
        }

        for (int x = 0; x < radialSegments; x++)
        {
            tris.Add(topCenterIdx);
            tris.Add(topCenterIdx + 1 + x + 1);
            tris.Add(topCenterIdx + 1 + x);
        }

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateNettingMesh()
    {
        Mesh mesh = new Mesh { name = "NettingMesh" };
        int radialSegments = 32;

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        // 1. Top Disc Lid Covering (sagging slightly into the barrel)
        int topCenterIdx = verts.Count;
        verts.Add(new Vector3(0f, 0.965f, 0f));
        norms.Add(Vector3.up);
        uvs.Add(new Vector2(0.5f, 0.5f));

        int topDiscStart = verts.Count;
        for (int seg = 0; seg <= radialSegments; seg++)
        {
            float u = (float)seg / radialSegments;
            float angle = u * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * 0.37f;
            float z = Mathf.Sin(angle) * 0.37f;

            verts.Add(new Vector3(x, 0.985f, z));
            norms.Add(Vector3.up);
            uvs.Add(new Vector2(0.5f + Mathf.Cos(angle) * 0.45f, 0.5f + Mathf.Sin(angle) * 0.45f));
        }

        for (int seg = 0; seg < radialSegments; seg++)
        {
            tris.Add(topCenterIdx);
            tris.Add(topDiscStart + seg + 1);
            tris.Add(topDiscStart + seg);

            tris.Add(topCenterIdx);
            tris.Add(topDiscStart + seg);
            tris.Add(topDiscStart + seg + 1);
        }

        // 2. Cascading and Draping Netting with Organic Fabric Stand-off & Wrinkles
        int drapeRings = 11;
        int drapeStartIdx = verts.Count;

        for (int ring = 0; ring <= drapeRings; ring++)
        {
            float t = (float)ring / drapeRings;

            for (int seg = 0; seg <= radialSegments; seg++)
            {
                float u = (float)seg / radialSegments;
                float angle = u * Mathf.PI * 2f;

                // Cascade arc covers the front ~180 degrees (angle between 0.1 and 3.0 rad)
                float bell = 0f;
                if (angle >= 0.1f && angle <= 3.04f)
                {
                    bell = Mathf.Sin((angle - 0.1f) / (3.04f - 0.1f) * Mathf.PI);
                }

                // Front cascade flows down barrel and puddles onto ground (1.45m total length)
                // Back drape hangs down past belly rope with scalloped cuts (0.50m - 0.65m)
                float shortHem = 0.55f + 0.08f * Mathf.Sin(angle * 5f);
                float maxDrapeLength = Mathf.Lerp(shortHem, 1.45f, bell);

                float dist = t * maxDrapeLength;

                float x, y, z;
                Vector3 normal;

                if (dist <= 0.95f)
                {
                    // Ring 0 starts rolling over the top rim at y = 1.025m
                    y = (dist < 0.08f) ? Mathf.Lerp(1.025f, 0.96f, dist / 0.08f) : (1.04f - dist);

                    float barrelRadius = 0.38f + 0.07f * (1.0f - Mathf.Pow(2f * (Mathf.Clamp01(y) - 0.5f), 2f));

                    // Physical fabric stand-off and cloth wrinkles
                    float fabricFolds = 0.015f * Mathf.Sin(angle * 7f + dist * 5f) + 0.008f * Mathf.Cos(angle * 13f - dist * 4f);
                    float looseSag = (dist > 0.35f) ? Mathf.Sin((dist - 0.35f) / 0.60f * Mathf.PI) * 0.022f : 0f;
                    float baseStandOff = (dist < 0.08f) ? 0.015f : 0.038f;

                    float drapeRadius = barrelRadius + baseStandOff + fabricFolds + looseSag;
                    x = Mathf.Cos(angle) * drapeRadius;
                    z = Mathf.Sin(angle) * drapeRadius;
                    normal = new Vector3(Mathf.Cos(angle), 0.12f, Mathf.Sin(angle)).normalized;
                }
                else
                {
                    // Spreading out on the ground in front of the barrel
                    float groundDist = dist - 0.95f;
                    y = Mathf.Max(0.015f, 0.045f - 0.030f * (groundDist / 0.50f));
                    float baseRadius = 0.38f + 0.038f;
                    float drapeRadius = baseRadius + groundDist;
                    x = Mathf.Cos(angle) * drapeRadius;
                    z = Mathf.Sin(angle) * drapeRadius;
                    normal = Vector3.up;
                }

                verts.Add(new Vector3(x, y, z));
                norms.Add(normal);

                float uCoord = u * 4.0f;
                float vCoord = dist / 0.60f;
                uvs.Add(new Vector2(uCoord, vCoord));
            }
        }

        for (int ring = 0; ring < drapeRings; ring++)
        {
            int row1 = drapeStartIdx + ring * (radialSegments + 1);
            int row2 = drapeStartIdx + (ring + 1) * (radialSegments + 1);

            for (int seg = 0; seg < radialSegments; seg++)
            {
                tris.Add(row1 + seg);
                tris.Add(row2 + seg);
                tris.Add(row1 + seg + 1);

                tris.Add(row1 + seg + 1);
                tris.Add(row2 + seg);
                tris.Add(row2 + seg + 1);

                tris.Add(row1 + seg);
                tris.Add(row1 + seg + 1);
                tris.Add(row2 + seg);

                tris.Add(row1 + seg + 1);
                tris.Add(row2 + seg + 1);
                tris.Add(row2 + seg);
            }
        }

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateRopeMesh()
    {
        Mesh mesh = new Mesh { name = "RopeMesh" };
        int circleSegments = 24;
        int crossSegments = 8;
        float torusMajorR = 0.448f;
        float torusMinorR = 0.022f;
        float torusY = 0.78f;

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        for (int i = 0; i <= circleSegments; i++)
        {
            float u = (float)i / circleSegments;
            float majorAngle = u * Mathf.PI * 2f;
            Vector3 center = new Vector3(Mathf.Cos(majorAngle) * torusMajorR, torusY, Mathf.Sin(majorAngle) * torusMajorR);
            Vector3 outDir = new Vector3(Mathf.Cos(majorAngle), 0f, Mathf.Sin(majorAngle));

            for (int j = 0; j <= crossSegments; j++)
            {
                float v = (float)j / crossSegments;
                float minorAngle = v * Mathf.PI * 2f;
                Vector3 offset = (outDir * Mathf.Cos(minorAngle) + Vector3.up * Mathf.Sin(minorAngle)) * torusMinorR;

                verts.Add(center + offset);
                norms.Add(offset.normalized);
                uvs.Add(new Vector2(u * 4f, v));
            }
        }

        for (int i = 0; i < circleSegments; i++)
        {
            int row1 = i * (crossSegments + 1);
            int row2 = (i + 1) * (crossSegments + 1);

            for (int j = 0; j < crossSegments; j++)
            {
                tris.Add(row1 + j);
                tris.Add(row2 + j);
                tris.Add(row1 + j + 1);

                tris.Add(row1 + j + 1);
                tris.Add(row2 + j);
                tris.Add(row2 + j + 1);
            }
        }

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }
}
