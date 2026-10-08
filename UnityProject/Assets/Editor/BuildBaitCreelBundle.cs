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
    private const string TargetModBundleDir = "/home/vapok/Modding/Valheim/BaitMeBruh/BaitMeBruh/Assets/Bundles";

    public static void Build()
    {
        Debug.Log("Starting Great Catch Traps AssetBundle build (Bait Creel, Coastal Net, Deep-Sea Net)...");

        EnsureDirectories();

        Texture2D woodTex = GenerateWoodTexture();
        Texture2D netTex = GenerateNetTexture();
        Texture2D netNormalTex = GenerateNetNormalTexture();
        Texture2D ropeTex = GenerateRopeTexture();

        Material woodMat = CreateWoodMaterial(woodTex);
        Material netMat = CreateNetMaterial(netTex, netNormalTex);
        Material ropeMat = CreateRopeMaterial(ropeTex);
        Material ironMat = CreateIronMaterial();

        // 1. Bait Creel
        Mesh creelBarrelMesh = CreateBarrelMesh();
        Mesh creelNettingMesh = CreateNettingMesh();
        Mesh creelRopeMesh = CreateRopeMesh();

        AssetDatabase.CreateAsset(creelBarrelMesh, "Assets/Prefabs/creel_barrel_mesh.asset");
        AssetDatabase.CreateAsset(creelNettingMesh, "Assets/Prefabs/creel_netting_mesh.asset");
        AssetDatabase.CreateAsset(creelRopeMesh, "Assets/Prefabs/creel_rope_mesh.asset");

        BuildBaitCreelPrefab(creelBarrelMesh, creelNettingMesh, creelRopeMesh, woodMat, netMat, ropeMat);

        // 2. Coastal Net
        Mesh coastalFrameMesh = CreateCoastalFrameMesh();
        Mesh coastalKegsMesh = CreateCoastalKegsMesh();
        Mesh coastalNetMesh = CreateCoastalNetMesh();
        Mesh coastalRopesMesh = CreateCoastalRopesMesh();

        AssetDatabase.CreateAsset(coastalFrameMesh, "Assets/Prefabs/coastal_frame_mesh.asset");
        AssetDatabase.CreateAsset(coastalKegsMesh, "Assets/Prefabs/coastal_kegs_mesh.asset");
        AssetDatabase.CreateAsset(coastalNetMesh, "Assets/Prefabs/coastal_net_mesh.asset");
        AssetDatabase.CreateAsset(coastalRopesMesh, "Assets/Prefabs/coastal_ropes_mesh.asset");

        BuildCoastalNetPrefab(coastalFrameMesh, coastalKegsMesh, coastalNetMesh, coastalRopesMesh, woodMat, netMat, ropeMat);

        // 3. Deep-Sea Anchored Net
        Mesh deepBuoyMesh = CreateDeepBuoyMesh();
        Mesh deepIronMesh = CreateDeepIronMesh();
        Mesh deepNetMesh = CreateDeepNetMesh();

        AssetDatabase.CreateAsset(deepBuoyMesh, "Assets/Prefabs/deep_buoy_mesh.asset");
        AssetDatabase.CreateAsset(deepIronMesh, "Assets/Prefabs/deep_iron_mesh.asset");
        AssetDatabase.CreateAsset(deepNetMesh, "Assets/Prefabs/deep_net_mesh.asset");

        BuildDeepNetPrefab(deepBuoyMesh, deepIronMesh, deepNetMesh, woodMat, ironMat, netMat);

        // 4. Coastal Net Crate
        Material iconCoastalMat = CreateIconMaterial("I_FishnetCoastal.png", "M_IconCoastal.mat");
        Mesh coastalCrateWoodMesh = CreateCoastalCrateWoodMesh();
        Mesh coastalCrateRopeMesh = CreateCoastalCrateRopeMesh();
        Mesh coastalCrateIconMesh = CreateCoastalCrateIconMesh();

        AssetDatabase.CreateAsset(coastalCrateWoodMesh, "Assets/Prefabs/coastal_crate_wood_mesh.asset");
        AssetDatabase.CreateAsset(coastalCrateRopeMesh, "Assets/Prefabs/coastal_crate_rope_mesh.asset");
        AssetDatabase.CreateAsset(coastalCrateIconMesh, "Assets/Prefabs/coastal_crate_icon_mesh.asset");

        BuildCoastalCratePrefab(coastalCrateWoodMesh, coastalCrateRopeMesh, coastalCrateIconMesh, woodMat, ropeMat, iconCoastalMat);

        // 5. Deep-Sea Net Crate
        Material iconDeepMat = CreateIconMaterial("I_FishnetDeep.png", "M_IconDeep.mat");
        Mesh deepCrateWoodMesh = CreateDeepCrateWoodMesh();
        Mesh deepCrateIronMesh = CreateDeepCrateIronMesh();
        Mesh deepCrateIconMesh = CreateDeepCrateIconMesh();

        AssetDatabase.CreateAsset(deepCrateWoodMesh, "Assets/Prefabs/deep_crate_wood_mesh.asset");
        AssetDatabase.CreateAsset(deepCrateIronMesh, "Assets/Prefabs/deep_crate_iron_mesh.asset");
        AssetDatabase.CreateAsset(deepCrateIconMesh, "Assets/Prefabs/deep_crate_icon_mesh.asset");

        BuildDeepCratePrefab(deepCrateWoodMesh, deepCrateIronMesh, deepCrateIconMesh, woodMat, ironMat, iconDeepMat);

        // 6. Trollfish Chowder Drop Model & HD Textures
        BuildTrollfishChowder.Build();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Tag Materials
        TagAssetBundle(AssetDatabase.GetAssetPath(woodMat), "greatcatch");
        TagAssetBundle(AssetDatabase.GetAssetPath(netMat), "greatcatch");
        TagAssetBundle(AssetDatabase.GetAssetPath(ropeMat), "greatcatch");
        TagAssetBundle(AssetDatabase.GetAssetPath(ironMat), "greatcatch");
        TagAssetBundle(AssetDatabase.GetAssetPath(iconCoastalMat), "greatcatch");
        TagAssetBundle(AssetDatabase.GetAssetPath(iconDeepMat), "greatcatch");

        // Tag and configure Icons
        ConfigureIconSprite("I_BaitCreel.png");
        ConfigureIconSprite("I_FishnetCoastal.png");
        ConfigureIconSprite("I_FishnetDeep.png");

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

        Debug.Log("AssetBundle build complete!");
    }

    private static void EnsureDirectories()
    {
        if (!Directory.Exists(PrefabsDir)) Directory.CreateDirectory(PrefabsDir);
        if (!Directory.Exists(MaterialsDir)) Directory.CreateDirectory(MaterialsDir);
        if (!Directory.Exists(TexturesDir)) Directory.CreateDirectory(TexturesDir);
        if (!Directory.Exists(BundlesDir)) Directory.CreateDirectory(BundlesDir);
    }

    private static void ConfigureIconSprite(string fileName)
    {
        string iconPath = Path.Combine(TexturesDir, fileName);
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

    private static void BuildBaitCreelPrefab(Mesh barrelMesh, Mesh nettingMesh, Mesh ropeMesh, Material woodMat, Material netMat, Material ropeMat)
    {
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
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        TagAssetBundle(prefabPath, "greatcatch");
    }

    private static void BuildCoastalNetPrefab(Mesh frameMesh, Mesh kegsMesh, Mesh netMesh, Mesh ropesMesh, Material woodMat, Material netMat, Material ropeMat)
    {
        GameObject root = new GameObject("piece_fishnet_coastal");
        root.layer = LayerMask.NameToLayer("piece") >= 0 ? LayerMask.NameToLayer("piece") : 0;

        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, -0.4f, 0f);
        collider.size = new Vector3(1.9f, 1.8f, 1.9f);

        GameObject visualRoot = new GameObject("Visual");
        visualRoot.transform.SetParent(root.transform, false);
        visualRoot.layer = root.layer;

        GameObject frameGo = new GameObject("Frame");
        frameGo.transform.SetParent(visualRoot.transform, false);
        frameGo.layer = root.layer;
        frameGo.AddComponent<MeshFilter>().sharedMesh = frameMesh;
        frameGo.AddComponent<MeshRenderer>().sharedMaterial = woodMat;

        GameObject kegsGo = new GameObject("Kegs");
        kegsGo.transform.SetParent(visualRoot.transform, false);
        kegsGo.layer = root.layer;
        kegsGo.AddComponent<MeshFilter>().sharedMesh = kegsMesh;
        kegsGo.AddComponent<MeshRenderer>().sharedMaterial = woodMat;

        GameObject netGo = new GameObject("Netting");
        netGo.transform.SetParent(visualRoot.transform, false);
        netGo.layer = root.layer;
        netGo.AddComponent<MeshFilter>().sharedMesh = netMesh;
        netGo.AddComponent<MeshRenderer>().sharedMaterial = netMat;

        GameObject ropesGo = new GameObject("Ropes");
        ropesGo.transform.SetParent(visualRoot.transform, false);
        ropesGo.layer = root.layer;
        ropesGo.AddComponent<MeshFilter>().sharedMesh = ropesMesh;
        ropesGo.AddComponent<MeshRenderer>().sharedMaterial = ropeMat;

        string prefabPath = Path.Combine(PrefabsDir, "piece_fishnet_coastal.prefab");
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        TagAssetBundle(prefabPath, "greatcatch");
    }

    private static void BuildDeepNetPrefab(Mesh buoyMesh, Mesh ironMesh, Mesh netMesh, Material woodMat, Material ironMat, Material netMat)
    {
        GameObject root = new GameObject("piece_fishnet_deep");
        root.layer = LayerMask.NameToLayer("piece") >= 0 ? LayerMask.NameToLayer("piece") : 0;

        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, 0.85f, 0f);
        collider.size = new Vector3(1.4f, 2.3f, 1.4f);

        GameObject visualRoot = new GameObject("Visual");
        visualRoot.transform.SetParent(root.transform, false);
        visualRoot.layer = root.layer;

        GameObject buoyGo = new GameObject("Buoy");
        buoyGo.transform.SetParent(visualRoot.transform, false);
        buoyGo.layer = root.layer;
        buoyGo.AddComponent<MeshFilter>().sharedMesh = buoyMesh;
        buoyGo.AddComponent<MeshRenderer>().sharedMaterial = woodMat;

        GameObject ironGo = new GameObject("Iron");
        ironGo.transform.SetParent(visualRoot.transform, false);
        ironGo.layer = root.layer;
        ironGo.AddComponent<MeshFilter>().sharedMesh = ironMesh;
        ironGo.AddComponent<MeshRenderer>().sharedMaterial = ironMat;

        GameObject netGo = new GameObject("Netting");
        netGo.transform.SetParent(visualRoot.transform, false);
        netGo.layer = root.layer;
        netGo.AddComponent<MeshFilter>().sharedMesh = netMesh;
        netGo.AddComponent<MeshRenderer>().sharedMaterial = netMat;

        string prefabPath = Path.Combine(PrefabsDir, "piece_fishnet_deep.prefab");
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        TagAssetBundle(prefabPath, "greatcatch");
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

                    float alongStrand = useD1 ? (x + y) : (x - y);
                    float fiberNoise = Mathf.PerlinNoise(x * 0.25f, y * 0.25f) * 0.12f;
                    float twist = Mathf.Sin(alongStrand * 0.35f) * 0.18f;

                    Color c = Color.Lerp(ropeDark, ropeBase, factor);
                    if (factor > 0.65f)
                    {
                        c = Color.Lerp(c, ropeHighlight, (factor - 0.65f) / 0.35f + twist + fiberNoise);
                    }
                    else if (factor < 0.25f)
                    {
                        c = Color.Lerp(ropeCrevice, c, factor / 0.25f);
                    }
                    tex.SetPixel(x, y, c);
                }
            }
        }

        tex.Apply();
        string path = Path.Combine(TexturesDir, "T_FishNet_Hemp.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static Texture2D GenerateNetNormalTexture()
    {
        int size = 512;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
        Color flatNormal = new Color(0.5f, 0.5f, 1.0f, 1f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                tex.SetPixel(x, y, flatNormal);
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
                    float factor = 1f - (knotDist / 17f);
                    float dirX = (knotDist == distCorner) ? (modX < cell / 2 ? -modX : cell - modX) : (dxCenter);
                    float dirY = (knotDist == distCorner) ? (modY < cell / 2 ? -modY : cell - modY) : (dyCenter);
                    Vector2 dirNorm = new Vector2(dirX, dirY).normalized;

                    float nx = 0.5f + dirNorm.x * factor * 0.40f;
                    float ny = 0.5f + dirNorm.y * factor * 0.40f;
                    float nz = Mathf.Sqrt(Mathf.Clamp01(1f - ((nx - 0.5f) * (nx - 0.5f) + (ny - 0.5f) * (ny - 0.5f)) * 4f));
                    tex.SetPixel(x, y, new Color(nx, ny, 0.5f + nz * 0.5f, 1f));
                }
                else if (d1 <= ropeRadius || d2 <= ropeRadius)
                {
                    bool useD1 = d1 <= d2;
                    float dist = useD1 ? d1 : d2;
                    float factor = 1f - (dist / ropeRadius);

                    float perpX = useD1 ? -0.7071f : 0.7071f;
                    float perpY = 0.7071f;
                    float sign = useD1 ? Mathf.Sign(modX - modY) : Mathf.Sign(modX - (cell - modY));

                    float alongStrand = useD1 ? (x + y) : (x - y);
                    float twist = Mathf.Sin(alongStrand * 0.45f) * 0.25f;

                    float nx = 0.5f + (perpX * sign * factor + twist * 0.15f) * 0.45f;
                    float ny = 0.5f + (perpY * sign * factor + twist * 0.15f) * 0.45f;
                    float nz = Mathf.Sqrt(Mathf.Clamp01(1f - ((nx - 0.5f) * (nx - 0.5f) + (ny - 0.5f) * (ny - 0.5f)) * 4f));
                    tex.SetPixel(x, y, new Color(nx, ny, 0.5f + nz * 0.5f, 1f));
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

    private static Material CreateIronMaterial()
    {
        Shader shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.color = new Color(0.20f, 0.20f, 0.22f, 1f);
        mat.SetFloat("_Glossiness", 0.40f);
        mat.SetFloat("_Metallic", 0.85f);
        string path = Path.Combine(MaterialsDir, "M_ForgedIron.mat");
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    // Geometry Helpers
    private static void AddCylinder(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, Vector3 start, Vector3 end, float radius, int segments = 8, float uvScale = 1f)
    {
        Vector3 dir = (end - start).normalized;
        float length = (end - start).magnitude;
        Vector3 perp1 = Vector3.Cross(dir, Vector3.up).normalized;
        if (perp1.sqrMagnitude < 0.001f) perp1 = Vector3.Cross(dir, Vector3.right).normalized;
        Vector3 perp2 = Vector3.Cross(dir, perp1).normalized;

        int startIdx = verts.Count;
        for (int i = 0; i <= segments; i++)
        {
            float a = (float)i / segments * Mathf.PI * 2f;
            Vector3 radOffset = (perp1 * Mathf.Cos(a) + perp2 * Mathf.Sin(a)) * radius;
            Vector3 normal = radOffset.normalized;

            verts.Add(start + radOffset);
            norms.Add(normal);
            uvs.Add(new Vector2((float)i / segments * uvScale, 0f));

            verts.Add(end + radOffset);
            norms.Add(normal);
            uvs.Add(new Vector2((float)i / segments * uvScale, length * uvScale));
        }

        for (int i = 0; i < segments; i++)
        {
            int b0 = startIdx + i * 2;
            int t0 = b0 + 1;
            int b1 = startIdx + (i + 1) * 2;
            int t1 = b1 + 1;

            tris.Add(b0); tris.Add(t0); tris.Add(b1);
            tris.Add(b1); tris.Add(t0); tris.Add(t1);
        }
    }

    private static void AddBox(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, Vector3 center, Vector3 size)
    {
        Vector3 h = size * 0.5f;
        Vector3[] corners = new Vector3[]
        {
            center + new Vector3(-h.x, -h.y, -h.z),
            center + new Vector3( h.x, -h.y, -h.z),
            center + new Vector3( h.x,  h.y, -h.z),
            center + new Vector3(-h.x,  h.y, -h.z),
            center + new Vector3(-h.x, -h.y,  h.z),
            center + new Vector3( h.x, -h.y,  h.z),
            center + new Vector3( h.x,  h.y,  h.z),
            center + new Vector3(-h.x,  h.y,  h.z)
        };

        int[][] faces = new int[][]
        {
            new int[] { 0, 3, 2, 1, 0, 0, -1 }, // Front
            new int[] { 5, 6, 7, 4, 0, 0,  1 }, // Back
            new int[] { 4, 7, 3, 0, -1, 0, 0 }, // Left
            new int[] { 1, 2, 6, 5,  1, 0, 0 }, // Right
            new int[] { 3, 7, 6, 2,  0, 1, 0 }, // Top
            new int[] { 4, 0, 1, 5,  0,-1, 0 }  // Bottom
        };

        foreach (int[] f in faces)
        {
            int startIdx = verts.Count;
            Vector3 norm = new Vector3(f[4], f[5], f[6]);
            verts.Add(corners[f[0]]); norms.Add(norm); uvs.Add(new Vector2(0f, 0f));
            verts.Add(corners[f[1]]); norms.Add(norm); uvs.Add(new Vector2(0f, 1f));
            verts.Add(corners[f[2]]); norms.Add(norm); uvs.Add(new Vector2(1f, 1f));
            verts.Add(corners[f[3]]); norms.Add(norm); uvs.Add(new Vector2(1f, 0f));

            tris.Add(startIdx); tris.Add(startIdx + 1); tris.Add(startIdx + 2);
            tris.Add(startIdx); tris.Add(startIdx + 2); tris.Add(startIdx + 3);
        }
    }

    private static void AddBarrel(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, Vector3 center, float radius, float height, float bulge = 0.04f, int radialSegs = 12, int vertSegs = 6)
    {
        int startIdx = verts.Count;
        for (int v = 0; v <= vertSegs; v++)
        {
            float vNorm = (float)v / vertSegs;
            float y = center.y - (height * 0.5f) + (vNorm * height);
            float r = radius + bulge * (1.0f - Mathf.Pow(2f * (vNorm - 0.5f), 2f));

            for (int u = 0; u <= radialSegs; u++)
            {
                float uNorm = (float)u / radialSegs;
                float angle = uNorm * Mathf.PI * 2f;
                float x = center.x + Mathf.Cos(angle) * r;
                float z = center.z + Mathf.Sin(angle) * r;

                Vector3 pos = new Vector3(x, y, z);
                Vector3 norm = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;

                verts.Add(pos);
                norms.Add(norm);
                uvs.Add(new Vector2(uNorm * 2.0f, vNorm));
            }
        }

        for (int v = 0; v < vertSegs; v++)
        {
            int row1 = startIdx + v * (radialSegs + 1);
            int row2 = startIdx + (v + 1) * (radialSegs + 1);
            for (int u = 0; u < radialSegs; u++)
            {
                // Outward faces (clockwise)
                tris.Add(row1 + u); tris.Add(row1 + u + 1); tris.Add(row2 + u);
                tris.Add(row1 + u + 1); tris.Add(row2 + u + 1); tris.Add(row2 + u);

                // Inward faces (counter-clockwise)
                tris.Add(row1 + u); tris.Add(row2 + u); tris.Add(row1 + u + 1);
                tris.Add(row1 + u + 1); tris.Add(row2 + u); tris.Add(row2 + u + 1);
            }
        }

        // Top lid
        int lidCenterIdx = verts.Count;
        verts.Add(new Vector3(center.x, center.y + height * 0.5f, center.z));
        norms.Add(Vector3.up);
        uvs.Add(new Vector2(0.5f, 0.5f));
        int topRow = startIdx + vertSegs * (radialSegs + 1);
        for (int u = 0; u < radialSegs; u++)
        {
            // Upward
            tris.Add(lidCenterIdx);
            tris.Add(topRow + u + 1);
            tris.Add(topRow + u);
            // Downward (two-sided)
            tris.Add(lidCenterIdx);
            tris.Add(topRow + u);
            tris.Add(topRow + u + 1);
        }

        // Bottom lid
        int botCenterIdx = verts.Count;
        verts.Add(new Vector3(center.x, center.y - height * 0.5f, center.z));
        norms.Add(Vector3.down);
        uvs.Add(new Vector2(0.5f, 0.5f));
        int botRow = startIdx;
        for (int u = 0; u < radialSegs; u++)
        {
            // Downward
            tris.Add(botCenterIdx);
            tris.Add(botRow + u);
            tris.Add(botRow + u + 1);
            // Upward (two-sided)
            tris.Add(botCenterIdx);
            tris.Add(botRow + u + 1);
            tris.Add(botRow + u);
        }
    }

    private static void AddHoop(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, float y, float radius, float tubeRadius = 0.025f, int segs = 16)
    {
        for (int i = 0; i < segs; i++)
        {
            float a0 = (float)i / segs * Mathf.PI * 2f;
            float a1 = (float)(i + 1) / segs * Mathf.PI * 2f;
            Vector3 p0 = new Vector3(Mathf.Cos(a0) * radius, y, Mathf.Sin(a0) * radius);
            Vector3 p1 = new Vector3(Mathf.Cos(a1) * radius, y, Mathf.Sin(a1) * radius);
            AddCylinder(verts, norms, uvs, tris, p0, p1, tubeRadius, 6, 0.5f);
        }
    }

    // --- Bait Creel Meshes ---
    private static Mesh CreateBarrelMesh()
    {
        Mesh mesh = new Mesh { name = "CreelBarrelMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        AddBarrel(verts, norms, uvs, tris, new Vector3(0f, 0.5f, 0f), 0.38f, 1.0f, 0.07f, 20, 10);

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateNettingMesh()
    {
        Mesh mesh = new Mesh { name = "CreelNettingMesh" };
        int radialSegments = 32;

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        // Uniform metric UV density across entire netting (top lid and side drape skirt)
        float rimRadius = 0.395f;
        float circumference = Mathf.PI * 2f * rimRadius;
        float uRepeats = 4.0f;
        float metersPerTile = circumference / uRepeats; // ~0.6205m per UV tile (15.5cm diamond cells)

        // 1. Top Lid Netting Cap (elevated dome with organic fabric texture, covers wood lid)
        int centerIdx = verts.Count;
        verts.Add(new Vector3(0f, 1.030f, 0f));
        norms.Add(Vector3.up);
        uvs.Add(new Vector2(0.5f, 0.5f));

        int topRings = 5;
        float[] topRadii = new float[] { 0.10f, 0.20f, 0.29f, 0.37f, 0.400f };
        float[] topHeights = new float[] { 1.028f, 1.025f, 1.021f, 1.016f, 1.008f };

        int prevTopRingStart = -1;

        for (int r = 0; r < topRings; r++)
        {
            int ringStart = verts.Count;
            float baseRadius = topRadii[r];
            float baseHeight = topHeights[r];

            for (int seg = 0; seg <= radialSegments; seg++)
            {
                float u = (float)seg / radialSegments;
                float angle = u * Mathf.PI * 2f;

                float wrinkle = (0.0025f * Mathf.Sin(angle * 6f) + 0.0015f * Mathf.Cos(angle * 10f)) * (baseRadius / 0.40f);
                float y = baseHeight + wrinkle;
                float x = Mathf.Cos(angle) * baseRadius;
                float z = Mathf.Sin(angle) * baseRadius;

                Vector3 pos = new Vector3(x, y, z);
                Vector3 norm = Vector3.Lerp(Vector3.up, new Vector3(Mathf.Cos(angle) * 0.7f, 0.7f, Mathf.Sin(angle) * 0.7f).normalized, (float)r / (topRings - 1));

                verts.Add(pos);
                norms.Add(norm);
                // Planar Cartesian UV projection using exact same metric scale as side skirt
                uvs.Add(new Vector2(0.5f + (x / metersPerTile), 0.5f + (z / metersPerTile)));
            }

            if (r == 0)
            {
                for (int seg = 0; seg < radialSegments; seg++)
                {
                    // Upward
                    tris.Add(centerIdx);
                    tris.Add(ringStart + seg);
                    tris.Add(ringStart + seg + 1);

                    // Downward (two-sided visibility)
                    tris.Add(centerIdx);
                    tris.Add(ringStart + seg + 1);
                    tris.Add(ringStart + seg);
                }
            }
            else
            {
                for (int seg = 0; seg < radialSegments; seg++)
                {
                    int p0 = prevTopRingStart + seg;
                    int p1 = prevTopRingStart + seg + 1;
                    int c0 = ringStart + seg;
                    int c1 = ringStart + seg + 1;

                    // Upward
                    tris.Add(p0); tris.Add(c0); tris.Add(p1);
                    tris.Add(p1); tris.Add(c0); tris.Add(c1);

                    // Downward
                    tris.Add(p0); tris.Add(p1); tris.Add(c0);
                    tris.Add(p1); tris.Add(c1); tris.Add(c0);
                }
            }

            prevTopRingStart = ringStart;
        }

        // 2. Side Drape Skirt (separate vertex loop starting seamlessly at rim, draping down into water)
        int skirtRings = 16;
        float skirtStartHeight = 1.014f;

        for (int r = 0; r < skirtRings; r++)
        {
            int ringStart = verts.Count;
            float t = (float)r / (skirtRings - 1);

            for (int seg = 0; seg <= radialSegments; seg++)
            {
                float u = (float)seg / radialSegments;
                float angle = u * Mathf.PI * 2f;

                float hemY = -0.05f + 0.04f * Mathf.Sin(angle * 5f) + 0.025f * Mathf.Cos(angle * 9f + 0.8f);
                float y = Mathf.Lerp(skirtStartHeight, hemY, t);

                float bY = Mathf.Clamp01(y);
                float barrelR = 0.38f + 0.07f * (1.0f - Mathf.Pow(2f * (bY - 0.5f), 2f));

                float x, z;
                Vector3 norm;

                if (y >= 0.78f)
                {
                    float upperT = (skirtStartHeight - y) / (skirtStartHeight - 0.78f);
                    float standOff = Mathf.Lerp(0.022f, 0.015f, upperT);
                    float folds = 0.006f * Mathf.Sin(angle * 8f);
                    float rad = barrelR + standOff + folds;

                    x = Mathf.Cos(angle) * rad;
                    z = Mathf.Sin(angle) * rad;
                    norm = new Vector3(Mathf.Cos(angle), 0.12f, Mathf.Sin(angle)).normalized;
                }
                else
                {
                    float lowerT = (0.78f - y) / (0.78f - hemY);
                    float standOff = Mathf.Lerp(0.015f, 0.060f, lowerT);
                    float folds = (0.020f * Mathf.Sin(angle * 7f) + 0.012f * Mathf.Cos(angle * 11f + 1.2f)) * Mathf.Sqrt(lowerT);
                    float rad = barrelR + standOff + folds;

                    x = Mathf.Cos(angle) * rad;
                    z = Mathf.Sin(angle) * rad;
                    norm = new Vector3(Mathf.Cos(angle), 0.10f, Mathf.Sin(angle)).normalized;
                }

                verts.Add(new Vector3(x, y, z));
                norms.Add(norm);

                float dist = skirtStartHeight - y;
                uvs.Add(new Vector2(u * uRepeats, dist / metersPerTile));
            }

            if (r > 0)
            {
                int prevSkirtRing = ringStart - (radialSegments + 1);
                for (int seg = 0; seg < radialSegments; seg++)
                {
                    int p0 = prevSkirtRing + seg;
                    int p1 = prevSkirtRing + seg + 1;
                    int c0 = ringStart + seg;
                    int c1 = ringStart + seg + 1;

                    // Outward
                    tris.Add(p0); tris.Add(c0); tris.Add(p1);
                    tris.Add(p1); tris.Add(c0); tris.Add(c1);

                    // Inward
                    tris.Add(p0); tris.Add(p1); tris.Add(c0);
                    tris.Add(p1); tris.Add(c1); tris.Add(c0);
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

    private static Mesh CreateRopeMesh()
    {
        Mesh mesh = new Mesh { name = "CreelRopeMesh" };
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
                tris.Add(row1 + j); tris.Add(row2 + j); tris.Add(row1 + j + 1);
                tris.Add(row1 + j + 1); tris.Add(row2 + j); tris.Add(row2 + j + 1);
            }
        }

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    // --- Coastal Net Meshes ---
    private static Mesh CreateCoastalFrameMesh()
    {
        Mesh mesh = new Mesh { name = "CoastalFrameMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        float w = 0.85f;
        float y = 0.05f;
        float rLog = 0.09f;

        // 4 border logs
        AddCylinder(verts, norms, uvs, tris, new Vector3(-w, y, -w), new Vector3(w, y, -w), rLog, 8, 2f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(-w, y,  w), new Vector3(w, y,  w), rLog, 8, 2f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(-w, y, -w), new Vector3(-w, y,  w), rLog, 8, 2f);
        AddCylinder(verts, norms, uvs, tris, new Vector3( w, y, -w), new Vector3( w, y,  w), rLog, 8, 2f);

        // Cross timber
        AddCylinder(verts, norms, uvs, tris, new Vector3(-w, y, 0f), new Vector3(w, y, 0f), 0.06f, 8, 2f);

        // Marker Mast
        AddCylinder(verts, norms, uvs, tris, new Vector3(0f, y, 0f), new Vector3(0f, y + 1.25f, 0f), 0.05f, 8, 1.5f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(-0.16f, y + 1.15f, 0f), new Vector3(0.16f, y + 1.15f, 0f), 0.025f, 6, 0.5f);

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateCoastalKegsMesh()
    {
        Mesh mesh = new Mesh { name = "CoastalKegsMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        float offset = 0.78f;
        float y = -0.05f;
        AddBarrel(verts, norms, uvs, tris, new Vector3(-offset, y, -offset), 0.16f, 0.38f, 0.03f, 10, 4);
        AddBarrel(verts, norms, uvs, tris, new Vector3( offset, y, -offset), 0.16f, 0.38f, 0.03f, 10, 4);
        AddBarrel(verts, norms, uvs, tris, new Vector3(-offset, y,  offset), 0.16f, 0.38f, 0.03f, 10, 4);
        AddBarrel(verts, norms, uvs, tris, new Vector3( offset, y,  offset), 0.16f, 0.38f, 0.03f, 10, 4);

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateCoastalNetMesh()
    {
        Mesh mesh = new Mesh { name = "CoastalNetMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        int rings = 7;
        int segsPerSide = 6;
        int totalSegs = segsPerSide * 4;

        for (int r = 0; r <= rings; r++)
        {
            float t = (float)r / rings;
            float y = Mathf.Lerp(0.02f, -1.35f, t);
            float halfWidth = Mathf.Lerp(0.78f, 0.58f, t);

            for (int i = 0; i < totalSegs; i++)
            {
                int side = i / segsPerSide;
                float s = (float)(i % segsPerSide) / segsPerSide;
                float x = 0f, z = 0f;

                if (side == 0) // Front (-w to +w at -z)
                {
                    x = Mathf.Lerp(-halfWidth, halfWidth, s);
                    z = -halfWidth;
                }
                else if (side == 1) // Right (+z to +w at +x)
                {
                    x = halfWidth;
                    z = Mathf.Lerp(-halfWidth, halfWidth, s);
                }
                else if (side == 2) // Back (+w to -w at +z)
                {
                    x = Mathf.Lerp(halfWidth, -halfWidth, s);
                    z = halfWidth;
                }
                else // Left (-x)
                {
                    x = -halfWidth;
                    z = Mathf.Lerp(halfWidth, -halfWidth, s);
                }

                // Fabric fold displacement
                float fold = (t > 0.1f) ? Mathf.Sin(x * 5f + z * 5f + y * 6f) * 0.025f : 0f;
                Vector3 p = new Vector3(x, y, z) + new Vector3(x, 0f, z).normalized * fold;
                Vector3 n = new Vector3(x, 0f, z).normalized;

                verts.Add(p);
                norms.Add(n);
                uvs.Add(new Vector2((float)i / segsPerSide, t * 3.5f));
            }
        }

        // Stitch quad rings
        for (int r = 0; r < rings; r++)
        {
            int row1 = r * totalSegs;
            int row2 = (r + 1) * totalSegs;
            for (int i = 0; i < totalSegs; i++)
            {
                int next = (i + 1) % totalSegs;
                tris.Add(row1 + i); tris.Add(row2 + i); tris.Add(row1 + next);
                tris.Add(row1 + next); tris.Add(row2 + i); tris.Add(row2 + next);
                // Two-sided
                tris.Add(row1 + i); tris.Add(row1 + next); tris.Add(row2 + i);
                tris.Add(row1 + next); tris.Add(row2 + next); tris.Add(row2 + i);
            }
        }

        // Bottom closed floor
        int botCenterIdx = verts.Count;
        verts.Add(new Vector3(0f, -1.38f, 0f));
        norms.Add(Vector3.down);
        uvs.Add(new Vector2(0.5f, 0.5f));
        int botRow = rings * totalSegs;
        for (int i = 0; i < totalSegs; i++)
        {
            int next = (i + 1) % totalSegs;
            tris.Add(botCenterIdx); tris.Add(botRow + next); tris.Add(botRow + i);
            tris.Add(botCenterIdx); tris.Add(botRow + i); tris.Add(botRow + next);
        }

        // Corner sinker stones at bottom
        AddBox(verts, norms, uvs, tris, new Vector3(-0.58f, -1.35f, -0.58f), new Vector3(0.10f, 0.10f, 0.10f));
        AddBox(verts, norms, uvs, tris, new Vector3( 0.58f, -1.35f, -0.58f), new Vector3(0.10f, 0.10f, 0.10f));
        AddBox(verts, norms, uvs, tris, new Vector3(-0.58f, -1.35f,  0.58f), new Vector3(0.10f, 0.10f, 0.10f));
        AddBox(verts, norms, uvs, tris, new Vector3( 0.58f, -1.35f,  0.58f), new Vector3(0.10f, 0.10f, 0.10f));

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateCoastalRopesMesh()
    {
        Mesh mesh = new Mesh { name = "CoastalRopesMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        float offset = 0.78f;
        float y = 0.05f;
        // Corner lashings
        AddCylinder(verts, norms, uvs, tris, new Vector3(-offset, y - 0.15f, -offset), new Vector3(-offset, y + 0.15f, -offset), 0.035f, 6, 1f);
        AddCylinder(verts, norms, uvs, tris, new Vector3( offset, y - 0.15f, -offset), new Vector3( offset, y + 0.15f, -offset), 0.035f, 6, 1f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(-offset, y - 0.15f,  offset), new Vector3(-offset, y + 0.15f,  offset), 0.035f, 6, 1f);
        AddCylinder(verts, norms, uvs, tris, new Vector3( offset, y - 0.15f,  offset), new Vector3( offset, y + 0.15f,  offset), 0.035f, 6, 1f);

        // Mast tie
        AddCylinder(verts, norms, uvs, tris, new Vector3(0f, y + 0.02f, 0f), new Vector3(0f, y + 0.16f, 0f), 0.065f, 6, 1f);

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    // --- Deep-Sea Net Meshes ---
    private static Mesh CreateDeepBuoyMesh()
    {
        Mesh mesh = new Mesh { name = "DeepBuoyMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        // Stout upright oceanic cask buoy: sits partially in water (y=0 water level)
        // Center: y = 0.15f, height = 0.85f (from y = -0.275f to +0.575f), radius = 0.52f, bulge = 0.08f
        AddBarrel(verts, norms, uvs, tris, new Vector3(0f, 0.15f, 0f), 0.52f, 0.85f, 0.08f, 20, 10);

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateDeepIronMesh()
    {
        Mesh mesh = new Mesh { name = "DeepIronMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        // 1. Heavy forged iron hoops clamping the cask staves
        AddHoop(verts, norms, uvs, tris, 0.46f, 0.565f, 0.024f, 20);
        AddHoop(verts, norms, uvs, tris, 0.15f, 0.605f, 0.024f, 20);
        AddHoop(verts, norms, uvs, tris, -0.16f, 0.565f, 0.024f, 20);

        // 2. Vertical clamping straps (at 0, 90, 180, 270 degrees)
        for (int i = 0; i < 4; i++)
        {
            float ang = i * Mathf.PI * 0.5f;
            float cos = Mathf.Cos(ang);
            float sin = Mathf.Sin(ang);

            // Vertical strap connecting hoops
            Vector3 strapTop = new Vector3(cos * 0.555f, 0.52f, sin * 0.555f);
            Vector3 strapMid = new Vector3(cos * 0.605f, 0.15f, sin * 0.605f);
            Vector3 strapBot = new Vector3(cos * 0.555f, -0.22f, sin * 0.555f);
            AddCylinder(verts, norms, uvs, tris, strapTop, strapMid, 0.020f, 6, 0.5f);
            AddCylinder(verts, norms, uvs, tris, strapMid, strapBot, 0.020f, 6, 0.5f);

            // Stud rivets at hoop intersections
            AddBox(verts, norms, uvs, tris, new Vector3(cos * 0.58f, 0.46f, sin * 0.58f), new Vector3(0.045f, 0.045f, 0.045f));
            AddBox(verts, norms, uvs, tris, new Vector3(cos * 0.62f, 0.15f, sin * 0.62f), new Vector3(0.045f, 0.045f, 0.045f));
            AddBox(verts, norms, uvs, tris, new Vector3(cos * 0.58f, -0.16f, sin * 0.58f), new Vector3(0.045f, 0.045f, 0.045f));
        }

        // 3. Sturdy forged iron tripod / cage beacon on top of buoy
        // 3 legs rooted firmly on top lid: y = 0.575f, radius = 0.42f
        Vector3[] feet = new Vector3[3];
        Vector3[] legMid = new Vector3[3];
        Vector3 apex = new Vector3(0f, 1.40f, 0f);

        for (int i = 0; i < 3; i++)
        {
            float ang = i * (Mathf.PI * 2f / 3f);
            float cos = Mathf.Cos(ang);
            float sin = Mathf.Sin(ang);

            feet[i] = new Vector3(cos * 0.42f, 0.575f, sin * 0.42f);
            legMid[i] = Vector3.Lerp(feet[i], apex, 0.55f);

            // Iron foot mounting bracket
            AddBox(verts, norms, uvs, tris, feet[i] + new Vector3(0f, 0.015f, 0f), new Vector3(0.08f, 0.035f, 0.08f));

            // Main tripod leg strut
            AddCylinder(verts, norms, uvs, tris, feet[i], apex, 0.032f, 6, 1.0f);
        }

        // Horizontal brace triangle connecting the 3 legs at mid-height
        for (int i = 0; i < 3; i++)
        {
            int next = (i + 1) % 3;
            AddCylinder(verts, norms, uvs, tris, legMid[i], legMid[next], 0.022f, 6, 0.5f);
        }

        // Apex collar
        AddHoop(verts, norms, uvs, tris, 1.38f, 0.07f, 0.022f, 8);

        // Central signal spire rising above tripod
        AddCylinder(verts, norms, uvs, tris, new Vector3(0f, 1.30f, 0f), new Vector3(0f, 1.82f, 0f), 0.030f, 8, 1.0f);

        // Signal crossbar
        AddCylinder(verts, norms, uvs, tris, new Vector3(-0.16f, 1.68f, 0f), new Vector3(0.16f, 1.68f, 0f), 0.018f, 6, 0.5f);

        // Top forged mooring / hoisting ring
        AddHoop(verts, norms, uvs, tris, 1.94f, 0.10f, 0.020f, 12);
        AddCylinder(verts, norms, uvs, tris, new Vector3(0f, 1.80f, 0f), new Vector3(0f, 1.85f, 0f), 0.025f, 6, 0.2f);

        // 4. Underwater iron spreader ring and mooring chains
        // Spreader ring at y = -0.65f, radius = 0.68f
        AddHoop(verts, norms, uvs, tris, -0.65f, 0.68f, 0.028f, 20);

        // Cross braces on spreader ring
        AddCylinder(verts, norms, uvs, tris, new Vector3(-0.68f, -0.65f, 0f), new Vector3(0.68f, -0.65f, 0f), 0.022f, 6, 1.0f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(0f, -0.65f, -0.68f), new Vector3(0f, -0.65f, 0.68f), 0.022f, 6, 1.0f);

        // 4 heavy chain strands from cask lower rim to spreader ring and down to sinker weights
        for (int i = 0; i < 4; i++)
        {
            float ang = i * Mathf.PI * 0.5f;
            float cos = Mathf.Cos(ang);
            float sin = Mathf.Sin(ang);

            Vector3 eyeBolt = new Vector3(cos * 0.56f, -0.16f, sin * 0.56f);
            Vector3 ringAttach = new Vector3(cos * 0.68f, -0.65f, sin * 0.68f);
            Vector3 sinkerPos = new Vector3(cos * 0.52f, -2.60f, sin * 0.52f);

            // Eye bolt on cask
            AddBox(verts, norms, uvs, tris, eyeBolt, new Vector3(0.06f, 0.08f, 0.06f));

            // Upper chain (cask to spreader ring)
            int upperLinks = 5;
            for (int l = 0; l < upperLinks; l++)
            {
                float t0 = (float)l / upperLinks;
                float t1 = (float)(l + 1) / upperLinks;
                Vector3 p0 = Vector3.Lerp(eyeBolt, ringAttach, t0);
                Vector3 p1 = Vector3.Lerp(eyeBolt, ringAttach, t1);
                AddCylinder(verts, norms, uvs, tris, p0, p1, 0.025f, 6, 0.4f);
            }

            // Lower chain (spreader ring down to sinker weight)
            int lowerLinks = 16;
            for (int l = 0; l < lowerLinks; l++)
            {
                float t0 = (float)l / lowerLinks;
                float t1 = (float)(l + 1) / lowerLinks;
                Vector3 p0 = Vector3.Lerp(ringAttach, sinkerPos, t0);
                Vector3 p1 = Vector3.Lerp(ringAttach, sinkerPos, t1);
                AddCylinder(verts, norms, uvs, tris, p0, p1, 0.026f, 6, 0.4f);
            }

            // Heavy forged iron sinker block
            AddBox(verts, norms, uvs, tris, sinkerPos, new Vector3(0.16f, 0.22f, 0.16f));
        }

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateDeepNetMesh()
    {
        Mesh mesh = new Mesh { name = "DeepNetMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        int rings = 12;
        int radialSegs = 18;

        for (int r = 0; r <= rings; r++)
        {
            float t = (float)r / rings;
            float y = Mathf.Lerp(-0.65f, -3.10f, t);

            // Shape: starts at spreader ring radius (0.68f), stays broad through throat, then tapers
            float radius;
            if (t < 0.35f)
            {
                radius = Mathf.Lerp(0.68f, 0.63f, t / 0.35f);
            }
            else
            {
                radius = Mathf.Lerp(0.63f, 0.22f, (t - 0.35f) / 0.65f);
            }

            for (int s = 0; s <= radialSegs; s++)
            {
                float u = (float)s / radialSegs;
                float angle = u * Mathf.PI * 2f;

                float fold = (t > 0.05f) ? Mathf.Sin(angle * 6f + y * 4.5f) * 0.030f : 0f;
                float rDist = radius + fold;

                float x = Mathf.Cos(angle) * rDist;
                float z = Mathf.Sin(angle) * rDist;

                verts.Add(new Vector3(x, y, z));
                norms.Add(new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized);
                uvs.Add(new Vector2(u * 5.0f, t * 6.0f));
            }
        }

        for (int r = 0; r < rings; r++)
        {
            int row1 = r * (radialSegs + 1);
            int row2 = (r + 1) * (radialSegs + 1);
            for (int s = 0; s < radialSegs; s++)
            {
                // Outward
                tris.Add(row1 + s); tris.Add(row2 + s); tris.Add(row1 + s + 1);
                tris.Add(row1 + s + 1); tris.Add(row2 + s); tris.Add(row2 + s + 1);
                // Inward (two-sided)
                tris.Add(row1 + s); tris.Add(row1 + s + 1); tris.Add(row2 + s);
                tris.Add(row1 + s + 1); tris.Add(row2 + s + 1); tris.Add(row2 + s);
            }
        }

        // Bottom gathering point with gathering ring
        int botIdx = verts.Count;
        verts.Add(new Vector3(0f, -3.20f, 0f));
        norms.Add(Vector3.down);
        uvs.Add(new Vector2(0.5f, 0.5f));
        int lastRow = rings * (radialSegs + 1);
        for (int s = 0; s < radialSegs; s++)
        {
            tris.Add(botIdx); tris.Add(lastRow + s + 1); tris.Add(lastRow + s);
            tris.Add(botIdx); tris.Add(lastRow + s); tris.Add(lastRow + s + 1);
        }

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Material CreateIconMaterial(string textureName, string matName)
    {
        string iconPath = Path.Combine(TexturesDir, textureName);
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        Shader shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.mainTexture = tex;
        mat.SetFloat("_Mode", 1); // Cutout
        mat.SetFloat("_Cutoff", 0.10f);
        mat.SetFloat("_Glossiness", 0.10f);
        mat.SetFloat("_Metallic", 0.05f);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.EnableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 2450;
        string path = Path.Combine(MaterialsDir, matName);
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static void BuildCoastalCratePrefab(Mesh woodMesh, Mesh ropeMesh, Mesh iconMesh, Material woodMat, Material ropeMat, Material iconMat)
    {
        GameObject root = new GameObject("crate_fishnet_coastal");
        root.layer = LayerMask.NameToLayer("item") >= 0 ? LayerMask.NameToLayer("item") : 0;

        GameObject woodGo = new GameObject("Wood");
        woodGo.transform.SetParent(root.transform, false);
        woodGo.AddComponent<MeshFilter>().sharedMesh = woodMesh;
        woodGo.AddComponent<MeshRenderer>().sharedMaterial = woodMat;

        GameObject ropeGo = new GameObject("Ropes");
        ropeGo.transform.SetParent(root.transform, false);
        ropeGo.AddComponent<MeshFilter>().sharedMesh = ropeMesh;
        ropeGo.AddComponent<MeshRenderer>().sharedMaterial = ropeMat;

        GameObject iconGo = new GameObject("Icons");
        iconGo.transform.SetParent(root.transform, false);
        iconGo.AddComponent<MeshFilter>().sharedMesh = iconMesh;
        iconGo.AddComponent<MeshRenderer>().sharedMaterial = iconMat;

        string prefabPath = Path.Combine(PrefabsDir, "crate_fishnet_coastal.prefab");
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        TagAssetBundle(prefabPath, "greatcatch");
    }

    private static void BuildDeepCratePrefab(Mesh woodMesh, Mesh ironMesh, Mesh iconMesh, Material woodMat, Material ironMat, Material iconMat)
    {
        GameObject root = new GameObject("crate_fishnet_deep");
        root.layer = LayerMask.NameToLayer("item") >= 0 ? LayerMask.NameToLayer("item") : 0;

        GameObject woodGo = new GameObject("Wood");
        woodGo.transform.SetParent(root.transform, false);
        woodGo.AddComponent<MeshFilter>().sharedMesh = woodMesh;
        woodGo.AddComponent<MeshRenderer>().sharedMaterial = woodMat;

        GameObject ironGo = new GameObject("Iron");
        ironGo.transform.SetParent(root.transform, false);
        ironGo.AddComponent<MeshFilter>().sharedMesh = ironMesh;
        ironGo.AddComponent<MeshRenderer>().sharedMaterial = ironMat;

        GameObject iconGo = new GameObject("Icons");
        iconGo.transform.SetParent(root.transform, false);
        iconGo.AddComponent<MeshFilter>().sharedMesh = iconMesh;
        iconGo.AddComponent<MeshRenderer>().sharedMaterial = iconMat;

        string prefabPath = Path.Combine(PrefabsDir, "crate_fishnet_deep.prefab");
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        TagAssetBundle(prefabPath, "greatcatch");
    }

    private static Mesh CreateCoastalCrateWoodMesh()
    {
        Mesh mesh = new Mesh { name = "CoastalCrateWoodMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        AddBox(verts, norms, uvs, tris, new Vector3(0f, 0.18f, 0f), new Vector3(0.52f, 0.36f, 0.42f));

        float cx = 0.245f;
        float cz = 0.195f;
        float postW = 0.05f;
        AddBox(verts, norms, uvs, tris, new Vector3(-cx, 0.18f, -cz), new Vector3(postW, 0.365f, postW));
        AddBox(verts, norms, uvs, tris, new Vector3( cx, 0.18f, -cz), new Vector3(postW, 0.365f, postW));
        AddBox(verts, norms, uvs, tris, new Vector3(-cx, 0.18f,  cz), new Vector3(postW, 0.365f, postW));
        AddBox(verts, norms, uvs, tris, new Vector3( cx, 0.18f,  cz), new Vector3(postW, 0.365f, postW));

        AddBox(verts, norms, uvs, tris, new Vector3(0f, 0.365f, -cz), new Vector3(0.48f, 0.025f, 0.04f));
        AddBox(verts, norms, uvs, tris, new Vector3(0f, 0.365f,  cz), new Vector3(0.48f, 0.025f, 0.04f));
        AddBox(verts, norms, uvs, tris, new Vector3(-cx, 0.365f, 0f), new Vector3(0.04f, 0.025f, 0.38f));
        AddBox(verts, norms, uvs, tris, new Vector3( cx, 0.365f, 0f), new Vector3(0.04f, 0.025f, 0.38f));

        AddBox(verts, norms, uvs, tris, new Vector3(0f, 0.015f, -cz), new Vector3(0.48f, 0.025f, 0.04f));
        AddBox(verts, norms, uvs, tris, new Vector3(0f, 0.015f,  cz), new Vector3(0.48f, 0.025f, 0.04f));

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateCoastalCrateRopeMesh()
    {
        Mesh mesh = new Mesh { name = "CoastalCrateRopeMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        float w = 0.265f;
        float d = 0.215f;
        float rRope = 0.016f;

        float y1 = 0.10f;
        AddCylinder(verts, norms, uvs, tris, new Vector3(-w, y1, -d), new Vector3(w, y1, -d), rRope, 6, 1f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(w, y1, -d), new Vector3(w, y1, d), rRope, 6, 1f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(w, y1, d), new Vector3(-w, y1, d), rRope, 6, 1f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(-w, y1, d), new Vector3(-w, y1, -d), rRope, 6, 1f);

        float y2 = 0.26f;
        AddCylinder(verts, norms, uvs, tris, new Vector3(-w, y2, -d), new Vector3(w, y2, -d), rRope, 6, 1f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(w, y2, -d), new Vector3(w, y2, d), rRope, 6, 1f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(w, y2, d), new Vector3(-w, y2, d), rRope, 6, 1f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(-w, y2, d), new Vector3(-w, y2, -d), rRope, 6, 1f);

        AddCylinder(verts, norms, uvs, tris, new Vector3(-w - 0.01f, 0.18f, -0.06f), new Vector3(-w - 0.04f, 0.22f, 0f), rRope, 6, 0.5f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(-w - 0.04f, 0.22f, 0f), new Vector3(-w - 0.01f, 0.18f, 0.06f), rRope, 6, 0.5f);
        AddCylinder(verts, norms, uvs, tris, new Vector3( w + 0.01f, 0.18f, -0.06f), new Vector3( w + 0.04f, 0.22f, 0f), rRope, 6, 0.5f);
        AddCylinder(verts, norms, uvs, tris, new Vector3( w + 0.04f, 0.22f, 0f), new Vector3( w + 0.01f, 0.18f, 0.06f), rRope, 6, 0.5f);

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateCoastalCrateIconMesh()
    {
        Mesh mesh = new Mesh { name = "CoastalCrateIconMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        AddHorizontalQuad(verts, norms, uvs, tris, new Vector3(0f, 0.365f, 0f), new Vector2(0.28f, 0.28f));
        AddVerticalQuadFront(verts, norms, uvs, tris, new Vector3(0f, 0.18f, -0.215f), new Vector2(0.22f, 0.22f));
        AddVerticalQuadBack(verts, norms, uvs, tris, new Vector3(0f, 0.18f, 0.215f), new Vector2(0.22f, 0.22f));

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateDeepCrateWoodMesh()
    {
        Mesh mesh = new Mesh { name = "DeepCrateWoodMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        AddBox(verts, norms, uvs, tris, new Vector3(0f, 0.21f, 0f), new Vector3(0.60f, 0.42f, 0.50f));

        float cx = 0.28f;
        float cz = 0.23f;
        float postW = 0.06f;
        AddBox(verts, norms, uvs, tris, new Vector3(-cx, 0.21f, -cz), new Vector3(postW, 0.425f, postW));
        AddBox(verts, norms, uvs, tris, new Vector3( cx, 0.21f, -cz), new Vector3(postW, 0.425f, postW));
        AddBox(verts, norms, uvs, tris, new Vector3(-cx, 0.21f,  cz), new Vector3(postW, 0.425f, postW));
        AddBox(verts, norms, uvs, tris, new Vector3( cx, 0.21f,  cz), new Vector3(postW, 0.425f, postW));

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateDeepCrateIronMesh()
    {
        Mesh mesh = new Mesh { name = "DeepCrateIronMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        float w = 0.605f;
        float d = 0.505f;
        float bandThickness = 0.015f;
        float bandH = 0.045f;

        AddBox(verts, norms, uvs, tris, new Vector3(0f, 0.10f, 0f), new Vector3(w + bandThickness, bandH, d + bandThickness));
        AddBox(verts, norms, uvs, tris, new Vector3(0f, 0.32f, 0f), new Vector3(w + bandThickness, bandH, d + bandThickness));

        float cx = 0.285f;
        float cz = 0.235f;
        AddBox(verts, norms, uvs, tris, new Vector3(-cx, 0.415f, -cz), new Vector3(0.08f, 0.03f, 0.08f));
        AddBox(verts, norms, uvs, tris, new Vector3( cx, 0.415f, -cz), new Vector3(0.08f, 0.03f, 0.08f));
        AddBox(verts, norms, uvs, tris, new Vector3(-cx, 0.415f,  cz), new Vector3(0.08f, 0.03f, 0.08f));
        AddBox(verts, norms, uvs, tris, new Vector3( cx, 0.415f,  cz), new Vector3(0.08f, 0.03f, 0.08f));

        AddCylinder(verts, norms, uvs, tris, new Vector3(-0.31f, 0.22f, -0.06f), new Vector3(-0.35f, 0.26f, 0f), 0.018f, 6, 0.5f);
        AddCylinder(verts, norms, uvs, tris, new Vector3(-0.35f, 0.26f, 0f), new Vector3(-0.31f, 0.22f, 0.06f), 0.018f, 6, 0.5f);
        AddCylinder(verts, norms, uvs, tris, new Vector3( 0.31f, 0.22f, -0.06f), new Vector3( 0.35f, 0.26f, 0f), 0.018f, 6, 0.5f);
        AddCylinder(verts, norms, uvs, tris, new Vector3( 0.35f, 0.26f, 0f), new Vector3( 0.31f, 0.22f, 0.06f), 0.018f, 6, 0.5f);

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateDeepCrateIconMesh()
    {
        Mesh mesh = new Mesh { name = "DeepCrateIconMesh" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        AddHorizontalQuad(verts, norms, uvs, tris, new Vector3(0f, 0.425f, 0f), new Vector2(0.34f, 0.34f));
        AddVerticalQuadFront(verts, norms, uvs, tris, new Vector3(0f, 0.21f, -0.255f), new Vector2(0.26f, 0.26f));
        AddVerticalQuadBack(verts, norms, uvs, tris, new Vector3(0f, 0.21f, 0.255f), new Vector2(0.26f, 0.26f));

        mesh.SetVertices(verts);
        mesh.SetNormals(norms);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static void AddHorizontalQuad(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, Vector3 center, Vector2 size)
    {
        Vector2 h = size * 0.5f;
        int idx = verts.Count;
        Vector3 norm = Vector3.up;

        verts.Add(center + new Vector3(-h.x, 0f,  h.y)); norms.Add(norm); uvs.Add(new Vector2(0f, 1f));
        verts.Add(center + new Vector3( h.x, 0f,  h.y)); norms.Add(norm); uvs.Add(new Vector2(1f, 1f));
        verts.Add(center + new Vector3( h.x, 0f, -h.y)); norms.Add(norm); uvs.Add(new Vector2(1f, 0f));
        verts.Add(center + new Vector3(-h.x, 0f, -h.y)); norms.Add(norm); uvs.Add(new Vector2(0f, 0f));

        tris.Add(idx); tris.Add(idx + 1); tris.Add(idx + 2);
        tris.Add(idx); tris.Add(idx + 2); tris.Add(idx + 3);
    }

    private static void AddVerticalQuadFront(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, Vector3 center, Vector2 size)
    {
        Vector2 h = size * 0.5f;
        int idx = verts.Count;
        Vector3 norm = new Vector3(0f, 0f, -1f);

        verts.Add(center + new Vector3(-h.x,  h.y, 0f)); norms.Add(norm); uvs.Add(new Vector2(0f, 1f));
        verts.Add(center + new Vector3( h.x,  h.y, 0f)); norms.Add(norm); uvs.Add(new Vector2(1f, 1f));
        verts.Add(center + new Vector3( h.x, -h.y, 0f)); norms.Add(norm); uvs.Add(new Vector2(1f, 0f));
        verts.Add(center + new Vector3(-h.x, -h.y, 0f)); norms.Add(norm); uvs.Add(new Vector2(0f, 0f));

        tris.Add(idx); tris.Add(idx + 1); tris.Add(idx + 2);
        tris.Add(idx); tris.Add(idx + 2); tris.Add(idx + 3);
    }

    private static void AddVerticalQuadBack(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, Vector3 center, Vector2 size)
    {
        Vector2 h = size * 0.5f;
        int idx = verts.Count;
        Vector3 norm = new Vector3(0f, 0f, 1f);

        verts.Add(center + new Vector3( h.x,  h.y, 0f)); norms.Add(norm); uvs.Add(new Vector2(0f, 1f));
        verts.Add(center + new Vector3(-h.x,  h.y, 0f)); norms.Add(norm); uvs.Add(new Vector2(1f, 1f));
        verts.Add(center + new Vector3(-h.x, -h.y, 0f)); norms.Add(norm); uvs.Add(new Vector2(1f, 0f));
        verts.Add(center + new Vector3( h.x, -h.y, 0f)); norms.Add(norm); uvs.Add(new Vector2(0f, 0f));

        tris.Add(idx); tris.Add(idx + 1); tris.Add(idx + 2);
        tris.Add(idx); tris.Add(idx + 2); tris.Add(idx + 3);
    }
}
